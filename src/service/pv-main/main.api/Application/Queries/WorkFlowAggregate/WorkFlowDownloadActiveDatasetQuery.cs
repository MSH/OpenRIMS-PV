using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    [DataContract]
    public class WorkFlowDownloadActiveDatasetQuery
        : IRequest<ArtifactDto>
    {
        [DataMember]
        public long CohortGroupId { get; private set; }

        public WorkFlowDownloadActiveDatasetQuery()
        {
        }

        public WorkFlowDownloadActiveDatasetQuery(long cohortGroupId) : this()
        {
            CohortGroupId = cohortGroupId;
        }
    }
}
