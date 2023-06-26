using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientConditionDetailQuery
        : IRequest<PatientConditionDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientConditionId { get; private set; }

        public PatientConditionDetailQuery()
        {
        }

        public PatientConditionDetailQuery(int patientId, int patientConditionId) : this()
        {
            PatientId = patientId;
            PatientConditionId = patientConditionId;
        }
    }
}
