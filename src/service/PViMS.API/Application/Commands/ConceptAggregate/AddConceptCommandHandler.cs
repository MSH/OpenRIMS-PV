using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    public class AddConceptCommandHandler
        : IRequestHandler<AddConceptCommand, ConceptIdentifierDto>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<MedicationForm> _medicationFormRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddConceptCommandHandler> _logger;

        public AddConceptCommandHandler(
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<MedicationForm> medicationFormRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddConceptCommandHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _medicationFormRepository = medicationFormRepository ?? throw new ArgumentNullException(nameof(medicationFormRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ConceptIdentifierDto> Handle(AddConceptCommand message, CancellationToken cancellationToken)
        {
            var medicationFormFromRepo = await _medicationFormRepository.GetAsync(mf => mf.Description == message.MedicationForm);
            if (medicationFormFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate medication form");
            }

            if (_conceptRepository.Exists(c => c.ConceptName == message.ConceptName &&
                c.MedicationForm.Id == medicationFormFromRepo.Id &&
                c.Strength == message.Strength))
            {
                throw new DomainException("Concept with same name, strength and form already exists");
            }

            var newConcept = new Concept(message.ConceptName, message.Strength, medicationFormFromRepo);

            await _conceptRepository.SaveAsync(newConcept);

            _logger.LogInformation($"----- Concept {message.ConceptName} created");

            var mappedConcept = _mapper.Map<ConceptIdentifierDto>(newConcept);

            CreateLinks(mappedConcept);

            return mappedConcept;
        }

        private void CreateLinks(ConceptIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Concept", dto.Id), "self", "GET"));
        }
    }
}
