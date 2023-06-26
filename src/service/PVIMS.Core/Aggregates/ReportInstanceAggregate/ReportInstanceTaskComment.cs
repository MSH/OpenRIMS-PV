using PVIMS.Core.Entities;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportInstanceTaskComment
        : AuditedEntityBase
	{
        public string Comment { get; private set; }
        public int ReportInstanceTaskId { get; private set; }

        public virtual ReportInstanceTask ReportInstanceTask { get; private set; }

        protected ReportInstanceTaskComment()
        {
        }

        public ReportInstanceTaskComment(string comment)
        {
            Comment = comment;
        }
    }
}