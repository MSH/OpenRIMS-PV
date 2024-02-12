using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An org unit representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class OrgUnitIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the org unit
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the org unit
        /// </summary>
        [DataMember]
        public string OrgUnitName { get; set; }

        /// <summary>
        /// The type of the org unit
        /// </summary>
        [DataMember]
        public string OrgUnitType { get; set; }
    }
}
