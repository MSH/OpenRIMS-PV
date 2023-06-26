using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class CareEventForUpdateDto
    {
        /// <summary>
        /// The name of the care event
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CareEventName { get; set; }
    }
}
