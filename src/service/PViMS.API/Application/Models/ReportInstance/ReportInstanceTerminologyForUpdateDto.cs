using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ReportInstanceTerminologyForUpdateDto
    {
        /// <summary>
        /// The unique identifier of the meddra term
        /// </summary>
        [Required]
        public int TerminologyMedDraId { get; set; }
    }
}
