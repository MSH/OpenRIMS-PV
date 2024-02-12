using Microsoft.AspNetCore.Authentication;

namespace OpenRIMS.PV.Main.API.Infrastructure.Auth
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";
        public const string ApiKey = "";
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}
