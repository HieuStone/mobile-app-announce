using CallAlert.Api.Entities;

namespace CallAlert.Api.Services.Abstractions;

public interface ITokenService
{
    string GenerateToken(User user);
}


