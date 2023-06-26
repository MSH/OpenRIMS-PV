using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// List of dataset categories associated to a dataset instance
    /// </summary>
    [DataContract()]
    public class DatasetCategoryViewDto
    {
        /// <summary>
        /// The unique id of the dataset category
        /// </summary>
        [DataMember]
        public int DatasetCategoryId { get; set; }

        /// <summary>
        /// The name of the dataset category
        /// </summary>
        [DataMember]
        public string DatasetCategoryName { get; set; }

        /// <summary>
        /// The friendly name of the dataset category
        /// </summary>
        [DataMember]
        public string DatasetCategoryDisplayName { get; set; }

        /// <summary>
        /// Additional help for the dataset category
        /// </summary>
        [DataMember]
        public string DatasetCategoryHelp { get; set; }

        /// <summary>
        /// Should the category be displayed
        /// </summary>
        [DataMember]
        public bool DatasetCategoryDisplayed { get; set; }

        /// <summary>
        /// List of dataset elements and value associated to this category
        /// </summary>
        [DataMember]
        public DatasetElementViewDto[] DatasetElements { get; set; }
    }
}
