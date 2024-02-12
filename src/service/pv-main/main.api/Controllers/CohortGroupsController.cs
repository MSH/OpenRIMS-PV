using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.CohortGroupAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/cohortgroups")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class CohortGroupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ILogger<CohortGroupsController> _logger;

        public CohortGroupsController(IMediator mediator,
            ITypeHelperService typeHelperService,
            ILogger<CohortGroupsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all cohort groups using a valid media type 
        /// </summary>
        /// <param name="cohortGroupResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CohortGroupIdentifierDto</returns>
        [HttpGet(Name = "GetCohortGroupsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>>> GetCohortGroupsByIdentifier(
            [FromQuery] IdResourceParameters cohortGroupResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CohortGroupIdentifierDto>
                (cohortGroupResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CohortGroupsIdentifierQuery(
                cohortGroupResourceParameters.OrderBy,
                cohortGroupResourceParameters.PageNumber,
                cohortGroupResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CohortGroupsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = cohortGroupResourceParameters.PageSize,
                currentPage = cohortGroupResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all cohort groups using a valid media type 
        /// </summary>
        /// <param name="cohortGroupResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CohortGroupDetailDto</returns>
        [HttpGet(Name = "GetCohortGroupsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>>> GetCohortGroupsByDetail(
            [FromQuery] IdResourceParameters cohortGroupResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CohortGroupDetailDto>
                (cohortGroupResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CohortGroupsDetailQuery(
                cohortGroupResourceParameters.OrderBy,
                cohortGroupResourceParameters.PageNumber,
                cohortGroupResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CohortGroupsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = cohortGroupResourceParameters.PageSize,
                currentPage = cohortGroupResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single cohort group unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the cohort group</param>
        /// <returns>An ActionResult of type CohortGroupIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetCohortGroupByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<CohortGroupIdentifierDto>> GetCohortGroupByIdentifier(int id)
        {
            var query = new CohortGroupIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: CohortGroupIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single facility using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the cohort group</param>
        /// <returns>An ActionResult of type CohortGroupDetailDto</returns>
        [HttpGet("{id}", Name = "GetCohortGroupByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CohortGroupDetailDto>> GetCohortGroupByDetail(int id)
        {
            var query = new CohortGroupDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: CohortGroupDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new cohort group
        /// </summary>
        /// <param name="cohortGroupForUpdate">The cohort group payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateCohortGroup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCohortGroup(
            [FromBody] CohortGroupForUpdateDto cohortGroupForUpdate)
        {
            if (cohortGroupForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new AddCohortGroupCommand(cohortGroupForUpdate.CohortName, cohortGroupForUpdate.CohortCode, cohortGroupForUpdate.StartDate, cohortGroupForUpdate.FinishDate, cohortGroupForUpdate.ConditionName);

            _logger.LogInformation(
                "----- Sending command: AddCohortGroupCommand - {cohortName}",
                command.CohortName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetCohortGroupByDetail",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing cohort group
        /// </summary>
        /// <param name="id">The unique id of the cohort group</param>
        /// <param name="cohortGroupForUpdate">The cohort group payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateCohortGroup")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCohortGroup(int id,
            [FromBody] CohortGroupForUpdateDto cohortGroupForUpdate)
        {
            if (cohortGroupForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeCohortGroupDetailsCommand(id, cohortGroupForUpdate.CohortName, cohortGroupForUpdate.CohortCode, cohortGroupForUpdate.StartDate, cohortGroupForUpdate.FinishDate, cohortGroupForUpdate.ConditionName);

            _logger.LogInformation(
                "----- Sending command: ChangeCohortGroupDetailsCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing cohort group
        /// </summary>
        /// <param name="id">The unique id of the cohort group</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteCohortGroup")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCohortGroup(int id)
        {
            var command = new DeleteCohortGroupCommand(id);

            _logger.LogInformation(
                "----- Sending command: DeleteCohortGroupCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }
    }
}
