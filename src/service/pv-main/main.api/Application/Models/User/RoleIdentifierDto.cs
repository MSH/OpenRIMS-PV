using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A role representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class RoleIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the role
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
