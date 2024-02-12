using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
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
