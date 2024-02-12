using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class CohortGroupEnrolmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CohortGroupEnrolmentsController> _logger;

        public CohortGroupEnrolmentsController(IMediator mediator,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IUnitOfWorkInt unitOfWork,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<User> userRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CohortGroupEnrolmentsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all enrolments for a specific cohort group using a valid media type 
        /// </summary>
        /// <param name="cohortGroupId">The unique identifier of the cohort group that is being queried</param>
        /// <param name="cohortGroupEnrolmentResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of EnrolmentDetailDto</returns>
        [HttpGet("cohortgroups/{cohortGroupId}/cohortgroupenrolments", Name = "GetCohortGroupEnrolmentsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<EnrolmentDetailDto>>> GetCohortGroupEnrolmentsByDetail(int cohortGroupId, 
            [FromQuery] CohortGroupEnrolmentResourceParameters cohortGroupEnrolmentResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<EnrolmentDetailDto>
                (cohortGroupEnrolmentResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CohortGroupEnrolmentsDetailQuery(
                cohortGroupId,
                cohortGroupEnrolmentResourceParameters.OrderBy,
                cohortGroupEnrolmentResourceParameters.PageNumber,
                cohortGroupEnrolmentResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CohortGroupEnrolmentsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = cohortGroupEnrolmentResourceParameters.PageSize,
                currentPage = cohortGroupEnrolmentResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single cohortGroupEnrolment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the CohortGroupEnrolment</param>
        /// <returns>An ActionResult of type EnrolmentDto</returns>
        [HttpGet("patients/{patientId}/cohortgroupenrolments/{id}", Name = "GetPatientEnrolmentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<EnrolmentIdentifierDto>> GetPatientEnrolmentByIdentifier(long patientId, long id)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return BadRequest();
            }

            var mappedCohortGroupEnrolment = await GetCohortGroupEnrolmentAsync<EnrolmentIdentifierDto>(patientFromRepo.Id, id);
            if (mappedCohortGroupEnrolment == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCohortGroupEnrolment<EnrolmentIdentifierDto>(mappedCohortGroupEnrolment));
        }

        /// <summary>
        /// Create a new enrolment into a cohort for a patient
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="enrolmentForCreationDto">The enrolment payload</param>
        /// <returns></returns>
        [HttpPost("patients/{patientId}/cohortgroupenrolments", Name = "CreatePatientEnrolment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientEnrolment(long patientId,
            [FromBody] EnrolmentForCreationDto enrolmentForCreationDto)
        {
            if (enrolmentForCreationDto == null)
            {
                ModelState.AddModelError("Message", "Enrolment payload not populated");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate patient record");
                return BadRequest(ModelState);
            }

            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(f => f.Id == enrolmentForCreationDto.CohortGroupId);
            if (cohortGroupFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate the cohort group");
                return BadRequest(ModelState);
            }

            var enrolmentFromRepo = await _cohortGroupEnrolmentRepository.GetAsync(f => f.Patient.Id == patientId && f.CohortGroup.Id == enrolmentForCreationDto.CohortGroupId);
            if (enrolmentFromRepo != null)
            {
                ModelState.AddModelError("Message", "Patient has already been enrolled into this cohort");
                return BadRequest(ModelState);
            }

            var enroledDate = enrolmentForCreationDto.EnrolmentDate.AddDays(1).Date;

            if (enroledDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Enrolment Date should be less than or the same date as today");
                return BadRequest(ModelState);
            }

            var conditionStartDate = patientFromRepo.GetConditionForGroupAndDate(cohortGroupFromRepo.Condition.Description, DateTime.Today).OnsetDate;
            if (enroledDate < conditionStartDate.Date)
            {
                ModelState.AddModelError("Message", "Enrolment Date should be after or the same date as the condition start date");
                return BadRequest(ModelState);
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newEnrolment = new CohortGroupEnrolment()
                {
                    CohortGroup = cohortGroupFromRepo,
                    EnroledDate = enroledDate,
                    Patient = patientFromRepo
                };

                _cohortGroupEnrolmentRepository.Save(newEnrolment);
                id = newEnrolment.Id;
            }

            var mappedEnrolment = await GetEnrolmentAsync(id);
            if (mappedEnrolment == null)
            {
                return StatusCode(500, "Unable to locate newly added enrolment");
            }

            return CreatedAtAction("GetPatientEnrolmentByIdentifier",
                new
                {
                    patientId,
                    id = mappedEnrolment.Id
                }, CreateLinksForEnrolment<EnrolmentIdentifierDto>(patientId, mappedEnrolment));
        }

        /// <summary>
        /// De-enrol a patient from a cohort
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="cohortGroupEnrolmentId">The unique identifier of the cohort group enrolment</param>
        /// <param name="deenrolmentForUpdateDto">The de-enrolment payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/cohortgroupenrolments/{cohortGroupEnrolmentId}", Name = "UpdatePatientDeenrolment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientDeenrolment(long patientId, long cohortGroupEnrolmentId,
            [FromBody] DeenrolmentForUpdateDto deenrolmentForUpdateDto)
        {
            if (deenrolmentForUpdateDto == null)
            {
                ModelState.AddModelError("Message", "De-enrolment payload not populated");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate patient record");
                return BadRequest(ModelState);
            }

            var enrolmentFromRepo = await _cohortGroupEnrolmentRepository.GetAsync(f => f.Id == cohortGroupEnrolmentId);
            if (enrolmentFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate enrolment record");
                return BadRequest(ModelState);
            }

            var deenroledDate = deenrolmentForUpdateDto.DeenroledDate.AddDays(1).Date;

            if (deenroledDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "De-enrolment Date should be less than or the same date as today");
                return BadRequest(ModelState);
            }

            if (deenroledDate < enrolmentFromRepo.EnroledDate.Date)
            {
                ModelState.AddModelError("Message", "De-enrolment Date should be after or the same date as the enrolment date");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                enrolmentFromRepo.DeenroledDate = deenroledDate;

                _cohortGroupEnrolmentRepository.Update(enrolmentFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return CreatedAtAction("GetPatientEnrolmentByIdentifier",
                new
                {
                    patientId,
                    id = enrolmentFromRepo.Id
                }, CreateLinksForEnrolment<EnrolmentIdentifierDto>(patientId, _mapper.Map<EnrolmentIdentifierDto>(enrolmentFromRepo)));
        }

        /// <summary>
        /// Archive a patient enrolment
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="cohortGroupEnrolmentId">The unique identifier of the cohort group enrolment</param>
        /// <param name="enrolmentForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/cohortgroupenrolments/{cohortGroupEnrolmentId}/archive", Name = "ArchivePatientEnrolment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> ArchivePatientEnrolment(long patientId, long cohortGroupEnrolmentId,
            [FromBody] ArchiveDto enrolmentForDelete)
        {
            if (enrolmentForDelete == null)
            {
                ModelState.AddModelError("Message", "Archive payload not populated");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var enrolmentFromRepo = await _cohortGroupEnrolmentRepository.GetAsync(f => f.Id == cohortGroupEnrolmentId);
            if (enrolmentFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(enrolmentForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < enrolmentForDelete.Reason.Length)
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
                enrolmentFromRepo.Archived = true;
                enrolmentFromRepo.ArchivedDate = DateTime.Now;
                enrolmentFromRepo.ArchivedReason = enrolmentForDelete.Reason;
                enrolmentFromRepo.AuditUser = user;

                _cohortGroupEnrolmentRepository.Update(enrolmentFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        private async Task<T> GetCohortGroupEnrolmentAsync<T>(long patientId, long id) where T : class
        {
            var predicate = PredicateBuilder.New<CohortGroupEnrolment>(true);

            // Build remaining expressions
            predicate = predicate.And(f => f.Patient.Id == patientId);
            predicate = predicate.And(f => f.Archived == false);
            predicate = predicate.And(f => f.Id == id);

            var cohortGroupEnrolmentFromRepo = await _cohortGroupEnrolmentRepository.GetAsync(predicate);

            if (cohortGroupEnrolmentFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroupEnrolment = _mapper.Map<T>(cohortGroupEnrolmentFromRepo);

                return mappedCohortGroupEnrolment;
            }

            return null;
        }

        private async Task<EnrolmentIdentifierDto> GetEnrolmentAsync(long enrolmentId)
        {
            var predicate = PredicateBuilder.New<CohortGroupEnrolment>(true);
            predicate = predicate.And(f => f.Id == enrolmentId);

            var enrolmentFromRepo = await _cohortGroupEnrolmentRepository.GetAsync(predicate);

            if (enrolmentFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedEnrolment = _mapper.Map<EnrolmentIdentifierDto>(enrolmentFromRepo);

                return mappedEnrolment;
            }

            return null;
        }

        private EnrolmentIdentifierDto CreateLinksForCohortGroupEnrolment<T>(T dto)
        {
            EnrolmentIdentifierDto identifier = (EnrolmentIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDeenrolmentForPatientResourceUri(identifier.PatientId, identifier.Id), "deenrol", "PUT"));

            return identifier;
        }

        private EnrolmentIdentifierDto CreateLinksForEnrolment<T>(long patientId, T dto)
        {
            EnrolmentIdentifierDto identifier = (EnrolmentIdentifierDto)(object)dto;

            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateUpdateHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "update", "PATCH"));
            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateRemoveHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "marknotcurrent", "DELETE"));

            return identifier;
        }
    }
}
