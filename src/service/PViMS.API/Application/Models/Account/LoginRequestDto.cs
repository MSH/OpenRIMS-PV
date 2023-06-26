using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class LoginRequestDto
    {
        /// <summary>
        /// Login username
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Login password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
