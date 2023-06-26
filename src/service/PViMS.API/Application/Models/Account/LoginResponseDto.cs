using PVIMS.API.Infrastructure.Auth;

namespace PVIMS.API.Models
{
    public class LoginResponseDto
    {
        public LoginResponseDto(AccessToken accessToken, string refreshToken, bool eulaAcceptanceRequired, bool allowDatasetDownload)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            EulaAcceptanceRequired = eulaAcceptanceRequired;
            AllowDatasetDownload = allowDatasetDownload;
        }

        public LoginResponseDto()
        {

        }

        /// <summary>
        /// Access token response model
        /// </summary>
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// Login refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Indicate if the user is required to accept the end user license agreement
        /// </summary>
        public bool EulaAcceptanceRequired { get; set; }

        /// <summary>
        /// Indicate if the user has ability to download a spontaneous or active dataset
        /// </summary>
        public bool AllowDatasetDownload { get; set; }

    }
}
