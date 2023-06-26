using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class UserForPasswordUpdateDto
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
