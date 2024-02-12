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
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientMedicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PatientMedicationsController> _logger;

        public PatientMedicationsController(IMediator mediator,
            ILogger<PatientMedicationsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient medication using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient medication</param>
        /// <returns>An ActionResult of type PatientMedicationIdentifierDto</returns>
        [HttpGet("{patientId}/medications/{id}", Name = "GetPatientMedicationByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<PatientMedicationIdentifierDto>> GetPatientMedicationByIdentifier(int patientId, int id)
        {
            var query = new PatientMedicationIdentifierQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientMedicationIdentifierQuery - {patientId} : {id}",
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
        /// Get a single patient medication using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient medication</param>
        /// <returns>An ActionResult of type PatientMedicationDetailDto</returns>
        [HttpGet("{patientId}/medications/{id}", Name = "GetPatientMedicationByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientMedicationDetailDto>> GetPatientMedicationByDetail(int patientId, int id)
        {
            var query = new PatientMedicationDetailQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientMedicationDetailQuery - {patientId} : {id}",
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
        /// Create a new patient medication record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="medicationForUpdate">The medication payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/medications", Name = "CreatePatientMedication")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientMedication(int patientId, 
            [FromBody] PatientMedicationForUpdateDto medicationForUpdate)
        {
            if (medicationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new medication");
                return BadRequest(ModelState);
            }

            var command = new AddMedicationToPatientCommand(patientId, 
                medicationForUpdate.SourceDescription, medicationForUpdate.ConceptId, medicationForUpdate.ProductId, medicationForUpdate.StartDate, medicationForUpdate.EndDate, medicationForUpdate.Dose,
                medicationForUpdate.DoseFrequency, medicationForUpdate.DoseUnit, medicationForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddMedicationToPatientCommand - {conceptId}",
                command.ConceptId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientMedicationByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing patient medication
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="medicationForUpdate">The medication payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/medications/{id}", Name = "UpdatePatientMedication")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientMedication(int patientId, int id,
            [FromBody] PatientMedicationForUpdateDto medicationForUpdate)
        {
            if (medicationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeMedicationDetailsCommand(patientId, id, medicationForUpdate.StartDate, medicationForUpdate.EndDate, medicationForUpdate.Dose, medicationForUpdate.DoseFrequency, medicationForUpdate.DoseUnit, medicationForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangeMedicationDetailsCommand - {patientId}: {patientMedicationId}",
                command.PatientId,
                command.PatientMedicationId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient medication
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="medicationForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/medications/{id}/archive", Name = "ArchivePatientMedication")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientMedication(int patientId, int id,
            [FromBody] ArchiveDto medicationForDelete)
        {
            if (medicationForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ArchivePatientMedicationCommand(patientId, id, medicationForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientMedicationCommand - {patientId}: {patientMedicationId}",
                command.PatientId,
                command.PatientMedicationId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }
    }
}
