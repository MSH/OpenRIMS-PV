using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A product representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ProductDetailDto : ProductIdentifierDto
    {
        /// <summary>
        /// Full concept name
        /// </summary>
        [DataMember]
        public string ConceptDisplayName { get; set; }

        /// <summary>
        /// The concept the product has implemented
        /// </summary>
        [DataMember]
        public string ConceptName { get; set; }

        /// <summary>
        /// The strength of the concept
        /// </summary>
        [DataMember]
        public string Strength { get; set; }

        /// <summary>
        /// The form of the concept
        /// </summary>
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// The manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer { get; set; }
    }
}
