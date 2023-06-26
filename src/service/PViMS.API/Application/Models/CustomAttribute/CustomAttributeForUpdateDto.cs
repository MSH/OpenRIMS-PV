using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models
{
    public class CustomAttributeForUpdateDto
    {
        /// <summary>
        /// The extendable entity type that the attribute is being added to
        /// </summary>
        public string ExtendableTypeName { get; set; }

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
        public int? StringMaxLength { get; set; }

        /// <summary>
        /// For numeric based attributes, what is the minimum value of the attribute
        /// </summary>
        public int? NumericMinValue { get; set; }

        /// <summary>
        /// For numeric based attributes, what is the maximum value of the attribute
        /// </summary>
        public int? NumericMaxValue { get; set; }

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
