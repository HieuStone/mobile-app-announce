using CallAlert.Api.Data;
using CallAlert.Api.Dtos.Auth;
using CallAlert.Api.Entities;
using CallAlert.Api.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CallAlert.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Username và password không được để trống.");
        }

        var normalizedUsername = request.Username.Trim();
        var exists = await _dbContext.Users.AnyAsync(u => u.Username == normalizedUsername);
        if (exists)
        {
            throw new InvalidOperationException("Username đã tồn tại.");
        }

        var user = new User
        {
            Username = normalizedUsername,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse(user.Id, user.Username, user.PhoneNumber, user.Email, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var normalizedUsername = request.Username.Trim();
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == normalizedUsername);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Sai thông tin đăng nhập.");
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Sai thông tin đăng nhập.");
        }

        var token = _tokenService.GenerateToken(user);
        return new AuthResponse(user.Id, user.Username, user.PhoneNumber, user.Email, token);
    }
}


