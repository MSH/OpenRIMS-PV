using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Application.Queries.OrgUnitAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using System;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/orgunits")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class OrgUnitsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrgUnitsController> _logger;

        public OrgUnitsController(IMediator mediator,
            ILogger<OrgUnitsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all organisation units by using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of OrgUnitIdentifierDto</returns>
        [HttpGet(Name = "GetOrgUnitsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<OrgUnitIdentifierDto>>> GetOrgUnitsByIdentifier(
            [FromQuery] OrgUnitResourceParameters orgUnitResourceParameters)
        {
            var query = new OrgUnitsIdentifierQuery(
                orgUnitResourceParameters.OrderBy,
                orgUnitResourceParameters.PageNumber,
                orgUnitResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: OrgUnitsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = orgUnitResourceParameters.PageSize,
                currentPage = orgUnitResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }
    }
}
