using MediatR;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.CustomAttributeAggregate
{
    [DataContract]
    public class CustomAttributesDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public ExtendableTypeNames ExtendableTypeName { get; private set; }

        [DataMember]
        public CustomAttributeTypes CustomAttributeType { get; private set; }

        [DataMember]
        public bool? IsSearchable { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public CustomAttributesDetailQuery()
        {
        }

        public CustomAttributesDetailQuery(string orderBy, ExtendableTypeNames extendableTypeName, CustomAttributeTypes customAttributeType, bool? isSearchable, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            ExtendableTypeName = extendableTypeName;
            CustomAttributeType = customAttributeType;
            IsSearchable = isSearchable;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
