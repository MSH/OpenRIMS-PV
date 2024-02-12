using MediatR;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.ValueTypes;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ConceptAggregate
{
    [DataContract]
    public class ProductsDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<ProductDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public string SearchTerm { get; private set; }

        [DataMember]
        public YesNoBothValueType Active { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public ProductsDetailQuery()
        {
        }

        public ProductsDetailQuery(string orderBy, string searchTerm, YesNoBothValueType active, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            SearchTerm = searchTerm;
            Active =  active;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
