using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ConceptAggregate
{
    [DataContract]
    public class ConceptIdentifierQuery
        : IRequest<ConceptIdentifierDto>
    {
        [DataMember]
        public int ConceptId { get; private set; }

        public ConceptIdentifierQuery()
        {
        }

        public ConceptIdentifierQuery(int conceptId) : this()
        {
            ConceptId = conceptId;
        }
    }
}
