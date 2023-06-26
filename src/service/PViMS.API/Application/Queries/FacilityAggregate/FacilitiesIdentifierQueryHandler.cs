using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.FacilityAggregate
{
    public class FacilitiesIdentifierQueryHandler
        : IRequestHandler<FacilitiesIdentifierQuery, LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<FacilitiesIdentifierQueryHandler> _logger;

        public FacilitiesIdentifierQueryHandler(
            IRepositoryInt<Facility> facilityRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<FacilitiesIdentifierQueryHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>> Handle(FacilitiesIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<Facility>(message.OrderBy, "asc");

            var pagedFacilitiesFromRepo = await _facilityRepository.ListAsync(pagingInfo, null, orderby, "");
            if (pagedFacilitiesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedFacilities = PagedCollection<FacilityIdentifierDto>.Create(_mapper.Map<PagedCollection<FacilityIdentifierDto>>(pagedFacilitiesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedFacilitiesFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedFacilities.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>(pagedFacilitiesFromRepo.TotalCount, mappedFacilities, pagedFacilitiesFromRepo.TotalPages);

                CreateLinksForFacilities(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedFacilitiesFromRepo.HasNext, pagedFacilitiesFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(FacilityIdentifierDto mappedFacility)
        {
            mappedFacility.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", mappedFacility.Id), "self", "GET"));
            mappedFacility.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", mappedFacility.Id), "self", "DELETE"));
        }

        private void CreateLinksForFacilities(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetFacilitiesByIdentifier", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetFacilitiesByIdentifier", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetFacilitiesByIdentifier", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
