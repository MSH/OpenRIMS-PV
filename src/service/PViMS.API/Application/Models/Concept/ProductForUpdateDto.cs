using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models
{
    public class ProductForUpdateDto
    {
        /// <summary>
        /// The name of the concept
        /// </summary>
        public string ConceptName { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The name of the manufacturer
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// A general description of the product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is this product currently active
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }
    }
}
