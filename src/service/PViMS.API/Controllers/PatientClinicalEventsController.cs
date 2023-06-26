using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.Queries.PatientAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientClinicalEventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PatientClinicalEventsController> _logger;

        public PatientClinicalEventsController(IMediator mediator, 
            ILogger<PatientClinicalEventsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventIdentifierDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientClinicalEventIdentifierDto>> GetPatientClinicalEventByIdentifier(int patientId, int id)
        {
            var query = new PatientClinicalEventIdentifierQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientClinicalEventIdentifierQuery - {patientId} : {id}",
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
        /// Get a single patient clinical event using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventDetailDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientClinicalEventDetailDto>> GetPatientClinicalEventByDetail(int patientId, int id)
        {
            var query = new PatientClinicalEventDetailQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientClinicalEventDetailQuery - {patientId} : {id}",
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
        /// Get a single patient clinical event using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventExpandedDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientClinicalEventExpandedDto>> GetPatientClinicalEventByExpanded(int patientId, int id)
        {
            var query = new PatientClinicalEventExpandedQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientClinicalEventExpandedQuery - {patientId} : {id}",
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
        /// Create a new patient clinical event record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="clinicalEventForUpdate">The clinical event payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/clinicalevents", Name = "CreatePatientClinicalEvent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientClinicalEvent(int patientId, 
            [FromBody] PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            if (clinicalEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new clinical event");
                return BadRequest(ModelState);
            }

            var command = new AddClinicalEventToPatientCommand(patientId,
                clinicalEventForUpdate.PatientIdentifier,
                clinicalEventForUpdate.SourceDescription, 
                clinicalEventForUpdate.SourceTerminologyMedDraId, 
                clinicalEventForUpdate.OnsetDate, 
                clinicalEventForUpdate.ResolutionDate, 
                clinicalEventForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddClinicalEventToPatientCommand - {sourceDescription}",
                command.SourceDescription);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientClinicalEventByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing patient clinical event
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the clinical event</param>
        /// <param name="clinicalEventForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/clinicalevents/{id}", Name = "UpdatePatientClinicalEvent")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientClinicalEvent(int patientId, int id,
            [FromBody] PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            if (clinicalEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeClinicalEventDetailsCommand(patientId, id, clinicalEventForUpdate.SourceDescription, clinicalEventForUpdate.SourceTerminologyMedDraId, clinicalEventForUpdate.OnsetDate, clinicalEventForUpdate.ResolutionDate, clinicalEventForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangeClinicalEventDetailsCommand - {patientId}: {patientClinicalEventId}",
                command.PatientId,
                command.PatientClinicalEventId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient clinical event
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the clinical event</param>
        /// <param name="conditionForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/clinicalevents/{id}/archive", Name = "ArchivePatientClinicalEvent")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientClinicalEvent(int patientId, int id,
            [FromBody] ArchiveDto clinicalEventForDelete)
        {
            if (clinicalEventForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ArchivePatientClinicalEventCommand(patientId, id, clinicalEventForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientClinicalEventCommand - {patientId}: {patientClinicalEventId}",
                command.PatientId,
                command.PatientClinicalEventId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }
    }
}
