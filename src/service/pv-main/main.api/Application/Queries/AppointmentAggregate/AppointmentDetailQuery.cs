using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate
{
    [DataContract]
    public class AppointmentDetailQuery
        : IRequest<AppointmentDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int AppointmentId { get; private set; }

        public AppointmentDetailQuery()
        {
        }

        public AppointmentDetailQuery(int patientId, int appointmentId): this()
        {
            PatientId = patientId;
            AppointmentId = appointmentId;
        }
    }
}
