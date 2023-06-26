using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    public class DatasetInstanceDto
    {
        /// <summary>
        /// Dataset id
        /// </summary>
        [DataMember]
        public int DatasetId { get; set; }

        /// <summary>
        /// Dataset instance id
        /// </summary>
        [DataMember]
        public int DatasetInstanceId { get; set; }
    }
}
