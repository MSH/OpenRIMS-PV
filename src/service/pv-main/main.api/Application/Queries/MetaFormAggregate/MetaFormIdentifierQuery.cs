using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.MetaFormAggregate
{
    [DataContract]
    public class MetaFormIdentifierQuery
        : IRequest<MetaFormIdentifierDto>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        public MetaFormIdentifierQuery()
        {
        }

        public MetaFormIdentifierQuery(int metaFormId) : this()
        {
            MetaFormId = metaFormId;
        }
    }
}