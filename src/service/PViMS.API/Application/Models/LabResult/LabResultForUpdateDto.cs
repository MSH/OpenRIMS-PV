using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class LabResultForUpdateDto
    {
        /// <summary>
        /// The name of the lab result
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LabResultName { get; set; }

        /// <summary>
        /// Is the lab result active
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }
    }
}
