using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta column representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaColumnIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta column
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta column
        /// </summary>
        [DataMember]
        public Guid MetaColumnGuid { get; set; }
    }
}
