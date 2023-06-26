using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient lab test representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class PatientLabTestIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the patient lab test
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the patient lab test
        /// </summary>
        [DataMember]
        public Guid PatientLabTestGuid { get; set; }
    }
}
