using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient clinical event representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class PatientClinicalEventDetailDto : PatientClinicalEventIdentifierDto
    {
        /// <summary>
        /// The source description of the clinical event
        /// </summary>
        [DataMember]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unqiue identifier of the source meddra term
        /// </summary>
        [DataMember]
        public int? SourceTerminologyMedDraId { get; set; }

        /// <summary>
        /// The source meddra term of the clinical event
        /// </summary>
        [DataMember]
        public string MedDraTerm { get; set; }

        /// <summary>
        /// The onset date of the event
        /// </summary>
        [DataMember]
        public string OnsetDate { get; set; }

        /// <summary>
        /// The resolution date of the event
        /// </summary>
        [DataMember]
        public string ResolutionDate { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<AttributeValueDto> ClinicalEventAttributes { get; set; } = new List<AttributeValueDto>();

        /// <summary>
        /// Custom attribute - date of the report
        /// </summary>
        [DataMember]
        public string ReportDate { get; set; }

        /// <summary>
        /// Is the report serious
        /// </summary>
        [DataMember]
        public string IsSerious { get; set; }
    }
}
