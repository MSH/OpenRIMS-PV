using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient condition representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class PatientConditionIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the patient condition
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the patient condition
        /// </summary>
        [DataMember]
        public Guid PatientConditionGuid { get; set; }
    }
}
