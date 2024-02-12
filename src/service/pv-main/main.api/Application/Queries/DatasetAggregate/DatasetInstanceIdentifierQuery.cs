using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.DatasetAggregate
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
