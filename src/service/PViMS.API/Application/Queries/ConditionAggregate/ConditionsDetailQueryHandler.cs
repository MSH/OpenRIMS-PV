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
using LinqKit;

namespace PVIMS.API.Application.Queries.ConditionAggregate
{
    public class ConditionsDetailQueryHandler
        : IRequestHandler<ConditionsDetailQuery, LinkedCollectionResourceWrapperDto<ConditionDetailDto>>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<Condition> _conditionRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConditionsDetailQueryHandler> _logger;

        public ConditionsDetailQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<Condition> conditionRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ConditionsDetailQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _conditionRepository = conditionRepository ?? throw new ArgumentNullException(nameof(conditionRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<ConditionDetailDto>> Handle(ConditionsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<Condition>(message.OrderBy, "asc");

            var predicate = PredicateBuilder.New<Condition>(true);
            if (message.Active != API.Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (message.Active == API.Models.ValueTypes.YesNoBothValueType.Yes));
            }

            var pagedConditionsFromRepo = await _conditionRepository.ListAsync(pagingInfo, null, orderby, new string[] { "ConditionLabTests.LabTest", "ConditionMedDras.TerminologyMedDra", "ConditionMedications.Product", "ConditionMedications.Concept", "CohortGroups" });
            if (pagedConditionsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConditions = PagedCollection<ConditionDetailDto>.Create(_mapper.Map<PagedCollection<ConditionDetailDto>>(pagedConditionsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedConditionsFromRepo.TotalCount);

                foreach (var mappedCondition in mappedConditions)
                {
                    await CustomMapAsync(mappedCondition);
                }

                // Add HATEOAS links to each individual resource
                mappedConditions.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<ConditionDetailDto>(pagedConditionsFromRepo.TotalCount, mappedConditions, pagedConditionsFromRepo.TotalPages);

                CreateLinksForConditions(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedConditionsFromRepo.HasNext, pagedConditionsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private async Task CustomMapAsync(ConditionDetailDto dto)
        {
            var cohortGroupsFromRepo = await _cohortGroupRepository.ListAsync(cg => cg.Condition.Id == dto.Id);
            dto.CohortGroups = _mapper.Map<PagedCollection<CohortGroupIdentifierDto>>(cohortGroupsFromRepo);
        }

        private void CreateLinks(ConditionDetailDto mappedCondition)
        {
            mappedCondition.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Condition", mappedCondition.Id), "self", "GET"));
        }

        private void CreateLinksForConditions(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetConditionsByDetail", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetConditionsByDetail", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetConditionsByDetail", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
