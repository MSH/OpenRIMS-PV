using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A concept representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ConceptIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the concept
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the concept
        /// </summary>
        [DataMember]
        public string ConceptName { get; set; }

        /// <summary>
        /// Concept name and form
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Is this concept currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }
    }
}
