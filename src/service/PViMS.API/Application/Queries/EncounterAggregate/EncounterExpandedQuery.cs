using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    [DataContract]
    public class EncounterExpandedQuery
        : IRequest<EncounterExpandedDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int EncounterId { get; private set; }

        public EncounterExpandedQuery()
        {
        }

        public EncounterExpandedQuery(int patientId, int encounterId) : this()
        {
            PatientId = patientId;
            EncounterId = encounterId;
        }
    }
}
