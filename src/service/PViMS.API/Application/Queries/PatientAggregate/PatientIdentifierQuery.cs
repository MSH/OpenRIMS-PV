using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientIdentifierQuery
        : IRequest<PatientIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        public PatientIdentifierQuery()
        {
        }

        public PatientIdentifierQuery(int patientId) : this()
        {
            PatientId = patientId;
        }
    }
}
