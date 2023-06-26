using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/auditlogs")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class AuditLogsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IExcelDocumentService _excelDocumentService;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogsController(ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<Attachment> attachmentRepository,
            IRepositoryInt<User> userRepository,
            IExcelDocumentService excelDocumentService,
            IHttpContextAccessor httpContextAccessor)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _excelDocumentService = excelDocumentService ?? throw new ArgumentNullException(nameof(excelDocumentService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Get all auditLogs using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of AuditLogIdentifierDto</returns>
        [HttpGet(Name = "GetAuditLogsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<AuditLogIdentifierDto>> GetAuditLogsByIdentifier(
            [FromQuery] AuditLogResourceParameters auditLogResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<AuditLogIdentifierDto>
                (auditLogResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedAuditLogsWithLinks = GetAuditLogs<AuditLogIdentifierDto>(auditLogResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AuditLogIdentifierDto>(mappedAuditLogsWithLinks.TotalCount, mappedAuditLogsWithLinks);
            var wrapperWithLinks = CreateLinksForAuditLogs(wrapper, auditLogResourceParameters,
                mappedAuditLogsWithLinks.HasNext, mappedAuditLogsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all auditLogs using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of AuditLogDetailDto</returns>
        [HttpGet(Name = "GetAuditLogsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<AuditLogDetailDto>> GetAuditLogsByDetail(
            [FromQuery] AuditLogResourceParameters auditLogResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<AuditLogDetailDto>
                (auditLogResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedAuditLogsWithLinks = GetAuditLogs<AuditLogDetailDto>(auditLogResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AuditLogDetailDto>(mappedAuditLogsWithLinks.TotalCount, mappedAuditLogsWithLinks);
            var wrapperWithLinks = CreateLinksForAuditLogs(wrapper, auditLogResourceParameters,
                mappedAuditLogsWithLinks.HasNext, mappedAuditLogsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Download a patient dataset for associated audit logs
        /// </summary>
        /// <param name="auditLogResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult</returns>
        [HttpGet(Name = "DownloadPatientDataset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.dataset.v1+json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DownloadPatientDataset(
            [FromQuery] AuditLogResourceParameters auditLogResourceParameters)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);

            if (!userFromRepo.AllowDatasetDownload)
            {
                ModelState.AddModelError("Message", "You do not have permissions to download a dataset");
                return BadRequest(ModelState);
            }

            if(auditLogResourceParameters.AuditType != AuditTypeFilter.SynchronisationSuccessful)
            {
                ModelState.AddModelError("Message", "Invalid audit type for dataset download");
                return BadRequest(ModelState);
            }

            var patientIds = GetPatientsFromAuditLogs(auditLogResourceParameters);

            var model = _excelDocumentService.CreateActiveDatasetForDownload(patientIds.ToArray(), 0);

            return PhysicalFile(model.FullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Get a single auditLog unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the auditLog</param>
        /// <returns>An ActionResult of type AuditLogIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetAuditLogByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<AuditLogIdentifierDto>> GetAuditLogByIdentifier(int id)
        {
            var mappedAuditLog = await GetAuditLogAsync<AuditLogIdentifierDto>(id);
            if (mappedAuditLog == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForAuditLog<AuditLogIdentifierDto>(mappedAuditLog));
        }

        /// <summary>
        /// Download the log for an audit trail
        /// </summary>
        /// <param name="id">The unique identifier for the auditLog</param>
        /// <returns>An ActionResult of type AuditLogIdentifierDto</returns>
        [HttpGet("{id}", Name = "DownloadAuditLog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.auditlog.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.auditlog.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadAuditLog(int id)
        {
            var auditLogFromRepo = await _auditLogRepository.GetAsync(f => f.Id == id);
            if (auditLogFromRepo == null)
            {
                return NotFound();
            }

            if(String.IsNullOrWhiteSpace(auditLogFromRepo.Log))
            {
                return BadRequest();
            }

            string fileNameAndPath = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(fileNameAndPath))
            {
                writer.WriteLine(auditLogFromRepo.Log);
            }

            return PhysicalFile(fileNameAndPath, "text/plain");
        }

        /// <summary>
        /// Get auditLogs from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="auditLogResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetAuditLogs<T>(AuditLogResourceParameters auditLogResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = auditLogResourceParameters.PageNumber,
                PageSize = auditLogResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<AuditLog>(auditLogResourceParameters.OrderBy, "asc");

            // FIlter audit logs
            var predicate = PredicateBuilder.New<AuditLog>(true);
            predicate = predicate.And(au => au.ActionDate >= auditLogResourceParameters.SearchFrom && au.ActionDate <= auditLogResourceParameters.SearchTo);

            switch (auditLogResourceParameters.AuditType)
            {
                case AuditTypeFilter.SubscriberAccess:
                    predicate = predicate.And(au => au.AuditType == AuditType.InvalidSubscriberAccess || au.AuditType == AuditType.ValidSubscriberAccess);
                    break;

                case AuditTypeFilter.SubscriberPost:
                    predicate = predicate.And(au => au.AuditType == AuditType.InValidSubscriberPost || au.AuditType == AuditType.ValidSubscriberPost);
                    break;

                case AuditTypeFilter.MeddraImport:
                    predicate = predicate.And(au => au.AuditType == AuditType.InValidMedDRAImport || au.AuditType == AuditType.ValidMedDRAImport);
                    break;

                case AuditTypeFilter.UserLogin:
                    predicate = predicate.And(au => au.AuditType == AuditType.UserLogin);
                    break;

                case AuditTypeFilter.SynchronisationSuccessful:
                    predicate = predicate.And(au => au.AuditType == AuditType.SynchronisationForm);
                    break;

                case AuditTypeFilter.SynchronisationError:
                    predicate = predicate.And(au => au.AuditType == AuditType.SynchronisationError);
                    break;

                case AuditTypeFilter.DataValidation:
                    predicate = predicate.And(au => au.AuditType == AuditType.DataValidation);
                    break;
            }

            if (auditLogResourceParameters.FacilityId > 0)
            {
                predicate = predicate.And(au => au.User.Facilities.Any(uf => uf.Facility.Id == auditLogResourceParameters.FacilityId));
            }

            var pagedAuditLogsFromRepo = _auditLogRepository.List(pagingInfo, predicate, orderby, new string[] { "User" });
            if (pagedAuditLogsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedAuditLogs = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedAuditLogsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedAuditLogsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedAuditLogs.TotalCount,
                    pageSize = mappedAuditLogs.PageSize,
                    currentPage = mappedAuditLogs.CurrentPage,
                    totalPages = mappedAuditLogs.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedAuditLogs.ForEach(dto => CreateLinksForAuditLog(dto));

                return mappedAuditLogs;
            }

            return null;
        }

        /// <summary>
        /// Get patients from auditLogs
        /// </summary>
        /// <param name="auditLogResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private List<long> GetPatientsFromAuditLogs(AuditLogResourceParameters auditLogResourceParameters)
        {
            var orderby = Extensions.GetOrderBy<AuditLog>(auditLogResourceParameters.OrderBy, "asc");

            // FIlter audit logs
            var predicate = PredicateBuilder.New<AuditLog>(true);
            predicate = predicate.And(au => au.ActionDate >= auditLogResourceParameters.SearchFrom && au.ActionDate <= auditLogResourceParameters.SearchTo);
            predicate = predicate.And(au => au.AuditType == AuditType.SynchronisationForm);

            if (auditLogResourceParameters.FacilityId > 0)
            {
                predicate = predicate.And(au => au.User.Facilities.Any(uf => uf.Facility.Id == auditLogResourceParameters.FacilityId));
            }

            var auditLogsFromRepo = _auditLogRepository.List(predicate, orderby, "");
            if (auditLogsFromRepo != null)
            {
                var returnIds = new List<long>();
                foreach (var auditLog in auditLogsFromRepo)
                {
                    // Extract form identifier
                    var formIdentifier = auditLog.Details.Replace("Form submission successful ", "");

                    // Locate patient
                    if(!string.IsNullOrWhiteSpace(formIdentifier))
                    {
                        var attachments = _attachmentRepository.List(a => a.Patient != null && a.Description == formIdentifier);
                        if(attachments.Count > 0)
                        {
                            var patient = attachments.First()?.Patient;
                            if (patient != null) { returnIds.Add(patient.Id); }
                        }
                    }
                }

                return returnIds;
            }

            return null;
        }

        /// <summary>
        /// Get auditLogs from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="auditLogResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private ICollection<T> GetAuditLogsForExtract<T>(AuditLogResourceParameters auditLogResourceParameters) where T : class
        {
            var orderby = Extensions.GetOrderBy<AuditLog>(auditLogResourceParameters.OrderBy, "asc");

            // FIlter audit logs
            var predicate = PredicateBuilder.New<AuditLog>(true);
            predicate = predicate.And(au => au.ActionDate >= auditLogResourceParameters.SearchFrom && au.ActionDate <= auditLogResourceParameters.SearchTo);

            switch (auditLogResourceParameters.AuditType)
            {
                case AuditTypeFilter.SubscriberAccess:
                    predicate = predicate.And(au => au.AuditType == AuditType.InvalidSubscriberAccess || au.AuditType == AuditType.ValidSubscriberAccess);
                    break;

                case AuditTypeFilter.SubscriberPost:
                    predicate = predicate.And(au => au.AuditType == AuditType.InValidSubscriberPost || au.AuditType == AuditType.ValidSubscriberPost);
                    break;

                case AuditTypeFilter.MeddraImport:
                    predicate = predicate.And(au => au.AuditType == AuditType.InValidMedDRAImport || au.AuditType == AuditType.ValidMedDRAImport);
                    break;

                case AuditTypeFilter.UserLogin:
                    predicate = predicate.And(au => au.AuditType == AuditType.UserLogin);
                    break;

                case AuditTypeFilter.SynchronisationSuccessful:
                    predicate = predicate.And(au => au.AuditType == AuditType.SynchronisationForm);
                    break;

                case AuditTypeFilter.SynchronisationError:
                    predicate = predicate.And(au => au.AuditType == AuditType.SynchronisationError);
                    break;
            }

            if (auditLogResourceParameters.FacilityId > 0)
            {
                predicate = predicate.And(au => au.User.Facilities.Any(uf => uf.Facility.Id == auditLogResourceParameters.FacilityId));
            }

            var auditLogsFromRepo = _auditLogRepository.List(predicate, orderby, "");
            if (auditLogsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedAuditLogs = _mapper.Map<ICollection<T>>(auditLogsFromRepo);

                return mappedAuditLogs;
            }

            return null;
        }

        /// <summary>
        /// Get single auditLog from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetAuditLogAsync<T>(int id) where T : class
        {
            var auditLogFromRepo = await _auditLogRepository.GetAsync(f => f.Id == id);

            if (auditLogFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedAuditLog = _mapper.Map<T>(auditLogFromRepo);

                return mappedAuditLog;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="auditLogResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForAuditLogs(
            LinkedResourceBaseDto wrapper,
            AuditLogResourceParameters auditLogResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAuditLogsResourceUri(ResourceUriType.Current, auditLogResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAuditLogsResourceUri(ResourceUriType.NextPage, auditLogResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAuditLogsResourceUri(ResourceUriType.PreviousPage, auditLogResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private AuditLogIdentifierDto CreateLinksForAuditLog<T>(T dto)
        {
            AuditLogIdentifierDto identifier = (AuditLogIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("AuditLog", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
