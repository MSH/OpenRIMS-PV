using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    public class EncounterDetailQueryHandler
        : IRequestHandler<EncounterDetailQuery, EncounterDetailDto>
    {
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<EncounterDetailQueryHandler> _logger;

        public EncounterDetailQueryHandler(
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<Encounter> encounterRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<EncounterDetailQueryHandler> logger)
        {
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EncounterDetailDto> Handle(EncounterDetailQuery message, CancellationToken cancellationToken)
        {
            var encounterFromRepo = await _encounterRepository.GetAsync(e => e.Patient.Id == message.PatientId
                    && e.Archived == false
                    && e.Id == message.EncounterId, 
                new string[] { "Patient" });

            if (encounterFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate encounter");
            }

            var mappedEncounter = _mapper.Map<EncounterDetailDto>(encounterFromRepo);

            await CustomMapAsync(encounterFromRepo, mappedEncounter);
            CreateLinks(encounterFromRepo.Patient.Id, encounterFromRepo.Id, mappedEncounter);

            return mappedEncounter;
        }

        private async Task CustomMapAsync(Encounter encounterFromRepo, EncounterDetailDto mappedEncounter)
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
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldValues"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetCategoryElementConditions"
                        , "DatasetInstanceValues"});

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
        
        private void CreateLinks(int patientId, int encounterId, EncounterDetailDto mappedEncounter)
        {
            mappedEncounter.Links.Add(new LinkDto(_linkGeneratorService.CreateEncounterForPatientResourceUri(patientId, encounterId), "self", "GET"));
        }
    }
}
