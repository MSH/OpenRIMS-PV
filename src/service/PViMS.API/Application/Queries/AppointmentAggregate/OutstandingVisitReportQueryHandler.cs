using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Paging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    public class OutstandingVisitReportQueryHandler
        : IRequestHandler<OutstandingVisitReportQuery, LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>>
    {
        private readonly IAppointmentQueries _appointmentQueries;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<OutstandingVisitReportQueryHandler> _logger;

        public OutstandingVisitReportQueryHandler(
            IAppointmentQueries appointmentQueries,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<OutstandingVisitReportQueryHandler> logger)
        {
            _appointmentQueries = appointmentQueries ?? throw new ArgumentNullException(nameof(appointmentQueries));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>> Handle(OutstandingVisitReportQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var results = await _appointmentQueries.GetOutstandingVisitsAsync(message.SearchFrom, message.SearchTo, message.FacilityId);
            var pagedResults = PagedCollection<OutstandingVisitReportDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

            var wrapper = new LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>(pagedResults.TotalCount, pagedResults);
            CreateLinksForOutstandingVisitReport(wrapper, message.PageNumber, message.PageSize,
                pagedResults.HasNext, pagedResults.HasPrevious);

            return wrapper;
        }

        private void CreateLinksForOutstandingVisitReport(
            LinkedResourceBaseDto wrapper,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateOutstandingVisitReportResourceUri(ResourceUriType.Current, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateOutstandingVisitReportResourceUri(ResourceUriType.NextPage, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateOutstandingVisitReportResourceUri(ResourceUriType.PreviousPage, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
