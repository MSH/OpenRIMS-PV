using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A representation of all activity summary 
    /// </summary>
    [DataContract()]
    public class ActivitySummaryDto
    {
        /// <summary>
        /// Qualified name of the activity
        /// </summary>
        [DataMember]
        public string QualifiedName { get; set; }

        /// <summary>
        /// The number of reports currently in this state
        /// </summary>
        [DataMember]
        public int ReportInstanceCount { get; set; }

        /// <summary>
        /// The number of new feedback items in this state
        /// </summary>
        [DataMember]
        public int NewFeedbackInstanceCount { get; set; }
    }
}
