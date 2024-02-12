using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta form representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaFormIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta form
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the meta form
        /// </summary>
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta form
        /// </summary>
        [DataMember]
        public Guid MetaFormGuid { get; set; }

    }
}
