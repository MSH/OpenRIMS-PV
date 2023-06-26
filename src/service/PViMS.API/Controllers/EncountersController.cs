using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Queries.EncounterAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PVIMS.Core.Aggregates.UserAggregate;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class EncountersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IPatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EncountersController> _logger;

        public EncountersController(IMediator mediator, 
            IPropertyMappingService propertyMappingService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<DatasetElement> datasetElementRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<Attachment> attachmentRepository,
            IUnitOfWorkInt unitOfWork,
            IPatientService patientService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EncountersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all encounters using a valid media type 
        /// </summary>
        /// <param name="encounterResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of EncounterDetailDto</returns>
        [HttpGet("encounters", Name = "GetEncountersByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<EncounterDetailDto>>> GetEncountersByDetail(
            [FromQuery] EncounterResourceParameters encounterResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<EncounterDetailDto, Encounter>
               (encounterResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new EncountersDetailQuery(encounterResourceParameters.OrderBy,
                encounterResourceParameters.FacilityName,
                encounterResourceParameters.CustomAttributeId,
                encounterResourceParameters.CustomAttributeValue,
                encounterResourceParameters.PatientId,
                encounterResourceParameters.SearchFrom,
                encounterResourceParameters.SearchTo,
                encounterResourceParameters.FirstName,
                encounterResourceParameters.LastName,
                encounterResourceParameters.PageNumber,
                encounterResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: GetEncountersDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = encounterResourceParameters.PageSize,
                currentPage = encounterResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Encounter using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Encounter</param>
        /// <returns>An ActionResult of type EncounterIdentifierDto</returns>
        [HttpGet("patients/{patientId}/encounters/{id}", Name = "GetEncounterByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<EncounterIdentifierDto>> GetEncounterByIdentifier(long patientId, long id)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return BadRequest();
            }

            var mappedEncounter = await GetEncounterAsync<EncounterIdentifierDto>(patientFromRepo.Id, id);
            if (mappedEncounter == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForEncounter<EncounterIdentifierDto>(patientFromRepo.Id, mappedEncounter));
        }

        /// <summary>
        /// Get a single Encounter using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Encounter</param>
        /// <returns>An ActionResult of type EncounterDetailDto</returns>
        [HttpGet("patients/{patientId}/encounters/{id}", Name = "GetEncounterByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<EncounterDetailDto>> GetEncounterByDetail(int patientId, int id)
        {
            var query = new EncounterDetailQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: GetEncounterDetailQuery - {patientId}: {id}",
                patientId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Encounter using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Encounter</param>
        /// <returns>An ActionResult of type EncounterExpandedDto</returns>
        [HttpGet("patients/{patientId}/encounters/{id}", Name = "GetEncounterByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<EncounterExpandedDto>> GetEncounterByExpanded(int patientId, int id)
        {
            var query = new EncounterExpandedQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: GetEncounterExpandedQuery - {patientId}: {id}",
                patientId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new encounter record
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="encounterForCreation">The encounter payload</param>
        /// <returns></returns>
        [HttpPost("patients/{patientId}/encounters", Name = "CreateEncounter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateEncounter(long patientId, [FromBody] EncounterForCreationDto encounterForCreation)
        {
            if (encounterForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new encounter");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == patientId);
            if (patientFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate patient");
            }

            var encounterType = _encounterTypeRepository.Get(et => et.Id == encounterForCreation.EncounterTypeId);
            if (encounterType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate encounter type");
            }

            if (encounterForCreation.EncounterDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Encounter date should be before current date");
            }
            if (encounterForCreation.EncounterDate < patientFromRepo.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Encounter date should be after date of birth");
            }

            if (!String.IsNullOrEmpty(encounterForCreation.Notes))
            {
                if (Regex.Matches(encounterForCreation.Notes, @"[-a-zA-Z0-9<>/ '.]").Count < encounterForCreation.Notes.Length)
                {
                    ModelState.AddModelError("Message", "Notes contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe, period, hyphen)");
                }
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var encounterDetail = PrepareEncounterDetail(encounterForCreation);
                id = await _patientService.AddEncounterAsync(patientFromRepo, encounterDetail);
                await _unitOfWork.CompleteAsync();

                var mappedEncounter = await GetEncounterAsync<EncounterIdentifierDto>(patientId, id);
                if (mappedEncounter == null)
                {
                    return StatusCode(500, "Unable to locate newly added encounter");
                }

                return CreatedAtAction("GetEncounterByIdentifier",
                    new
                    {
                        id = mappedEncounter.Id
                    }, CreateLinksForEncounter<EncounterIdentifierDto>(patientId, mappedEncounter));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing encounter
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the encounter</param>
        /// <param name="encounterForUpdate">The encounter payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/encounters/{id}", Name = "UpdateEncounter")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateEncounter(long patientId, long id,
            [FromBody] EncounterForUpdateDto encounterForUpdate)
        {
            if (encounterForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var encounterFromRepo = await _encounterRepository.GetAsync(e => e.Patient.Id == patientId && e.Id == id, new string[] { "EncounterType" });
            if (encounterFromRepo == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(encounterForUpdate.Notes))
            {
                if (Regex.Matches(encounterForUpdate.Notes, @"[-a-zA-Z0-9<>/ '.]").Count < encounterForUpdate.Notes.Length)
                {
                    ModelState.AddModelError("Message", "Notes contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe, period, hyphen)");
                }
            }

            foreach (var element in encounterForUpdate.Elements)
            {
                var datasetElement = _datasetElementRepository.Get(de => de.Id == element.Key, new string[] { "Field.FieldType" });
                if(datasetElement == null)
                {
                    ModelState.AddModelError("Message", $"Unable to locate dataset element {datasetElement.Id}");
                }
                else
                {
                    if(!datasetElement.System)
                    {
                        try
                        {
                            datasetElement.Validate(element.Value);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("Message", ex.Message);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                encounterFromRepo.Notes = encounterForUpdate.Notes;
                _encounterRepository.Update(encounterFromRepo);

                var contextTypeId = (int)ContextTypes.Encounter;
                var datasetInstanceFromRepo = _datasetInstanceRepository.Get(
                    di => di.Dataset.ContextType.Id == contextTypeId
                    && di.ContextId == id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounterFromRepo.EncounterType.Id, new string[] {
                        "Dataset",
                        "DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub",
                        "DatasetInstanceValues.DatasetElement"
                    });

                if (datasetInstanceFromRepo != null)
                {
                    foreach (var element in encounterForUpdate.Elements)
                    {
                        var datasetElement = _datasetElementRepository.Get(de => de.Id == element.Key);
                        if (!datasetElement.System)
                        {
                            datasetInstanceFromRepo.SetInstanceValue(datasetElement, element.Value);
                        }
                    }

                    _datasetInstanceRepository.Update(datasetInstanceFromRepo);
                }

                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Archive an existing encounter
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the encounter</param>
        /// <param name="encounterForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/encounters/{id}/archive", Name = "ArchiveEncounter")]
        [Authorize(Roles = "Clinician")]
        public async Task<IActionResult> ArchiveEncounter(long patientId, long id,
            [FromBody] ArchiveDto encounterForDelete)
        {
            var encounterFromRepo = await _encounterRepository.GetAsync(e => e.Patient.Id == patientId && e.Id == id);
            if (encounterFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(encounterForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < encounterForDelete.Reason.Length)
            {
                ModelState.AddModelError("Message", "Reason contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("Message", "Unable to locate user");
            }

            if (ModelState.IsValid)
            {
                foreach (var attachment in encounterFromRepo.Attachments.Where(x => !x.Archived))
                {
                    attachment.ArchiveAttachment(user, encounterForDelete.Reason);
                    _attachmentRepository.Update(attachment);
                }

                foreach (var patientClinicalEvent in encounterFromRepo.PatientClinicalEvents.Where(x => !x.Archived))
                {
                    patientClinicalEvent.Archive(user, encounterForDelete.Reason);
                    _patientClinicalEventRepository.Update(patientClinicalEvent);
                }

                encounterFromRepo.Archived = true;
                encounterFromRepo.ArchivedDate = DateTime.Now;
                encounterFromRepo.ArchivedReason = encounterForDelete.Reason;
                encounterFromRepo.AuditUser = user;

                _encounterRepository.Update(encounterFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get single Encounter from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">unique identifier of the patient </param>
        /// <param name="id">Resource guid to search by</param>
        /// <returns></returns>
        private async Task<T> GetEncounterAsync<T>(long patientId, long id) where T : class
        {
            var predicate = PredicateBuilder.New<Encounter>(true);

            // Build remaining expressions
            predicate = predicate.And(f => f.Patient.Id == patientId);
            predicate = predicate.And(f => f.Archived == false);
            predicate = predicate.And(f => f.Id == id);

            var encounterFromRepo = await _encounterRepository.GetAsync(predicate);

            if (encounterFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedEncounter = _mapper.Map<T>(encounterFromRepo);

                return mappedEncounter;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private EncounterIdentifierDto CreateLinksForEncounter<T>(long patientId, T dto)
        {
            EncounterIdentifierDto identifier = (EncounterIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateEncounterForPatientResourceUri(patientId, identifier.Id), "self", "GET"));

            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateUpdateHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "update", "PATCH"));
            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateRemoveHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "marknotcurrent", "DELETE"));

            return identifier;
        }

        /// <summary>
        /// Prepare the model for adding a new encounter
        /// </summary>
        private EncounterDetail PrepareEncounterDetail(EncounterForCreationDto encounterForCreation)
        {
            var encounterDetail = new EncounterDetail();
            encounterDetail = _mapper.Map<EncounterDetail>(encounterForCreation);

            return encounterDetail;
        }

        /// <summary>
        /// Determine if this element should be displayed
        /// </summary>
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

        /// <summary>
        /// Determine if the category should be displayed
        /// </summary>
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

        /// <summary>
        /// Determine if this element is chronic in nature
        /// </summary>
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