using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ConceptAggregate
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
