using System;
using System.Collections.Generic;

namespace PVIMS.API.Models
{
    public class PatientMedicationForUpdateDto
    {
        /// <summary>
        /// The description of the source medication
        /// </summary>
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unique identifier of the medication concept
        /// </summary>
        public int ConceptId { get; set; }

        /// <summary>
        /// The unique identifier of the medication product
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// The start date of the medication
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the medication
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The dose of the medication
        /// </summary>
        public string Dose { get; set; }

        /// <summary>
        /// The dose frequency of the medication
        /// </summary>
        public string DoseFrequency { get; set; }

        /// <summary>
        /// The unit of the dose
        /// </summary>
        public string DoseUnit { get; set; }

        /// <summary>
        /// Medication custom attributes
        /// </summary>
        public ICollection<AttributeValueForPostDto> Attributes { get; set; }
    }
}
