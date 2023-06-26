using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Paging;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    public class CausalityReportQueryHandler
        : IRequestHandler<CausalityReportQuery, LinkedCollectionResourceWrapperDto<CausalityReportDto>>
    {
        private readonly IReportInstanceQueries _reportInstanceQueries;
        private readonly IInfrastructureService _infrastructureService;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<CausalityReportQueryHandler> _logger;

        public CausalityReportQueryHandler(
            IReportInstanceQueries reportInstanceQueries,
            IInfrastructureService infrastructureService,
            ILinkGeneratorService linkGeneratorService,
            ILogger<CausalityReportQueryHandler> logger)
        {
            _reportInstanceQueries = reportInstanceQueries ?? throw new ArgumentNullException(nameof(reportInstanceQueries));
            _infrastructureService = infrastructureService ?? throw new ArgumentNullException(nameof(infrastructureService));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<CausalityReportDto>> Handle(CausalityReportQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            // Determine causality configuration
            var config = _infrastructureService.GetOrCreateConfig(ConfigType.AssessmentScale);
            var configValue = (CausalityConfigType)Enum.Parse(typeof(CausalityConfigType), config.ConfigValue.Replace(" ", ""));

            var results = await _reportInstanceQueries.GetCausalityNotSetAsync(message.SearchFrom, message.SearchTo, configValue, message.FacilityId, message.CausalityCriteria);
            var pagedResults = PagedCollection<CausalityReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

            var wrapper = new LinkedCollectionResourceWrapperDto<CausalityReportDto>(pagedResults.TotalCount, pagedResults);
            CreateLinksForCausalityReport(wrapper, message.WorkFlowGuid, message.PageNumber, message.PageSize, message.FacilityId, message.CausalityCriteria,
                pagedResults.HasNext, pagedResults.HasPrevious);

            return wrapper;
        }

        private void CreateLinksForCausalityReport(
            LinkedResourceBaseDto wrapper,
            Guid workFlowGuid,
            int pageNumber, int pageSize, 
            int facilityId, CausalityCriteria causalityCriteria,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateCausalityReportResourceUri(workFlowGuid, ResourceUriType.Current, pageNumber, pageSize, facilityId, causalityCriteria),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateCausalityReportResourceUri(workFlowGuid, ResourceUriType.NextPage, pageNumber, pageSize, facilityId, causalityCriteria),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateCausalityReportResourceUri(workFlowGuid, ResourceUriType.PreviousPage, pageNumber, pageSize, facilityId, causalityCriteria),
                       "previousPage", "GET"));
            }
        }
    }
}
