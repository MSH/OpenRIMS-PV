using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.CohortGroupAggregate
{
    [DataContract]
    public class CohortGroupDetailQuery
        : IRequest<CohortGroupDetailDto>
    {
        [DataMember]
        public int CohortGroupId { get; private set; }

        public CohortGroupDetailQuery()
        {
        }

        public CohortGroupDetailQuery(int cohortGroupId) : this()
        {
            CohortGroupId = cohortGroupId;
        }
    }
}
