using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Application.Commands.FacilityAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.FacilityAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Entities;
using System;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class FacilitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ILogger<FacilitiesController> _logger;

        public FacilitiesController(IMediator mediator, 
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            ILogger<FacilitiesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all facilities using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of FacilityIdentifierDto</returns>
        [HttpGet("facilities", Name = "GetFacilitiesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>>> GetFacilitiesByIdentifier(
            [FromQuery] FacilityResourceParameters facilityResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<FacilityIdentifierDto>
                (facilityResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new FacilitiesIdentifierQuery(
                facilityResourceParameters.OrderBy,
                facilityResourceParameters.PageNumber,
                facilityResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: FacilitiesIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = facilityResourceParameters.PageSize,
                currentPage = facilityResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all facilities using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of FacilityDetailDto</returns>
        [HttpGet("facilities", Name = "GetFacilitiesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<FacilityDetailDto>>> GetFacilitiesByDetail(
            [FromQuery] FacilityResourceParameters facilityResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<FacilityDetailDto>
                (facilityResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new FacilitiesDetailQuery(
                facilityResourceParameters.OrderBy,
                facilityResourceParameters.PageNumber,
                facilityResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: FacilitiesDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = facilityResourceParameters.PageSize,
                currentPage = facilityResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single facility using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Facility</param>
        /// <returns>An ActionResult of type FacilityIdentifierDto</returns>
        [HttpGet("facilities/{id}", Name = "GetFacilityByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<FacilityIdentifierDto>> GetFacilityByIdentifier(int id)
        {
            var query = new FacilityIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: FacilityIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single facility using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Facility</param>
        /// <returns>An ActionResult of type FacilityDetailDto</returns>
        [HttpGet("facilities/{id}", Name = "GetFacilityByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<FacilityDetailDto>> GetFacilityByDetail(int id)
        {
            var query = new FacilityDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: FacilityDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all form types using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of FacilityTypeIdentifierDto</returns>
        [HttpGet("facilitytypes", Name = "GetFacilityTypesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>>> GetFacilityTypesByIdentifier(
            [FromQuery] FacilityTypeResourceParameters facilityTypeResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<FacilityTypeIdentifierDto, FacilityType>
               (facilityTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new FacilityTypesIdentifierQuery(
                facilityTypeResourceParameters.OrderBy,
                facilityTypeResourceParameters.PageNumber,
                facilityTypeResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: FacilityTypesIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = facilityTypeResourceParameters.PageSize,
                currentPage = facilityTypeResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new facility
        /// </summary>
        /// <param name="facilityForUpdate">The facility payload</param>
        /// <returns></returns>
        [HttpPost("facilities", Name = "CreateFacility")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateFacility(
            [FromBody] FacilityForUpdateDto facilityForUpdate)
        {
            if (facilityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new facility");
                return BadRequest(ModelState);
            }

            var command = new AddFacilityCommand(facilityForUpdate.FacilityName, facilityForUpdate.FacilityCode, facilityForUpdate.FacilityType, facilityForUpdate.TelNumber, facilityForUpdate.MobileNumber, facilityForUpdate.FaxNumber, facilityForUpdate.OrgUnitId);

            _logger.LogInformation(
                "----- Sending command: AddFacilityCommand - {facilityName}",
                command.FacilityName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetFacilityByDetail",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing facility
        /// </summary>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="facilityForUpdate">The facility payload</param>
        /// <returns></returns>
        [HttpPut("facilities/{id}", Name = "UpdateFacility")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateFacility(int id,
            [FromBody] FacilityForUpdateDto facilityForUpdate)
        {
            if (facilityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for facility");
                return BadRequest(ModelState);
            }

            var command = new ChangeFacilityDetailsCommand(id, facilityForUpdate.FacilityName, facilityForUpdate.FacilityCode, facilityForUpdate.FacilityType, facilityForUpdate.TelNumber, facilityForUpdate.MobileNumber, facilityForUpdate.FaxNumber, facilityForUpdate.OrgUnitId);

            _logger.LogInformation(
                "----- Sending command: ChangeFacilityDetailsCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing facility
        /// </summary>
        /// <param name="id">The unique id of the facility</param>
        /// <returns></returns>
        [HttpDelete("facilities/{id}", Name = "DeleteFacility")]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var command = new DeleteFacilityCommand(id);

            _logger.LogInformation(
                "----- Sending command: DeleteFacilityCommand - {Id}",
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
