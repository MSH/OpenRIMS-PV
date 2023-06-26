using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.AppointmentAggregate
{
    [DataContract]
    public class AddAppointmentCommand
        : IRequest<AppointmentIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public DateTime AppointmentDate { get; private set; }

        [DataMember]
        public string Reason { get; private set; }

        public AddAppointmentCommand()
        {
        }

        public AddAppointmentCommand(int patientId, DateTime appointmentDate, string reason): this()
        {
            PatientId = patientId;
            AppointmentDate = appointmentDate;
            Reason = reason;
        }
    }
}
