using System;
using System.Collections.Generic;

namespace PVIMS.API.Models
{
    public class PatientClinicalEventForUpdateDto
    {
        /// <summary>
        /// A unique identifier that defines the identity of the patient at the point of registering the event (e.g. medical record number, case number etc.)
        /// </summary>
        public string PatientIdentifier { get; set; }

        /// <summary>
        /// The description of the source event
        /// </summary>
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unique identifier of the meddra term
        /// </summary>
        public int? SourceTerminologyMedDraId { get; set; }

        /// <summary>
        /// The onset date of the clinical event
        /// </summary>
        public DateTime OnsetDate { get; set; }

        /// <summary>
        /// The resolution date of the clinical event
        /// </summary>
        public DateTime? ResolutionDate { get; set; }

        /// <summary>
        /// Clinical event custom attributes
        /// </summary>
        public ICollection<AttributeValueForPostDto> Attributes { get; set; }
    }
}
