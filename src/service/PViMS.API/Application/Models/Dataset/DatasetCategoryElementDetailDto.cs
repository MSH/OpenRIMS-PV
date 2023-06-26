using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dataset category element representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetCategoryElementDetailDto : DatasetCategoryElementIdentifierDto
    {
        /// <summary>
        /// The order of the field in relation to the category
        /// </summary>
        [DataMember]
        public short FieldOrder { get; set; }

        /// <summary>
        /// Is this a system defined element
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// Is this element used for acute data collection
        /// </summary>
        [DataMember]
        public string Acute { get; set; }

        /// <summary>
        /// Is this element used for chronic data collection
        /// </summary>
        [DataMember]
        public string Chronic { get; set; }

        /// <summary>
        /// The friendly name of the dataset category element
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Additional help for the dataset category element
        /// </summary>
        [DataMember]
        public string Help { get; set; }
    }
}
