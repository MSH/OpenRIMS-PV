using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset category representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetCategoryDetailDto : DatasetCategoryIdentifierDto
    {
        /// <summary>
        /// The order of the category in relation to the dataset
        /// </summary>
        [DataMember]
        public short CategoryOrder { get; set; }

        /// <summary>
        /// Is this a system defined category
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// Is this a category used for acute data collection
        /// </summary>
        [DataMember]
        public string Acute { get; set; }

        /// <summary>
        /// Is this a category used for chronic data collection
        /// </summary>
        [DataMember]
        public string Chronic { get; set; }

        /// <summary>
        /// Number of elements within this category
        /// </summary>
        [DataMember]
        public short ElementCount { get; set; }

        /// <summary>
        /// The friendly name of the dataset category
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Additional help for the dataset category
        /// </summary>
        [DataMember]
        public string Help { get; set; }
    }
}
