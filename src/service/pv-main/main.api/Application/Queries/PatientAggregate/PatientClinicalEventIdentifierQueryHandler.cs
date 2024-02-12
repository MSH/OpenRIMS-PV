using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    public class PatientClinicalEventIdentifierQueryHandler
        : IRequestHandler<PatientClinicalEventIdentifierQuery, PatientClinicalEventIdentifierDto>
    {
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientClinicalEventIdentifierQueryHandler> _logger;

        public PatientClinicalEventIdentifierQueryHandler(
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientClinicalEventIdentifierQueryHandler> logger)
        {
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientClinicalEventIdentifierDto> Handle(PatientClinicalEventIdentifierQuery message, CancellationToken cancellationToken)
        {
            var patientClinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(pm => pm.Patient.Id == message.PatientId && pm.Id == message.PatientClinicalEventId);

            if (patientClinicalEventFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient clinical event");
            }

            var mappedPatientClinicalEvent = _mapper.Map<PatientClinicalEventIdentifierDto>(patientClinicalEventFromRepo);

            return CreateLinks(mappedPatientClinicalEvent);
        }

        private PatientClinicalEventIdentifierDto CreateLinks(PatientClinicalEventIdentifierDto mappedPatientClinicalEvent)
        {
            mappedPatientClinicalEvent.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientClinicalEvent", mappedPatientClinicalEvent.Id), "self", "GET"));

            return mappedPatientClinicalEvent;
        }
    }
}
