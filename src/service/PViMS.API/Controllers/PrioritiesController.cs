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
    [Route("api/priorities")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PrioritiesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Priority> _priorityRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public PrioritiesController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Priority> priorityRepository)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _priorityRepository = priorityRepository ?? throw new ArgumentNullException(nameof(priorityRepository));
        }

        /// <summary>
        /// Get all priorities using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of PriorityIdentifierDto</returns>
        [HttpGet(Name = "GetPrioritiesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<PriorityIdentifierDto>> GetPrioritiesByIdentifier(
            [FromQuery] IdResourceParameters priorityResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<PriorityIdentifierDto>
                (priorityResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedPrioritesWithLinks = GetPriorites<PriorityIdentifierDto>(priorityResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<PriorityIdentifierDto>(mappedPrioritesWithLinks.TotalCount, mappedPrioritesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, encounterTypeResourceParameters,
            //    mappedPrioritysWithLinks.HasNext, mappedPrioritysWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single priority using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the encounterType</param>
        /// <returns>An ActionResult of type PriorityIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetPriorityByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PriorityIdentifierDto>> GetPriorityByIdentifier(long id)
        {
            var mappedPriority = await GetPriorityAsync<PriorityIdentifierDto>(id);
            if (mappedPriority == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPriority<PriorityIdentifierDto>(mappedPriority));
        }

        /// <summary>
        /// Get priorities from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="priorityResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetPriorites<T>(IdResourceParameters priorityResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = priorityResourceParameters.PageNumber,
                PageSize = priorityResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Priority>(priorityResourceParameters.OrderBy, "asc");

            var pagedPrioritesFromRepo = _priorityRepository.List(pagingInfo, null, orderby, "");
            if (pagedPrioritesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPriorites = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedPrioritesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedPrioritesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedPriorites.TotalCount,
                    pageSize = mappedPriorites.PageSize,
                    currentPage = mappedPriorites.CurrentPage,
                    totalPages = mappedPriorites.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedPriorites.ForEach(dto => CreateLinksForPriority(dto));

                return mappedPriorites;
            }

            return null;
        }

        /// <summary>
        /// Get single priority from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPriorityAsync<T>(long id) where T : class
        {
            var encounterTypeFromRepo = await _priorityRepository.GetAsync(f => f.Id == id);

            if (encounterTypeFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPriority = _mapper.Map<T>(encounterTypeFromRepo);

                return mappedPriority;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PriorityIdentifierDto CreateLinksForPriority<T>(T dto)
        {
            PriorityIdentifierDto identifier = (PriorityIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Priority", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
