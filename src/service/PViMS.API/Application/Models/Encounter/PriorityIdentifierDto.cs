using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A priority representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class PriorityIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the priority
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the priority
        /// </summary>
        [DataMember]
        public string PriorityName { get; set; }
    }
}
