using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.CohortGroupAggregate
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
