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

namespace PVIMS.API.Application.Queries.CohortGroupAggregate
{
    public class CohortGroupsDetailQueryHandler
        : IRequestHandler<CohortGroupsDetailQuery, LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CohortGroupsDetailQueryHandler> _logger;

        public CohortGroupsDetailQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CohortGroupsDetailQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>> Handle(CohortGroupsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<CohortGroup>(message.OrderBy, "asc");

            var pagedCohortGroupsFromRepo = await _cohortGroupRepository.ListAsync(pagingInfo, null, orderby, new string[] { "Condition", "CohortGroupEnrolments" });
            if (pagedCohortGroupsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroups = PagedCollection<CohortGroupDetailDto>.Create(_mapper.Map<PagedCollection<CohortGroupDetailDto>>(pagedCohortGroupsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCohortGroupsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedCohortGroups.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>(pagedCohortGroupsFromRepo.TotalCount, mappedCohortGroups, pagedCohortGroupsFromRepo.TotalPages);

                CreateLinksForCohortGroups(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedCohortGroupsFromRepo.HasNext, pagedCohortGroupsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(CohortGroupDetailDto mappedCohortGroup)
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
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetCohortGroupsByDetail", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetCohortGroupsByDetail", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetCohortGroupsByDetail", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
