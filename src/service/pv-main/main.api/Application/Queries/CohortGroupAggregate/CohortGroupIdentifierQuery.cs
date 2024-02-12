using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate
{
    [DataContract]
    public class CohortGroupIdentifierQuery
        : IRequest<CohortGroupIdentifierDto>
    {
        [DataMember]
        public int CohortGroupId { get; private set; }

        public CohortGroupIdentifierQuery()
        {
        }

        public CohortGroupIdentifierQuery(int cohortGroupId) : this()
        {
            CohortGroupId = cohortGroupId;
        }
    }
}
