using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    [DataContract]
    public class ProductIdentifierQuery
        : IRequest<ProductIdentifierDto>
    {
        [DataMember]
        public int ProductId { get; private set; }

        public ProductIdentifierQuery()
        {
        }

        public ProductIdentifierQuery(int productId) : this()
        {
            ProductId = productId;
        }
    }
}
