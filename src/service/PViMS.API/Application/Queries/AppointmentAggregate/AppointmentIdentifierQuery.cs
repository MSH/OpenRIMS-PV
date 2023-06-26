using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    [DataContract]
    public class AppointmentIdentifierQuery
        : IRequest<AppointmentIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int AppointmentId { get; private set; }

        public AppointmentIdentifierQuery()
        {
        }

        public AppointmentIdentifierQuery(int patientId, int appointmentId): this()
        {
            PatientId = patientId;
            AppointmentId = appointmentId;
        }
    }
}
