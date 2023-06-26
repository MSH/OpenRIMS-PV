using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta attribute representation DTO
    /// </summary>
    [DataContract()]
    public class MetaAttributeDto
    {
        /// <summary>
        /// The order of the attribute
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// The unique attribute name
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// Stratification aggregator applied to this column
        /// </summary>
        [DataMember]
        public string Aggregate { get; set; }

        /// <summary>
        /// The attribute display name
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
    }
}
