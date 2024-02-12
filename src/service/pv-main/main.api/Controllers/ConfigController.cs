using AutoMapper;
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
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class ConfigsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public ConfigsController(ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Config> configRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all configurations using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConfigIdentifierDto</returns>
        [HttpGet("configs", Name = "GetConfigsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<ConfigIdentifierDto>> GetConfigsByIdentifier(
            [FromQuery] ConfigResourceParameters configResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConfigIdentifierDto>
                (configResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedConfigsWithLinks = GetConfigs<ConfigIdentifierDto>(configResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ConfigIdentifierDto>(mappedConfigsWithLinks.TotalCount, mappedConfigsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, configResourceParameters,
            //    mappedConfigsWithLinks.HasNext, mappedConfigsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single configuration using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab result</param>
        /// <returns>An ActionResult of type ConfigIdentifierDto</returns>
        [HttpGet("configs/{id}", Name = "GetConfigByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<ConfigIdentifierDto>> GetConfigByIdentifier(long id)
        {
            var mappedConfig = await GetConfigAsync<ConfigIdentifierDto>(id);
            if (mappedConfig == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForConfig<ConfigIdentifierDto>(mappedConfig));
        }

        /// <summary>
        /// Update an existing configuration
        /// </summary>
        /// <param name="id">The unique id of the config</param>
        /// <param name="configForUpdate">The config payload</param>
        /// <returns></returns>
        [HttpPut("configs/{id}", Name = "UpdateConfig")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateConfig(long id,
            [FromBody] ConfigForUpdateDto configForUpdate)
        {
            if (configForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var configFromRepo = await _configRepository.GetAsync(f => f.Id == id);
            if (configFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                configFromRepo.ConfigValue = String.IsNullOrWhiteSpace(configForUpdate.ConfigValue) ? "-- not specified --" : configForUpdate.ConfigValue;

                _configRepository.Update(configFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Seed source database
        /// </summary>
        /// <returns></returns>
        [HttpPost("configs/seed", Name = "SeedDatabase")]
        public IActionResult SeedDatabase()
        {
            _unitOfWork.Seed();

            return Ok();
        }

        /// <summary>
        /// Get lab results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="configResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetConfigs<T>(ConfigResourceParameters configResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = configResourceParameters.PageNumber,
                PageSize = configResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Config>(configResourceParameters.OrderBy, "asc");

            var pagedConfigsFromRepo = _configRepository.List(pagingInfo, null, orderby, "");
            if (pagedConfigsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConfigs = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedConfigsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedConfigsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedConfigs.TotalCount,
                    pageSize = mappedConfigs.PageSize,
                    currentPage = mappedConfigs.CurrentPage,
                    totalPages = mappedConfigs.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedConfigs.ForEach(dto => CreateLinksForConfig(dto));

                return mappedConfigs;
            }

            return null;
        }

        /// <summary>
        /// Get single lab result from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetConfigAsync<T>(long id) where T : class
        {
            var configFromRepo = await _configRepository.GetAsync(f => f.Id == id);

            if (configFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConfig = _mapper.Map<T>(configFromRepo);

                return mappedConfig;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private ConfigIdentifierDto CreateLinksForConfig<T>(T dto)
        {
            ConfigIdentifierDto identifier = (ConfigIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Config", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
