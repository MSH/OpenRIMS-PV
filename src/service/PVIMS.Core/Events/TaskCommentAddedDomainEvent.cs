using MediatR;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PViMS.Core.Events
{
    /// <summary>
    /// Event used when a new report instance task is added
    /// </summary>
    public class TaskCommentAddedDomainEvent : INotification
    {
        public ReportInstanceTaskComment Comment { get; }

        public TaskCommentAddedDomainEvent(ReportInstanceTaskComment comment)
        {
            Comment = comment;
        }
    }
}
