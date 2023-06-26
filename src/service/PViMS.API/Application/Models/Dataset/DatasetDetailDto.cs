using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dataset representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetDetailDto : DatasetIdentifierDto
    {
        /// <summary>
        /// Is this dataset active
        /// </summary>
        [DataMember]
        public string Active { get; set; }

        /// <summary>
        /// Is this a system defined dataset
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// A detailed definition of the dataset
        /// </summary>
        [DataMember]
        public string Help { get; set; }

        /// <summary>
        /// Details of the household creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the household
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }

        /// <summary>
        /// The contextual entity that the dataset is associated to
        /// </summary>
        [DataMember]
        public string ContextType { get; set; }
    }
}
