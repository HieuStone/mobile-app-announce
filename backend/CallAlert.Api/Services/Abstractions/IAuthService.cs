using CallAlert.Api.Dtos.Auth;

namespace CallAlert.Api.Services.Abstractions;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}


