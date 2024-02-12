using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A work flow representation DTO - SUMMARY OF REPORTS SUBMITTED
    /// </summary>
    [DataContract()]
    public class WorkFlowSummaryDto : WorkFlowIdentifierDto
    {
        /// <summary>
        /// The total number of reports that have been submitted, including reports that have been deleted
        /// </summary>
        [DataMember]
        public int SubmissionCount { get; set; }

        /// <summary>
        /// The total number of reports that have been deleted
        /// </summary>
        [DataMember]
        public int DeletionCount { get; set; }

        /// <summary>
        /// The total number of reports that have been confirmed
        /// </summary>
        [DataMember]
        public int ReportDataConfirmedCount { get; set; }

        /// <summary>
        /// The total number of reports that have MedDRA and Causality completed and confirmed
        /// </summary>
        [DataMember]
        public int TerminologyAndCausalityConfirmedCount { get; set; }

        /// <summary>
        /// Summary by classification
        /// </summary>
        [DataMember]
        public IEnumerable<ClassificationSummaryDto> Classifications { get; set; } = new List<ClassificationSummaryDto>();
    }
}
