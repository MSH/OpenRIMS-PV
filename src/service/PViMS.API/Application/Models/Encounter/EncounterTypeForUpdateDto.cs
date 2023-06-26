using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class EncounterTypeForUpdateDto
    {
        /// <summary>
        /// The name of the encounter type
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EncounterTypeName { get; set; }

        /// <summary>
        /// A detailed definition of the encounter type
        /// </summary>
        [StringLength(250)]
        public string Help { get; set; }

        /// <summary>
        /// The name of the work plan
        /// </summary>
        [StringLength(50)]
        public string WorkPlanName { get; set; }
    }
}
