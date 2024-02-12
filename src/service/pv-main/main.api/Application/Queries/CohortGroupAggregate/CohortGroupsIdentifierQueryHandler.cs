using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate
{
    public class CohortGroupsIdentifierQueryHandler
        : IRequestHandler<CohortGroupsIdentifierQuery, LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CohortGroupsIdentifierQueryHandler> _logger;

        public CohortGroupsIdentifierQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CohortGroupsIdentifierQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>> Handle(CohortGroupsIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<CohortGroup>(message.OrderBy, "asc");

            var pagedCohortGroupsFromRepo = await _cohortGroupRepository.ListAsync(pagingInfo, null, orderby, "");
            if (pagedCohortGroupsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroups = PagedCollection<CohortGroupIdentifierDto>.Create(_mapper.Map<PagedCollection<CohortGroupIdentifierDto>>(pagedCohortGroupsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCohortGroupsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedCohortGroups.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>(pagedCohortGroupsFromRepo.TotalCount, mappedCohortGroups, pagedCohortGroupsFromRepo.TotalPages);

                CreateLinksForCohortGroups(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedCohortGroupsFromRepo.HasNext, pagedCohortGroupsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(CohortGroupIdentifierDto mappedCohortGroup)
        {
            mappedCohortGroup.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CohortGroup", mappedCohortGroup.Id), "self", "GET"));
        }

        private void CreateLinksForCohortGroups(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetCohortGroupsByIdentifier", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetCohortGroupsByIdentifier", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetCohortGroupsByIdentifier", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
