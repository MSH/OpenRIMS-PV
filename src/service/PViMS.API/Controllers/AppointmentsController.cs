using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Application.Commands.AppointmentAggregate;
using PVIMS.API.Application.Queries.AppointmentAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IMediator mediator,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<Appointment> appointmentRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<AppointmentsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all appointments using a valid media type 
        /// </summary>
        /// <param name="appointmentResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of AppointmentSearchDto</returns>
        [HttpGet("appointments", Name = "GetAppointmentsForSearch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.search.v1+json", "application/vnd.pvims.search.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.search.v1+json", "application/vnd.pvims.search.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<AppointmentSearchDto>>> GetAppointmentsForSearch(
            [FromQuery] AppointmentResourceParameters appointmentResourceParameters)
        {
            if (appointmentResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var query = new AppointmentsSearchQuery(
                appointmentResourceParameters.OrderBy,
                appointmentResourceParameters.FacilityName,
                appointmentResourceParameters.PatientId,
                appointmentResourceParameters.FirstName,
                appointmentResourceParameters.LastName,
                appointmentResourceParameters.CriteriaId,
                appointmentResourceParameters.CustomAttributeId,
                appointmentResourceParameters.CustomAttributeValue,
                appointmentResourceParameters.SearchFrom,
                appointmentResourceParameters.SearchTo,
                appointmentResourceParameters.PageNumber,
                appointmentResourceParameters.PageSize);

            _logger.LogInformation("----- Sending query: AppointmentsSearchQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = appointmentResourceParameters.PageSize,
                currentPage = appointmentResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single appointment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Appointment</param>
        /// <returns>An ActionResult of type AppointmentIdentifierDto</returns>
        [HttpGet("patients/{patientId}/appointments/{id}", Name = "GetAppointmentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<AppointmentIdentifierDto>> GetAppointmentByIdentifier(int patientId, int id)
        {
            var query = new AppointmentIdentifierQuery(patientId, id);

            _logger.LogInformation(
                $"----- Sending query: AppointmentIdentifierQuery - {patientId} - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single appointment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Appointment</param>
        /// <returns>An ActionResult of type AppointmentDetailDto</returns>
        [HttpGet("patients/{patientId}/appointments/{id}", Name = "GetAppointmentByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<AppointmentDetailDto>> GetAppointmentByDetail(int patientId, int id)
        {
            var query = new AppointmentDetailQuery(patientId, id);

            _logger.LogInformation(
                $"----- Sending query: AppointmentDetailQuery - {patientId} - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a list of outstanding visits
        /// </summary>
        /// <param name="outstandingVisitResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type OutstandingVisitReportDto</returns>
        [HttpGet("appointments", Name = "GetOutstandingVisitReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.outstandingvisitreport.v1+json", "application/vnd.pvims.outstandingvisitreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.outstandingvisitreport.v1+json", "application/vnd.pvims.outstandingvisitreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>>> GetOutstandingVisitReport(
                        [FromQuery] OutstandingVisitResourceParameters outstandingVisitResourceParameters)
        {
            if (outstandingVisitResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter paramters payload");
                return BadRequest(ModelState);
            }

            var query = new OutstandingVisitReportQuery(outstandingVisitResourceParameters.PageNumber,
                outstandingVisitResourceParameters.PageSize,
                outstandingVisitResourceParameters.SearchFrom,
                outstandingVisitResourceParameters.SearchTo,
                outstandingVisitResourceParameters.FacilityId);

            _logger.LogInformation("----- Sending query: OutstandingVisitReportQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = outstandingVisitResourceParameters.PageSize,
                currentPage = outstandingVisitResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new appointment for a patient
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="appointmentForCreation">The appointment payload</param>
        /// <returns></returns>
        [HttpPost("patients/{patientId}/appointments", Name = "CreateAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateAppointment(int patientId, 
            [FromBody] AppointmentForCreationDto appointmentForCreation)
        {
            if (appointmentForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new appointment");
                return BadRequest(ModelState);
            }

            var command = new AddAppointmentCommand(patientId, appointmentForCreation.AppointmentDate, appointmentForCreation.Reason);

            _logger.LogInformation(
                $"----- Sending command: AddAppointmentCommand - {command.PatientId} - {command.AppointmentDate}");

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetAppointmentByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing appointment
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the appointment</param>
        /// <param name="appointmentForUpdate">The appointment payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/appointments/{id}", Name = "UpdateAppointment")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateAppointment(int patientId, int id,
            [FromBody] AppointmentForUpdateDto appointmentForUpdate)
        {
            if (appointmentForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeAppointmentDetailsCommand(patientId, id, appointmentForUpdate.AppointmentDate, appointmentForUpdate.Reason, appointmentForUpdate.Cancelled == Models.ValueTypes.YesNoValueType.Yes, appointmentForUpdate.CancellationReason);

            _logger.LogInformation(
                $"----- Sending command: ChangeAppointmentDetailsCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Update an existing appointment as did not arrive
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the appointment</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/appointments/{id}/dna", Name = "UpdateAppointmentAsDNA")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateAppointmentAsDNA(long patientId, long id)
        {
            var appointmentFromRepo = await _appointmentRepository.GetAsync(f => f.PatientId == patientId && f.Id == id);
            if (appointmentFromRepo == null)
            {
                return NotFound();
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate patient");
            }

            if (appointmentFromRepo.AppointmentDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Appointment date has not passed");
            }

            if (appointmentFromRepo.Dna)
            {
                ModelState.AddModelError("Message", "Appointment already marked as DNA");
            }

            if (ModelState.IsValid)
            {
                appointmentFromRepo.MarkAsDNA();

                _appointmentRepository.Update(appointmentFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Archive an existing appointment
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the appointment</param>
        /// <param name="appointmentForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/appointments/{id}/archive", Name = "ArchiveAppointment")]
        public async Task<IActionResult> ArchiveAppointment(int patientId, int id,
            [FromBody] ArchiveDto appointmentForDelete)
        {
            if (appointmentForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ArchiveAppointmentCommand(patientId, id, appointmentForDelete.Reason);

            _logger.LogInformation(
                $"----- Sending command: ArchiveAppointmentCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }
    }
}
