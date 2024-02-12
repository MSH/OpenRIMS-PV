using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Paging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate
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
