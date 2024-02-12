using MediatR;
using OpenRIMS.PV.Main.API.Application.Models.Patient;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class AdverseEventFrequencyReportQuery
        : IRequest<LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>>
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
        public FrequencyCriteria FrequencyCriteria { get; private set; }

        public AdverseEventFrequencyReportQuery()
        {
        }

        public AdverseEventFrequencyReportQuery(int pageNumber, int pageSize, DateTime searchFrom, DateTime searchTo, FrequencyCriteria frequencyCriteria) : this()
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            FrequencyCriteria = frequencyCriteria;
        }
    }
}
