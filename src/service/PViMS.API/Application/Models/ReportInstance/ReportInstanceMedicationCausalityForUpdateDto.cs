using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ReportInstanceMedicationCausalityForUpdateDto
    {
        /// <summary>
        /// The causality scale to be used
        /// </summary>
        [Required]
        [ValidEnumValue]
        public CausalityConfigType CausalityConfigType { get; set; }

        /// <summary>
        /// The causality status
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Causality { get; set; }
    }
}
