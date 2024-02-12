using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class AddLabTestToPatientCommandHandler
        : IRequestHandler<AddLabTestToPatientCommand, PatientLabTestIdentifierDto>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<LabTest> _labTestRepository;
        private readonly IRepositoryInt<LabTestUnit> _labTestUnitRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddLabTestToPatientCommandHandler> _logger;

        public AddLabTestToPatientCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<LabTest> labTestRepository,
            IRepositoryInt<LabTestUnit> labTestUnitRepository,
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddLabTestToPatientCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _labTestRepository = labTestRepository ?? throw new ArgumentNullException(nameof(labTestRepository));
            _labTestUnitRepository = labTestUnitRepository ?? throw new ArgumentNullException(nameof(labTestUnitRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientLabTestIdentifierDto> Handle(AddLabTestToPatientCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] {
                "PatientLabTests.LabTest",
                "PatientLabTests.TestUnit"
            });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate patient {message.PatientId}");
            }

            var labTestFromRepo = await _labTestRepository.GetAsync(lt => lt.Description == message.LabTest);
            if (labTestFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate lab test with a description of {message.LabTest}");
            }

            LabTestUnit labTestUnitFromRepo = null;
            if (!String.IsNullOrWhiteSpace(message.TestUnit))
            {
                labTestUnitFromRepo = _labTestUnitRepository.Get(u => u.Description == message.TestUnit);
                if (labTestUnitFromRepo == null)
                {
                    throw new KeyNotFoundException($"Unable to locate lab test unit with a description of {message.TestUnit}");
                }
            }

            var labTestDetail = await PrepareLabTestDetailAsync(message.Attributes);
            if (!labTestDetail.IsValid())
            {
                labTestDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            var newPatientLabTest = patientFromRepo.AddLabTest(
                message.TestDate, 
                message.TestResultCoded,
                labTestFromRepo,
                labTestUnitFromRepo,
                message.TestResultValue,
                message.ReferenceLower,
                message.ReferenceUpper
            );
            _modelExtensionBuilder.UpdateExtendable(newPatientLabTest, labTestDetail.CustomAttributes, "Admin");

            _patientRepository.Update(patientFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Lab Test {message.LabTest} created");

            var mappedPatientLabTest = _mapper.Map<PatientLabTestIdentifierDto>(newPatientLabTest);

            CreateLinks(mappedPatientLabTest);

            return mappedPatientLabTest;
        }

        private async Task<LabTestDetail> PrepareLabTestDetailAsync(IDictionary<int, string> attributes)
        {
            var labTestDetail = new LabTestDetail();
            labTestDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientLabTest>();

            //clinicalEventDetail = _mapper.Map<ClinicalEventDetail>(clinicalEventForUpdate);
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = labTestDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient lab test {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }

            return labTestDetail;
        }

        private void CreateLinks(PatientLabTestIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientLabTest", dto.Id), "self", "GET"));
        }
    }
}
