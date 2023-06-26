using Microsoft.Extensions.Options;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Aggregates.UserAggregate;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Auth
{
    public sealed class JwtFactory : IJwtFactory
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IJwtTokenHandler jwtTokenHandler, 
            IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _jwtOptions = jwtOptions.Value;

            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<AccessToken> GenerateEncodedToken(User userFromRepo, IList<string> roles)
        {
            var identity = GenerateClaimsIdentity(userFromRepo.IdentityId, userFromRepo.UserName, userFromRepo.UserType);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userFromRepo.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Email, userFromRepo.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, $"{userFromRepo.FirstName} {userFromRepo.LastName}"),
                new Claim(JwtRegisteredClaimNames.UniqueName, userFromRepo.Id.ToString()),
                identity.FindFirst(JwtConstants.Strings.JwtClaimIdentifiers.Rol),
                identity.FindFirst(JwtConstants.Strings.JwtClaimIdentifiers.Id),
                identity.FindFirst(JwtConstants.Strings.JwtClaimIdentifiers.UserType)
            };

            roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
            userFromRepo.Facilities.ForEach(dto => claims.Add(new Claim(ClaimTypes.NameIdentifier, dto.Facility.FacilityName)));

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience,
                claims, _jwtOptions.NotBefore, _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new AccessToken(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);
        }

        private static ClaimsIdentity GenerateClaimsIdentity(Guid id, string userName, UserType userType) =>
            new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[] {
                new Claim(JwtConstants.Strings.JwtClaimIdentifiers.Id, id.ToString()),
                new Claim(JwtConstants.Strings.JwtClaimIdentifiers.Rol, JwtConstants.Strings.JwtClaims.ApiAccess),
                new Claim(JwtConstants.Strings.JwtClaimIdentifiers.UserType, userType.ToString())
            });

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }

}
