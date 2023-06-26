using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A care event representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class CareEventIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the care event
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the care evebt
        /// </summary>
        [DataMember]
        public string CareEventName { get; set; }
    }
}
