using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// List of dataset elements associated to a dataset category
    /// </summary>
    [DataContract()]
    public class DatasetElementSubViewDto
    {
        /// <summary>
        /// The unique id of the dataset sub element
        /// </summary>
        [DataMember]
        public int DatasetElementSubId { get; set; }

        /// <summary>
        /// The name of the dataset sub element
        /// </summary>
        [DataMember]
        public string DatasetElementSubName { get; set; }

        /// <summary>
        /// The name of the dataset sub element as it should be displayed
        /// </summary>
        [DataMember]
        public string DatasetElementSubDisplayName { get; set; }

        /// <summary>
        /// Additional help text that is associated to the element sub
        /// </summary>
        [DataMember]
        public string DatasetElementSubHelp { get; set; }

        /// <summary>
        /// The type of sub element
        /// </summary>
        [DataMember]
        public string DatasetElementSubType { get; set; }

        /// <summary>
        /// Is this a system sub element
        /// </summary>
        [DataMember]
        public bool DatasetElementSubSystem { get; set; }

        /// <summary>
        /// Is this sub element mandatory
        /// </summary>
        [DataMember]
        public bool Required { get; set; }

        /// <summary>
        /// Maximum string length for string based attributes
        /// </summary>
        [DataMember]
        public int? StringMaxLength { get; set; }

        /// <summary>
        /// Minimum value in a min max range for numeric based elements
        /// </summary>
        [DataMember]
        public decimal? NumericMinValue { get; set; }

        /// <summary>
        /// Maximum value in a min max range for numeric based elements
        /// </summary>
        [DataMember]
        public decimal? NumericMaxValue { get; set; }

        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<SelectionDataItemDto> SelectionDataItems { get; set; } = new List<SelectionDataItemDto>();
    }
}
