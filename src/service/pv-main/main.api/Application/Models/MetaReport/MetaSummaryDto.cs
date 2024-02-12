using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta summary representation DTO
    /// </summary>
    [DataContract()]
    public class MetaSummaryDto
    {
        /// <summary>
        /// The last time meta data has been refreshed
        /// </summary>
        [DataMember]
        public string LatestRefreshDate { get; set; }

        /// <summary>
        /// Total number of patient records included in meta refresh
        /// </summary>
        [DataMember]
        public int PatientCount { get; set; }
    }
}
