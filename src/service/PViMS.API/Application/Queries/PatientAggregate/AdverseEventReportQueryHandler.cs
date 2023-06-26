using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Paging;
using PVIMS.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class AdverseEventReportQueryHandler
        : IRequestHandler<AdverseEventReportQuery, LinkedCollectionResourceWrapperDto<AdverseEventReportDto>>
    {
        private readonly IPatientQueries _patientQueries;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<AdverseEventReportQueryHandler> _logger;

        public AdverseEventReportQueryHandler(
            IPatientQueries patientQueries,
            ILinkGeneratorService linkGeneratorService,
            ILogger<AdverseEventReportQueryHandler> logger)
        {
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<AdverseEventReportDto>> Handle(AdverseEventReportQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.AgeGroup)
            {
                var results = await _patientQueries.GetAdverseEventsByAgeGroupAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.FacilityRegion)
            {
                var results = await _patientQueries.GetAdverseEventsByFacilityRegionAsync(message.SearchFrom, 
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria, 
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.Outcome)
            {
                var results = await _patientQueries.GetAdverseEventsByOutcomeAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.Gender)
            {
                var results = await _patientQueries.GetAdverseEventsByGenderAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.Regimen)
            {
                var results = await _patientQueries.GetAdverseEventsByRegimenAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.IsSerious)
            {
                var results = await _patientQueries.GetAdverseEventsByIsSeriousAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.Seriousness)
            {
                var results = await _patientQueries.GetAdverseEventsBySeriousnessAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.AdverseEventStratifyCriteria == AdverseEventStratifyCriteria.Classification)
            {
                var results = await _patientQueries.GetAdverseEventsByClassificationAsync(message.SearchFrom,
                    message.SearchTo,
                    message.AgeGroupCriteria,
                    message.GenderId,
                    message.RegimenId,
                    message.OrganisationUnitId,
                    message.OutcomeId,
                    message.IsSeriousId,
                    message.SeriousnessId,
                    message.ClassificationId);
                var pagedResults = PagedCollection<AdverseEventReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventReport(wrapper, message.PageNumber, message.PageSize, message.AdverseEventStratifyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private void CreateLinksForAdverseEventReport(
            LinkedResourceBaseDto wrapper,
            int pageNumber, int pageSize,
            AdverseEventStratifyCriteria adverseEventStratifyCriteria,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAdverseEventReportResourceUri(ResourceUriType.Current, pageNumber, pageSize, adverseEventStratifyCriteria),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAdverseEventReportResourceUri(ResourceUriType.NextPage, pageNumber, pageSize, adverseEventStratifyCriteria),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAdverseEventReportResourceUri(ResourceUriType.PreviousPage, pageNumber, pageSize, adverseEventStratifyCriteria),
                       "previousPage", "GET"));
            }
        }
    }
}
