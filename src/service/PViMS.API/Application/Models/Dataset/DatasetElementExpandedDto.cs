using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dataset element representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetElementExpandedDto : DatasetElementDetailDto
    {
        /// <summary>
        /// E2B OID
        /// </summary>
        [DataMember]
        public string SingleDatasetRule { get; set; }
    }
}
