using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A smmmary breakdown by classification
    /// </summary>
    [DataContract()]
    public class ClassificationSummaryDto
    {
        /// <summary>
        /// The classification of the report (SAE, AESI, Clinically Significant)
        /// </summary>
        [DataMember]
        public string Classification { get; set; }

        /// <summary>
        /// The total number of reports with this classification
        /// </summary>
        [DataMember]
        public int ClassificationCount { get; set; }

        /// <summary>
        /// The total number of reports that have at least one drug marked as causative by either of the causality scales
        /// </summary>
        [DataMember]
        public int CausativeCount { get; set; }

        /// <summary>
        /// The total number of reports that have an E2B specification file generated
        /// </summary>
        [DataMember]
        public int E2BCount { get; set; }
    }
}
