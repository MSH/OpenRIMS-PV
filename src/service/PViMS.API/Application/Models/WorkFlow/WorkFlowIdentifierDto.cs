using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A work flow representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class WorkFlowIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the work flow
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the work flow
        /// </summary>
        [DataMember]
        public Guid WorkFlowGuid { get; set; }

        /// <summary>
        /// The name of the work flow
        /// </summary>
        [DataMember]
        public string WorkFlowName { get; set; }
    }
}
