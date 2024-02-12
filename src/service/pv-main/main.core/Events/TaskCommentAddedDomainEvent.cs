using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Core.Events
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
