using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A custom attribute representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class CustomAttributeDetailDto : CustomAttributeIdentifierDto
    {
        /// <summary>
        /// The category that the custom attribute belongs to
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Additional info for the attribute
        /// </summary>
        [DataMember]
        public string AttributeDetail { get; set; }

        /// <summary>
        /// The type of attribute  
        /// Valid options are string, selection, DateTime, Numeric
        /// </summary>
        [DataMember]
        public string CustomAttributeType { get; set; }

        /// <summary>
        /// Is this attribute mandatory
        /// </summary>
        [DataMember]
        public bool Required { get; set; }

        /// <summary>
        /// Is this attribute archived
        /// </summary>
        [DataMember]
        public bool Archived { get; set; }

        /// <summary>
        /// Maximum string length for string based attributes
        /// </summary>
        [DataMember]
        public int? StringMaxLength { get; set; }

        /// <summary>
        /// Minimum value in a min max range for numeric based attributes
        /// </summary>
        [DataMember]
        public int? NumericMinValue { get; set; }

        /// <summary>
        /// Maximum value in a min max range for numeric based attributes
        /// </summary>
        [DataMember]
        public int? NumericMaxValue { get; set; }


        /// <summary>
        /// Is this attribute searchable
        /// </summary>
        [DataMember]
        public bool IsSearchable { get; set; } = false;

        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<SelectionDataItemDto> SelectionDataItems { get; set; } = new List<SelectionDataItemDto>();
    }

}
