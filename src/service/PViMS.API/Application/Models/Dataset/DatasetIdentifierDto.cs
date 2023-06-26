using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dataset representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class DatasetIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dataset
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the dataset
        /// </summary>
        [DataMember]
        public string DatasetName { get; set; }
    }
}
