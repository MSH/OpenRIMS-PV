using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate
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
