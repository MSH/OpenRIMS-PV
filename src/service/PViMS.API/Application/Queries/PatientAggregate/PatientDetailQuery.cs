using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
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
