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
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class TreatmentOutcomesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<TreatmentOutcome> _treatmentOutcomeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public TreatmentOutcomesController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<TreatmentOutcome> treatmentOutcomeRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _treatmentOutcomeRepository = treatmentOutcomeRepository ?? throw new ArgumentNullException(nameof(treatmentOutcomeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all treatmentOutcomes using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of TreatmentOutcomeIdentifierDto</returns>
        [HttpGet("treatmentoutcomes", Name = "GetTreatmentOutcomesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<TreatmentOutcomeIdentifierDto>> GetTreatmentOutcomesByIdentifier(
            [FromQuery] IdResourceParameters baseResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<TreatmentOutcomeIdentifierDto>
                (baseResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedTreatmentOutcomesWithLinks = GetTreatmentOutcomes<TreatmentOutcomeIdentifierDto>(baseResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<TreatmentOutcomeIdentifierDto>(mappedTreatmentOutcomesWithLinks.TotalCount, mappedTreatmentOutcomesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, treatmentOutcomeResourceParameters,
            //    mappedTreatmentOutcomesWithLinks.HasNext, mappedTreatmentOutcomesWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single treatmentOutcome using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the treatmentOutcome</param>
        /// <returns>An ActionResult of type TreatmentOutcomeIdentifierDto</returns>
        [HttpGet("treatmentoutcomes/{id}", Name = "GetTreatmentOutcomeByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<TreatmentOutcomeIdentifierDto>> GetTreatmentOutcomeByIdentifier(long id)
        {
            var mappedTreatmentOutcome = await GetTreatmentOutcomeAsync<TreatmentOutcomeIdentifierDto>(id);
            if (mappedTreatmentOutcome == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForTreatmentOutcome<TreatmentOutcomeIdentifierDto>(mappedTreatmentOutcome));
        }

        /// <summary>
        /// Get treatmentOutcomes from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="baseResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetTreatmentOutcomes<T>(IdResourceParameters baseResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = baseResourceParameters.PageNumber,
                PageSize = baseResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<TreatmentOutcome>(baseResourceParameters.OrderBy, "asc");

            var pagedTreatmentOutcomesFromRepo = _treatmentOutcomeRepository.List(pagingInfo, null, orderby, "");
            if (pagedTreatmentOutcomesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedTreatmentOutcomes = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedTreatmentOutcomesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedTreatmentOutcomesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedTreatmentOutcomes.TotalCount,
                    pageSize = mappedTreatmentOutcomes.PageSize,
                    currentPage = mappedTreatmentOutcomes.CurrentPage,
                    totalPages = mappedTreatmentOutcomes.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedTreatmentOutcomes.ForEach(dto => CreateLinksForTreatmentOutcome(dto));

                return mappedTreatmentOutcomes;
            }

            return null;
        }

        /// <summary>
        /// Get single treatmentOutcome from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetTreatmentOutcomeAsync<T>(long id) where T : class
        {
            var treatmentOutcomeFromRepo = await _treatmentOutcomeRepository.GetAsync(f => f.Id == id);

            if (treatmentOutcomeFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedTreatmentOutcome = _mapper.Map<T>(treatmentOutcomeFromRepo);

                return mappedTreatmentOutcome;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private TreatmentOutcomeIdentifierDto CreateLinksForTreatmentOutcome<T>(T dto)
        {
            TreatmentOutcomeIdentifierDto identifier = (TreatmentOutcomeIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("TreatmentOutcome", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
