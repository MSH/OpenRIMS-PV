using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.Core.Models
{
    public class UserListItem
    {
        public long UserId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
    }
}
