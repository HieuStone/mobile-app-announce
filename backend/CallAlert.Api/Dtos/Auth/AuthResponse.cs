namespace CallAlert.Api.Dtos.Auth;

public record AuthResponse(
    int UserId,
    string Username,
    string? PhoneNumber,
    string? Email,
    string AccessToken);


