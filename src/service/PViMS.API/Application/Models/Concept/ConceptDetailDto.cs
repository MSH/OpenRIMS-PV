using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A concept representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ConceptDetailDto : ConceptIdentifierDto
    {
        /// <summary>
        /// The form of the concept
        /// </summary>
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// The strength of the concept
        /// </summary>
        [DataMember]
        public string Strength { get; set; }
    }
}
