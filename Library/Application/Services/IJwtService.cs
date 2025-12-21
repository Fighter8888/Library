using System.Security.Claims;

namespace Library.Application.Services;

public interface IJwtService
{
    string GenerateToken(Guid accountId, string username, IEnumerable<string> roles);
    ClaimsPrincipal? ValidateToken(string token);
}

