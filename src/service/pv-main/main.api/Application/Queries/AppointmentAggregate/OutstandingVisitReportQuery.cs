using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate
{
    [DataContract]
    public class OutstandingVisitReportQuery
        : IRequest<LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>>
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
        public int FacilityId { get; private set; }

        public OutstandingVisitReportQuery()
        {
        }

        public OutstandingVisitReportQuery(int pageNumber, int pageSize, DateTime searchFrom, DateTime searchTo, int facilityId) : this()
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            FacilityId = facilityId;
        }
    }
}
