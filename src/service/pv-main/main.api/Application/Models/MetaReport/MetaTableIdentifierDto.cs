using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta table representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaTableIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta table
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta table
        /// </summary>
        [DataMember]
        public Guid MetaTableGuid { get; set; }
    }
}
