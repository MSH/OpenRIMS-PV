using MediatR;
using PVIMS.API.Models;
using PVIMS.API.Models.ValueTypes;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ConditionAggregate
{
    [DataContract]
    public class ConditionsIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<ConditionIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public YesNoBothValueType Active { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public ConditionsIdentifierQuery()
        {
        }

        public ConditionsIdentifierQuery(string orderBy, YesNoBothValueType active, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            Active =  active;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
