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

namespace PVIMS.API.Application.Queries.OrgUnitAggregate
{
    public class OrgUnitsIdentifierQueryHandler
        : IRequestHandler<OrgUnitsIdentifierQuery, LinkedCollectionResourceWrapperDto<OrgUnitIdentifierDto>>
    {
        private readonly IRepositoryInt<OrgUnit> _orgUnitRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrgUnitsIdentifierQueryHandler> _logger;

        public OrgUnitsIdentifierQueryHandler(
            IRepositoryInt<OrgUnit> orgUnitRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<OrgUnitsIdentifierQueryHandler> logger)
        {
            _orgUnitRepository = orgUnitRepository ?? throw new ArgumentNullException(nameof(orgUnitRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<OrgUnitIdentifierDto>> Handle(OrgUnitsIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<OrgUnit>("Id", "asc");

            var pagedOrgUnitsFromRepo = await _orgUnitRepository.ListAsync(pagingInfo, null, orderby, new string[] { "OrgUnitType" });
            if (pagedOrgUnitsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedOrgUnits = PagedCollection<OrgUnitIdentifierDto>.Create(_mapper.Map<PagedCollection<OrgUnitIdentifierDto>>(pagedOrgUnitsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedOrgUnitsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedOrgUnits.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<OrgUnitIdentifierDto>(pagedOrgUnitsFromRepo.TotalCount, mappedOrgUnits, pagedOrgUnitsFromRepo.TotalPages);

                CreateLinksForOrgUnits(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedOrgUnitsFromRepo.HasNext, pagedOrgUnitsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(OrgUnitIdentifierDto mappedOrgUnit)
        {
            mappedOrgUnit.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("OrgUnit", mappedOrgUnit.Id), "self", "GET"));
        }

        private void CreateLinksForOrgUnits(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetOrgUnitsByIdentifier", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetOrgUnitsByIdentifier", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetOrgUnitsByIdentifier", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
