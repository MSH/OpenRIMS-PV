using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A facility representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class FacilityIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the facility
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the facility
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }
    }
}
