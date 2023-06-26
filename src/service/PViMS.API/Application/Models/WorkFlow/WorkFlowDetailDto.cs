using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A work flow representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class WorkFlowDetailDto : WorkFlowIdentifierDto
    {
        /// <summary>
        /// The number of new reports for this work flow
        /// </summary>
        [DataMember]
        public int NewReportInstanceCount { get; set; }

        /// <summary>
        /// Activity summary for this work flow item for analysis purposes
        /// </summary>
        [DataMember]
        public ActivitySummaryDto[] AnalysisActivity { get; set; }

        /// <summary>
        /// Activity summary for this work flow item for feedback purposes
        /// </summary>
        [DataMember]
        public ActivitySummaryDto[] FeedbackActivity { get; set; }
    }
}
