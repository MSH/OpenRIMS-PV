using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta widget representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaWidgetIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta widget
        /// </summary>
        [DataMember]
        public int Id { get; set; }


        /// <summary>
        /// The globally unique identifier for the meta widget
        /// </summary>
        [DataMember]
        public Guid MetaWidgetGuid { get; set; }

        /// <summary>
        /// The name of the meta widget
        /// </summary>
        [DataMember]
        public string WidgetName { get; set; }
    }
}
