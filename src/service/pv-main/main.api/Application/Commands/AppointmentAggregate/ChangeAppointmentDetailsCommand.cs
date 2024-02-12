using MediatR;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.AppointmentAggregate
{
    [DataContract]
    public class ChangeAppointmentDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int AppointmentId { get; private set; }

        [DataMember]
        public DateTime AppointmentDate { get; private set; }

        [DataMember]
        public string Reason { get; private set; }

        [DataMember]
        public bool Cancelled { get; private set; }

        [DataMember]
        public string CancellationReason { get; private set; }

        public ChangeAppointmentDetailsCommand()
        {
        }

        public ChangeAppointmentDetailsCommand(int patientId, int appointmentId, DateTime appointmentDate, string reason, bool cancelled, string cancellationReason): this()
        {
            PatientId = patientId;
            AppointmentId = appointmentId;
            AppointmentDate = appointmentDate;
            Reason = reason;
            Cancelled = cancelled;
            CancellationReason = cancellationReason;
        }
    }
}
