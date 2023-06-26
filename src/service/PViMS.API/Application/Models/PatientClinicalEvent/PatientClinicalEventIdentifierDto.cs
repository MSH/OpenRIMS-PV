using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient clinical event representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class PatientClinicalEventIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the patient clinical event
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the patient clinical event
        /// </summary>
        [DataMember]
        public Guid PatientClinicalEventGuid { get; set; }
    }
}
