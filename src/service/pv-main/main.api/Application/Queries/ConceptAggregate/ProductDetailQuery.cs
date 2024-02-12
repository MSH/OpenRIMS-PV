using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ConceptAggregate
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
