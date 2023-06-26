using AutoMapper;
using Ionic.Zip;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Commands.AttachmentAggregate;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.Queries.PatientAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IPatientQueries _patientQueries;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly IRepositoryInt<PatientFacility> _patientFacilityRepository;
        private readonly IRepositoryInt<PatientStatusHistory> _patientStatusHistoryRepository;
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IReportService _reportService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IMediator mediator, 
            IPropertyMappingService propertyMappingService, 
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IPatientQueries patientQueries,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            IRepositoryInt<PatientFacility> patientFacilityRepository,
            IRepositoryInt<PatientStatusHistory> patientStatusHistoryRepository,
            IRepositoryInt<Appointment> appointmentRepository,
            IRepositoryInt<Attachment> attachmentRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<User> userRepository,
            IReportService reportService,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PatientsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _patientFacilityRepository = patientFacilityRepository ?? throw new ArgumentNullException(nameof(patientFacilityRepository));
            _patientStatusHistoryRepository = patientStatusHistoryRepository ?? throw new ArgumentNullException(nameof(patientStatusHistoryRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all Patients using a valid media type 
        /// </summary>
        /// <param name="patientResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of PatientIdentifierDto</returns>
        [HttpGet(Name = "GetPatientsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<PatientIdentifierDto>>> GetPatientsByIdentifier(
            [FromQuery] PatientResourceParameters patientResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<PatientIdentifierDto>
                (patientResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new PatientsIdentifierQuery(patientResourceParameters.OrderBy,
                patientResourceParameters.FacilityName,
                patientResourceParameters.CustomAttributeId,
                patientResourceParameters.CustomAttributeValue,
                patientResourceParameters.PatientId,
                patientResourceParameters.DateOfBirth,
                patientResourceParameters.FirstName,
                patientResourceParameters.LastName,
                patientResourceParameters.PageNumber,
                patientResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: GetPatientsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = patientResourceParameters.PageSize,
                currentPage = patientResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all Patients using a valid media type 
        /// </summary>
        /// <param name="patientResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of PatientDetailDto</returns>
        [HttpGet(Name = "GetPatientsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<PatientDetailDto>>> GetPatientsByDetail(
            [FromQuery] PatientResourceParameters patientResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<PatientDetailDto, Patient>
               (patientResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new PatientsDetailQuery(patientResourceParameters.OrderBy,
                patientResourceParameters.FacilityName,
                patientResourceParameters.CustomAttributeId,
                patientResourceParameters.CustomAttributeValue,
                patientResourceParameters.PatientId,
                patientResourceParameters.DateOfBirth,
                patientResourceParameters.FirstName,
                patientResourceParameters.LastName,
                patientResourceParameters.CaseNumber,
                patientResourceParameters.PageNumber,
                patientResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: GetPatientsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = patientResourceParameters.PageSize,
                currentPage = patientResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single patient by searching for a matching concomitant condition
        /// </summary>
        /// <param name="patientByConditionResourceParameters">
        /// Specify condition search term
        /// </param>
        /// <returns>An ActionResult of type PatientExpandedDto</returns>
        [HttpGet(Name = "GetPatientByCondition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientExpandedDto>> GetPatientByCondition(
            [FromQuery] PatientByConditionResourceParameters patientByConditionResourceParameters)
        {
            var query = new PatientExpandedByConditionTermQuery(
                patientByConditionResourceParameters.CaseNumber);

            _logger.LogInformation(
                "----- Sending query: PatientExpandedByConditionTermQuery");

            var queryResult = await _mediator.Send(query);

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Patient using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Patient</param>
        /// <returns>An ActionResult of type PatientIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetPatientByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientIdentifierDto>> GetPatientByIdentifier(int id)
        {
            var query = new PatientIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: PatientIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Patient using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Patient</param>
        /// <returns>An ActionResult of type ProgramDetailDto</returns>
        [HttpGet("{id}", Name = "GetPatientByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientDetailDto>> GetPatientByDetail(int id)
        {
            var query = new PatientDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: PatientDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Patient using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Patient</param>
        /// <returns>An ActionResult of type PatientExpandedDto</returns>
        [HttpGet("{id}", Name = "GetPatientByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientExpandedDto>> GetPatientByExpanded(int id)
        {
            var query = new PatientExpandedQuery(id);

            _logger.LogInformation(
                "----- Sending query: PatientExpandedQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Adverse event report
        /// </summary>
        /// <param name="adverseEventReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AdverseEventReportDto</returns>
        [HttpGet(Name = "GetAdverseEventReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.adverseventreport.v1+json", "application/vnd.pvims.adverseventreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.adverseventreport.v1+json", "application/vnd.pvims.adverseventreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<AdverseEventReportDto>>> GetAdverseEventReport(
                        [FromQuery] AdverseEventReportResourceParameters adverseEventReportResourceParameters)
        {
            if (adverseEventReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var query = new AdverseEventReportQuery(adverseEventReportResourceParameters.PageNumber,
                adverseEventReportResourceParameters.PageSize,
                adverseEventReportResourceParameters.SearchFrom,
                adverseEventReportResourceParameters.SearchTo,
                adverseEventReportResourceParameters.AdverseEventStratifyCriteria,
                adverseEventReportResourceParameters.AgeGroupCriteria,
                adverseEventReportResourceParameters.GenderId,
                adverseEventReportResourceParameters.RegimenId,
                adverseEventReportResourceParameters.OrganisationUnitId,
                adverseEventReportResourceParameters.OutcomeId,
                adverseEventReportResourceParameters.IsSeriousId,
                adverseEventReportResourceParameters.SeriousnessId,
                adverseEventReportResourceParameters.ClassificationId);

            _logger.LogInformation("----- Sending query: AdverseEventReportQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = adverseEventReportResourceParameters.PageSize,
                currentPage = adverseEventReportResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Adverse event frequency report
        /// </summary>
        /// <param name="adverseEventFrequencyReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AdverseEventFrequencyReportDto</returns>
        [HttpGet(Name = "GetAdverseEventFrequencyReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.adverseventfrequencyreport.v1+json", "application/vnd.pvims.adverseventfrequencyreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.adverseventfrequencyreport.v1+json", "application/vnd.pvims.adverseventfrequencyreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>>> GetAdverseEventFrequencyReport(
                        [FromQuery] AdverseEventFrequencyReportResourceParameters adverseEventFrequencyReportResourceParameters)
        {
            if (adverseEventFrequencyReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var query = new AdverseEventFrequencyReportQuery(adverseEventFrequencyReportResourceParameters.PageNumber,
                adverseEventFrequencyReportResourceParameters.PageSize,
                adverseEventFrequencyReportResourceParameters.SearchFrom,
                adverseEventFrequencyReportResourceParameters.SearchTo,
                adverseEventFrequencyReportResourceParameters.FrequencyCriteria);

            _logger.LogInformation("----- Sending query: AdverseEventFrequencyReportQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = adverseEventFrequencyReportResourceParameters.PageSize,
                currentPage = adverseEventFrequencyReportResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Patient on treatment report
        /// </summary>
        /// <param name="patientTreatmentReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type PatientTreatmentReportDto</returns>
        [HttpGet(Name = "GetPatientTreatmentReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.patienttreatmentreport.v1+json", "application/vnd.pvims.patienttreatmentreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.patienttreatmentreport.v1+json", "application/vnd.pvims.patienttreatmentreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<PatientsOnTreatmentDto>>> GetPatientTreatmentReport(
                        [FromQuery] PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters)
        {
            if (patientTreatmentReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var query = new PatientTreatmentReportQuery(patientTreatmentReportResourceParameters.PageNumber, 
                patientTreatmentReportResourceParameters.PageSize, 
                patientTreatmentReportResourceParameters.SearchFrom, 
                patientTreatmentReportResourceParameters.SearchTo,
                patientTreatmentReportResourceParameters.PatientOnStudyCriteria);

            _logger.LogInformation("----- Sending query: PatientTreatmentReportQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = patientTreatmentReportResourceParameters.PageSize,
                currentPage = patientTreatmentReportResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Patient by drug report
        /// </summary>
        /// <param name="patientMedicationReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type PatientMedicationReportDto</returns>
        [HttpGet(Name = "GetPatientByMedicationReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.patientmedicationreport.v1+json", "application/vnd.pvims.patientmedicationreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.patientmedicationreport.v1+json", "application/vnd.pvims.patientmedicationreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<PatientMedicationReportDto>>> GetPatientByMedicationReport(
                        [FromQuery] PatientMedicationReportResourceParameters patientMedicationReportResourceParameters)
        {
            if (patientMedicationReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetPatientMedicationResults<PatientMedicationReportDto>(patientMedicationReportResourceParameters);

            // Add custom mappings to patients
            foreach(var mappedResult in mappedResults)
            {
                await CustomPatientMedicationReportMapAsync(mappedResult);
            }

            var wrapper = new LinkedCollectionResourceWrapperDto<PatientMedicationReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForPatientMedicationReport(wrapper, patientMedicationReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single patient attachment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the attachment</param>
        /// <returns>An ActionResult of type PatientIdentifierDto</returns>
        [HttpGet("{patientId}/attachments/{id}", Name = "GetPatientAttachmentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<AttachmentIdentifierDto>> GetPatientAttachmentByIdentifier(int patientId, int id)
        {
            var mappedAttachment = await GetAttachmentAsync<AttachmentIdentifierDto>(patientId, id);
            if (mappedAttachment == null)
            {
                return NotFound();
            }


            return Ok(CreateLinksForAttachment<AttachmentIdentifierDto>(patientId, mappedAttachment));
        }

        /// <summary>
        /// Download a file attachment
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the attachment</param>
        /// <returns>An ActionResult</returns>
        [HttpGet("{patientId}/attachments/{id}", Name = "DownloadSingleAttachment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.attachment.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadSingleAttachment(int patientId, int id)
        {
            var attachmentFromRepo = await _attachmentRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id, new string[] { "AttachmentType" });
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
        /// Download all file attachments
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <returns>An ActionResult of type AuditLogIdentifierDto</returns>
        [HttpGet("{patientId}/attachments", Name = "DownloadAllAttachment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.attachment.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.attachment.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadAllAttachment(int patientId)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            FileStream fs;
            var attachmentFileNames = new List<string>();
            var generatedDate = DateTime.Now;

            foreach (Attachment attachment in patientFromRepo.Attachments.Where(a => a.Archived == false))
            {
                byte[] buffer = (byte[])attachment.Content;
                var attachmentFile = $"{Path.GetTempPath()}{attachment.FileName}";
                fs = new FileStream(attachmentFile, FileMode.Create, FileAccess.Write);

                // Writes a block of bytes to this stream using data from // a byte array.
                fs.Write(buffer, 0, attachment.Content.Length);

                // close file stream
                fs.Close();
                attachmentFileNames.Add(attachment.FileName);
            }

            var destFile = $"{Path.GetTempPath()}Patient_{patientId.ToString()}_Attachments_{generatedDate.ToString("yyyyMMddhhmmss")}.zip";

            using (var zip = new ZipFile())
            {
                zip.AddFiles(attachmentFileNames.Select(f => $"{Path.GetTempPath()}{f}").ToList(), string.Empty);
                zip.Save(destFile);
            }

            return PhysicalFile(destFile, "application/zip");
        }

        /// <summary>
        /// Create a new patient record
        /// </summary>
        /// <param name="patientForCreation">The patient payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreatePatient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientForCreationDto patientForCreation)
        {
            if (patientForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new patient");
                return BadRequest(ModelState);
            }

            var command = new AddPatientCommand(patientForCreation.FirstName,
                patientForCreation.LastName,
                patientForCreation.MiddleName,
                patientForCreation.DateOfBirth,
                patientForCreation.FacilityName,
                patientForCreation.ConditionGroupId,
                patientForCreation.MeddraTermId,
                patientForCreation.CohortGroupId,
                patientForCreation.EnroledDate,
                patientForCreation.StartDate,
                patientForCreation.OutcomeDate,
                patientForCreation.CaseNumber,
                patientForCreation.Comments,
                patientForCreation.EncounterTypeId,
                patientForCreation.PriorityId,
                patientForCreation.EncounterDate,
                patientForCreation.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddPatientCommand - {lastName}",
                command.LastName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update the custom attributes of the patient
        /// </summary>
        /// <param name="id">The unique id of the patient</param>
        /// <param name="patientCustomAttributesForUpdate">The patient custom attributes payload</param>
        /// <returns></returns>
        [HttpPut("{id}/custom", Name = "UpdatePatientCustomAttributes")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientCustomAttributes(int id,
            [FromBody] PatientCustomAttributesForUpdateDto patientCustomAttributesForUpdate)
        {
            if (patientCustomAttributesForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangePatientCustomAttributesCommand(id, patientCustomAttributesForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangePatientCustomAttributesCommand - {patientId}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Update the date of the birth of the patient
        /// </summary>
        /// <param name="id">The unique id of the patient</param>
        /// <param name="patientDateOfBirthForUpdate">The patient date of birth payload</param>
        /// <returns></returns>
        [HttpPut("{id}/dateofbirth", Name = "UpdatePatientDateOfBirth")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientDateOfBirth(int id,
            [FromBody] PatientDateOfBirthForUpdateDto patientDateOfBirthForUpdate)
        {
            if (patientDateOfBirthForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangePatientDateOfBirthCommand(id, patientDateOfBirthForUpdate.DateOfBirth);

            _logger.LogInformation(
                "----- Sending command: ChangePatientDateOfBirthCommand - {patientId}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Update the current facility of the patient
        /// </summary>
        /// <param name="id">The unique id of the patient</param>
        /// <param name="patientFacilityForUpdate">The patient facility payload</param>
        /// <returns></returns>
        [HttpPut("{id}/facility", Name = "UpdatePatientFacility")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientFacility(int id,
            [FromBody] PatientFacilityForUpdateDto patientFacilityForUpdate)
        {
            if (patientFacilityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangePatientFacilityCommand(id, patientFacilityForUpdate.FacilityName);

            _logger.LogInformation(
                "----- Sending command: ChangePatientFacilityCommand - {patientId}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Update the name of the patient
        /// </summary>
        /// <param name="id">The unique id of the patient</param>
        /// <param name="patientNameForUpdate">The patient name payload</param>
        /// <returns></returns>
        [HttpPut("{id}/name", Name = "UpdatePatientName")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientName(int id, 
            [FromBody] PatientNameForUpdateDto patientNameForUpdate)
        {
            if (patientNameForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangePatientNameCommand(id, patientNameForUpdate.FirstName, patientNameForUpdate.MiddleName, patientNameForUpdate.LastName);

            _logger.LogInformation(
                "----- Sending command: ChangePatientNameCommand - {patientId}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Update the generic notes of the patient
        /// </summary>
        /// <param name="id">The unique id of the patient</param>
        /// <param name="patientNotesForUpdate">The patient notes payload</param>
        /// <returns></returns>
        [HttpPut("{id}/notes", Name = "UpdatePatientNotes")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientNotes(int id,
            [FromBody] PatientNotesForUpdateDto patientNotesForUpdate)
        {
            if (patientNotesForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangePatientNotesCommand(id, patientNotesForUpdate.Notes);

            _logger.LogInformation(
                "----- Sending command: ChangePatientNotesCommand - {patientId}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Archive an existing patient
        /// </summary>
        /// <param name="id">The unique identifier of the patient</param>
        /// <param name="patientForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{Id}/archive", Name = "ArchivePatient")]
        [Authorize(Roles = "Clinician")]
        public async Task<IActionResult> ArchivePatient(long id,
            [FromBody] ArchiveDto patientForDelete)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == id);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(patientForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < patientForDelete.Reason.Length)
            {
                ModelState.AddModelError("Message", "Reason contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("Message", "Unable to locate user");
            }

            if (ModelState.IsValid)
            {
                foreach (var appointment in patientFromRepo.Appointments.Where(x => !x.Archived))
                {
                    appointment.Archive(user, patientForDelete.Reason);
                    _appointmentRepository.Update(appointment);
                }

                foreach (var attachment in patientFromRepo.Attachments.Where(x => !x.Archived))
                {
                    attachment.ArchiveAttachment(user, patientForDelete.Reason);
                    _attachmentRepository.Update(attachment);
                }

                foreach (var enrolment in patientFromRepo.CohortEnrolments.Where(x => !x.Archived))
                {
                    enrolment.Archived = true;
                    enrolment.ArchivedDate = DateTime.Now;
                    enrolment.ArchivedReason = patientForDelete.Reason;
                    enrolment.AuditUser = user;
                    _cohortGroupEnrolmentRepository.Update(enrolment);
                }

                foreach (var encounter in patientFromRepo.Encounters.Where(x => !x.Archived))
                {
                    encounter.Archived = true;
                    encounter.ArchivedDate = DateTime.Now;
                    encounter.ArchivedReason = patientForDelete.Reason;
                    encounter.AuditUser = user;
                    _encounterRepository.Update(encounter);
                }

                foreach (var patientFacility in patientFromRepo.PatientFacilities.Where(x => !x.Archived))
                {
                    patientFacility.Archived = true;
                    patientFacility.ArchivedDate = DateTime.Now;
                    patientFacility.ArchivedReason = patientForDelete.Reason;
                    patientFacility.AuditUser = user;
                    _patientFacilityRepository.Update(patientFacility);
                }

                foreach (var patientClinicalEvent in patientFromRepo.PatientClinicalEvents.Where(x => !x.Archived))
                {
                    patientClinicalEvent.Archive(user, patientForDelete.Reason);
                    _patientClinicalEventRepository.Update(patientClinicalEvent);
                }

                foreach (var patientCondition in patientFromRepo.PatientConditions.Where(x => !x.Archived))
                {
                    patientCondition.Archived = true;
                    patientCondition.ArchivedDate = DateTime.Now;
                    patientCondition.ArchivedReason = patientForDelete.Reason;
                    patientCondition.AuditUser = user;
                    _patientConditionRepository.Update(patientCondition);
                }

                foreach (var patientLabTest in patientFromRepo.PatientLabTests.Where(x => !x.Archived))
                {
                    patientLabTest.Archive(user, patientForDelete.Reason);
                    _patientLabTestRepository.Update(patientLabTest);
                }

                foreach (var patientMedication in patientFromRepo.PatientMedications.Where(x => !x.Archived))
                {
                    patientMedication.Archive(user, patientForDelete.Reason);
                    _patientMedicationRepository.Update(patientMedication);
                }

                foreach (var patientStatusHistory in patientFromRepo.PatientStatusHistories.Where(x => !x.Archived))
                {
                    patientStatusHistory.Archived = true;
                    patientStatusHistory.ArchivedDate = DateTime.Now;
                    patientStatusHistory.ArchivedReason = patientForDelete.Reason;
                    patientStatusHistory.AuditUser = user;
                    _patientStatusHistoryRepository.Update(patientStatusHistory);
                }

                patientFromRepo.Archived = true;
                patientFromRepo.ArchivedDate = DateTime.Now;
                patientFromRepo.ArchivedReason = patientForDelete.Reason;
                patientFromRepo.AuditUser = user;
                _patientRepository.Update(patientFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new patient attachment
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="patientAttachmentForCreation">The attachment payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/attachments", Name = "CreatePatientAttachment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePatientAttachment(int patientId, [FromForm] PatientAttachmentForCreationDto patientAttachmentForCreation)
        {
            if (patientAttachmentForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new attachment");
            }

            var command = new AddAttachmentCommand(patientId, patientAttachmentForCreation.Description, patientAttachmentForCreation.Attachment);

            _logger.LogInformation(
                "----- Sending command: AddAttachmentCommand - {patientId}",
                patientId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientAttachmentByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Archive an existing attachment
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the attachment</param>
        /// <param name="attachmentForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/attachments/{id}/archive", Name = "ArchivePatientAttachment")]
        public async Task<IActionResult> ArchivePatientAttachment(int patientId, int id,
            [FromBody] ArchiveDto attachmentForDelete)
        {
            if (attachmentForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ArchiveAttachmentCommand(patientId, id, attachmentForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchiveAttachmentCommand - {patientId}: {attachmentId}",
                command.PatientId,
                command.AttachmentId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get single attachment from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Resource parent id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetAttachmentAsync<T>(int patientId, int id) where T : class
        {
            var attachmentFromRepo = await _attachmentRepository.GetAsync(a => a.Patient.Id == patientId && a.Id == id);

            if (attachmentFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedAttachment = _mapper.Map<T>(attachmentFromRepo);

                return mappedAttachment;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="patientMedicationReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForPatientMedicationReport(
            LinkedResourceBaseDto wrapper,
            PatientMedicationReportResourceParameters patientMedicationReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreatePatientMedicationReportResourceUri(ResourceUriType.Current, patientMedicationReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientMedicationReportResourceUri(ResourceUriType.NextPage, patientMedicationReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientMedicationReportResourceUri(ResourceUriType.PreviousPage, patientMedicationReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="patientId">The unique identifier of the parent resource</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private AttachmentIdentifierDto CreateLinksForAttachment<T>(int patientId, T dto)
        {
            AttachmentIdentifierDto identifier = (AttachmentIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreatePatientAppointmentResourceUri(patientId, identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private async Task CustomPatientMedicationReportMapAsync(PatientMedicationReportDto dto)
        {
            dto.Patients = (List<PatientListDto>) await _patientQueries.GetPatientListByConceptAsync(dto.ConceptId);
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="patientMedicationReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetPatientMedicationResults<T>(PatientMedicationReportResourceParameters patientMedicationReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = patientMedicationReportResourceParameters.PageNumber,
                PageSize = patientMedicationReportResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<DrugList>.Create(_reportService.GetPatientsByDrugItems(patientMedicationReportResourceParameters.SearchTerm), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }
    }
}
