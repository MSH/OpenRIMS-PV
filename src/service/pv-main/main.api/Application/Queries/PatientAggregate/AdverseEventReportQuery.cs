using MediatR;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class AdverseEventReportQuery
        : IRequest<LinkedCollectionResourceWrapperDto<AdverseEventReportDto>>
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
        public AdverseEventStratifyCriteria AdverseEventStratifyCriteria { get; private set; }

        [DataMember]
        public AgeGroupCriteria AgeGroupCriteria { get; private set; }

        [DataMember]
        public string GenderId { get; private set; }

        [DataMember]
        public string RegimenId { get; private set; }

        [DataMember]
        public int OrganisationUnitId { get; private set; }

        [DataMember]
        public string OutcomeId { get; private set; }

        [DataMember]
        public string IsSeriousId { get; private set; }

        [DataMember]
        public string SeriousnessId { get; private set; }

        [DataMember]
        public string ClassificationId { get; private set; }

        public AdverseEventReportQuery()
        {
        }

        public AdverseEventReportQuery(int pageNumber, int pageSize, DateTime searchFrom, DateTime searchTo, 
            AdverseEventStratifyCriteria adverseEventStratifyCriteria,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId) : this()
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            AdverseEventStratifyCriteria = adverseEventStratifyCriteria;
            AgeGroupCriteria = ageGroupCriteria;
            GenderId = genderId;
            RegimenId = regimenId;
            OrganisationUnitId = organisationUnitId;
            OutcomeId = outcomeId;
            IsSeriousId = isSeriousId;
            SeriousnessId = seriousnessId;
            ClassificationId = classificationId;
        }
    }
}
