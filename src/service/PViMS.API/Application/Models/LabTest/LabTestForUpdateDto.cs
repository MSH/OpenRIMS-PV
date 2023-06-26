using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class LabTestForUpdateDto
    {
        /// <summary>
        /// The name of the lab test
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LabTestName { get; set; }

        /// <summary>
        /// Is the lab test active
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }
    }
}
