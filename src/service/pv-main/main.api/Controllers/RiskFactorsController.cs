using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    /// <summary>
    /// A representation of all risk factors.
    /// </summary>
    [Route("api/riskfactors")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class RiskFactorsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<RiskFactor> _riskFactorRepository;
        private readonly IRepositoryInt<RiskFactorOption> _riskFactorOptionRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public RiskFactorsController(ITypeHelperService typeHelperService,
                IRepositoryInt<RiskFactor> riskFactorRepository,
                IRepositoryInt<RiskFactorOption> riskFactorOptionRepository,
                IMapper mapper,
                ILinkGeneratorService linkGeneratorService)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _riskFactorRepository = riskFactorRepository ?? throw new ArgumentNullException(nameof(riskFactorRepository));
            _riskFactorOptionRepository = riskFactorOptionRepository ?? throw new ArgumentNullException(nameof(riskFactorOptionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
        }

        /// <summary>
        /// Get all risk factors using a valid media type 
        /// </summary>
        /// <param name="riskFactorResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of RiskFactorDetailDto</returns>
        [HttpGet(Name = "GetRiskFactorsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<RiskFactorDetailDto>> GetRiskFactorsByDetail(
            [FromQuery] IdResourceParameters riskFactorResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<RiskFactorDetailDto>
                (riskFactorResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedRiskFactorsWithLinks = GetRiskFactors<RiskFactorDetailDto>(riskFactorResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<RiskFactorDetailDto>(mappedRiskFactorsWithLinks.TotalCount, mappedRiskFactorsWithLinks);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single risk factor using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the risk factor</param>
        /// <returns>An ActionResult of type RiskFactorIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetRiskFactorByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<RiskFactorIdentifierDto>> GetRiskFactorByIdentifier(long id)
        {
            var mappedRiskFactor = await GetRiskFactorAsync<RiskFactorIdentifierDto>(id);
            if (mappedRiskFactor == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForRiskFactor<RiskFactorIdentifierDto>(mappedRiskFactor));
        }

        /// <summary>
        /// Get a single risk factor using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the risk factor</param>
        /// <returns>An ActionResult of type RiskFactorDetailDto</returns>
        [HttpGet("{id}", Name = "GetRiskFactorByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<RiskFactorDetailDto>> GetRiskFactorByDetail(long id)
        {
            var mappedRiskFactor = await GetRiskFactorAsync<RiskFactorDetailDto>(id);
            if (mappedRiskFactor == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForRiskFactor<RiskFactorDetailDto>(mappedRiskFactor));
        }

        /// <summary>
        /// Get risk factors from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="riskFactorResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetRiskFactors<T>(IdResourceParameters riskFactorResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = riskFactorResourceParameters.PageNumber,
                PageSize = riskFactorResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<RiskFactor>(riskFactorResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<RiskFactor>(true);
            predicate = predicate.And(f => f.Active);

            var pagedRiskFactorsFromRepo = _riskFactorRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedRiskFactorsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedRiskFactors = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedRiskFactorsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedRiskFactorsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedRiskFactors.TotalCount,
                    pageSize = mappedRiskFactors.PageSize,
                    currentPage = mappedRiskFactors.CurrentPage,
                    totalPages = mappedRiskFactors.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedRiskFactors.ForEach(dto => CreateLinksForRiskFactor(dto));

                return mappedRiskFactors;
            }

            return null;
        }

        /// <summary>
        /// Get single risk factor from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource guid to search by</param>
        /// <returns></returns>
        private async Task<T> GetRiskFactorAsync<T>(long id) where T : class
        {
            var riskFactorFromRepo = await _riskFactorRepository.GetAsync(f => f.Id == id);

            if (riskFactorFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedRiskFactor = _mapper.Map<T>(riskFactorFromRepo);

                return mappedRiskFactor;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private RiskFactorIdentifierDto CreateLinksForRiskFactor<T>(T dto)
        {
            RiskFactorIdentifierDto identifier = (RiskFactorIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("RiskFactor", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
