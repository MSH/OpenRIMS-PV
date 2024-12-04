using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    [DataContract]
    public class WorkFlowDownloadSpontaneousDatasetQuery
        : IRequest<ArtifactDto>
    {
        public WorkFlowDownloadSpontaneousDatasetQuery()
        {
        }
    }
}
