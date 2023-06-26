using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Models.Encounter;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    public class EncountersDetailQueryHandler
        : IRequestHandler<EncountersDetailQuery, LinkedCollectionResourceWrapperDto<EncounterDetailDto>>
    {
        private readonly IEncounterQueries _encounterQueries;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EncountersDetailQueryHandler> _logger;

        public EncountersDetailQueryHandler(
            IEncounterQueries encounterQueries,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EncountersDetailQueryHandler> logger)
        {
            _encounterQueries = encounterQueries ?? throw new ArgumentNullException(nameof(encounterQueries));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<EncounterDetailDto>> Handle(EncountersDetailQuery message, CancellationToken cancellationToken)
        {
            return await GetEncountersAsync(message.PageNumber, message.PageSize, message.OrderBy, message.FacilityName, message.CustomAttributeId, message.CustomAttributeValue, message.PatientId, message.FirstName, message.LastName, message.SearchFrom, message.SearchTo);
        }

        private async Task<LinkedCollectionResourceWrapperDto<EncounterDetailDto>> GetEncountersAsync( 
            int pageNumber, 
            int pageSize,
            string orderBy,
            string facilityName,
            int customAttributeId,
            string customAttributeValue,
            int patientId,
            string firstName,
            string lastName,
            DateTime searchFrom,
            DateTime searchTo)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var currentUserId = await GetUserIdAsync();
            var searchFacilityId = await GetFacilityIdAsync(facilityName);
            var custom = await GetCustomAsync(customAttributeId);

            var results = await _encounterQueries.SearchEncountersAsync(
                currentUserId,
                searchFacilityId,
                patientId == 0 ? null : patientId,
                firstName,
                lastName,
                searchFrom == DateTime.MinValue ? null : searchFrom,
                searchTo == DateTime.MinValue ? null : searchTo,
                custom.attributeKey,
                customAttributeValue);

            var pagedResults = PagedCollection<SearchEncounterDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

            if (pagedResults != null)
            {
                var mappedEncountersWithLinks = new List<EncounterDetailDto>();

                foreach (var pagedEncounter in pagedResults)
                {
                    var mappedEncounter = _mapper.Map<EncounterDetailDto>(pagedEncounter);

                    await CustomMapAsync(pagedEncounter, mappedEncounter);

                    CreateLinks(pagedEncounter.PatientId, pagedEncounter.EncounterId, mappedEncounter);

                    mappedEncountersWithLinks.Add(mappedEncounter);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<EncounterDetailDto>(pagedResults.TotalCount, mappedEncountersWithLinks, pagedResults.TotalPages);

                CreateLinksForEncounters(wrapper, orderBy, facilityName, searchFrom, searchTo, pageNumber, pageSize,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private async Task<int> GetUserIdAsync()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName, new string[] { });
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }
            return userFromRepo.Id;
        }

        private async Task<int> GetFacilityIdAsync(string facilityName)
        {
            if (!String.IsNullOrWhiteSpace(facilityName))
            {
                var facility = await _facilityRepository.GetAsync(f => f.FacilityName == facilityName);
                if (facility != null)
                {
                    return facility.Id;
                }
            }
            return 0;
        }

        private async Task<(string path, string attributeKey)> GetCustomAsync(int customAttributeId)
        {
            if (customAttributeId > 0)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == customAttributeId);
                if(customAttribute!= null)
                {
                    return (customAttribute.CustomAttributeType == CustomAttributeType.Selection ? "CustomSelectionAttribute" : "CustomStringAttribute", customAttribute.AttributeKey);
                }
            }

            return (string.Empty, string.Empty);
        }

        private async Task CustomMapAsync(SearchEncounterDto encounterListFromRepo, EncounterDetailDto mappedEncounterDto)
        {
            if (encounterListFromRepo == null)
            {
                throw new ArgumentNullException(nameof(encounterListFromRepo));
            }

            if (mappedEncounterDto == null)
            {
                throw new ArgumentNullException(nameof(mappedEncounterDto));
            }

            var encounterFromRepo = await _encounterRepository.GetAsync(p => p.Id == mappedEncounterDto.Id, new string[] { "Patient.PatientConditions", "Patient.PatientFacilities.Facility", "EncounterType" });
            if (encounterFromRepo == null)
            {
                return;
            }

            mappedEncounterDto.Patient = _mapper.Map<PatientDetailDto>(encounterFromRepo.Patient);

            var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter
                    && di.ContextId == mappedEncounterDto.Id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounterFromRepo.EncounterType.Id
                    , new string[] { "Dataset.ContextType"
                        , "EncounterTypeWorkPlan.EncounterType"
                        , "Dataset.DatasetCategories.DatasetCategoryElements"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldValues"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType"
                        , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetCategoryElementConditions" });

            if (datasetInstanceFromRepo != null)
            {
                var groupedDatasetCategories = datasetInstanceFromRepo.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                mappedEncounterDto.DatasetCategories = groupedDatasetCategories
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
        }

        private void CreateLinks(int patientId, int encounterId, EncounterDetailDto mappedEncounter)
        {
            mappedEncounter.Links.Add(new LinkDto(_linkGeneratorService.CreateEncounterForPatientResourceUri(patientId, encounterId), "self", "GET"));
        }

        private void CreateLinksForEncounters(LinkedResourceBaseDto wrapper, 
            string orderBy, string facilityName, DateTime searchFrom, DateTime searchTo, int pageNumber, int pageSize, bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateEncountersResourceUri(ResourceUriType.Current, orderBy, facilityName, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateEncountersResourceUri(ResourceUriType.NextPage, orderBy, facilityName, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateEncountersResourceUri(ResourceUriType.PreviousPage, orderBy, facilityName, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
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
    }
}
