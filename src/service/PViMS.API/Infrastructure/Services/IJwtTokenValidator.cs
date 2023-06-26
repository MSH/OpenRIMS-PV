using System.Security.Claims;

namespace PVIMS.API.Infrastructure.Services
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey, string issuer, string audience);
    }
}
