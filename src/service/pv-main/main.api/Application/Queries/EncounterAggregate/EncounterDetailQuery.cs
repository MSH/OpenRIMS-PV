using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.EncounterAggregate
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
