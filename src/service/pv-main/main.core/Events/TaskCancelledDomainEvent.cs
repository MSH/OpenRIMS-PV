using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Core.Events
{
    /// <summary>
    /// Event used when a new report instance task is cancelled
    /// </summary>
    public class TaskCancelledDomainEvent : INotification
    {
        public ReportInstanceTask Task { get; }

        public TaskCancelledDomainEvent(ReportInstanceTask task)
        {
            Task = task;
        }
    }
}
