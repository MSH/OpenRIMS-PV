using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using PVIMS.API.Application.Queries.PatientAggregate;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientLabTestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PatientLabTestsController> _logger;

        public PatientLabTestsController(
            IMediator mediator,
            ILogger<PatientLabTestsController> logger
        )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient lab test using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient lab test</param>
        /// <returns>An ActionResult of type PatientLabTestIdentifierDto</returns>
        [HttpGet("{patientId}/labtests/{id}", Name = "GetPatientLabTestByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientLabTestIdentifierDto>> GetPatientLabTestByIdentifier(int patientId, int id)
        {
            var query = new PatientLabTestIdentifierQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientLabTestIdentifierQuery - {patientId} : {id}",
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
        /// Get a single patient lab test using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient labTest</param>
        /// <returns>An ActionResult of type PatientLabTestDetailDto</returns>
        [HttpGet("{patientId}/labtests/{id}", Name = "GetPatientLabTestByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientLabTestDetailDto>> GetPatientLabTestByDetail(int patientId, int id)
        {
            var query = new PatientLabTestDetailQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientLabTestDetailQuery - {patientId} : {id}",
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
        /// Create a new patient lab test record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/labtests", Name = "CreatePatientLabTest")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientLabTest(int patientId,
            [FromBody] PatientLabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new labTest");
                return BadRequest(ModelState);
            }

            var command = new AddLabTestToPatientCommand(patientId,
                labTestForUpdate.LabTest,
                labTestForUpdate.TestDate,
                labTestForUpdate.TestResultCoded,
                labTestForUpdate.TestResultValue,
                labTestForUpdate.TestUnit,
                labTestForUpdate.ReferenceLower,
                labTestForUpdate.ReferenceUpper,
                labTestForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddLabTestToPatientCommand - {LabTest}",
                command.LabTest);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientLabTestByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing patient lab test
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/labTests/{id}", Name = "UpdatePatientLabTest")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientLabTest(int patientId, int id,
            [FromBody] PatientLabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeLabTestDetailsCommand(patientId,
                id,
                labTestForUpdate.LabTest,
                labTestForUpdate.TestDate,
                labTestForUpdate.TestResultCoded,
                labTestForUpdate.TestResultValue,
                labTestForUpdate.TestUnit,
                labTestForUpdate.ReferenceLower,
                labTestForUpdate.ReferenceUpper,
                labTestForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangeLabTestDetailsCommand - {patientId}: {patientLabTestId}",
                command.PatientId,
                command.PatientLabTestId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient lab test
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/labTests/{id}/archive", Name = "ArchivePatientLabTest")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientLabTest(int patientId, int id,
            [FromBody] ArchiveDto labTestForDelete)
        {
            if (labTestForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ArchivePatientLabTestCommand(patientId, id, labTestForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientLabTestCommand - {patientId}: {patientLabTestId}",
                command.PatientId,
                command.PatientLabTestId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }
    }
}
