using MediatR;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PViMS.Core.Events
{
    /// <summary>
    /// Event used when an E2B XML file is submitted
    /// </summary>
    public class E2BSubmittedDomainEvent : INotification
    {
        public ReportInstance ReportInstance { get; }

        public E2BSubmittedDomainEvent(ReportInstance reportInstance)
        {
            ReportInstance = reportInstance;
        }
    }
}
