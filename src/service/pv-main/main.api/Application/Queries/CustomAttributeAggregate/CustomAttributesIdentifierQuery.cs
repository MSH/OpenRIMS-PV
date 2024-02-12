using MediatR;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.CustomAttributeAggregate
{
    [DataContract]
    public class CustomAttributesIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>>
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

        public CustomAttributesIdentifierQuery()
        {
        }

        public CustomAttributesIdentifierQuery(string orderBy, ExtendableTypeNames extendableTypeName, CustomAttributeTypes customAttributeType, bool? isSearchable, int pageNumber, int pageSize) : this()
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
