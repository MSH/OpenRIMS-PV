using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    [DataContract]
    public class EncountersDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<EncounterDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public string FacilityName { get; private set; }

        [DataMember]
        public int CustomAttributeId { get; private set; }

        [DataMember]
        public string CustomAttributeValue { get; private set; }

        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string FirstName { get; private set; }

        [DataMember]
        public string LastName { get; private set; }

        [DataMember]
        public DateTime SearchFrom { get; private set; }

        [DataMember]
        public DateTime SearchTo { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public EncountersDetailQuery()
        {
        }

        public EncountersDetailQuery(string orderBy, string facilityName, int customAttributeId, string customAttributeValue, int patientId, DateTime searchFrom, DateTime searchTo, string firstName, string lastName, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            FacilityName = facilityName;
            CustomAttributeId = customAttributeId;
            CustomAttributeValue = customAttributeValue;
            PatientId = patientId;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            FirstName = firstName;
            LastName = lastName;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
