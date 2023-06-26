using AutoMapper;
using LinqKit;
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

namespace PVIMS.API.Application.Queries.ConditionAggregate
{
    public class ConditionsIdentifierQueryHandler
        : IRequestHandler<ConditionsIdentifierQuery, LinkedCollectionResourceWrapperDto<ConditionIdentifierDto>>
    {
        private readonly IRepositoryInt<Condition> _conditionRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConditionsIdentifierQueryHandler> _logger;

        public ConditionsIdentifierQueryHandler(
            IRepositoryInt<Condition> conditionRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ConditionsIdentifierQueryHandler> logger)
        {
            _conditionRepository = conditionRepository ?? throw new ArgumentNullException(nameof(conditionRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<ConditionIdentifierDto>> Handle(ConditionsIdentifierQuery message, CancellationToken cancellationToken)
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

            var pagedConditionsFromRepo = await _conditionRepository.ListAsync(pagingInfo, null, orderby);
            if (pagedConditionsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConditions = PagedCollection<ConditionIdentifierDto>.Create(_mapper.Map<PagedCollection<ConditionIdentifierDto>>(pagedConditionsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedConditionsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedConditions.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<ConditionIdentifierDto>(pagedConditionsFromRepo.TotalCount, mappedConditions, pagedConditionsFromRepo.TotalPages);

                CreateLinksForConditions(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedConditionsFromRepo.HasNext, pagedConditionsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(ConditionIdentifierDto mappedCondition)
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
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetConditionsByIdentifier", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetConditionsByIdentifier", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetConditionsByIdentifier", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
