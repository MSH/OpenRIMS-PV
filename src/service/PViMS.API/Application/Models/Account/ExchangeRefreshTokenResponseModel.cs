using PVIMS.API.Infrastructure.Auth;

namespace PVIMS.API.Models.Account
{
    public class ExchangeRefreshTokenResponseModel
    {
        public AccessToken AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
