using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate
{
    public class AppointmentsSearchQueryHandler
        : IRequestHandler<AppointmentsSearchQuery, LinkedCollectionResourceWrapperDto<AppointmentSearchDto>>
    {
        private readonly IAppointmentQueries _appointmentQueries;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<AppointmentsSearchQueryHandler> _logger;

        public AppointmentsSearchQueryHandler(
            IAppointmentQueries appointmentQueries,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Facility> facilityRepository,
            ILinkGeneratorService linkGeneratorService,
            ILogger<AppointmentsSearchQueryHandler> logger)
        {
            _appointmentQueries = appointmentQueries ?? throw new ArgumentNullException(nameof(appointmentQueries));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<AppointmentSearchDto>> Handle(AppointmentsSearchQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var searchFacilityId = await GetFacilityIdAsync(message.FacilityName);
            var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == message.CustomAttributeId);

            var results = await _appointmentQueries.SearchAppointmentsAsync(
                (int)message.CriteriaId,
                searchFacilityId,
                message.PatientId == 0 ? null : message.PatientId,
                message.FirstName,
                message.LastName,
                message.SearchFrom == DateTime.MinValue ? null : message.SearchFrom,
                message.SearchTo == DateTime.MinValue ? null : message.SearchTo,
                customAttribute?.AttributeKey,
                message.CustomAttributeValue);
            var pagedResults = PagedCollection<AppointmentSearchDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

            var wrapper = new LinkedCollectionResourceWrapperDto<AppointmentSearchDto>(pagedResults.TotalCount, pagedResults);

            CreateLinksForAppointments(wrapper, message.PageNumber, message.PageSize, pagedResults.HasNext, pagedResults.HasPrevious);

            return wrapper;

            return null;
        }

        private async Task<int?> GetFacilityIdAsync(string facilityName)
        {
            if (!String.IsNullOrWhiteSpace(facilityName))
            {
                var facility = await _facilityRepository.GetAsync(f => f.FacilityName == facilityName);
                if (facility != null)
                {
                    return facility.Id;
                }
            }
            return null;
        }

        private LinkedResourceBaseDto CreateLinksForAppointments(
            LinkedResourceBaseDto wrapper,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.Current, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.NextPage, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.PreviousPage, pageNumber, pageSize),
                       "previousPage", "GET"));
            }

            return wrapper;
        }
    }
}
