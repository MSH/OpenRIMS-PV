using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A cohort group representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class CohortGroupDetailDto : CohortGroupIdentifierDto
    {
        /// <summary>
        /// The finish date of the cohort
        /// </summary>
        [DataMember]
        public string FinishDate { get; set; }

        /// <summary>
        /// The condition that the cohort group has been created for
        /// </summary>
        [DataMember]
        public string ConditionName { get; set; }

        /// <summary>
        /// The number of enrolments in this cohort
        /// </summary>
        [DataMember]
        public int EnrolmentCount { get; set; }
    }
}
