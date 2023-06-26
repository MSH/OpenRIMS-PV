using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class MetaReportForUpdateDto
    {
        /// <summary>
        /// The name of the meta report
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ReportName { get; set; }

        /// <summary>
        /// What the purpose of the report is
        /// </summary>
        [StringLength(250)]
        public string ReportDefinition { get; set; }

        /// <summary>
        /// Report configuration details. Currently not in use
        /// </summary>
        public string MetaDefinition { get; set; }

        /// <summary>
        /// Report breadcrumb. Currently not in use
        /// </summary>
        [StringLength(250)]
        public string Breadcrumb { get; set; }

        /// <summary>
        /// Is the report in a published or unpublished state
        /// </summary>
        [ValidEnumValue]
        public MetaReportStatus ReportStatus { get; set; }

        /// <summary>
        /// Is the report a list or summary based report
        /// </summary>
        [ValidEnumValue]
        public MetaReportTypes ReportType { get; set; }

        /// <summary>
        /// The core entity of the report
        /// </summary>
        [StringLength(50)]
        [Required]
        public string CoreEntity { get; set; }

    }
}
