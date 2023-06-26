using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.DatasetAggregate
{
    [DataContract]
    public class DatasetInstanceDetailQuery
        : IRequest<DatasetInstanceDetailDto>
    {
        [DataMember]
        public int DatasetId { get; private set; }

        [DataMember]
        public int DatasetInstanceId { get; private set; }

        public DatasetInstanceDetailQuery()
        {
        }

        public DatasetInstanceDetailQuery(int datasetId, int datasetInstanceId) : this()
        {
            DatasetId = datasetId;
            DatasetInstanceId = datasetInstanceId;
        }
    }
}
