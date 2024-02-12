using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Core.Events
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
