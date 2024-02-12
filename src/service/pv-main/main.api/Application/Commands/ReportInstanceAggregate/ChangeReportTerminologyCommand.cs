using MediatR;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class ChangeReportTerminologyCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int TerminologyMedDraId { get; private set; }

        public ChangeReportTerminologyCommand()
        {
        }

        public ChangeReportTerminologyCommand(Guid workFlowGuid, int reportInstanceId, int terminologyMedDraId) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            TerminologyMedDraId = terminologyMedDraId;
        }
    }
}
