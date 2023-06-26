using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    public class ConceptIdentifierQueryHandler
        : IRequestHandler<ConceptIdentifierQuery, ConceptIdentifierDto>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConceptIdentifierQueryHandler> _logger;

        public ConceptIdentifierQueryHandler(
            IRepositoryInt<Concept> conceptRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ConceptIdentifierQueryHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ConceptIdentifierDto> Handle(ConceptIdentifierQuery message, CancellationToken cancellationToken)
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(c => c.Id == message.ConceptId, new string[] { "MedicationForm" });

            if (conceptFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate concept");
            }

            var mappedConcept = _mapper.Map<ConceptIdentifierDto>(conceptFromRepo);

            return CreateLinks(mappedConcept);
        }

        private ConceptIdentifierDto CreateLinks(ConceptIdentifierDto mappedConcept)
        {
            mappedConcept.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Concept", mappedConcept.Id), "self", "GET"));

            return mappedConcept;
        }
    }
}
