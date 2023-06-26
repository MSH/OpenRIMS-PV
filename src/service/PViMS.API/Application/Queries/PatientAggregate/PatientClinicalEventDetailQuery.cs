using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientClinicalEventDetailQuery
        : IRequest<PatientClinicalEventDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientClinicalEventId { get; private set; }

        public PatientClinicalEventDetailQuery()
        {
        }

        public PatientClinicalEventDetailQuery(int patientId, int patientClinicalEventId) : this()
        {
            PatientId = patientId;
            PatientClinicalEventId = patientClinicalEventId;
        }
    }
}
