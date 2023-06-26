using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PVIMS.API.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace PVIMS.API.Infrastructure.Auth
{
    public class ApiKeyHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string _API_KEY_HEADERNAME = "X-Api-Key";
        private const long _REQUEST_MAX_AGE_IN_SECONDS = 300;
        
        private static readonly DateTime _EPOCH_START = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        private IMemoryCache _memoryCache;

        public ApiKeyHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMemoryCache memoryCache) 
                : base(options, logger, encoder, clock) 
        {
            _memoryCache = memoryCache;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var serverConfigurations = PrepareConfigurations();

            if (!Request.Headers.TryGetValue(_API_KEY_HEADERNAME, out var apiKeyHeaderHashedValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKeySignature = apiKeyHeaderHashedValues.FirstOrDefault();
            if (apiKeyHeaderHashedValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKeySignature))
            {
                return AuthenticateResult.NoResult();
            }

            string[] authenticationParameters = providedApiKeySignature.Split(new char[] { ':' });
            if (!IsParametersLengthCorrect(authenticationParameters))
            {
                return AuthenticateResult.Fail("Invalid parameters count");
            }

            var clientParameters = PrepareClientParameters(authenticationParameters);

            if (!IsApiKeyValid(serverConfigurations.apiKey, clientParameters.apiKey))
            {
                return AuthenticateResult.Fail("Api Key Is not valid");
            }

            if (IsReplayRequest(clientParameters.nonce, clientParameters.timeStamp))
            {
                return AuthenticateResult.Fail("Security breach due to replay request");
            }

            var rawSignature = PrepareRawSignature(serverConfigurations.apiKey, clientParameters.timeStamp, clientParameters.nonce);

            if (!IsAuthenticationTokenVerified(rawSignature, serverConfigurations.secretKey, clientParameters.hashedBase64String))
            {
                return AuthenticateResult.Fail("Security breach due to token verification");
            }

            var authenticationTicket = PrepareAuthenticationTicket(serverConfigurations.clientName);

            return AuthenticateResult.Success(authenticationTicket);
        }

        private bool IsParametersLengthCorrect(string[] authenticationParameters)
        {
            return authenticationParameters.Length == 4;
        }

        private bool IsApiKeyValid(string serverApiKey, string clientApiKey)
        {
            return serverApiKey.Equals(clientApiKey, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsReplayRequest(string clientNonce, string clientTimeStamp)
        {
            if (_memoryCache.TryGetValue<string>(clientNonce, out string nonce))
                return true;

            TimeSpan currentTs = DateTime.UtcNow - _EPOCH_START;
            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(clientTimeStamp);

            if (serverTotalSeconds - requestTotalSeconds > _REQUEST_MAX_AGE_IN_SECONDS)
            {
                return true;
            }

            _memoryCache.Set(clientNonce, clientTimeStamp, DateTimeOffset.UtcNow.AddSeconds(_REQUEST_MAX_AGE_IN_SECONDS));

            return false;
        }

        private bool IsAuthenticationTokenVerified(string rawSignature, string serverSecretKey, string clientHashedBase64String)
        {
            var secretKeyBytes = Convert.FromBase64String(serverSecretKey);
            var signature = Encoding.UTF8.GetBytes(rawSignature.Trim());

            using (HMACSHA512 hmac = new HMACSHA512(secretKeyBytes))
            {
                var hashedBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(hashedBytes);

                if (!clientHashedBase64String.Equals(requestSignatureBase64String, StringComparison.Ordinal))
                    return false;
            }

            return true;
        }

        private (string apiKey, string secretKey, string clientName) PrepareConfigurations()
        {
            var configuration = Request.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var configAuthSettings = configuration.GetSection(nameof(AuthSettings));

            return (configAuthSettings.GetValue<string>(key: "ApiKey"), configAuthSettings.GetValue<string>(key: "ApiSecretKey"), configAuthSettings.GetValue<string>(key: "ApiClientName"));
        }

        private (string apiKey, string hashedBase64String, string nonce, string timeStamp) PrepareClientParameters(string[] authenticationParameters)
        {
            return (authenticationParameters[0], authenticationParameters[1], authenticationParameters[2], authenticationParameters[3]);
        }

        private string PrepareRawSignature(string serverApiKey, string clientTimeStamp, string clientNonce)
        {
            var requestHttpMethod = Context.Request.Method;
            var requestUri = HttpUtility.UrlEncode(Context.Request.Scheme + "://" + Context.Request.Host + Context.Request.Path);
            return $"{serverApiKey}{requestHttpMethod}{requestUri}{clientTimeStamp}{clientNonce}";
        }

        private AuthenticationTicket PrepareAuthenticationTicket(string apiClientName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, apiClientName),
                new Claim(ClaimTypes.Role, "ClientApp")
            };

            var identities = new List<ClaimsIdentity> { new ClaimsIdentity(claims, Options.AuthenticationType) };
            var principal = new ClaimsPrincipal(identities);
            return new AuthenticationTicket(principal, Options.Scheme);
        }
    }
}
