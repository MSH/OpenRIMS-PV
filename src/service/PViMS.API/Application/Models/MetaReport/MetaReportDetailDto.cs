using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta report representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaReportDetailDto : MetaReportIdentifierDto
    {
        /// <summary>
        /// The definition of the report
        /// </summary>
        [DataMember]
        public string ReportDefinition { get; set; }

        /// <summary>
        /// The meta definition of the report
        /// </summary>
        [DataMember]
        public string MetaDefinition { get; set; }

        /// <summary>
        /// The page breadcrumb
        /// </summary>
        [DataMember]
        public string Breadcrumb { get; set; }

        /// <summary>
        /// Is this a system report
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// Report status: Published or unpublished
        /// </summary>
        [DataMember]
        public string ReportStatus { get; set; }

        /// <summary>
        /// The type of report: List or summary
        /// </summary>
        [DataMember]
        public string ReportType { get; set; }

        /// <summary>
        /// The core entity for the report
        /// </summary>
        [DataMember]
        public string CoreEntity { get; set; }
    }
}
