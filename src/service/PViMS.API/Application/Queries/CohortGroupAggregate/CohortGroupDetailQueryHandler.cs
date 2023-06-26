using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.CohortGroupAggregate
{
    public class CohortGroupDetailQueryHandler
        : IRequestHandler<CohortGroupDetailQuery, CohortGroupDetailDto>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CohortGroupDetailQueryHandler> _logger;

        public CohortGroupDetailQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CohortGroupDetailQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CohortGroupDetailDto> Handle(CohortGroupDetailQuery message, CancellationToken cancellationToken)
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == message.CohortGroupId, new string[] { "Condition", "CohortGroupEnrolments" });

            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate cohort group");
            }

            var mappedCohortGroup = _mapper.Map<CohortGroupDetailDto>(cohortGroupFromRepo);

            return CreateLinks(mappedCohortGroup);
        }

        private CohortGroupDetailDto CreateLinks(CohortGroupDetailDto mappedCohortGroup)
        {
            mappedCohortGroup.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CohortGroup", mappedCohortGroup.Id), "self", "GET"));

            return mappedCohortGroup;
        }
    }
}
