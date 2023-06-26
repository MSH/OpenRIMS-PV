using MediatR;
using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientTreatmentReportQuery
        : IRequest<LinkedCollectionResourceWrapperDto<PatientsOnTreatmentDto>>
    {
        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        [DataMember]
        public DateTime SearchFrom { get; private set; }

        [DataMember]
        public DateTime SearchTo { get; private set; }

        [DataMember]
        public PatientOnStudyCriteria PatientOnStudyCriteria { get; private set; }

        public PatientTreatmentReportQuery()
        {
        }

        public PatientTreatmentReportQuery(int pageNumber, int pageSize, DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria) : this()
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            PatientOnStudyCriteria = patientOnStudyCriteria;
        }
    }
}
