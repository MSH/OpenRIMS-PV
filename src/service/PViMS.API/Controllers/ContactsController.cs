using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Commands.ContactAggregate;
using PVIMS.API.Application.Queries.ContactAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class ContactsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IMediator mediator, 
            ITypeHelperService typeHelperService,
            ILogger<ContactsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all contact details using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ContactDetailDto</returns>
        [HttpGet("contactdetails", Name = "GetContactsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ContactDetailDto>>> GetContactsByDetail(
            [FromQuery] ContactResourceParameters contactResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ContactDetailDto>
                (contactResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ContactsDetailQuery(
                contactResourceParameters.OrderBy,
                contactResourceParameters.PageNumber,
                contactResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ContactsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = contactResourceParameters.PageSize,
                currentPage = contactResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single contact detail using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type ContactIdentifierDto</returns>
        [HttpGet("contactdetails/{id}", Name = "GetContactByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ContactIdentifierDto>> GetContactByIdentifier(int id)
        {
            var query = new ContactIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: ContactIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single contact detail using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type ContactDetailDto</returns>
        [HttpGet("contactdetails/{id}", Name = "GetContactByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ContactDetailDto>> GetContactByDetail(int id)
        {
            var query = new ContactDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: ContactDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Update an existing contact detail
        /// </summary>
        /// <param name="id">The unique id of the contact detail</param>
        /// <param name="contactForUpdateDto">The contact payload</param>
        /// <returns></returns>
        [HttpPut("contactdetails/{id}", Name = "UpdateContactDetail")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateContactDetail(int id,
            [FromBody] ContactForUpdateDto contactForUpdateDto)
        {
            if (contactForUpdateDto == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeContactDetailsCommand(id,
                contactForUpdateDto.OrganisationType, 
                contactForUpdateDto.OrganisationName, 
                contactForUpdateDto.DepartmentName,
                contactForUpdateDto.ContactFirstName,
                contactForUpdateDto.ContactLastName,
                contactForUpdateDto.StreetAddress,
                contactForUpdateDto.City,
                contactForUpdateDto.State,
                contactForUpdateDto.PostCode,
                contactForUpdateDto.CountryCode,
                contactForUpdateDto.ContactNumber,
                contactForUpdateDto.ContactEmail
            );

            _logger.LogInformation($"----- Sending command: ChangeContactDetailsCommand - {command.Id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }
    }
}
