using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    [DataContract]
    public class ProductDetailQuery
        : IRequest<ProductDetailDto>
    {
        [DataMember]
        public int ProductId { get; private set; }

        public ProductDetailQuery()
        {
        }

        public ProductDetailQuery(int productId) : this()
        {
            ProductId = productId;
        }
    }
}
