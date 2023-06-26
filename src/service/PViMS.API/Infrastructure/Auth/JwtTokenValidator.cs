using Microsoft.IdentityModel.Tokens;
using PVIMS.API.Infrastructure.Services;
using System.Security.Claims;
using System.Text;

namespace PVIMS.API.Infrastructure.Auth
{
    public sealed class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;

        public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler) => _jwtTokenHandler = jwtTokenHandler;

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey, string issuer, string audience) =>
            _jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidateIssuer = true,

                ValidAudience = audience,
                ValidateAudience = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)),
                ValidateIssuerSigningKey = true,

                ValidateLifetime = false // we check expired tokens here
            });
    }
}
