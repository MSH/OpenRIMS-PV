using Microsoft.AspNetCore.Authentication;
using PVIMS.API.Infrastructure.Auth;
using System;

namespace PVIMS.API.Infrastructure.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
        {
            return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyHandler>(ApiKeyAuthenticationOptions.DefaultScheme, options);
        }
    }
}
