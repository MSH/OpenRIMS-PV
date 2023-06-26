using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models
{
    public class CustomAttributeForCreationDto
    {
        /// <summary>
        /// The extendable entity type that the attribute is being added to
        /// </summary>
        public ExtendableTypeNames ExtendableTypeName { get; set; }

        /// <summary>
        /// The type of attribute that is being added
        /// </summary>
        public CustomAttributeTypes CustomAttributeType { get; set; }

        /// <summary>
        /// The category that the attribute should be grouped into
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The unique name of the attribute
        /// </summary>
        public string AttributeKey { get; set; }

        /// <summary>
        /// Any additional detail for the attribute
        /// </summary>
        public string AttributeDetail { get; set; }

        /// <summary>
        /// Is this attribute mandatory
        /// </summary>
        public YesNoValueType IsRequired { get; set; }

        /// <summary>
        /// For string based attributes, what is the maximum length of the attribute
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// For numeric based attributes, what is the minimum value of the attribute
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// For numeric based attributes, what is the maximum value of the attribute
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// For date based attributes, must the value be in the future
        /// </summary>
        public YesNoValueType FutureDateOnly { get; set; }

        /// <summary>
        /// For date based attributes, must the value be in the past
        /// </summary>
        public YesNoValueType PastDateOnly { get; set; }

        /// <summary>
        /// Can this attribute be searched on
        /// </summary>
        public YesNoValueType IsSearchable { get; set; }
    }
}
