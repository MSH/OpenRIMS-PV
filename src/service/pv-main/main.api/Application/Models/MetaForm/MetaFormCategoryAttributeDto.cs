using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta form element representation containing a list of elements available in a form category
    /// </summary>
    [DataContract()]
    public class MetaFormCategoryAttributeDto
    {
        /// <summary>
        /// The unique id of the category attribute
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta form category attribute
        /// </summary>
        [DataMember]
        public Guid? MetaFormCategoryAttributeGuid { get; set; }

        /// <summary>
        /// The unique id of the custom attribute
        /// </summary>
        [DataMember]
        public int? AttributeId { get; set; }

        /// <summary>
        /// The name of the custom attribute
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// Form label associated to the category attribute
        /// </summary>
        [DataMember]
        public string Label { get; set; }

        /// <summary>
        /// Additional help associated to the category attribute
        /// </summary>
        [DataMember]
        public string Help { get; set; }

        /// <summary>
        /// Is the attribute a placeholder or has it been configured previously
        /// </summary>
        [DataMember]
        public bool Selected { get; set; }

        /// <summary>
        /// The type of attribute  
        /// Valid options are string, selection, DateTime, Numeric
        /// </summary>
        [DataMember]
        public string FormAttributeType { get; set; }

        /// <summary>
        /// Is this attribute mandatory
        /// </summary>
        [DataMember]
        public bool Required { get; set; }

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
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<SelectionDataItemDto> SelectionDataItems { get; set; } = new List<SelectionDataItemDto>();
    }
}
