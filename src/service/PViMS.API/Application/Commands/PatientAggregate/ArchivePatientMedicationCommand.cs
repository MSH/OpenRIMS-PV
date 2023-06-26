using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ArchivePatientMedicationCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientMedicationId { get; private set; }

        [DataMember]
        public string Reason { get; private set; }


        public ArchivePatientMedicationCommand()
        {
        }

        public ArchivePatientMedicationCommand(int patientId, int patientMedicationId, string reason) : this()
        {
            PatientId = patientId;
            PatientMedicationId = patientMedicationId;
            Reason = reason;
        }
    }
}
