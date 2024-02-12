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
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class AddClinicalEventToPatientCommandHandler
        : IRequestHandler<AddClinicalEventToPatientCommand, PatientClinicalEventIdentifierDto>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMedicationToPatientCommandHandler> _logger;

        public AddClinicalEventToPatientCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IWorkFlowService workFlowService,
            IMapper mapper,
            ILogger<AddMedicationToPatientCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientClinicalEventIdentifierDto> Handle(AddClinicalEventToPatientCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] {
                "PatientClinicalEvents.SourceTerminologyMedDra",
                "PatientMedications.Concept",
                "PatientFacilities.Facility" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            TerminologyMedDra sourceTermFromRepo = null;
            if (message.SourceTerminologyMedDraId.HasValue)
            {
                if(message.SourceTerminologyMedDraId > 0)
                {
                    sourceTermFromRepo = await _terminologyMeddraRepository.GetAsync(message.SourceTerminologyMedDraId); ;
                    if (sourceTermFromRepo == null)
                    {
                        throw new KeyNotFoundException("Unable to locate terminology for MedDRA");
                    }
                }
            }

            var clinicalEventDetail = await PrepareClinicalEventDetailAsync(message.Attributes);
            if (!clinicalEventDetail.IsValid())
            {
                clinicalEventDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            var newPatientClinicalEvent = patientFromRepo.AddClinicalEvent(message.OnsetDate, message.ResolutionDate, sourceTermFromRepo, message.SourceDescription);
            _modelExtensionBuilder.UpdateExtendable(newPatientClinicalEvent, clinicalEventDetail.CustomAttributes, "Admin");

            _patientRepository.Update(patientFromRepo);

            // TODO Move to domain event
            await _workFlowService.CreateWorkFlowInstanceAsync(
                workFlowName: "New Active Surveilliance Report",
                contextGuid: newPatientClinicalEvent.PatientClinicalEventGuid,
                patientIdentifier: String.IsNullOrWhiteSpace(message.PatientIdentifier) ? patientFromRepo.FullName : $"{patientFromRepo.FullName} ({message.PatientIdentifier})",
                sourceIdentifier: newPatientClinicalEvent.SourceTerminologyMedDra?.DisplayName ?? newPatientClinicalEvent.SourceDescription,
                facilityIdentifier: patientFromRepo.CurrentFacilityCode);

            await LinkMedicationsToClinicalEvent(patientFromRepo, newPatientClinicalEvent.OnsetDate, newPatientClinicalEvent.PatientClinicalEventGuid);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Clinical Event {message.SourceDescription} created");

            var mappedPatientClinicalEvent = _mapper.Map<PatientClinicalEventIdentifierDto>(newPatientClinicalEvent);

            return CreateLinks(mappedPatientClinicalEvent);
        }

        private async Task<ClinicalEventDetail> PrepareClinicalEventDetailAsync(IDictionary<int, string> attributes)
        {
            var clinicalEventDetail = new ClinicalEventDetail();
            clinicalEventDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientClinicalEvent>();

            //clinicalEventDetail = _mapper.Map<ClinicalEventDetail>(clinicalEventForUpdate);
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = clinicalEventDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient clinical event {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }

            return clinicalEventDetail;
        }

        private async Task LinkMedicationsToClinicalEvent(Patient patientFromRepo, DateTime? onsetDate, Guid patientClinicalEventGuid)
        {
            var weeks = await GetNumberOfWeeksToCheckAsync();

            // Prepare medications
            List<ReportInstanceMedicationListItem> medications = new List<ReportInstanceMedicationListItem>();
            foreach (var med in patientFromRepo.PatientMedications.Where(m => m.Archived == false
                    && (m.EndDate == null && m.StartDate.AddDays(weeks * -7) <= onsetDate)
                    || (m.EndDate != null && m.StartDate.AddDays(weeks * -7) <= onsetDate && Convert.ToDateTime(m.EndDate).AddDays(weeks * 7) >= onsetDate))
                .OrderBy(m => m.Concept.ConceptName))
            {
                var item = new ReportInstanceMedicationListItem()
                {
                    MedicationIdentifier = med.DisplayName,
                    ReportInstanceMedicationGuid = med.PatientMedicationGuid
                };
                medications.Add(item);
            }
            await _workFlowService.AddOrUpdateMedicationsForWorkFlowInstanceAsync(patientClinicalEventGuid, medications);
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

        private PatientClinicalEventIdentifierDto CreateLinks(PatientClinicalEventIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientClinicalEvent", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
