using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.MetaFormAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/metaforms")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class MetaFormsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly FormHandler _formHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MetaFormsController> _logger;

        public MetaFormsController(
            IMediator mediator,
            ITypeHelperService typeHelperService,
            IRepositoryInt<MetaForm> metaFormRepository,
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            FormHandler formHandler,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MetaFormsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _formHandler = formHandler ?? throw new ArgumentNullException(nameof(formHandler));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single meta form unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta form</param>
        /// <returns>An ActionResult of type MetaFormIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetMetaFormByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<MetaFormIdentifierDto>> GetMetaFormByIdentifier(int id)
        {
            var query = new MetaFormIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: MetaFormIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single meta form unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta form</param>
        /// <returns>An ActionResult of type MetaFormExpandedDto</returns>
        [HttpGet("{id}", Name = "GetMetaFormByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.expanded.v1+json", "application/vnd.main.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.expanded.v1+json", "application/vnd.main.expanded.v1+xml")]
        public async Task<ActionResult<MetaFormExpandedDto>> GetMetaFormByExpanded(int id)
        {
            var query = new MetaFormExpandedQuery(id);

            _logger.LogInformation(
                "----- Sending query: MetaFormExpandedQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all meta forms using a valid media type 
        /// </summary>
        /// <param name="metaFormResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaFormDetailDto</returns>
        [HttpGet(Name = "GetMetaFormsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<MetaFormDetailDto>>> GetMetaFormsByDetail(
            [FromQuery] MetaFormResourceParameters metaFormResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaFormDetailDto>
                (metaFormResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new MetaFormsDetailQuery(
                metaFormResourceParameters.OrderBy,
                metaFormResourceParameters.PageNumber,
                metaFormResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: MetaFormsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = metaFormResourceParameters.PageSize,
                currentPage = metaFormResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new meta form
        /// </summary>
        /// <param name="metaFormForUpdate">The meta form payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateMetaForm")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMetaForm(
            [FromBody] MetaFormForUpdateDto metaFormForUpdate)
        {
            if (metaFormForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new AddMetaFormCommand(
                metaFormForUpdate.CohortGroupId, 
                metaFormForUpdate.FormName, 
                metaFormForUpdate.ActionName);

            _logger.LogInformation(
                "----- Sending command: AddMetaFormCommand - {FormName}",
                command.FormName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetMetaFormByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Handle form upload and creation
        /// </summary>
        /// <param name="formForCreation">The form payload</param>
        /// <returns></returns>
        [HttpPut(Name = "CreateForm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateForm([FromBody] FormForCreationDto formForCreation)
        {
            if (formForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new form");
                return BadRequest(ModelState);
            }

            if (formForCreation.HasAttachment == false)
            {
                ModelState.AddModelError("Message", "Unable to process form as no attachment has been submitted");
                return BadRequest(ModelState);
            }

            // Meta form for template extraction
            var metaFormFromRepo = await _metaFormRepository.GetAsync(f => f.ActionName.ToLower().Replace(" ", "") == formForCreation.FormType.ToLower().Replace(" ", ""));
            if (metaFormFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate meta form for template extraction");
                return BadRequest(ModelState);
            }

            // Store user for audit log generation purposes
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = _userRepository.Get(u => u.UserName == userName);
            AuditLog auditLog = null;

            // TODO Use meta form to identify context handler (add/update encounter/patient)

            //var handler = new FormValueHandler(formForCreation.FormValues.Where(fv => fv.FormControlKey == "Object").Select(fv => fv.FormControlValue).ToList());
            _formHandler.SetForm(formForCreation);

            // Validation of the source entity
            _formHandler.ValidateSourceIdentifier();
            if (_formHandler.GetValidationErrors().Count > 0)
            {
                foreach (string message in _formHandler.GetValidationErrors())
                {
                    ModelState.AddModelError("Message", message);
                }
            }

            if (ModelState.IsValid)
            {
                _formHandler.PreparePatientAndClinicalDetail();
                if (_formHandler.GetValidationErrors().Count > 0)
                {
                    foreach (string message in _formHandler.GetValidationErrors())
                    {
                        ModelState.AddModelError("Message", message);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    auditLog = new AuditLog()
                    {
                        AuditType = AuditType.SynchronisationForm,
                        User = userFromRepo,
                        ActionDate = DateTime.Now,
                        Details = $"Form submission successful {formForCreation.FormIdentifier}",
                        Log = JsonConvert.SerializeObject(formForCreation, Formatting.Indented)
                    };
                    await _auditLogRepository.SaveAsync(auditLog);

                    await _formHandler.ProcessFormForCreationOrUpdateAsync();
                    await _unitOfWork.CompleteAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    var error = new Dictionary<string, string>
                    {
                        {"Type", ex.GetType().ToString()},
                        {"Message", ex.Message},
                        {"StackTrace", ex.StackTrace}
                    };
                    auditLog = new AuditLog()
                    {
                        AuditType = AuditType.SynchronisationError,
                        User = userFromRepo,
                        ActionDate = DateTime.Now,
                        Details = $"Error on form {formForCreation.FormIdentifier}",
                        Log = JsonConvert.SerializeObject(error, Formatting.Indented)
                    };
                    _auditLogRepository.Save(auditLog);

                    return StatusCode(500, ex.Message + " " + ex.InnerException?.Message);
                }
            }

            var audit = new AuditLog()
            {
                AuditType = AuditType.SynchronisationError,
                User = userFromRepo,
                ActionDate = DateTime.Now,
                Details = $"Form submission with model error {formForCreation.FormIdentifier}",
                Log = JsonConvert.SerializeObject(formForCreation, Formatting.Indented)
            };
            await _auditLogRepository.SaveAsync(audit);

            return BadRequest(ModelState);
        }
    }
}
