using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Core.Events
{
    /// <summary>
    /// Event used when a new report instance task is added
    /// </summary>
    public class TaskAddedDomainEvent : INotification
    {
        public ReportInstanceTask Task { get; }

        public TaskAddedDomainEvent(ReportInstanceTask task)
        {
            Task = task;
        }
    }
}
