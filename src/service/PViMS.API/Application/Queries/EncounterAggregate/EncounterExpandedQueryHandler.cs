using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    public class EncounterExpandedQueryHandler
        : IRequestHandler<EncounterExpandedQuery, EncounterExpandedDto>
    {
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<EncounterExpandedQueryHandler> _logger;
        private readonly IPatientService _patientService;

        public EncounterExpandedQueryHandler(
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<EncounterExpandedQueryHandler> logger,
            IPatientService patientService)
        {
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
        }

        public async Task<EncounterExpandedDto> Handle(EncounterExpandedQuery message, CancellationToken cancellationToken)
        {
            var encounterFromRepo = await _encounterRepository.GetAsync(e => e.Patient.Id == message.PatientId
                    && e.Archived == false
                    && e.Id == message.EncounterId, 
                new string[] { 
                    "Patient.PatientClinicalEvents", 
                    "Patient.PatientConditions.Outcome", 
                    "Patient.PatientLabTests.TestUnit", 
                    "Patient.PatientLabTests.LabTest", 
                    "Patient.PatientMedications", 
                    "EncounterType" });

            if (encounterFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate encounter");
            }

            var mappedEncounter = _mapper.Map<EncounterExpandedDto>(encounterFromRepo);

            await CustomMapAsync(encounterFromRepo, mappedEncounter);
            CreateLinks(encounterFromRepo.Patient.Id, encounterFromRepo.Id, mappedEncounter);

            return mappedEncounter;
        }

        private async Task CustomMapAsync(Encounter encounterFromRepo, EncounterExpandedDto mappedEncounter)
        {
            if (encounterFromRepo == null)
            {
                throw new ArgumentNullException(nameof(encounterFromRepo));
            }

            mappedEncounter.Patient = _mapper.Map<PatientDetailDto>(encounterFromRepo.Patient);

            var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter
                    && di.ContextId == mappedEncounter.Id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounterFromRepo.EncounterType.Id
                    , new string[] { "Dataset.ContextType"
                        , "EncounterTypeWorkPlan.EncounterType"
                        , "Dataset.DatasetCategories.DatasetCategoryElements"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldValues"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetCategoryElementConditions"
                        , "DatasetInstanceValues"
                    });

            if (datasetInstanceFromRepo != null)
            {
                var groupedDatasetCategories = datasetInstanceFromRepo.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                mappedEncounter.DatasetCategories = groupedDatasetCategories
                    .Select(dsc => new DatasetCategoryViewDto
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryName = dsc.Key.DatasetCategoryName,
                        DatasetCategoryDisplayed = ShouldCategoryBeDisplayed(encounterFromRepo, dsc.Key),
                        DatasetElements = dsc.Select(element => new DatasetElementViewDto
                        {
                            DatasetElementId = element.DatasetElement.Id,
                            DatasetElementName = element.DatasetElement.ElementName,
                            DatasetElementDisplayName = element.FriendlyName ?? element.DatasetElement.ElementName,
                            DatasetElementHelp = element.Help,
                            DatasetElementDisplayed = ShouldElementBeDisplayed(encounterFromRepo, element),
                            DatasetElementChronic = IsElementChronic(encounterFromRepo, element),
                            DatasetElementSystem = element.DatasetElement.System,
                            DatasetElementType = element.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = datasetInstanceFromRepo.GetInstanceValue(element.DatasetElement.ElementName),
                            StringMaxLength = element.DatasetElement.Field.MaxLength,
                            NumericMinValue = element.DatasetElement.Field.MinSize,
                            NumericMaxValue = element.DatasetElement.Field.MaxSize,
                            Required = element.DatasetElement.Field.Mandatory,
                            SelectionDataItems = element.DatasetElement.Field.FieldValues.Select(fv => new SelectionDataItemDto() { SelectionKey = fv.Value, Value = fv.Value }).ToList(),
                            DatasetElementSubs = element.DatasetElement.DatasetElementSubs.Select(elementSub => new DatasetElementSubViewDto
                            {
                                DatasetElementSubId = elementSub.Id,
                                DatasetElementSubName = elementSub.ElementName,
                                DatasetElementSubType = elementSub.Field.FieldType.Description
                            }).ToArray()
                        })
                        .ToArray()
                    })
                    .ToArray();
            }

            // Condition groups
            int[] terms = _patientConditionRepository.List(pc => pc.Patient.Id == encounterFromRepo.Patient.Id && !pc.Archived && !pc.Patient.Archived, null, new string[] { "TerminologyMedDra" })
                .Select(p => p.TerminologyMedDra.Id)
                .ToArray();

            List<PatientConditionGroupDto> groupArray = new List<PatientConditionGroupDto>();
            foreach (var conditionMeddra in _conditionMeddraRepository.List(cm => terms.Contains(cm.TerminologyMedDra.Id), null, new string[] { "Condition" })
                .ToList())
            {
                var tempCondition = conditionMeddra.GetConditionForPatient(encounterFromRepo.Patient);
                if (tempCondition != null)
                {
                    var group = new PatientConditionGroupDto()
                    {
                        ConditionGroup = conditionMeddra.Condition.Description,
                        Status = tempCondition.OutcomeDate != null ? "Case Closed" : "Case Open",
                        PatientConditionId = tempCondition.Id,
                        StartDate = tempCondition.OnsetDate.ToString("yyyy-MM-dd"),
                        Detail = $"{tempCondition.TerminologyMedDra.DisplayName} started on {tempCondition.OnsetDate.ToString("yyyy-MM-dd")}"
                    };
                    groupArray.Add(group);
                }
            }
            mappedEncounter.ConditionGroups = groupArray;

            // Weight history
            mappedEncounter.WeightSeries = _patientService.GetElementValues(encounterFromRepo.Patient.Id, "Weight (kg)", 5);

            // patient custom mapping
            IExtendable patientExtended = encounterFromRepo.Patient;
            var attribute = patientExtended.GetAttributeValue("Medical Record Number");
            mappedEncounter.Patient.MedicalRecordNumber = attribute != null ? attribute.ToString() : "";
        }

        private bool ShouldCategoryBeDisplayed(Encounter encounter, DatasetCategory datasetCategory)
        {
            if (datasetCategory.Chronic)
            {
                if (!encounter.Patient.HasCondition(datasetCategory.DatasetCategoryConditions.Select(c => c.Condition).ToList()))
                {
                    return false;
                }

            }

            return true;
        }

        private bool ShouldElementBeDisplayed(Encounter encounter, DatasetCategoryElement datasetCategoryElement)
        {
            if (datasetCategoryElement.Chronic)
            {
                // Does patient have chronic condition
                if (!encounter.Patient.HasCondition(datasetCategoryElement.DatasetCategoryElementConditions.Select(c => c.Condition).ToList()))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsElementChronic(Encounter encounter, DatasetCategoryElement datasetCategoryElement)
        {
            // Encounter type is chronic then element must have chronic selected and patient must have condition
            if (datasetCategoryElement.Chronic)
            {
                return !encounter.Patient.HasCondition(datasetCategoryElement.DatasetCategoryElementConditions.Select(c => c.Condition).ToList());
            }
            else
            {
                return false;
            }
        }
        
        private void CreateLinks(int patientId, int encounterId, EncounterExpandedDto mappedEncounter)
        {
            mappedEncounter.Links.Add(new LinkDto(_linkGeneratorService.CreateEncounterForPatientResourceUri(patientId, encounterId), "self", "GET"));
        }
    }
}
