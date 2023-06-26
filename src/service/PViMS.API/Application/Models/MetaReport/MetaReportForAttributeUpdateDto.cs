using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class MetaReportForAttributeUpdateDto
    {
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

        /// <summary>
        /// The attributes associated to the report
        /// </summary>
        public MetaAttributeDto[] Attributes { get; set; }

        /// <summary>
        /// The filters associated to the report
        /// </summary>
        public MetaFilterDto[] Filters { get; set; }
    }
}
