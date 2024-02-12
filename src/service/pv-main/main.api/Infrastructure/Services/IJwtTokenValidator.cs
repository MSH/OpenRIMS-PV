using System.Security.Claims;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey, string issuer, string audience);
    }
}
