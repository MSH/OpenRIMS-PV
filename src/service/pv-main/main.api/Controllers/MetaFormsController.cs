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
        [HttpGet("{id}", Name = "GetMetaFormByExpandedWithMappedAttributes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.expandedwithunmappedattributes.v1+json", "application/vnd.main.expandedwithunmappedattributes.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.expandedwithunmappedattributes.v1+json", "application/vnd.main.expandedwithunmappedattributes.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaFormExpandedDto>> GetMetaFormByExpandedWithMappedAttributes(int id)
        {
            var query = new MetaFormExpandedQuery(id, true);

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
        /// Get a single meta form unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta form</param>
        /// <returns>An ActionResult of type MetaFormExpandedDto</returns>
        [HttpGet("{id}", Name = "GetMetaFormByExpandedWithoutMappedAttributes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.expandedwithoutunmappedattributes.v1+json", "application/vnd.main.expandedwithoutunmappedattributes.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.expandedwithoutunmappedattributes.v1+json", "application/vnd.main.expandedwithoutunmappedattributes.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaFormExpandedDto>> GetMetaFormByExpandedWithoutMappedAttributes(int id)
        {
            var query = new MetaFormExpandedQuery(id, false);

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
        /// Update a meta form
        /// </summary>
        /// <param name="id">The unique id of the meta form</param>
        /// <param name="metaFormForUpdate">The meta form payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateMetaForm")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateMetaForm(int id,
            [FromBody] MetaFormForUpdateDto metaFormForUpdate)
        {
            if (metaFormForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeMetaFormDetailsCommand(
                id,
                metaFormForUpdate.FormName,
                metaFormForUpdate.ActionName);

            _logger.LogInformation(
                $"----- Sending command: ChangeMetaFormDetailsCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete a meta form
        /// </summary>
        /// <param name="id">The unique id of the form</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteMetaForm")]
        public async Task<IActionResult> DeleteMetaForm(int id)
        {
            var command = new DeleteMetaFormCommand(
                id);

            _logger.LogInformation(
                $"----- Sending command: DeleteMetaFormCommand - {command.MetaFormId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new meta form category
        /// </summary>
        /// <param name="metaFormId">The unique id of the meta form</param>
        /// <param name="metaFormCategoryForUpdate">The meta form category payload</param>
        /// <returns></returns>
        [HttpPost("{metaFormId}/categories", Name = "CreateMetaFormCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMetaFormCategory(int metaFormId,
            [FromBody] MetaFormCategoryForUpdateDto metaFormCategoryForUpdate)
        {
            if (metaFormCategoryForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new AddMetaFormCategoryCommand(
                metaFormId,
                metaFormCategoryForUpdate.MetaTableId,
                metaFormCategoryForUpdate.CategoryName,
                metaFormCategoryForUpdate.Help,
                metaFormCategoryForUpdate.Icon);

            _logger.LogInformation(
                "----- Sending command: AddMetaFormCategoryCommand - {FormName}",
                command.CategoryName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetMetaFormCategory",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update a meta form category
        /// </summary>
        /// <param name="metaFormId">The unique id of the meta form</param>
        /// <param name="id">The unique id of the meta form category</param>
        /// <param name="metaFormCategoryForUpdate">The meta form category payload</param>
        /// <returns></returns>
        [HttpPut("{metaFormId}/categories/{id}", Name = "UpdateMetaFormCategory")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateMetaFormCategory(int metaFormId, int id,
            [FromBody] MetaFormCategoryForUpdateDto metaFormCategoryForUpdate)
        {
            if (metaFormCategoryForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeMetaFormCategoryDetailsCommand(
                metaFormId,
                id,
                metaFormCategoryForUpdate.CategoryName,
                metaFormCategoryForUpdate.Help,
                metaFormCategoryForUpdate.Icon);

            _logger.LogInformation(
                $"----- Sending command: ChangeMetaFormCategoryDetailsCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete a meta form category
        /// </summary>
        /// <param name="metaFormId">The unique id of the meta form</param>
        /// <param name="id">The unique id of the category</param>
        /// <returns></returns>
        [HttpDelete("{metaFormId}/categories/{id}", Name = "DeleteMetaFormCategory")]
        public async Task<IActionResult> DeleteMetaFormCategory(int metaFormId, int id)
        {
            var command = new DeleteMetaFormCategoryCommand(
                metaFormId,
                id);

            _logger.LogInformation(
                $"----- Sending command: DeleteMetaFormCategoryCommand - {command.MetaFormCategoryId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new meta form category attribute
        /// </summary>
        /// <param name="metaFormId">The unique id of the meta form category</param>
        /// <param name="metaFormCategoryId">The unique id of the meta form category</param>
        /// <param name="metaFormCategoryAttributeForUpdate">The meta form category attribute payload</param>
        /// <returns></returns>
        [HttpPost("{metaFormId}/categories/{metaFormCategoryId}/attributes", Name = "CreateMetaFormCategoryAttribute")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMetaFormCategoryAttribute(int metaFormId, int metaFormCategoryId,
            [FromBody] MetaFormCategoryAttributeForUpdateDto metaFormCategoryAttributeForUpdate)
        {
            if (metaFormCategoryAttributeForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new AddMetaFormCategoryAttributeCommand(
                metaFormId,
                metaFormCategoryId,
                metaFormCategoryAttributeForUpdate.AttributeName,
                metaFormCategoryAttributeForUpdate.CustomAttributeConfigurationId,
                metaFormCategoryAttributeForUpdate.Label,
                metaFormCategoryAttributeForUpdate.Help);

            _logger.LogInformation(
                "----- Sending command: AddMetaFormCategoryAttributeCommand - {FormName}",
                command.Label);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetMetaFormCategoryAttribute",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update a meta form category attribute
        /// </summary>
        /// <param name="metaFormId">The unique id of the meta form</param>
        /// <param name="metaFormCategoryId">The unique id of the meta form category</param>
        /// <param name="id">The unique id of the attribute</param>
        /// <param name="metaFormCategoryAttributeForUpdate">The meta form category attribute payload</param>
        /// <returns></returns>
        [HttpPut("{metaFormId}/categories/{metaFormCategoryId}/attributes/{id}", Name = "UpdateMetaFormCategoryAttribute")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateMetaFormCategoryAttribute(int metaFormId, int metaFormCategoryId, int id,
            [FromBody] MetaFormCategoryAttributeForUpdateDto metaFormCategoryAttributeForUpdate)
        {
            if (metaFormCategoryAttributeForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeMetaFormCategoryAttributeDetailsCommand(
                metaFormId,
                metaFormCategoryId,
                id,
                metaFormCategoryAttributeForUpdate.Label,
                metaFormCategoryAttributeForUpdate.Help);

            _logger.LogInformation(
                $"----- Sending command: ChangeMetaFormCategoryAttributeDetailsCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete a meta form category attribute
        /// </summary>
        /// <param name="metaFormId">The unique id of the meta form</param>
        /// <param name="metaFormCategoryId">The unique id of the meta form category</param>
        /// <param name="id">The unique id of the attribute</param>
        /// <returns></returns>
        [HttpDelete("{metaFormId}/categories/{metaFormCategoryId}/attributes/{id}", Name = "DeleteMetaFormCategoryAttribute")]
        public async Task<IActionResult> DeleteMetaFormCategoryAttribute(int metaFormId, int metaFormCategoryId, int id)
        {
            var command = new DeleteMetaFormCategoryAttributeCommand(
                metaFormId,
                metaFormCategoryId,
                id);

            _logger.LogInformation(
                $"----- Sending command: DeleteMetaFormCategoryAttributeCommand - {command.MetaFormCategoryAttributeId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
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
