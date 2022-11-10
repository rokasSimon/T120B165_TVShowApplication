using System.Security.Claims;

namespace TVShowApplication.Services.Interfaces
{
    public interface IJwtGenerator
    {
        string GenerateToken(IDictionary<string, string> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
