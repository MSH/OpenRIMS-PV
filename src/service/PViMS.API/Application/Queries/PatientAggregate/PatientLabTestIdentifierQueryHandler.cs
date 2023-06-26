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
    public class PatientLabTestIdentifierQueryHandler
        : IRequestHandler<PatientLabTestIdentifierQuery, PatientLabTestIdentifierDto>
    {
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientLabTestIdentifierQueryHandler> _logger;

        public PatientLabTestIdentifierQueryHandler(
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientLabTestIdentifierQueryHandler> logger)
        {
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientLabTestIdentifierDto> Handle(PatientLabTestIdentifierQuery message, CancellationToken cancellationToken)
        {
            var patientLabTestFromRepo = await _patientLabTestRepository.GetAsync(plt => plt.Patient.Id == message.PatientId && plt.Id == message.PatientLabTestId);

            if (patientLabTestFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate patient lab test {message.PatientLabTestId} for patient {message.PatientId}");
            }

            var mappedPatientLabTest = _mapper.Map<PatientLabTestIdentifierDto>(patientLabTestFromRepo);

            CreateLinks(mappedPatientLabTest);

            return mappedPatientLabTest;
        }

        private void CreateLinks(PatientLabTestIdentifierDto mappedPatientLabTest)
        {
            mappedPatientLabTest.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientLabTest", mappedPatientLabTest.Id), "self", "GET"));
        }
    }
}
