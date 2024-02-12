using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class ReportInstancesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IRepositoryInt<ActivityExecutionStatusEvent> _activityExecutionStatusEventRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IArtefactService _artefactService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReportInstancesController> _logger;

        public ReportInstancesController(
            IMediator mediator,
            IPropertyMappingService propertyMappingService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<ActivityExecutionStatusEvent> activityExecutionStatusEventRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<WorkFlow> workFlowRepository,
            IArtefactService artefactService,
            IWorkFlowService workFlowService,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReportInstancesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _activityExecutionStatusEventRepository = activityExecutionStatusEventRepository ?? throw new ArgumentNullException(nameof(activityExecutionStatusEventRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single report instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult of type ReportInstanceIdentifierDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "GetReportInstanceByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<ReportInstanceIdentifierDto>> GetReportInstanceByIdentifier(Guid workFlowGuid, int id)
        {
            var mappedReportInstance = await GetReportInstanceAsync<ReportInstanceIdentifierDto>(workFlowGuid, id);
            if (mappedReportInstance == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForReportInstance<ReportInstanceIdentifierDto>(workFlowGuid, mappedReportInstance));
        }

        /// <summary>
        /// Get a single report instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult of type ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "GetReportInstanceByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<ReportInstanceDetailDto>> GetReportInstanceByDetail(Guid workFlowGuid, int id)
        {
            var query = new ReportInstanceDetailQuery(workFlowGuid, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceDetailQuery - {workFlowGuid}: {id}",
                workFlowGuid.ToString(),
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single report instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult of type ReportInstanceExpandedDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "GetReportInstanceByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.expanded.v1+json", "application/vnd.main.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.expanded.v1+json", "application/vnd.main.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<ReportInstanceExpandedDto>> GetReportInstanceByExpanded(Guid workFlowGuid, int id)
        {
            var query = new ReportInstanceExpandedQuery(workFlowGuid, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceExpandedQuery - {workFlowGuid}: {id}",
                workFlowGuid.ToString(),
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Download a summary for the patient
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "DownloadPatientSummary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.patientsummary.v1+json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult> DownloadPatientSummary(Guid workFlowGuid, int id)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == id);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            var model = workFlowGuid == new Guid("892F3305-7819-4F18-8A87-11CBA3AEE219") ?
                await _artefactService.CreatePatientSummaryForActiveReportAsync(reportInstanceFromRepo.ContextGuid) :
                await _artefactService.CreatePatientSummaryForSpontaneousReportAsync(reportInstanceFromRepo.ContextGuid);

            return PhysicalFile(model.FullPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        /// <summary>
        /// Get all report instances using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetReportInstancesByDetail(Guid workFlowGuid, 
            [FromQuery] ReportInstanceResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesDetailQuery(workFlowGuid, 
                false, 
                false,
                reportInstanceResourceParameters.ActiveReportsOnly == Models.ValueTypes.YesNoValueType.Yes, 
                reportInstanceResourceParameters.SearchFrom,
                reportInstanceResourceParameters.SearchTo,
                reportInstanceResourceParameters.SearchTerm,
                reportInstanceResourceParameters.QualifiedName, 
                reportInstanceResourceParameters.PageNumber, 
                reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesDetailQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all new report instances using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetNewReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.new.v1+json", "application/vnd.main.new.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.new.v1+json", "application/vnd.main.new.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetNewReportInstancesByDetail(Guid workFlowGuid,
            [FromQuery] IdResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesAnalysisQuery(workFlowGuid, "", reportInstanceResourceParameters.PageNumber, reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesAnalysisQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all report instances for analysis using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetAnalysisReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.analysis.v1+json", "application/vnd.main.analysis.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.analysis.v1+json", "application/vnd.main.analysis.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetAnalysisReportInstancesByDetail(Guid workFlowGuid,
            [FromQuery] ReportInstanceActivityResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesAnalysisQuery(workFlowGuid, reportInstanceResourceParameters.QualifiedName, reportInstanceResourceParameters.PageNumber, reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesAnalysisQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all report instances for feedback using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetFeedbackReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.feedback.v1+json", "application/vnd.main.feedback.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.feedback.v1+json", "application/vnd.main.feedback.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst,Clinician")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetFeedbackReportInstancesByDetail(Guid workFlowGuid,
            [FromQuery] ReportInstanceActivityResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesFeedbackQuery(workFlowGuid, reportInstanceResourceParameters.QualifiedName, reportInstanceResourceParameters.PageNumber, reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesFeedbackQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Download a file attachment
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportinstanceId">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="activityExecutionStatusEventId">The unique identifier of the activity</param>
        /// <param name="id">The unique identifier for the attachment</param>
        /// <returns>An ActionResult</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportinstanceId}/activity/{activityExecutionStatusEventId}/attachments/{id}", Name = "DownloadActivitySingleAttachment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.attachment.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadActivitySingleAttachment(Guid workFlowGuid, int reportinstanceId, int activityExecutionStatusEventId, int  id)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == reportinstanceId);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            var activityExecutionStatusEvent = await _activityExecutionStatusEventRepository.GetAsync(f => f.Id == activityExecutionStatusEventId, new string[] { "Attachments.AttachmentType" });
            if (activityExecutionStatusEvent == null)
            {
                return NotFound();
            }

            var attachmentFromRepo = activityExecutionStatusEvent.Attachments.SingleOrDefault(f => f.Id == id);
            if (attachmentFromRepo == null)
            {
                return NotFound();
            }

            byte[] buffer = (byte[])attachmentFromRepo.Content;

            var destFile = $"{Path.GetTempPath()}{attachmentFromRepo.FileName}";
            FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write);
            // Writes a block of bytes to this stream using data from // a byte array.
            fs.Write(buffer, 0, attachmentFromRepo.Content.Length);
            // close file stream
            fs.Close();

            var mime = "";
            switch (attachmentFromRepo.AttachmentType.Key)
            {
                case "docx":
                    mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;

                case "doc":
                    mime = "application/msword";
                    break;

                case "xlsx":
                    mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;

                case "xls":
                    mime = "application/vnd.ms-excel";
                    break;

                case "pdf":
                    mime = "application/pdf";
                    break;

                case "png":
                    mime = "image/png";
                    break;

                case "jpg":
                case "jpeg":
                    mime = "image/jpeg";
                    break;

                case "bmp":
                    mime = "image/bmp";
                    break;

                case "xml":
                    mime = "application/xml";
                    break;
            }

            return PhysicalFile(destFile, mime);
        }

        /// <summary>
        /// Causality report
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="causalityReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type CausalityReportDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetCausalityReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.causalityreport.v1+json", "application/vnd.main.causalityreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.causalityreport.v1+json", "application/vnd.main.causalityreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CausalityReportDto>>> GetCausalityReport(Guid workFlowGuid, 
                        [FromQuery] CausalityReportResourceParameters causalityReportResourceParameters)
        {
            var workFlowFromRepo = await _workFlowRepository.GetAsync(f => f.WorkFlowGuid == workFlowGuid);
            if (workFlowFromRepo == null)
            {
                return NotFound();
            }

            if (causalityReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var query = new CausalityReportQuery(workFlowGuid,
                causalityReportResourceParameters.PageNumber,
                causalityReportResourceParameters.PageSize,
                causalityReportResourceParameters.SearchFrom,
                causalityReportResourceParameters.SearchTo,
                causalityReportResourceParameters.FacilityId,
                causalityReportResourceParameters.CausalityCriteria);

            _logger.LogInformation("----- Sending query: CausalityReportQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = causalityReportResourceParameters.PageSize,
                currentPage = causalityReportResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Change report instance activity
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <param name="activityChange">The payload for setting the new status</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/activity", Name = "UpdateReportInstanceActivity")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceActivity(Guid workFlowGuid, int id,
            [FromBody] ActivityChangeDto activityChange)
        {
            if (activityChange == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeReportInstanceActivityCommand(workFlowGuid, id, activityChange.Comments, activityChange.CurrentExecutionStatus, activityChange.NewExecutionStatus, activityChange.ContextCode, activityChange.ContextDate);

            _logger.LogInformation(
                "----- Sending command: ChangeReportInstanceActivityCommand - {id}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Change report classification
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that the report instance is associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <param name="reportInstanceClassificationForUpdate">The payload for setting the new classification</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/classification", Name = "UpdateReportInstanceClassification")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceClassification(Guid workFlowGuid, int id,
            [FromBody] ReportInstanceClassificationForUpdateDto reportInstanceClassificationForUpdate)
        {
            if (reportInstanceClassificationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var reportClassification = ReportClassification.FromName(reportInstanceClassificationForUpdate.ReportClassification);
            var command = new ChangeReportClassificationCommand(workFlowGuid, id, reportClassification);

            _logger.LogInformation(
                "----- Sending command: ChangeReportClassificationCommand - {workFlowGuid}: {reportInstanceId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Set report instance terminology
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <param name="terminologyForUpdate">The payload for setting the new status</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/terminology", Name = "UpdateReportInstanceTerminology")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceTerminology(Guid workFlowGuid, int id,
            [FromBody] ReportInstanceTerminologyForUpdateDto terminologyForUpdate)
        {
            if (terminologyForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeReportTerminologyCommand(workFlowGuid, id, terminologyForUpdate.TerminologyMedDraId);

            _logger.LogInformation(
                "----- Sending command: ChangeReportTerminologyCommand - {workFlowGuid}: {reportInstanceId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Set naranjo or who causality for a report instance medication
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="id">The unique identifier of the reporting instance medication</param>
        /// <param name="causalityForUpdate">The payload for setting the new causality</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/medications/{id}/causality", Name = "UpdateReportInstanceMedicationCausality")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceMedicationCausality(Guid workFlowGuid, int reportInstanceId, int id,
            [FromBody] ReportInstanceMedicationCausalityForUpdateDto causalityForUpdate)
        {
            var command = new ChangeReportMedicationCausalityCommand(workFlowGuid, reportInstanceId, id, causalityForUpdate.CausalityConfigType, causalityForUpdate.Causality);

            _logger.LogInformation(
                "----- Sending command: ChangeReportMedicationCausalityCommand - {workFlowGuid}: {reportInstanceId}: {reportInstanceMedicationId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceMedicationId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Create an E2B instance for the report
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/createe2b", Name = "CreateE2BInstance")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateE2BInstance(Guid workFlowGuid, int id)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == id);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, "E2BINITIATED") == false)
            {
                ModelState.AddModelError("Message", "Invalid status for activity");
            }

            if (ModelState.IsValid)
            {
                if (workFlowGuid == new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986"))
                {
                    var spontaneousCommand = new CreateE2BForSpontaneousCommand(workFlowGuid, id);

                    _logger.LogInformation(
                        "----- Sending command: CreateE2BForSpontaneousCommand - {workFlowGuid}: {reportInstanceId}",
                        spontaneousCommand.WorkFlowGuid.ToString(),
                        spontaneousCommand.ReportInstanceId);

                    var commandResult = await _mediator.Send(spontaneousCommand);

                    if (!commandResult)
                    {
                        return BadRequest("Command not created");
                    }
                }
                else
                {
                    var activeCommand = new CreateE2BForActiveCommand(workFlowGuid, id);

                    _logger.LogInformation(
                        "----- Sending command: CreateE2BForActiveCommand - {workFlowGuid}: {reportInstanceId}",
                        activeCommand.WorkFlowGuid.ToString(),
                        activeCommand.ReportInstanceId);

                    var commandResult = await _mediator.Send(activeCommand);

                    if (!commandResult)
                    {
                        return BadRequest("Command not created");
                    }
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get a single report instance task using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique id of the report instance</param>
        /// <param name="id">The unique id of the report instance task</param>
        /// <returns>An ActionResult of type ReportInstanceTaskIdentifierDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}", Name = "GetReportInstanceTaskByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<ReportInstanceTaskIdentifierDto>> GetReportInstanceTaskByIdentifier(Guid workFlowGuid, int reportInstanceId, int id)
        {
            var query = new ReportInstanceTaskIdentifierQuery(workFlowGuid, reportInstanceId, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceTaskIdentifierQuery - {workFlowGuid}: {reportInstanceId}: {id}",
                workFlowGuid.ToString(),
                reportInstanceId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single report instance task using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique id of the report instance</param>
        /// <param name="id">The unique id of the report instance task</param>
        /// <returns>An ActionResult of type TaskDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}", Name = "GetReportInstanceTaskByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<TaskDto>> GetReportInstanceTaskByDetail(Guid workFlowGuid, int reportInstanceId, int id)
        {
            var query = new ReportInstanceTaskDetailQuery(workFlowGuid, reportInstanceId, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceTaskDetailQuery - {workFlowGuid}: {reportInstanceId}: {id}",
                workFlowGuid.ToString(),
                reportInstanceId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Add a new task to a report instance
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="reportInstanceTaskForCreation">The payload containing details of the task</param>
        /// <returns></returns>
        [HttpPost("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks", Name = "CreateReportInstanceTask")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateReportInstanceTask(Guid workFlowGuid, int reportInstanceId,
            [FromBody] ReportInstanceTaskForCreationDto reportInstanceTaskForCreation)
        {
            if (reportInstanceTaskForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for command");
                return BadRequest(ModelState);
            }

            var taskType = TaskType.FromName(reportInstanceTaskForCreation.TaskType.ToString());
            var command = new AddTaskToReportInstanceCommand(workFlowGuid, reportInstanceId, reportInstanceTaskForCreation.Source, reportInstanceTaskForCreation.Description, taskType);

            _logger.LogInformation(
                "----- Sending command: AddTaskToReportInstanceCommand - {workFlowGuid}: {reportInstanceId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetReportInstanceTaskByIdentifier",
                new
                {
                    workFlowGuid,
                    reportInstanceId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Change task details for report instance task
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="id">The unique identifier of the reporting instance task</param>
        /// <param name="reportInstanceTaskDetailsForUpdate">The payload for updating task details</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}/details", Name = "UpdateReportInstanceTaskDetails")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceTaskDetails(Guid workFlowGuid, int reportInstanceId, int id,
            [FromBody] ReportInstanceTaskDetailsForUpdateDto reportInstanceTaskDetailsForUpdate)
        {
            var command = new ChangeTaskDetailsCommand(workFlowGuid, reportInstanceId, id, reportInstanceTaskDetailsForUpdate.Source, reportInstanceTaskDetailsForUpdate.Description);

            _logger.LogInformation(
                "----- Sending command: ChangeTaskDetailsCommand - {workFlowGuid}: {reportInstanceId}: {id}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceTaskId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Change current status for report instance task
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="id">The unique identifier of the reporting instance task</param>
        /// <param name="reportInstanceTaskStatusForUpdate">The payload for updating task status</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}/status", Name = "UpdateReportInstanceTaskStatus")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceTaskStatus(Guid workFlowGuid, int reportInstanceId, int id,
            [FromBody] ReportInstanceTaskStatusForUpdateDto reportInstanceTaskStatusForUpdate)
        {
            var taskStatus = Core.Aggregates.ReportInstanceAggregate.TaskStatus.FromName(reportInstanceTaskStatusForUpdate.TaskStatus.ToString());
            var command = new ChangeTaskStatusCommand(workFlowGuid, reportInstanceId, id, taskStatus);

            _logger.LogInformation(
                "----- Sending command: ChangeTaskStatusCommand - {workFlowGuid}: {reportInstanceId}: {id}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceTaskId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get a single report instance task comment using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique id of the report instance</param>
        /// <param name="reportInstanceTaskId">The unique id of the report instance task</param>
        /// <param name="id">The unique id of the report instance task</param>
        /// <returns>An ActionResult of type ReportInstanceTaskCommentIdentifierDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{reportInstanceTaskId}/comments/{id}", Name = "GetReportInstanceTaskCommentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<ReportInstanceTaskCommentIdentifierDto>> GetReportInstanceTaskCommentByIdentifier(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, int id)
        {
            var query = new ReportInstanceTaskCommentIdentifierQuery(workFlowGuid, reportInstanceId, reportInstanceTaskId, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceTaskCommentIdentifierQuery - {workFlowGuid}: {reportInstanceId}: {reportInstanceTaskId}: {id}",
                workFlowGuid.ToString(),
                reportInstanceId,
                reportInstanceTaskId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Add a new comment to a report instance task
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="reportInstanceTaskId">The unique identifier of the reporting instance task</param>
        /// <param name="reportInstanceTaskCommentForCreation">The payload containing details of the task comment</param>
        /// <returns></returns>
        [HttpPost("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{reportInstanceTaskId}/comments", Name = "CreateReportInstanceTaskComment")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateReportInstanceTaskComment(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, 
            [FromBody] ReportInstanceTaskCommentForCreationDto reportInstanceTaskCommentForCreation)
        {
            if (reportInstanceTaskCommentForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for command");
                return BadRequest(ModelState);
            }

            var command = new AddCommentToReportInstanceTaskCommand(workFlowGuid, reportInstanceId, reportInstanceTaskId, reportInstanceTaskCommentForCreation.Comment);

            _logger.LogInformation(
                "----- Sending command: AddCommentToReportInstanceTaskCommand - {workFlowGuid}: {reportInstanceId}: {reportInstanceTaskId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceTaskId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetReportInstanceTaskCommentByIdentifier",
                new
                {
                    workFlowGuid,
                    reportInstanceId,
                    reportInstanceTaskId,
                    id = commandResult.Id
                }, commandResult);
        }

        private async Task<T> GetReportInstanceAsync<T>(Guid workFlowGuid, long id) where T : class
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == id, new string[] { "Activities", "Tasks" });

            if (reportInstanceFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedReportInstance = _mapper.Map<T>(reportInstanceFromRepo);

                return mappedReportInstance;
            }

            return null;
        }

        private ReportInstanceIdentifierDto CreateLinksForReportInstance<T>(Guid workFlowGuid, T dto)
        {
            ReportInstanceIdentifierDto identifier = (ReportInstanceIdentifierDto)(object)dto;

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateReportInstanceResourceUri(workFlowGuid, identifier.Id), "self", "GET"));

            var reportInstance = _reportInstanceRepository.Get(r => r.Id == identifier.Id, new string[] { "WorkFlow", "Activities.ExecutionEvents", "Activities.CurrentStatus" });
            if (reportInstance == null)
            {
                return identifier;
            }

            var currentActivityInstance = reportInstance.Activities.Single(a => a.Current == true);

            switch (reportInstance.CurrentActivity.QualifiedName)
            {
                case "Confirm Report Data":
                    if (currentActivityInstance.CurrentStatus.Description == "UNCONFIRMED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "confirm", "PUT"));
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "delete", "PUT"));
                    }

                    break;

                case "Set MedDRA and Causality":
                    identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceTerminology", workFlowGuid, identifier.Id), "setmeddra", "PUT"));

                    if (currentActivityInstance.CurrentStatus.Description != "NOTSET")
                    {
                        // ConfigType.AssessmentScale
                        var configValue = _configRepository.Get(c => c.ConfigType == ConfigType.AssessmentScale).ConfigValue;

                        if (configValue == "Both Scales" || configValue == "WHO Scale")
                        {
                            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "whocausalityset", "PUT"));
                        }

                        if (configValue == "Both Scales" || configValue == "Naranjo Scale")
                        {
                            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "naranjocausalityset", "PUT"));
                        }

                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "causalityset", "PUT"));
                    }

                    break;

                case "Extract E2B":
                    if (currentActivityInstance.CurrentStatus.Description == "NOTGENERATED" || currentActivityInstance.CurrentStatus.Description == "E2BSUBMITTED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("CreateE2BInstance", workFlowGuid, identifier.Id), "createe2b", "PUT"));
                    }
                    if (currentActivityInstance.CurrentStatus.Description == "E2BINITIATED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "preparereporte2b", "PUT"));

                        var evt = currentActivityInstance.ExecutionEvents
                            .OrderByDescending(ee => ee.EventDateTime)
                            .First(ee => ee.ExecutionStatus.Id == currentActivityInstance.CurrentStatus.Id);
                        var tag = (reportInstance.WorkFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

                        var datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                            .Queryable()
                            .Where(di => di.Tag == tag
                                && di.ContextId == evt.Id)
                            .SingleOrDefault();

                        if(datasetInstance != null)
                        {
                            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDatasetInstanceResourceUri(datasetInstance.Dataset.Id, datasetInstance.Id), "updatee2b", "PUT"));
                        }
                    }

                    if (currentActivityInstance.CurrentStatus.Description == "E2BGENERATED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "confirmsubmissione2b", "PUT"));

                        var executionEvent = currentActivityInstance.ExecutionEvents
                            .OrderByDescending(ee => ee.EventDateTime)
                            .First(ee => ee.ExecutionStatus.Description == "E2BGENERATED");
                        if (executionEvent != null)
                        {
                            var e2bAttachment = executionEvent.Attachments.SingleOrDefault(att => att.Description == "E2b");
                            if (e2bAttachment != null)
                            {
                                identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateDownloadActivitySingleAttachmentResourceUri(workFlowGuid, reportInstance.Id, executionEvent.Id, e2bAttachment.Id), "downloadxml", "GET"));
                            }
                        }
                    }

                    break;

                default:
                    break;
            }

            var validRoles = new string[] { "RegClerk", "DataCap", "Clinician" };
            //if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report" && _userRoleRepository.Exists(ur => ur.User.Id == user.Id && validRoles.Contains(ur.Role.Key)))
            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", identifier.Id), "viewpatient", "GET"));
            }

            if (reportInstance.WorkFlow.Description == "New Spontaneous Surveilliance Report")
            {
                var datasetInstance = _datasetInstanceRepository.Get(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                if (datasetInstance != null)
                {
                    identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDatasetInstanceResourceUri(datasetInstance.Dataset.Id, datasetInstance.Id), "updatespont", "PUT"));
                }
            }

            return identifier;
        }
    }
}
