namespace CallAlert.Api.Dtos.Auth;

public record RegisterRequest(
    string Username,
    string Password,
    string? PhoneNumber,
    string? Email);


