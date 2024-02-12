using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ArchivePatientLabTestCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientLabTestId { get; private set; }

        [DataMember]
        public string Reason { get; private set; }


        public ArchivePatientLabTestCommand()
        {
        }

        public ArchivePatientLabTestCommand(int patientId, int patientLabTestId, string reason) : this()
        {
            PatientId = patientId;
            PatientLabTestId = patientLabTestId;
            Reason = reason;
        }
    }
}