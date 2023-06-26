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

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientMedicationIdentifierQueryHandler
        : IRequestHandler<PatientMedicationIdentifierQuery, PatientMedicationIdentifierDto>
    {
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientMedicationIdentifierQueryHandler> _logger;

        public PatientMedicationIdentifierQueryHandler(
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientMedicationIdentifierQueryHandler> logger)
        {
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientMedicationIdentifierDto> Handle(PatientMedicationIdentifierQuery message, CancellationToken cancellationToken)
        {
            var patientMedicationFromRepo = await _patientMedicationRepository.GetAsync(pm => pm.Patient.Id == message.PatientId && pm.Id == message.PatientMedicationId);

            if (patientMedicationFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient medication");
            }

            var mappedPatientMedication = _mapper.Map<PatientMedicationIdentifierDto>(patientMedicationFromRepo);

            return CreateLinks(mappedPatientMedication);
        }

        private PatientMedicationIdentifierDto CreateLinks(PatientMedicationIdentifierDto mappedPatientMedication)
        {
            mappedPatientMedication.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientMedication", mappedPatientMedication.Id), "self", "GET"));

            return mappedPatientMedication;
        }
    }
}
