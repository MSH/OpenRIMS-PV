using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta page representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaPageIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta page
        /// </summary>
        [DataMember]
        public int Id { get; set; }


        /// <summary>
        /// The globally unique identifier for the meta page
        /// </summary>
        [DataMember]
        public Guid MetaPageGuid { get; set; }

        /// <summary>
        /// The name of the meta page
        /// </summary>
        [DataMember]
        public string PageName { get; set; }
    }
}
