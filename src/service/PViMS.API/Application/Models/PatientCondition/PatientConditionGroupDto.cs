using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a list of condition groups a patient belongs to
    /// </summary>
    [DataContract()]
    public class PatientConditionGroupDto
    {
        /// <summary>
        /// The condition group that the patient belongs to
        /// </summary>
        [DataMember]
        public string ConditionGroup { get; set; }

        /// <summary>
        /// The current status of the condition
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Addition details to be displayed in patient view
        /// </summary>
        [DataMember]
        public string Detail { get; set; }

        /// <summary>
        /// Condition start date
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// The unique ID of the patient condition
        /// </summary>
        [DataMember]
        public int PatientConditionId { get; set; }
    }
}
