using OpenRIMS.PV.Main.API.Infrastructure.Auth;

namespace OpenRIMS.PV.Main.API.Models.Account
{
    public class ExchangeRefreshTokenResponseModel
    {
        public AccessToken AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
