using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A facility type representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class FacilityTypeIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the facility type
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The typer of the facility
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }
    }
}
