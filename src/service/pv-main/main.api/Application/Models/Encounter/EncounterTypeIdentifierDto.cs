using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An encounter type representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class EncounterTypeIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the encounter type
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the dataset
        /// </summary>
        [DataMember]
        public string EncounterTypeName { get; set; }
    }
}
