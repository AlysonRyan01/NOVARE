using Gateway.Api.Interfaces;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Services;

public class AuthService :  IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("AuthService");
        _logger = logger;
    }
    
    public async Task<Result<string>> Authenticate(AuthenticateUserDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth", dto);
        
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<Result<string>>();
                
            _logger.LogInformation("Authentication successful for user: {Email}", dto.Email);
            return Result<string>.Ok(authResponse!.Value!);
        }
        
        return Result<string>.Fail(["E-mail ou senha inválidos"]);
    }
}