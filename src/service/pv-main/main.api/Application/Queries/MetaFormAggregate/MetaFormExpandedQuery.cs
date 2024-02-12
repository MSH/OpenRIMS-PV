using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.MetaFormAggregate
{
    [DataContract]
    public class MetaFormExpandedQuery
        : IRequest<MetaFormExpandedDto>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        public MetaFormExpandedQuery()
        {
        }

        public MetaFormExpandedQuery(int metaFormId) : this()
        {
            MetaFormId = metaFormId;
        }
    }
}