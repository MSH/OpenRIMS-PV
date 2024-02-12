using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Core.Events
{
    /// <summary>
    /// Event used when causality for a report has been confirmed for all medications
    /// </summary>
    public class CausalityConfirmedDomainEvent : INotification
    {
        public ReportInstance ReportInstance { get; }

        public CausalityConfirmedDomainEvent(ReportInstance reportInstance)
        {
            ReportInstance = reportInstance;
        }
    }
}
