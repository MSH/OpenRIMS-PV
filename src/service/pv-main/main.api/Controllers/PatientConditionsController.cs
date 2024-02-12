using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientConditionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientStatus> _patientStatusRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IRepositoryInt<Outcome> _outcomeRepository;
        private readonly IRepositoryInt<TreatmentOutcome> _treatmentOutcomeRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PatientConditionsController> _logger;

        public PatientConditionsController(IMediator mediator,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientStatus> patientStatusRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IRepositoryInt<Outcome> outcomeRepository,
            IRepositoryInt<TreatmentOutcome> treatmentOutcomeRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PatientConditionsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientStatusRepository = patientStatusRepository ?? throw new ArgumentNullException(nameof(patientStatusRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _outcomeRepository = outcomeRepository ?? throw new ArgumentNullException(nameof(outcomeRepository));
            _treatmentOutcomeRepository = treatmentOutcomeRepository ?? throw new ArgumentNullException(nameof(treatmentOutcomeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient condition using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient condition</param>
        /// <returns>An ActionResult of type PatientConditionIdentifierDto</returns>
        [HttpGet("{patientId}/conditions/{id}", Name = "GetPatientConditionByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<PatientConditionIdentifierDto>> GetPatientConditionByIdentifier(long patientId, long id)
        {
            var mappedPatientCondition = await GetPatientConditionAsync<PatientConditionIdentifierDto>(patientId, id);
            if (mappedPatientCondition == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientCondition<PatientConditionIdentifierDto>(mappedPatientCondition));
        }

        /// <summary>
        /// Get a single patient condition using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient condition</param>
        /// <returns>An ActionResult of type PatientConditionDetailDto</returns>
        [HttpGet("{patientId}/conditions/{id}", Name = "GetPatientConditionByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientConditionDetailDto>> GetPatientConditionByDetail(int patientId, int id)
        {
            var query = new PatientConditionDetailQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientConditionDetailQuery - {patientId} : {id}",
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
        /// Create a new patient condition record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="conditionForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/conditions", Name = "CreatePatientCondition")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientCondition(int patientId, 
            [FromBody] PatientConditionForUpdateDto conditionForUpdate)
        {
            if (conditionForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new condition");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var sourceTermFromRepo = _terminologyMeddraRepository.Get(conditionForUpdate.SourceTerminologyMedDraId);
            if (sourceTermFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate source term");
            }

            Outcome outcomeFromRepo = null;
            if(!String.IsNullOrWhiteSpace(conditionForUpdate.Outcome))
            {
                outcomeFromRepo = _outcomeRepository.Get(o => o.Description == conditionForUpdate.Outcome);
                if (outcomeFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate outcome");
                }
            }

            TreatmentOutcome treatmentOutcomeFromRepo = null;
            if (!String.IsNullOrWhiteSpace(conditionForUpdate.TreatmentOutcome))
            {
                treatmentOutcomeFromRepo = _treatmentOutcomeRepository.Get(to => to.Description == conditionForUpdate.TreatmentOutcome);
                if (treatmentOutcomeFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate treatment outcome");
                }
            }

            ValidateConditionForUpdateModel(patientFromRepo, conditionForUpdate, 0);

            // Custom validation
            if (outcomeFromRepo != null && treatmentOutcomeFromRepo != null)
            {
                if (outcomeFromRepo.Description == "Fatal" && treatmentOutcomeFromRepo.Description != "Died")
                {
                    ModelState.AddModelError("Message", "Treatment Outcome not consistent with Condition Outcome");
                }
                if (outcomeFromRepo.Description != "Fatal" && treatmentOutcomeFromRepo.Description == "Died")
                {
                    ModelState.AddModelError("Message", "Condition Outcome not consistent with Treatment Outcome");
                }
            }

            if (ModelState.IsValid)
            {
                var conditionDetail = PrepareConditionDetail(conditionForUpdate);
                if (!conditionDetail.IsValid())
                {
                    conditionDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    var patientCondition = patientFromRepo.AddOrUpdatePatientCondition(0,
                                sourceTermFromRepo, 
                                conditionForUpdate.StartDate, 
                                conditionForUpdate.OutcomeDate, 
                                outcomeFromRepo, 
                                treatmentOutcomeFromRepo, 
                                conditionForUpdate.CaseNumber,
                                conditionForUpdate.Comments, 
                                conditionForUpdate.SourceDescription,
                                _patientStatusRepository.Get(ps => ps.Description == "Died"));

                    //throw new Exception(JsonConvert.SerializeObject(patientCondition));
                    _modelExtensionBuilder.UpdateExtendable(patientCondition, conditionDetail.CustomAttributes, "Admin");

                    _patientConditionRepository.Save(patientCondition);
                    await _unitOfWork.CompleteAsync();

                    var mappedPatientCondition = _mapper.Map<PatientConditionIdentifierDto>(patientCondition);
                    if (mappedPatientCondition == null)
                    {
                        return StatusCode(500, "Unable to locate newly added condition");
                    }

                    return CreatedAtAction("GetPatientConditionByIdentifier",
                        new
                        {
                            id = mappedPatientCondition.Id
                        }, CreateLinksForPatientCondition<PatientConditionIdentifierDto>(mappedPatientCondition));
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing patient condition
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the condition</param>
        /// <param name="conditionForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/conditions/{id}", Name = "UpdatePatientCondition")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientCondition(int patientId, int id,
            [FromBody] PatientConditionForUpdateDto conditionForUpdate)
        {
            if (conditionForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeConditionDetailsCommand(patientId, id, conditionForUpdate.SourceTerminologyMedDraId, conditionForUpdate.StartDate, conditionForUpdate.OutcomeDate, conditionForUpdate.Outcome, conditionForUpdate.TreatmentOutcome, conditionForUpdate.CaseNumber, conditionForUpdate.Comments, conditionForUpdate.Attributes);

            _logger.LogInformation(
                "----- Sending command: ChangeConditionDetailsCommand - {patientId}: {patientConditionId}",
                command.PatientId,
                command.PatientConditionId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient condition
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the condition</param>
        /// <param name="conditionForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/conditions/{id}/archive", Name = "ArchivePatientCondition")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientCondition(long patientId, long id,
            [FromBody] ArchiveDto conditionForDelete)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var conditionFromRepo = await _patientConditionRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(conditionForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < conditionForDelete.Reason.Length)
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
                conditionFromRepo.Archived = true;
                conditionFromRepo.ArchivedDate = DateTime.Now;
                conditionFromRepo.ArchivedReason = conditionForDelete.Reason;
                conditionFromRepo.AuditUser = user;
                _patientConditionRepository.Update(conditionFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get single patient condition from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientConditionAsync<T>(long patientId, long id) where T : class
        {
            var patientConditionFromRepo = await _patientConditionRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientConditionFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientCondition = _mapper.Map<T>(patientConditionFromRepo);

                return mappedPatientCondition;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientConditionIdentifierDto CreateLinksForPatientCondition<T>(T dto)
        {
            PatientConditionIdentifierDto identifier = (PatientConditionIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientCondition", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Validate the input model for updating a condition
        /// </summary>
        private void ValidateConditionForUpdateModel(Patient patientFromRepo, PatientConditionForUpdateDto conditionForUpdateDto, long patientConditionId)
        {
            if (Regex.Matches(conditionForUpdateDto.SourceDescription, @"[-a-zA-Z0-9 .,()']").Count < conditionForUpdateDto.SourceDescription.Length)
            {
                ModelState.AddModelError("Message", "Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");
            }

            if (conditionForUpdateDto.StartDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Start Date should be before current date");
            }
            if (conditionForUpdateDto.StartDate < patientFromRepo.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Start Date should be after Date Of Birth");
            }

            if(conditionForUpdateDto.OutcomeDate.HasValue)
            {
                if (conditionForUpdateDto.OutcomeDate > DateTime.Today)
                {
                    ModelState.AddModelError("Message", "Outcome Date should be before current date");
                }
                if (conditionForUpdateDto.OutcomeDate < patientFromRepo.DateOfBirth)
                {
                    ModelState.AddModelError("Message", "Outcome Date should be after Date Of Birth");
                }
                if (conditionForUpdateDto.OutcomeDate < conditionForUpdateDto.StartDate)
                {
                    ModelState.AddModelError("Message", "Outcome Date should be after Start Date");
                }
            }
        }

        /// <summary>
        /// Prepare the model for the condition
        /// </summary>
        private ConditionDetail PrepareConditionDetail(PatientConditionForUpdateDto conditionForUpdate)
        {
            var conditionDetail = new ConditionDetail();
            conditionDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientCondition>();

            //conditionDetail = _mapper.Map<ConditionDetail>(conditionForUpdate);
            foreach (var newAttribute in conditionForUpdate.Attributes)
            {
                var customAttribute = _customAttributeRepository.Get(ca => ca.Id == newAttribute.Key);
                if (customAttribute != null)
                {
                // Validate attribute exists for household entity and is a PMT attribute
                var attributeDetail = conditionDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                    
                if (attributeDetail == null)
                    {
                        ModelState.AddModelError("Message", $"Unable to locate custom attribute on patient condition {newAttribute.Key}");
                    }
                    else
                    {
                        attributeDetail.Value = newAttribute.Value;
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", $"Unable to locate custom attribute {newAttribute.Key}");
                }
            }
            // Update patient custom attributes from source
            return conditionDetail;
        }
    }
}
