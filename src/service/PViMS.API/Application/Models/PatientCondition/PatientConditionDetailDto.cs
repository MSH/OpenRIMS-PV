using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient condition representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class PatientConditionDetailDto : PatientConditionIdentifierDto
    {
        /// <summary>
        /// The source description of the condition
        /// </summary>
        [DataMember]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unqiue identifier of the source meddra term
        /// </summary>
        [DataMember]
        public long SourceTerminologyMedDraId { get; set; }

        /// <summary>
        /// The source meddra term of the condition
        /// </summary>
        [DataMember]
        public string MedDraTerm { get; set; }

        /// <summary>
        /// The start date of the condition
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// The outcome date of the condition
        /// </summary>
        [DataMember]
        public string OutcomeDate { get; set; }

        /// <summary>
        /// The outcome of the condition
        /// </summary>
        [DataMember]
        public string Outcome { get; set; }

        /// <summary>
        /// The treatment outcome of the condition
        /// </summary>
        [DataMember]
        public string TreatmentOutcome { get; set; }

        /// <summary>
        /// The case number of the condition episode
        /// </summary>
        [DataMember]
        public string CaseNumber { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient condition
        /// </summary>
        [DataMember]
        public ICollection<AttributeValueDto> ConditionAttributes { get; set; } = new List<AttributeValueDto>();
    }
}
