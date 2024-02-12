using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// List of dataset categories associated to a dataset instance
    /// </summary>
    [DataContract()]
    public class GroupValueDto
    {
        /// <summary>
        /// The value of the dataset element being grouped
        /// </summary>
        [DataMember]
        public string GroupValue { get; set; }

        /// <summary>
        /// The number of records grouped
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
