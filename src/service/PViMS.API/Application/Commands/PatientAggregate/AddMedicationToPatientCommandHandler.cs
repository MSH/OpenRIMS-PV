using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class AddMedicationToPatientCommandHandler
        : IRequestHandler<AddMedicationToPatientCommand, PatientMedicationIdentifierDto>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMedicationToPatientCommandHandler> _logger;

        public AddMedicationToPatientCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<Product> productRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IWorkFlowService workFlowService,
            IMapper mapper,
            ILogger<AddMedicationToPatientCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientMedicationIdentifierDto> Handle(AddMedicationToPatientCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { 
                "PatientClinicalEvents", 
                "PatientMedications.Concept.MedicationForm" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient"); 
            }

            Concept conceptFromRepo = null;
            if (message.ConceptId > 0)
            {
                conceptFromRepo = await _conceptRepository.GetAsync(message.ConceptId, new string[] { "MedicationForm" });
                if (conceptFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate concept");
                }
            }

            Product productFromRepo = null;
            if (message.ProductId.HasValue && message.ProductId > 0)
            {
                productFromRepo = await _productRepository.GetAsync(p => p.Id == message.ProductId);
                if (productFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate product");
                }
                conceptFromRepo = productFromRepo.Concept;
            }

            if (conceptFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate concept");
            }

            var medicationDetail = await PrepareMedicationDetailAsync(message.Attributes);
            if (!medicationDetail.IsValid())
            {
                medicationDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            var newPatientMedication = patientFromRepo.AddMedication(conceptFromRepo, message.StartDate, message.EndDate, message.Dose, message.DoseFrequency, message.DoseUnit, productFromRepo, message.SourceDescription);
            _modelExtensionBuilder.UpdateExtendable(newPatientMedication, medicationDetail.CustomAttributes, "Admin");

            _patientRepository.Update(patientFromRepo);

            await AddOrUpdateMedicationsOnReportInstanceAsync(patientFromRepo, newPatientMedication.StartDate, newPatientMedication.EndDate, newPatientMedication.DisplayName, newPatientMedication.PatientMedicationGuid);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Medication {conceptFromRepo.ConceptName} created");

            var mappedPatientMedication = _mapper.Map<PatientMedicationIdentifierDto>(newPatientMedication);

            return CreateLinks(mappedPatientMedication);
        }

        private async Task<MedicationDetail> PrepareMedicationDetailAsync(IDictionary<int, string> attributes)
        {
            var medicationDetail = new MedicationDetail();
            medicationDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientMedication>();

            //medicationDetail = _mapper.Map<MedicationDetail>(medicationForUpdate);
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = medicationDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient medication {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }

            return medicationDetail;
        }

        private async Task AddOrUpdateMedicationsOnReportInstanceAsync(Patient patientFromRepo, DateTime medicationStartDate, DateTime? medicationEndDate, string medicationDisplayName, Guid medicationGuid)
        {
            var weeks = await GetNumberOfWeeksToCheckAsync();
            var clinicalEvents = GetClinicalEventsWhichOccuredDuringMedicationPeriod(patientFromRepo, medicationStartDate, medicationEndDate, weeks);
            var medications = PrepareMedicationsForLinkingToReport(medicationDisplayName, medicationGuid);

            foreach (var clinicalEvent in clinicalEvents)
            {
                await _workFlowService.AddOrUpdateMedicationsForWorkFlowInstanceAsync(clinicalEvent.PatientClinicalEventGuid, medications);
            }
        }

        private async Task<int> GetNumberOfWeeksToCheckAsync()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks);
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    return Convert.ToInt32(config.ConfigValue);
                }
            }
            return 0;
        }

        private IEnumerable<PatientClinicalEvent> GetClinicalEventsWhichOccuredDuringMedicationPeriod(Patient patientFromRepo, DateTime medicationStartDate, DateTime? medicationEndDate, int weekCount)
        {
            if (!medicationEndDate.HasValue)
            {
                return patientFromRepo.PatientClinicalEvents.Where(pce => pce.OnsetDate >= medicationStartDate.AddDays(weekCount * -7) && pce.Archived == false);
            }

            return patientFromRepo.PatientClinicalEvents.Where(pce => pce.OnsetDate >= medicationStartDate.AddDays(weekCount * -7) && pce.OnsetDate <= medicationEndDate.Value.AddDays(weekCount * 7) && pce.Archived == false);
        }

        private List<ReportInstanceMedicationListItem> PrepareMedicationsForLinkingToReport(string displayName, Guid patientMedicationGuid)
        {
            var instanceMedications = new List<ReportInstanceMedicationListItem>();
            instanceMedications.Add(new ReportInstanceMedicationListItem()
            {
                MedicationIdentifier = displayName,
                ReportInstanceMedicationGuid = patientMedicationGuid
            });
            return instanceMedications;
        }

        private PatientMedicationIdentifierDto CreateLinks(PatientMedicationIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientMedication", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
