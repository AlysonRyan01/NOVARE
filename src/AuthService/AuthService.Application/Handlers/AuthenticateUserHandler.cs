using AuthService.Application.Commands;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using FluentValidation;
using MediatR;
using SharedService.Shared;

namespace AuthService.Application.Handlers;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, Result<string>>
{
    private readonly IUserCommandRepository _userCommandRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AuthenticateUserCommand> _validator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtBearerService _jwtBearerService;

    public AuthenticateUserHandler(
        IUserCommandRepository userCommandRepository, 
        IUserQueryRepository userQueryRepository, 
        IUnitOfWork unitOfWork,
        IValidator<AuthenticateUserCommand> validator,
        IPasswordHasher passwordHasher,
        IJwtBearerService jwtBearerService)
    {
        _userCommandRepository = userCommandRepository;
        _userQueryRepository = userQueryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _passwordHasher = passwordHasher;
        _jwtBearerService = jwtBearerService;
    }

    public async Task<Result<string>> Handle(
        AuthenticateUserCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<string>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            string jwt;
            
            var existUser = await _userQueryRepository.GetByEmailAsync(request.Email);
            if (!existUser.IsSuccess)
            {
                var passwordHash = _passwordHasher.HashPassword(request.Password);
                
                var createUserResult = User.Create(request.Email, passwordHash);
                if (!createUserResult.IsSuccess)
                    return Result<string>.Fail(createUserResult.Errors!);
                
                var user = createUserResult.Value;
                if (user == null)
                    return Result<string>.Fail(["O usuário não pode ser nulo"]);
                
                var insertRepositoryResult = await _userCommandRepository.AddAsync(user);
                if (!insertRepositoryResult.IsSuccess)
                    return Result<string>.Fail(insertRepositoryResult.Errors!);
                
                var rowsAffected =  await _unitOfWork.CommitAsync();
                if (rowsAffected == 0)
                    return Result<string>.Fail(["Erro ao adicionar o usuário no banco de dados"]);

                jwt = await _jwtBearerService.Generate(user);
                
                return Result<string>.Ok(jwt);
            }
            
            if (existUser.Value == null)
                return Result<string>.Fail(["Usuário nulo"]);
            
            if (!_passwordHasher.VerifyHashedPassword(existUser.Value.Password.Value, request.Password))
                return Result<string>.Fail(["E-mail ou senha inválidos"]);
            
            jwt = await _jwtBearerService.Generate(existUser.Value);
            
            return Result<string>.Ok(jwt);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<string>.Fail([ex.Message]);
        }
    }
}