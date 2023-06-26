using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    [DataContract]
    public class EncounterDetailQuery
        : IRequest<EncounterDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int EncounterId { get; private set; }

        public EncounterDetailQuery()
        {
        }

        public EncounterDetailQuery(int patientId, int encounterId) : this()
        {
            PatientId = patientId;
            EncounterId = encounterId;
        }
    }
}
