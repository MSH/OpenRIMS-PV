using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class WorkFlowsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IExcelDocumentService _excelDocumentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WorkFlowsController> _logger;

        public WorkFlowsController(
            IMediator mediator, 
            IRepositoryInt<WorkFlow> workFlowRepository,
            IRepositoryInt<User> userRepository,
            IExcelDocumentService excelDocumentService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<WorkFlowsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _excelDocumentService = excelDocumentService ?? throw new ArgumentNullException(nameof(excelDocumentService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single workFlow using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the workFlow</param>
        /// <returns>An ActionResult of type WorkFlowIdentifierDto</returns>
        [HttpGet("workflow/{id}", Name = "GetWorkFlowByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<WorkFlowIdentifierDto>> GetWorkFlowByIdentifier(Guid id)
        {
            var query = new WorkFlowIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: WorkFlowIdentifierQuery - {workFlowGuid}",
                id.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single workFlow using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the workFlow</param>
        /// <returns>An ActionResult of type WorkFlowDetailDto</returns>
        [HttpGet("workflow/{id}", Name = "GetWorkFlowByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<WorkFlowDetailDto>> GetWorkFlowByDetail(Guid id)
        {
            var query = new WorkFlowDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: WorkFlowDetailQuery - {workFlowGuid}",
                id.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single workFlow using it's unique id and valid media type - shaped for summarise record counts per work flow
        /// </summary>
        /// <param name="id">The unique identifier for the workFlow</param>
        /// <returns>An ActionResult of type WorkFlowSummaryDto</returns>
        [HttpGet("workflow/{id}", Name = "GetWorkFlowBySummary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.summary.v1+json", "application/vnd.main.summary.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.summary.v1+json", "application/vnd.main.summary.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<WorkFlowSummaryDto>> GetWorkFlowBySummary(Guid id)
        {
            var query = new WorkFlowSummaryQuery(id);

            _logger.LogInformation(
                "----- Sending query: WorkFlowSummaryQuery - {workFlowGuid}",
                id.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Download a dataset for corresponding work flow
        /// </summary>
        /// <param name="id">The unique id of the work flow you would like to download the dataset for</param>
        /// <param name="analyserDatasetResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult</returns>
        [HttpGet("workflow/{id}", Name = "DownloadDataset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.dataset.v1+json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult> DownloadDataset(Guid id,
            [FromQuery] AnalyserDatasetResourceParameters analyserDatasetResourceParameters)
        {
            var workflowFromRepo = await _workFlowRepository.GetAsync(f => f.WorkFlowGuid == id);
            if (workflowFromRepo == null)
            {
                return NotFound();  
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = _userRepository.Get(u => u.UserName == userName);

            if (!userFromRepo.AllowDatasetDownload)
            {
                ModelState.AddModelError("Message", "You do not have permissions to download a dataset");
                return BadRequest(ModelState);
            }

            var model = id == new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986") 
                ? _excelDocumentService.CreateSpontaneousDatasetForDownload() 
                : _excelDocumentService.CreateActiveDatasetForDownload(new long[] { }, analyserDatasetResourceParameters?.CohortGroupId ?? 0);

            return PhysicalFile(model.FullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
