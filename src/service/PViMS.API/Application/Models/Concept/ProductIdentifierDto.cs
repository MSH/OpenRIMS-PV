using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A product representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ProductIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the product
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// Product name and concept combined
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Is this product currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }
    }
}
