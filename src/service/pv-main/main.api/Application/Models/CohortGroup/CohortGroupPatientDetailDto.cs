using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A cohort group enrolment representation for a patient DTO
    /// </summary>
    [DataContract()]
    public class CohortGroupPatientDetailDto : CohortGroupIdentifierDto
    {
        /// <summary>
        /// The condition that links the patient to the condition group
        /// </summary>
        [DataMember]
        public string Condition { get; set; }

        /// <summary>
        /// The start date for the patient's condition in this cohort
        /// </summary>
        [DataMember]
        public string ConditionStartDate { get; set; }

        /// <summary>
        /// The corresponding enrolment record for this patient into this cohort
        /// </summary>
        [DataMember]
        public EnrolmentIdentifierDto CohortGroupEnrolment { get; set; }
    }
}
