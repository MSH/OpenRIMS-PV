using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientClinicalEventIdentifierQuery
        : IRequest<PatientClinicalEventIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientClinicalEventId { get; private set; }

        public PatientClinicalEventIdentifierQuery()
        {
        }

        public PatientClinicalEventIdentifierQuery(int patientId, int patientClinicalEventId) : this()
        {
            PatientId = patientId;
            PatientClinicalEventId = patientClinicalEventId;
        }
    }
}
