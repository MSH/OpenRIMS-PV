using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta dependency representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaDependencyIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta dependency
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta dependency
        /// </summary>
        [DataMember]
        public Guid MetaDependencyGuid { get; set; }
    }
}
