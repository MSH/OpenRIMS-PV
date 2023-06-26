using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// List of dataset elements associated to a dataset category
    /// </summary>
    [DataContract()]
    public class DatasetElementViewDto
    {
        /// <summary>
        /// The unique id of the dataset element
        /// </summary>
        [DataMember]
        public int DatasetElementId { get; set; }

        /// <summary>
        /// The name of the dataset element
        /// </summary>
        [DataMember]
        public string DatasetElementName { get; set; }

        /// <summary>
        /// The name of the dataset element as it should be displayed
        /// </summary>
        [DataMember]
        public string DatasetElementDisplayName { get; set; }

        /// <summary>
        /// Additional help text that is associated to the element
        /// </summary>
        [DataMember]
        public string DatasetElementHelp { get; set; }

        /// <summary>
        /// The type of element
        /// </summary>
        [DataMember]
        public string DatasetElementType { get; set; }

        /// <summary>
        /// Current value of the element
        /// </summary>
        [DataMember]
        public string DatasetElementValue { get; set; }

        /// <summary>
        /// Should the element be displayed
        /// </summary>
        [DataMember]
        public bool DatasetElementDisplayed { get; set; }

        /// <summary>
        /// Is this a chronic element
        /// </summary>
        [DataMember]
        public bool DatasetElementChronic { get; set; }

        /// <summary>
        /// Is this a system element
        /// </summary>
        [DataMember]
        public bool DatasetElementSystem { get; set; }

        /// <summary>
        /// Is this element mandatory
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
        /// List of dataset sub elements
        /// </summary>
        [DataMember]
        public DatasetElementSubViewDto[] DatasetElementSubs { get; set; }

        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<SelectionDataItemDto> SelectionDataItems { get; set; } = new List<SelectionDataItemDto>();
    }
}
