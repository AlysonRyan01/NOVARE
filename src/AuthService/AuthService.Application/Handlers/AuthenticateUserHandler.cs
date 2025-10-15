using AuthService.Application.Commands;
using AuthService.Application.Services;
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

    public AuthenticateUserHandler(
        IUserCommandRepository userCommandRepository, 
        IUserQueryRepository userQueryRepository, 
        IUnitOfWork unitOfWork,
        IValidator<AuthenticateUserCommand> validator)
    {
        _userCommandRepository = userCommandRepository;
        _userQueryRepository = userQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if  (!validationResult.IsValid)
            return Result<string>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            
    }
}