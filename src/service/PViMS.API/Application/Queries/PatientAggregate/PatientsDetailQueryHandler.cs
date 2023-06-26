using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Models.Patient;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientsDetailQueryHandler
        : IRequestHandler<PatientsDetailQuery, LinkedCollectionResourceWrapperDto<PatientDetailDto>>
    {
        private readonly IPatientQueries _patientQueries;
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PatientsDetailQueryHandler> _logger;

        public PatientsDetailQueryHandler(
            IPatientQueries patientQueries,
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IRepositoryInt<User> userRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PatientsDetailQueryHandler> logger)
        {
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<PatientDetailDto>> Handle(PatientsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var currentUserId = await GetUserIdAsync();
            var searchFacilityId = await GetFacilityIdAsync(message.FacilityName);
            var custom = await GetCustomAsync(message.CustomAttributeId);

            var results = await _patientQueries.SearchPatientsAsync(
                currentUserId,
                searchFacilityId, 
                message.PatientId == 0 ? null : message.PatientId,
                message.FirstName,
                message.LastName,
                message.CaseNumber,
                message.DateOfBirth == DateTime.MinValue ? null : message.DateOfBirth,
                custom.attributeKey,
                message.CustomAttributeValue);

            var pagedResults = PagedCollection<PatientSearchDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

            if (pagedResults != null)
            {
                var mappedPatientsWithLinks = new List<PatientDetailDto>();

                foreach (var pagedPatient in pagedResults)
                {
                    var mappedPatient = _mapper.Map<PatientDetailDto>(pagedPatient);

                    await CustomMapAsync(pagedPatient, mappedPatient);
                    await MapCaseNumberAsync(mappedPatient);

                    CreateLinks(mappedPatient);

                    mappedPatientsWithLinks.Add(mappedPatient);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<PatientDetailDto>(pagedResults.TotalCount, mappedPatientsWithLinks, pagedResults.TotalPages);

                CreateLinksForPatients(wrapper, message.OrderBy, message.FacilityName, message.PageNumber, message.PageSize,
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

        private async Task<int?> GetFacilityIdAsync(string facilityName)
        {
            if (!String.IsNullOrWhiteSpace(facilityName))
            {
                var facility = await _facilityRepository.GetAsync(f => f.FacilityName == facilityName);
                if (facility != null)
                {
                    return facility.Id;
                }
            }
            return null;
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

        private async Task CustomMapAsync(PatientSearchDto patientListFromRepo, PatientDetailDto mappedPatient)
        {
            if (patientListFromRepo == null)
            {
                throw new ArgumentNullException(nameof(patientListFromRepo));
            }

            if (mappedPatient == null)
            {
                throw new ArgumentNullException(nameof(mappedPatient));
            }

            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == mappedPatient.Id);
            if (patientFromRepo == null)
            {
                return;
            }

            IExtendable patientExtended = patientFromRepo;

            // Map all custom attributes
            mappedPatient.PatientAttributes = _modelExtensionBuilder.BuildModelExtension(patientExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.Value.ToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            var attribute = patientExtended.GetAttributeValue("Medical Record Number");
            mappedPatient.MedicalRecordNumber = attribute != null ? attribute.ToString() : "";
        }

        private async Task MapCaseNumberAsync(PatientDetailDto mappedPatient)
        {
            int[] terms = _patientConditionRepository.List(pc => pc.Patient.Id == mappedPatient.Id && pc.TerminologyMedDra != null && !pc.Archived && !pc.Patient.Archived, null, new string[] { "Condition", "TerminologyMedDra" })
                .Select(p => p.TerminologyMedDra.Id)
                .ToArray();
            var conditionMeddras = await _conditionMeddraRepository.ListAsync(cm => terms.Contains(cm.TerminologyMedDra.Id), null, new string[] { "Condition", "TerminologyMedDra" });
            var patientFromRepo = await _patientRepository.GetAsync(p => p.Archived == false
                    && p.Id == mappedPatient.Id,
                new string[] { "PatientConditions.TerminologyMedDra", "PatientConditions.Outcome", "PatientConditions.TreatmentOutcome" });

            List<PatientConditionGroupDto> groupArray = new List<PatientConditionGroupDto>();
            foreach (var conditionMeddra in conditionMeddras)
            {
                var currentConditionGroup = conditionMeddra.GetConditionForPatient(patientFromRepo);
                if (currentConditionGroup != null)
                {
                    if(!String.IsNullOrWhiteSpace(currentConditionGroup.CaseNumber))
                    {
                        mappedPatient.CaseNumber.Add(currentConditionGroup.CaseNumber);
                    }
                }
            }
        }

        private string GetSelectionValue(CustomAttributeType attributeType, string attributeKey, string selectionKey)
        {
            if(attributeType == CustomAttributeType.Selection)
            {
                var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

                return selectionitem == null ? string.Empty : selectionitem.Value;
            }

            return "";
        }

        private void CreateLinks(PatientDetailDto mappedPatient)
        {
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", mappedPatient.Id), "self", "GET"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(mappedPatient.Id), "newAppointment", "POST"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(mappedPatient.Id), "newEnrolment", "POST"));
        }

        private void CreateLinksForPatients(LinkedResourceBaseDto wrapper, 
            string orderBy, string facilityName, int pageNumber, int pageSize, bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreatePatientsResourceUri(ResourceUriType.Current, orderBy, facilityName, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientsResourceUri(ResourceUriType.NextPage, orderBy, facilityName, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientsResourceUri(ResourceUriType.PreviousPage, orderBy, facilityName, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
