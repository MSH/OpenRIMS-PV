using AutoMapper;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/metaforms")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class MetaFormsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FormHandler _formHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MetaFormsController(ITypeHelperService typeHelperService,
            IMapper mapper,
            IRepositoryInt<MetaForm> metaFormRepository,
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            FormHandler formHandler,
            IHttpContextAccessor httpContextAccessor)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _formHandler = formHandler ?? throw new ArgumentNullException(nameof(formHandler));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Get all meta forms using a valid media type 
        /// </summary>
        /// <param name="metaFormResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaFormDetailDto</returns>
        [HttpGet(Name = "GetMetaFormsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaFormDetailDto>> GetMetaFormsByDetail(
            [FromQuery] MetaFormResourceParameters metaFormResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaFormDetailDto>
                (metaFormResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedFormsWithLinks = GetMetaForms<MetaFormDetailDto>(metaFormResourceParameters);

            // Add custom mappings to patients
            //mappedFormsWithLinks.ForEach(dto => CustomPatientMap(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaFormDetailDto>(mappedFormsWithLinks.TotalCount, mappedFormsWithLinks);
            //var wrapperWithLinks = CreateLinksForPatients(wrapper, metaFormResourceParameters,
                //mappedFormsWithLinks.HasNext, mappedFormsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Handle form upload and creation
        /// </summary>
        /// <param name="formForCreation">The form payload</param>
        /// <returns></returns>
        [HttpPut(Name = "CreateForm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateForm([FromBody] FormForCreationDto formForCreation)
        {
            if (formForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new form");
                return BadRequest(ModelState);
            }

            if (formForCreation.HasAttachment == false)
            {
                ModelState.AddModelError("Message", "Unable to process form as no attachment has been submitted");
                return BadRequest(ModelState);
            }

            // Meta form for template extraction
            var metaFormFromRepo = await _metaFormRepository.GetAsync(f => f.ActionName.ToLower().Replace(" ", "") == formForCreation.FormType.ToLower().Replace(" ", ""));
            if (metaFormFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate meta form for template extraction");
                return BadRequest(ModelState);
            }

            // Store user for audit log generation purposes
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = _userRepository.Get(u => u.UserName == userName);
            AuditLog auditLog = null;

            // TODO Use meta form to identify context handler (add/update encounter/patient)

            //var handler = new FormValueHandler(formForCreation.FormValues.Where(fv => fv.FormControlKey == "Object").Select(fv => fv.FormControlValue).ToList());
            _formHandler.SetForm(formForCreation);

            // Validation of the source entity
            _formHandler.ValidateSourceIdentifier();
            if (_formHandler.GetValidationErrors().Count > 0)
            {
                foreach (string message in _formHandler.GetValidationErrors())
                {
                    ModelState.AddModelError("Message", message);
                }
            }

            if (ModelState.IsValid)
            {
                _formHandler.PreparePatientAndClinicalDetail();
                if (_formHandler.GetValidationErrors().Count > 0)
                {
                    foreach (string message in _formHandler.GetValidationErrors())
                    {
                        ModelState.AddModelError("Message", message);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    auditLog = new AuditLog()
                    {
                        AuditType = AuditType.SynchronisationForm,
                        User = userFromRepo,
                        ActionDate = DateTime.Now,
                        Details = $"Form submission successful {formForCreation.FormIdentifier}",
                        Log = JsonConvert.SerializeObject(formForCreation, Formatting.Indented)
                    };
                    await _auditLogRepository.SaveAsync(auditLog);

                    await _formHandler.ProcessFormForCreationOrUpdateAsync();
                    await _unitOfWork.CompleteAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    var error = new Dictionary<string, string>
                    {
                        {"Type", ex.GetType().ToString()},
                        {"Message", ex.Message},
                        {"StackTrace", ex.StackTrace}
                    };
                    auditLog = new AuditLog()
                    {
                        AuditType = AuditType.SynchronisationError,
                        User = userFromRepo,
                        ActionDate = DateTime.Now,
                        Details = $"Error on form {formForCreation.FormIdentifier}",
                        Log = JsonConvert.SerializeObject(error, Formatting.Indented)
                    };
                    _auditLogRepository.Save(auditLog);

                    return StatusCode(500, ex.Message + " " + ex.InnerException?.Message);
                }
            }

            var audit = new AuditLog()
            {
                AuditType = AuditType.SynchronisationError,
                User = userFromRepo,
                ActionDate = DateTime.Now,
                Details = $"Form submission with model error {formForCreation.FormIdentifier}",
                Log = JsonConvert.SerializeObject(formForCreation, Formatting.Indented)
            };
            await _auditLogRepository.SaveAsync(audit);

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get meta forms from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaFormResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMetaForms<T>(MetaFormResourceParameters metaFormResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = metaFormResourceParameters.PageNumber,
                PageSize = metaFormResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaForm>(metaFormResourceParameters.OrderBy, "asc");

            var pagedFormsFromRepo = _metaFormRepository.List(pagingInfo, null, orderby, "");
            if (pagedFormsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedForms = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedFormsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedFormsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedForms.TotalCount,
                    pageSize = mappedForms.PageSize,
                    currentPage = mappedForms.CurrentPage,
                    totalPages = mappedForms.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedFacilities.ForEach(dto => CreateLinksForFacility(dto));

                return mappedForms;
            }

            return null;
        }
    }
}
