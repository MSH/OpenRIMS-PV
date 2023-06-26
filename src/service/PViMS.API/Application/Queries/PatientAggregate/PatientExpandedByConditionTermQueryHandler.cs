using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.ReportInstanceAggregate;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientExpandedByConditionTermQueryHandler
        : IRequestHandler<PatientExpandedByConditionTermQuery, PatientExpandedDto>
    {
        private readonly IPatientQueries _patientQueries;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IReportInstanceQueries _reportInstanceQueries;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientExpandedByConditionTermQueryHandler> _logger;
        private readonly ICustomAttributeService _customAttributeService;

        public PatientExpandedByConditionTermQueryHandler(
            IPatientQueries patientQueries,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IReportInstanceQueries reportInstanceQueries,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientExpandedByConditionTermQueryHandler> logger,
            ICustomAttributeService customAttributeService)
        {
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _reportInstanceQueries = reportInstanceQueries ?? throw new ArgumentNullException(nameof(reportInstanceQueries));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
        }

        public async Task<PatientExpandedDto> Handle(PatientExpandedByConditionTermQuery message, CancellationToken cancellationToken)
        {
            var results = await _patientQueries.SearchPatientsByConditionCaseNumberAsync(message.CaseNumber);

            if (results.Count() != 1)
            {
                return null;
            }

            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == results.First().PatientId, new string[] {
                "PatientFacilities.Facility.OrgUnit", 
                "PatientClinicalEvents.SourceTerminologyMedDra", 
                "PatientConditions.TerminologyMedDra", 
                "PatientMedications.Concept.MedicationForm", 
                "PatientMedications.Product" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var mappedPatient = _mapper.Map<PatientExpandedDto>(patientFromRepo);

            await CustomMapAsync(patientFromRepo, mappedPatient);
            CreateLinks(mappedPatient);

            return mappedPatient;
        }

        private async Task CustomMapAsync(Patient patientFromRepo, PatientExpandedDto mappedPatient)
        {
            if (patientFromRepo == null)
            {
                throw new ArgumentNullException(nameof(patientFromRepo));
            }

            if (mappedPatient == null)
            {
                throw new ArgumentNullException(nameof(mappedPatient));
            }

            CustomAttributeMap(patientFromRepo, mappedPatient);

            await CustomClinicalMapAsync(mappedPatient);
            await CustomCohortMapAsync(patientFromRepo, mappedPatient);
            await CustomConditionMapAsync(patientFromRepo, mappedPatient);
            await CustomActivityMapAsync(patientFromRepo, mappedPatient);
        }

        private async Task CustomConditionMapAsync(Patient patientFromRepo, PatientExpandedDto mappedPatient)
        {
            int[] terms = _patientConditionRepository.List(pc => pc.Patient.Id == mappedPatient.Id && pc.TerminologyMedDra != null && !pc.Archived && !pc.Patient.Archived, null, new string[] { "Condition" })
                .Select(p => p.TerminologyMedDra.Id)
                .ToArray();
            var conditionMeddras = await _conditionMeddraRepository.ListAsync(cm => terms.Contains(cm.TerminologyMedDra.Id), null, new string[] { "Condition", "TerminologyMedDra" });

            List<PatientConditionGroupDto> groupArray = new List<PatientConditionGroupDto>();
            foreach (var conditionMeddra in conditionMeddras)
            {
                var tempCondition = conditionMeddra.GetConditionForPatient(patientFromRepo);
                if (tempCondition != null)
                {
                    var group = new PatientConditionGroupDto()
                    {
                        ConditionGroup = conditionMeddra.Condition.Description,
                        Status = tempCondition.OutcomeDate != null ? "Case Closed" : "Case Open",
                        PatientConditionId = tempCondition.Id,
                        StartDate = tempCondition.OnsetDate.ToString("yyyy-MM-dd"),
                        Detail = String.Format("{0} started on {1}", tempCondition.TerminologyMedDra.DisplayName, tempCondition.OnsetDate.ToString("yyyy-MM-dd"))
                    };
                    groupArray.Add(group);
                }
            }
            mappedPatient.ConditionGroups = groupArray;
        }

        private void CustomAttributeMap(Patient patientFromRepo, PatientExpandedDto mappedPatient)
        {
            IExtendable patientExtended = patientFromRepo;

            mappedPatient.PatientAttributes = _modelExtensionBuilder.BuildModelExtension(patientExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.Value.ToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            // Map additional attributes to main dto
            var attribute = patientExtended.GetAttributeValue("Medical Record Number");
            mappedPatient.MedicalRecordNumber = attribute != null ? attribute.ToString() : "";
        }

        private async Task CustomClinicalMapAsync(PatientExpandedDto mappedPatient)
        {
            foreach (var dto in mappedPatient.PatientClinicalEvents)
            {
                await CustomClinicalEventMapAsync(dto);
            }

            foreach (var dto in mappedPatient.PatientMedications)
            {
                await CustomMedicationMapAsync(dto);
            }
        }

        private async Task CustomClinicalEventMapAsync(PatientClinicalEventDetailDto dto)
        {
            var clinicalEvent = await _patientClinicalEventRepository.GetAsync(p => p.Id == dto.Id);
            if (clinicalEvent == null)
            {
                return;
            }

            IExtendable clinicalEventExtended = clinicalEvent;

            dto.ReportDate = await _customAttributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Date of Report", clinicalEventExtended);
            dto.IsSerious = await _customAttributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Is the adverse event serious?", clinicalEventExtended);
        }

        private async Task CustomMedicationMapAsync(PatientMedicationDetailDto dto)
        {
            var medication = await _patientMedicationRepository.GetAsync(p => p.Id == dto.Id);
            if (medication == null)
            {
                return;
            }

            IExtendable medicationExtended = medication;

            // Map all custom attributes
            dto.MedicationAttributes = _modelExtensionBuilder.BuildModelExtension(medicationExtended)
                .Select(h => new AttributeValueDto()
                {
                    Id = h.Id,
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();


            dto.IndicationType = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Type of Indication", medicationExtended);
            dto.ReasonForStopping = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Reason For Stopping", medicationExtended);
            dto.ClinicianAction = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Clinician action taken with regard to medicine if related to AE", medicationExtended);
            dto.ChallengeEffect = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Effect OF Dechallenge (D) & Rechallenge (R)", medicationExtended);
        }

        private string GetSelectionValue(CustomAttributeType attributeType, string attributeKey, string selectionKey)
        {
            if (attributeType == CustomAttributeType.Selection)
            {
                var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

                return selectionitem == null ? string.Empty : selectionitem.Value;
            }

            return "";
        }

        private async Task CustomCohortMapAsync(Patient patientFromRepo, PatientExpandedDto mappedPatient)
        {
            var mappedCohortGroups = await GetCohortGroupsAsync();
            foreach (var dto in mappedCohortGroups)
            {
                await CustomCohortGroupMapAsync(dto, patientFromRepo);
            }
            mappedPatient.CohortGroups = mappedCohortGroups;
        }

        private async Task<List<CohortGroupPatientDetailDto>> GetCohortGroupsAsync()
        {
            var cohortGroupsFromRepo = await _cohortGroupRepository.ListAsync(null, null, new string[] { "Condition" });
            if (cohortGroupsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroups = _mapper.Map<List<CohortGroupPatientDetailDto>>(cohortGroupsFromRepo);

                return mappedCohortGroups;
            }

            return null;
        }

        private async Task CustomCohortGroupMapAsync(CohortGroupPatientDetailDto dto, Patient patient)
        {
            var mappedCohortGroupEnrolment = await GetCohortGroupEnrolmentAsync(dto.Id, patient.Id);
            if (mappedCohortGroupEnrolment != null)
            {
                CreateLinksForEnrolment(mappedCohortGroupEnrolment);
                dto.CohortGroupEnrolment = mappedCohortGroupEnrolment;
            }
            
            dto.ConditionStartDate = patient.GetConditionForGroupAndDate(dto.Condition, DateTime.Today)?.OnsetDate.ToString("yyyy-MM-dd");

            CreateLinksForCohortGroup(dto);
        }

        private async Task<EnrolmentIdentifierDto> GetCohortGroupEnrolmentAsync(int cohortGroupId, int patientId)
        {
            var cohortGroupEnrolmentsFromRepo = await _cohortGroupEnrolmentRepository.ListAsync(cge => cge.CohortGroup.Id == cohortGroupId && cge.Patient.Id == patientId && !cge.Archived, null, new string[] { "CohortGroup" });
            if (cohortGroupEnrolmentsFromRepo != null)
            {
                if (cohortGroupEnrolmentsFromRepo.Count > 0)
                {
                    var cohortGroupEnrolment = cohortGroupEnrolmentsFromRepo.First();
                    // Map EF entity to Dto
                    var mappedCohortGroupEnrolment = _mapper.Map<EnrolmentIdentifierDto>(cohortGroupEnrolment);

                    return mappedCohortGroupEnrolment;
                }
            }

            return null;
        }

        private async Task CustomActivityMapAsync(Patient patientFromRepo, PatientExpandedDto mappedPatient)
        {
            var activity = await _reportInstanceQueries.GetExecutionStatusEventsForPatientViewAsync(patientFromRepo.Id);
            mappedPatient.Activity = activity.ToList();
        }

        private void CreateLinksForEnrolment(EnrolmentIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateEnrolmentForPatientResourceUri(dto.PatientId, dto.Id), "self", "GET"));
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDeenrolmentForPatientResourceUri(dto.PatientId, dto.Id), "deenrol", "PUT"));
        }

        private void CreateLinksForCohortGroup(CohortGroupPatientDetailDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CohortGroup", dto.Id), "self", "GET"));
        }

        private void CreateLinks(PatientExpandedDto mappedPatient)
        {
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", mappedPatient.Id), "self", "GET"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(mappedPatient.Id), "newAppointment", "POST"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(mappedPatient.Id), "newEnrolment", "POST"));
        }
    }
}
