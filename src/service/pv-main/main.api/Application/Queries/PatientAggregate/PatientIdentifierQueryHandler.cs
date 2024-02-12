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
    public class PatientIdentifierQueryHandler
        : IRequestHandler<PatientIdentifierQuery, PatientIdentifierDto>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientIdentifierQueryHandler> _logger;

        public PatientIdentifierQueryHandler(
            IRepositoryInt<Patient> patientRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientIdentifierQueryHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientIdentifierDto> Handle(PatientIdentifierQuery message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(p => p.Archived == false
                    && p.Id == message.PatientId,
                    new string[] { "PatientFacilities.Facility.OrgUnit" });

            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var mappedPatient = _mapper.Map<PatientIdentifierDto>(patientFromRepo);

            CreateLinks(mappedPatient);

            return mappedPatient;
        }

        private void CreateLinks(PatientIdentifierDto mappedPatient)
        {
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", mappedPatient.Id), "self", "GET"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(mappedPatient.Id), "newAppointment", "POST"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(mappedPatient.Id), "newEnrolment", "POST"));
        }
    }
}
