using MediatR;
using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    [DataContract]
    public class AppointmentsSearchQuery
        : IRequest<LinkedCollectionResourceWrapperDto<AppointmentSearchDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public string FacilityName { get; private set; }

        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string FirstName { get; private set; }

        [DataMember]
        public string LastName { get; private set; }

        [DataMember]
        public EncounterSearchCriteria CriteriaId { get; private set; }

        [DataMember]
        public int CustomAttributeId { get; private set; }

        [DataMember]
        public string CustomAttributeValue { get; private set; }

        [DataMember]
        public DateTime SearchFrom { get; private set; }

        [DataMember]
        public DateTime SearchTo { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public AppointmentsSearchQuery()
        {
        }

        public AppointmentsSearchQuery(string orderBy, string facilityName, int patientId, string firstName, string lastName, EncounterSearchCriteria criteriaId, int customAttributeId, string customAttributeValue, DateTime searchFrom, DateTime searchTo, int pageNumber, int pageSize): this()
        {
            OrderBy = orderBy;
            FacilityName = facilityName;
            PatientId = patientId;
            FirstName = firstName;
            LastName = lastName;
            CriteriaId = criteriaId;
            CustomAttributeId = customAttributeId;
            CustomAttributeValue = customAttributeValue;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
