using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A custom attribute representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class CustomAttributeIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the custom attribute
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The extendable type name of the custom attribute (attribute owner)
        /// </summary>
        [DataMember]
        public string ExtendableTypeName { get; set; }

        /// <summary>
        /// The unique name of the custom attribute
        /// </summary>
        [DataMember]
        public string AttributeKey { get; set; }
    }
}
