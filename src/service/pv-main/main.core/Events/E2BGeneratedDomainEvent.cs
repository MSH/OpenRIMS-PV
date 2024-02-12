using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Core.Events
{
    /// <summary>
    /// Event used when an E2B XML file is generated
    /// </summary>
    public class E2BGeneratedDomainEvent : INotification
    {
        public ReportInstance ReportInstance { get; }

        public E2BGeneratedDomainEvent(ReportInstance reportInstance)
        {
            ReportInstance = reportInstance;
        }
    }
}
