using MediatR;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PViMS.Core.Events
{
    /// <summary>
    /// Event used when a new report instance task is attended to by the reporter
    /// </summary>
    public class TaskAttendedToDomainEvent : INotification
    {
        public ReportInstanceTask Task { get; }

        public TaskAttendedToDomainEvent(ReportInstanceTask task)
        {
            Task = task;
        }
    }
}
