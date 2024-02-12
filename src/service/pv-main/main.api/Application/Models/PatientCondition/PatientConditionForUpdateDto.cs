using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class PatientConditionForUpdateDto
    {
        /// <summary>
        /// The description of the source condition
        /// </summary>
        [Required]
        [StringLength(200)]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unique identifier of the meddra term
        /// </summary>
        public int SourceTerminologyMedDraId { get; set; }

        /// <summary>
        /// The start date of the condition
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date of the condition outcome
        /// </summary>
        public DateTime? OutcomeDate { get; set; }

        /// <summary>
        /// The look up value for the condition outcome
        /// </summary>
        [StringLength(50)]
        public string Outcome { get; set; }

        /// <summary>
        /// The look up value for the treatment outcome
        /// </summary>
        [StringLength(50)]
        public string TreatmentOutcome { get; set; }

        /// <summary>
        /// Case number associated to the condition
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        /// Comments associated to the condition
        /// </summary>
        [StringLength(500)]
        public string Comments { get; set; }

        /// <summary>
        /// Condition custom attributes
        /// </summary>
        public IDictionary<int, string> Attributes { get; set; }
    }
}
