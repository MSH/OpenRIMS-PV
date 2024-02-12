using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A attribute value representation containing the custom attribute values that have been assigned to a household or member
    /// </summary>
    [DataContract()]
    public class AttributeValueForPostDto
    {
        /// <summary>
        /// The unique id of the attribute
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The value of the attribute
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
