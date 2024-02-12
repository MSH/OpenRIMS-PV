using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientDetailQuery
        : IRequest<PatientDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        public PatientDetailQuery()
        {
        }

        public PatientDetailQuery(int patientId) : this()
        {
            PatientId = patientId;
        }
    }
}
