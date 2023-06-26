using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A condition representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ConditionIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the condition
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the condition
        /// </summary>
        [DataMember]
        public string ConditionName { get; set; }

        /// <summary>
        /// Is this condition currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }
    }
}
