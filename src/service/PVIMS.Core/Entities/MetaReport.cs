using PVIMS.Core.ValueTypes;
using System;

namespace PVIMS.Core.Entities
{
    public class MetaReport : EntityBase
    {
        public MetaReport()
        {
            MetaReportGuid = Guid.NewGuid();
            Breadcrumb = "** NOT DEFINED **";
            ReportStatus = MetaReportStatus.Unpublished;
            IsSystem = false;
        }

        public Guid MetaReportGuid { get; set; }
        public string ReportName { get; set; }
        public string ReportDefinition { get; set; }
        public string MetaDefinition { get; set; }
        public string Breadcrumb { get; set; }
        public string SqlDefinition { get; set; }
        public bool IsSystem { get; set; }
        public MetaReportStatus ReportStatus { get; set; }
    }
}
