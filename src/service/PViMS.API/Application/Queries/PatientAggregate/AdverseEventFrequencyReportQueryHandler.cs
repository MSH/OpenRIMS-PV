using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Models.Patient;
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
    public class AdverseEventFrequencyReportQueryHandler
        : IRequestHandler<AdverseEventFrequencyReportQuery, LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>>
    {
        private readonly IPatientQueries _patientQueries;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<AdverseEventFrequencyReportQueryHandler> _logger;

        public AdverseEventFrequencyReportQueryHandler(
            IPatientQueries patientQueries,
            ILinkGeneratorService linkGeneratorService,
            ILogger<AdverseEventFrequencyReportQueryHandler> logger)
        {
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>> Handle(AdverseEventFrequencyReportQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            if (message.FrequencyCriteria == FrequencyCriteria.Annual)
            {
                var results = await _patientQueries.GetAdverseEventsByAnnualAsync(message.SearchFrom, message.SearchTo);
                var pagedResults = PagedCollection<AdverseEventFrequencyReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventFrequencyReport(wrapper, message.PageNumber, message.PageSize,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.FrequencyCriteria == FrequencyCriteria.Quarterly)
            {
                var results = await _patientQueries.GetAdverseEventsByQuarterAsync(message.SearchFrom, message.SearchTo);
                var pagedResults = PagedCollection<AdverseEventFrequencyReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventFrequencyReport(wrapper, message.PageNumber, message.PageSize,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.FrequencyCriteria == FrequencyCriteria.Monthly)
            {
                var results = await _patientQueries.GetAdverseEventsByMonthAsync(message.SearchFrom, message.SearchTo);
                var pagedResults = PagedCollection<AdverseEventFrequencyReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForAdverseEventFrequencyReport(wrapper, message.PageNumber, message.PageSize,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private void CreateLinksForAdverseEventFrequencyReport(
            LinkedResourceBaseDto wrapper,
            int pageNumber, int pageSize, 
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.Current, "GetAdverseEventFrequencyReport", pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.NextPage, "GetAdverseEventFrequencyReport", pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.PreviousPage, "GetAdverseEventFrequencyReport", pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
