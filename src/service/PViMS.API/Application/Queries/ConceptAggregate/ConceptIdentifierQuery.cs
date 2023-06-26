using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ConceptAggregate
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
