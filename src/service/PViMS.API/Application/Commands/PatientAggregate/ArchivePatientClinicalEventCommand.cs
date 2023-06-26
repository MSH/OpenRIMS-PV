using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ArchivePatientClinicalEventCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientClinicalEventId { get; private set; }

        [DataMember]
        public string Reason { get; private set; }


        public ArchivePatientClinicalEventCommand()
        {
        }

        public ArchivePatientClinicalEventCommand(int patientId, int patientClinicalEventId, string reason) : this()
        {
            PatientId = patientId;
            PatientClinicalEventId = patientClinicalEventId;
            Reason = reason;
        }
    }
}
