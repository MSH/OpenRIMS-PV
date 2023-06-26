using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.DatasetAggregate
{
    [DataContract]
    public class DatasetInstanceIdentifierQuery
        : IRequest<DatasetInstanceIdentifierDto>
    {
        [DataMember]
        public int DatasetId { get; private set; }

        [DataMember]
        public int DatasetInstanceId { get; private set; }

        public DatasetInstanceIdentifierQuery()
        {
        }

        public DatasetInstanceIdentifierQuery(int datasetId, int datasetInstanceId) : this()
        {
            DatasetId = datasetId;
            DatasetInstanceId = datasetInstanceId;
        }
    }
}
