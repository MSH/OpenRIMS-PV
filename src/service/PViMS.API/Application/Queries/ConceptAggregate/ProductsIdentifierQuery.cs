using MediatR;
using PVIMS.API.Models;
using PVIMS.API.Models.ValueTypes;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    [DataContract]
    public class ProductsIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<ProductIdentifierDto>>
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

        public ProductsIdentifierQuery()
        {
        }

        public ProductsIdentifierQuery(string orderBy, string searchTerm, YesNoBothValueType active, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            SearchTerm = searchTerm;
            Active =  active;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
