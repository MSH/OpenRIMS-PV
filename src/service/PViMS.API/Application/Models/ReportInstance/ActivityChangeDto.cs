using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a change in activity status
    /// </summary>
    [DataContract()]
    public class ActivityChangeDto
    {
        /// <summary>
        /// Comments associated to the change in status
        /// </summary>
        [DataMember]
        public string Comments { get; set; }

        /// <summary>
        /// The current status of the activity
        /// </summary>
        [DataMember]
        public string CurrentExecutionStatus { get; set; }

        /// <summary>
        /// The new status of the activity
        /// </summary>
        [DataMember]
        public string NewExecutionStatus { get; set; }

        /// <summary>
        /// Code for submitting E2B
        /// </summary>
        [DataMember]
        public string ContextCode { get; set; }

        /// <summary>
        /// Date of E2B submission
        /// </summary>
        [DataMember]
        public DateTime? ContextDate { get; set; }
    }
}
