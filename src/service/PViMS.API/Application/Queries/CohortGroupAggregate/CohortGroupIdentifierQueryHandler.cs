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
    public class CohortGroupIdentifierQueryHandler
        : IRequestHandler<CohortGroupIdentifierQuery, CohortGroupIdentifierDto>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CohortGroupIdentifierQueryHandler> _logger;

        public CohortGroupIdentifierQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CohortGroupIdentifierQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CohortGroupIdentifierDto> Handle(CohortGroupIdentifierQuery message, CancellationToken cancellationToken)
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == message.CohortGroupId);

            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate cohort group");
            }

            var mappedCohortGroup = _mapper.Map<CohortGroupIdentifierDto>(cohortGroupFromRepo);

            return CreateLinks(mappedCohortGroup);
        }

        private CohortGroupIdentifierDto CreateLinks(CohortGroupIdentifierDto mappedCohortGroup)
        {
            mappedCohortGroup.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CohortGroup", mappedCohortGroup.Id), "self", "GET"));

            return mappedCohortGroup;
        }
    }
}
