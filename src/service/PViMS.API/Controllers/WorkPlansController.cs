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
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/workplans")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class WorkPlansController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<WorkPlan> _workPlanRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public WorkPlansController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<WorkPlan> workPlanRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _workPlanRepository = workPlanRepository ?? throw new ArgumentNullException(nameof(workPlanRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all work plans using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of WorkPlanIdentifierDto</returns>
        [HttpGet(Name = "GetWorkPlansByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<WorkPlanIdentifierDto>> GetWorkPlansByIdentifier(
            [FromQuery] IdResourceParameters workPlanResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<WorkPlanIdentifierDto>
                (workPlanResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedWorkPlansWithLinks = GetWorkPlans<WorkPlanIdentifierDto>(workPlanResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<WorkPlanIdentifierDto>(mappedWorkPlansWithLinks.TotalCount, mappedWorkPlansWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, workPlanResourceParameters,
            //    mappedWorkPlansWithLinks.HasNext, mappedWorkPlansWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all work plans using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of WorkPlanDetailDto</returns>
        [HttpGet(Name = "GetWorkPlansByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<WorkPlanDetailDto>> GetWorkPlansByDetail(
            [FromQuery] IdResourceParameters workPlanResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<WorkPlanDetailDto>
                (workPlanResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedWorkPlansWithLinks = GetWorkPlans<WorkPlanDetailDto>(workPlanResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<WorkPlanDetailDto>(mappedWorkPlansWithLinks.TotalCount, mappedWorkPlansWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, workPlanResourceParameters,
            //    mappedWorkPlansWithLinks.HasNext, mappedWorkPlansWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single workPlan using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the workPlan</param>
        /// <returns>An ActionResult of type WorkPlanIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetWorkPlanByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<WorkPlanIdentifierDto>> GetWorkPlanByIdentifier(long id)
        {
            var mappedWorkPlan = await GetWorkPlanAsync<WorkPlanIdentifierDto>(id);
            if (mappedWorkPlan == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForWorkPlan<WorkPlanIdentifierDto>(mappedWorkPlan));
        }

        /// <summary>
        /// Get a single workPlan using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the workPlan</param>
        /// <returns>An ActionResult of type WorkPlanDetailDto</returns>
        [HttpGet("{id}", Name = "GetWorkPlanByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<WorkPlanDetailDto>> GetWorkPlanByDetail(long id)
        {
            var mappedWorkPlan = await GetWorkPlanAsync<WorkPlanDetailDto>(id);
            if (mappedWorkPlan == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForWorkPlan<WorkPlanDetailDto>(mappedWorkPlan));
        }

        private PagedCollection<T> GetWorkPlans<T>(IdResourceParameters workPlanResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = workPlanResourceParameters.PageNumber,
                PageSize = workPlanResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<WorkPlan>(workPlanResourceParameters.OrderBy, "asc");

            var pagedWorkPlansFromRepo = _workPlanRepository.List(pagingInfo, null, orderby, new string[] { "Dataset" });
            if (pagedWorkPlansFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedWorkPlans = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedWorkPlansFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedWorkPlansFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedWorkPlans.TotalCount,
                    pageSize = mappedWorkPlans.PageSize,
                    currentPage = mappedWorkPlans.CurrentPage,
                    totalPages = mappedWorkPlans.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedWorkPlans.ForEach(dto => CreateLinksForWorkPlan(dto));

                return mappedWorkPlans;
            }

            return null;
        }

        /// <summary>
        /// Get single workPlan from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetWorkPlanAsync<T>(long id) where T : class
        {
            var workPlanFromRepo = await _workPlanRepository.GetAsync(f => f.Id == id);

            if (workPlanFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedWorkPlan = _mapper.Map<T>(workPlanFromRepo);

                return mappedWorkPlan;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private WorkPlanIdentifierDto CreateLinksForWorkPlan<T>(T dto)
        {
            WorkPlanIdentifierDto identifier = (WorkPlanIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("WorkPlan", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
