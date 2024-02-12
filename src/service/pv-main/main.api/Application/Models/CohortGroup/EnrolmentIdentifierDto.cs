using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A cohort group enrolment DTO
    /// </summary>
    [DataContract()]
    public class EnrolmentIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the cohort group enrolment
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id of the patient
        /// </summary>
        [DataMember]
        public int PatientId { get; set; }

        /// <summary>
        /// The unique id of the cohort group
        /// </summary>
        [DataMember]
        public int CohortGroupId { get; set; }

        /// <summary>
        /// The date the patient was enrolled
        /// </summary>
        [DataMember]
        public string EnroledDate { get; set; }

        /// <summary>
        /// The date the patient was de-enrolled
        /// </summary>
        [DataMember]
        public string DeenroledDate { get; set; }
    }
}
