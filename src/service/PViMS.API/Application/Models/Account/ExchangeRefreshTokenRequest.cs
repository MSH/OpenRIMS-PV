using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models.Account
{
    public class ExchangeRefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
