using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Models.Patient;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    public class PatientsIdentifierQueryHandler
        : IRequestHandler<PatientsIdentifierQuery, LinkedCollectionResourceWrapperDto<PatientIdentifierDto>>
    {
        private readonly IPatientQueries _patientQueries;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PatientsIdentifierQueryHandler> _logger;

        public PatientsIdentifierQueryHandler(
            IPatientQueries patientQueries,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PatientsIdentifierQueryHandler> logger)
        {
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<PatientIdentifierDto>> Handle(PatientsIdentifierQuery message, CancellationToken cancellationToken)
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
                string.Empty,
                message.DateOfBirth == DateTime.MinValue ? null : message.DateOfBirth,
                custom.attributeKey,
                message.CustomAttributeValue);

            var pagedResults = PagedCollection<PatientSearchDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

            if (pagedResults != null)
            {
                var mappedPatientsWithLinks = new List<PatientIdentifierDto>();

                foreach (var pagedPatient in pagedResults)
                {
                    var mappedPatient = _mapper.Map<PatientIdentifierDto>(pagedPatient);

                    CreateLinks(mappedPatient);

                    mappedPatientsWithLinks.Add(mappedPatient);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<PatientIdentifierDto>(pagedResults.TotalCount, mappedPatientsWithLinks, pagedResults.TotalPages);

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

        private void CreateLinks(PatientIdentifierDto mappedPatient)
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
