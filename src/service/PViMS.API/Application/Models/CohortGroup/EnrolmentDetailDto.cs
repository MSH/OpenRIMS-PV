using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A cohort group enrolment DTO
    /// </summary>
    [DataContract()]
    public class EnrolmentDetailDto : EnrolmentIdentifierDto
    {
        /// <summary>
        /// The full name of the patient
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// The current facility the patient is associated to
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        [DataMember]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// The age of the patient
        /// </summary>
        [DataMember]
        public int Age { get; set; }

        /// <summary>
        /// The latest encounter date of the patient
        /// </summary>
        [DataMember]
        public string LatestEncounterDate { get; set; }

        /// <summary>
        /// The current weight of the patient
        /// </summary>
        [DataMember]
        public decimal? CurrentWeight { get; set; }

        /// <summary>
        /// The number of non-serious events the patient has
        /// </summary>
        [DataMember]
        public int NonSeriousEventCount { get; set; }

        /// <summary>
        /// The number of serious events the patient has
        /// </summary>
        [DataMember]
        public int SeriousEventCount { get; set; }
    }
}
