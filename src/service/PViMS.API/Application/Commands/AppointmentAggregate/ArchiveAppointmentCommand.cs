using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.AppointmentAggregate
{
    [DataContract]
    public class ArchiveAppointmentCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int AppointmentId { get; private set; }

        [DataMember]
        public string Reason { get; private set; }

        public ArchiveAppointmentCommand()
        {
        }

        public ArchiveAppointmentCommand(int patientId, int appointmentId, string reason) : this()
        {
            PatientId = patientId;
            AppointmentId = appointmentId;
            Reason = reason;
        }
    }
}
