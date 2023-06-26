using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    [DataContract]
    public class MedicationFormsIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public MedicationFormsIdentifierQuery()
        {
        }

        public MedicationFormsIdentifierQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}