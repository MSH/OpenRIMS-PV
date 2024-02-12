using MediatR;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class ChangeReportMedicationCausalityCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int ReportInstanceMedicationId { get; private set; }

        [DataMember]
        public CausalityConfigType CausalityConfigType { get; set; }

        [DataMember]
        public string Causality { get; set; }

        public ChangeReportMedicationCausalityCommand()
        {
        }

        public ChangeReportMedicationCausalityCommand(Guid workFlowGuid, int reportInstanceId, int reportInstanceMedicationId, CausalityConfigType causalityConfigType, string causality) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            ReportInstanceMedicationId = reportInstanceMedicationId;
            CausalityConfigType = causalityConfigType;
            Causality = causality;
        }
    }
}
