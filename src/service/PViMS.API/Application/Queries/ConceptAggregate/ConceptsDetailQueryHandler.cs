using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.ValueTypes;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    public class ConceptsDetailQueryHandler
        : IRequestHandler<ConceptsDetailQuery, LinkedCollectionResourceWrapperDto<ConceptDetailDto>>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConceptsDetailQueryHandler> _logger;

        public ConceptsDetailQueryHandler(
            IRepositoryInt<Concept> conceptRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ConceptsDetailQueryHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<ConceptDetailDto>> Handle(ConceptsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<Concept>(message.OrderBy, "asc");

            var predicate = PredicateBuilder.New<Concept>(true);
            if (message.Active != YesNoBothValueType.Both)
            {
                predicate = predicate.And(c => c.Active == (message.Active == YesNoBothValueType.Yes));
            }
            if (!String.IsNullOrWhiteSpace(message.SearchTerm))
            {
                predicate = predicate.And(c => c.ConceptName.Contains(message.SearchTerm.Trim()));
            }

            var pagedConceptsFromRepo = await _conceptRepository.ListAsync(pagingInfo, predicate, orderby, new string[] { "MedicationForm" });
            if (pagedConceptsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConcepts = PagedCollection<ConceptDetailDto>.Create(_mapper.Map<PagedCollection<ConceptDetailDto>>(pagedConceptsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedConceptsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedConcepts.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<ConceptDetailDto>(pagedConceptsFromRepo.TotalCount, mappedConcepts, pagedConceptsFromRepo.TotalPages);

                CreateLinksForConcepts(wrapper, message.OrderBy, message.SearchTerm, message.Active, message.PageNumber, message.PageSize,
                    pagedConceptsFromRepo.HasNext, pagedConceptsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(ConceptDetailDto mappedConcept)
        {
            mappedConcept.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Concept", mappedConcept.Id), "self", "GET"));
        }

        private void CreateLinksForConcepts(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            string searchTerm,
            YesNoBothValueType active,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateConceptsResourceUri(ResourceUriType.Current, orderBy, searchTerm, active, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateConceptsResourceUri(ResourceUriType.NextPage, orderBy, searchTerm, active, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateConceptsResourceUri(ResourceUriType.PreviousPage, orderBy, searchTerm, active, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
