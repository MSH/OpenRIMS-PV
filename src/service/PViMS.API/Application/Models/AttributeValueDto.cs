using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A attribute value representation containing the custom attribute values that have been assigned to a household or member
    /// </summary>
    [DataContract()]
    public class AttributeValueDto
    {
        /// <summary>
        /// The unique id of the attribute
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The category that the attribute key has been associated to
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// The attribute key for the custom attribute
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// The value of the attribute
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// The value of the attribute
        /// </summary>
        [DataMember]
        public string SelectionValue { get; set; }
    }
}
