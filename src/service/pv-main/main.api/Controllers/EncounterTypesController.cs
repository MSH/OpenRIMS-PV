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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/encountertypes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class EncounterTypesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<WorkPlan> _workPlanRepository;
        private readonly IRepositoryInt<EncounterTypeWorkPlan> _encounterTypeWorkPlanRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public EncounterTypesController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<WorkPlan> workPlanRepository,
            IRepositoryInt<EncounterTypeWorkPlan> encounterTypeWorkPlanRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _workPlanRepository = workPlanRepository ?? throw new ArgumentNullException(nameof(workPlanRepository));
            _encounterTypeWorkPlanRepository = encounterTypeWorkPlanRepository ?? throw new ArgumentNullException(nameof(encounterTypeWorkPlanRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all encounter types using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of EncounterTypeIdentifierDto</returns>
        [HttpGet(Name = "GetEncounterTypesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<EncounterTypeIdentifierDto>> GetEncounterTypesByIdentifier(
            [FromQuery] IdResourceParameters encounterTypeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<EncounterTypeIdentifierDto>
                (encounterTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedEncounterTypesWithLinks = GetEncounterTypes<EncounterTypeIdentifierDto>(encounterTypeResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<EncounterTypeIdentifierDto>(mappedEncounterTypesWithLinks.TotalCount, mappedEncounterTypesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, encounterTypeResourceParameters,
            //    mappedEncounterTypesWithLinks.HasNext, mappedEncounterTypesWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all encounter types using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of EncounterTypeDetailDto</returns>
        [HttpGet(Name = "GetEncounterTypesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<EncounterTypeDetailDto>> GetEncounterTypesByDetail(
            [FromQuery] IdResourceParameters encounterTypeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<EncounterTypeDetailDto>
                (encounterTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedEncounterTypesWithLinks = GetEncounterTypes<EncounterTypeDetailDto>(encounterTypeResourceParameters);

            // Apply custom mapping to dto
            mappedEncounterTypesWithLinks.ForEach(dto => CustomTypeMap(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<EncounterTypeDetailDto>(mappedEncounterTypesWithLinks.TotalCount, mappedEncounterTypesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, encounterTypeResourceParameters,
            //    mappedEncounterTypesWithLinks.HasNext, mappedEncounterTypesWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single encounterType using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the encounterType</param>
        /// <returns>An ActionResult of type EncounterTypeIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetEncounterTypeByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<EncounterTypeIdentifierDto>> GetEncounterTypeByIdentifier(long id)
        {
            var mappedEncounterType = await GetEncounterTypeAsync<EncounterTypeIdentifierDto>(id);
            if (mappedEncounterType == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForEncounterType<EncounterTypeIdentifierDto>(mappedEncounterType));
        }

        /// <summary>
        /// Get a single encounterType using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the encounterType</param>
        /// <returns>An ActionResult of type EncounterTypeDetailDto</returns>
        [HttpGet("{id}", Name = "GetEncounterTypeByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<EncounterTypeDetailDto>> GetEncounterTypeByDetail(long id)
        {
            var mappedEncounterType = await GetEncounterTypeAsync<EncounterTypeDetailDto>(id);
            if (mappedEncounterType == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForEncounterType<EncounterTypeDetailDto>(CustomTypeMap(mappedEncounterType)));
        }

        /// <summary>
        /// Create a new encounter type
        /// </summary>
        /// <param name="encounterTypeForUpdate">The encounter type payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateEncounterType")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateEncounterType(
            [FromBody] EncounterTypeForUpdateDto encounterTypeForUpdate)
        {
            if (encounterTypeForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(encounterTypeForUpdate.EncounterTypeName, @"[a-zA-Z ']").Count < encounterTypeForUpdate.EncounterTypeName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(encounterTypeForUpdate.Help))
            {
                if (Regex.Matches(encounterTypeForUpdate.Help, @"[a-zA-Z0-9. ']").Count < encounterTypeForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<EncounterType>().Queryable().
                Where(l => l.Description == encounterTypeForUpdate.EncounterTypeName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var workPlan = _workPlanRepository.Get(wp => wp.Description == encounterTypeForUpdate.WorkPlanName);
            if (workPlan == null)
            {
                ModelState.AddModelError("Message", "Unable to locate work plan");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newEncounterType = new EncounterType()
                {
                    Description = encounterTypeForUpdate.EncounterTypeName,
                    Help = encounterTypeForUpdate.Help
                };

                var newEncounterTypeWorkPlan = new EncounterTypeWorkPlan() 
                { 
                    CohortGroup = null, 
                    EncounterType = newEncounterType, 
                    WorkPlan = workPlan 
                };

                _encounterTypeRepository.Save(newEncounterType);
                _encounterTypeWorkPlanRepository.Save(newEncounterTypeWorkPlan);
                id = newEncounterType.Id;

                var mappedEncounterType = await GetEncounterTypeAsync<EncounterTypeIdentifierDto>(id);
                if (mappedEncounterType == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetEncounterTypeByIdentifier",
                    new
                    {
                        id = mappedEncounterType.Id
                    }, CreateLinksForEncounterType<EncounterTypeIdentifierDto>(mappedEncounterType));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing encounterType
        /// </summary>
        /// <param name="id">The unique id of the encounterType</param>
        /// <param name="encounterTypeForUpdate">The encounterType payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateEncounterType")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateEncounterType(long id,
            [FromBody] EncounterTypeForUpdateDto encounterTypeForUpdate)
        {
            if (encounterTypeForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(encounterTypeForUpdate.EncounterTypeName, @"[a-zA-Z ']").Count < encounterTypeForUpdate.EncounterTypeName.Length)
            {
                ModelState.AddModelError("Message", "EncounterType name contains invalid characters (Enter A-Z, a-z, space)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<EncounterType>().Queryable().
                Where(l => l.Description == encounterTypeForUpdate.EncounterTypeName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            var encounterTypeFromRepo = await _encounterTypeRepository.GetAsync(f => f.Id == id);
            if (encounterTypeFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                encounterTypeFromRepo.Description = encounterTypeForUpdate.EncounterTypeName;
                encounterTypeFromRepo.Help = encounterTypeForUpdate.Help;

                _encounterTypeRepository.Update(encounterTypeFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing encounterType
        /// </summary>
        /// <param name="id">The unique id of the encounterType</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteEncounterType")]
        public async Task<IActionResult> DeleteEncounterType(long id)
        {
            var encounterTypeFromRepo = await _encounterTypeRepository.GetAsync(f => f.Id == id);
            if (encounterTypeFromRepo == null)
            {
                return NotFound();
            }

            if (encounterTypeFromRepo.Encounters.Count > 0)
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
            }

            if (ModelState.IsValid)
            {
                ICollection<EncounterTypeWorkPlan> deleteItems = new Collection<EncounterTypeWorkPlan>();
                foreach (var item in encounterTypeFromRepo.EncounterTypeWorkPlans)
                {
                    deleteItems.Add(item);
                }
                foreach (var item in deleteItems)
                {
                    _encounterTypeWorkPlanRepository.Delete(item);
                }

                _encounterTypeRepository.Delete(encounterTypeFromRepo);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get encounterTypes from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="encounterTypeResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetEncounterTypes<T>(IdResourceParameters encounterTypeResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = encounterTypeResourceParameters.PageNumber,
                PageSize = encounterTypeResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<EncounterType>(encounterTypeResourceParameters.OrderBy, "asc");

            var pagedEncounterTypesFromRepo = _encounterTypeRepository.List(pagingInfo, null, orderby, new string[] { "EncounterTypeWorkPlans.WorkPlan" });
            if (pagedEncounterTypesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedEncounterTypes = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedEncounterTypesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedEncounterTypesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedEncounterTypes.TotalCount,
                    pageSize = mappedEncounterTypes.PageSize,
                    currentPage = mappedEncounterTypes.CurrentPage,
                    totalPages = mappedEncounterTypes.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedEncounterTypes.ForEach(dto => CreateLinksForEncounterType(dto));

                return mappedEncounterTypes;
            }

            return null;
        }

        /// <summary>
        /// Get single encounterType from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetEncounterTypeAsync<T>(long id) where T : class
        {
            var encounterTypeFromRepo = await _encounterTypeRepository.GetAsync(f => f.Id == id);

            if (encounterTypeFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedEncounterType = _mapper.Map<T>(encounterTypeFromRepo);

                return mappedEncounterType;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private EncounterTypeIdentifierDto CreateLinksForEncounterType<T>(T dto)
        {
            EncounterTypeIdentifierDto identifier = (EncounterTypeIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("EncounterType", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private EncounterTypeDetailDto CustomTypeMap(EncounterTypeDetailDto dto)
        {
            var encounterTypeFromRepo = _encounterTypeRepository.Get(p => p.Id == dto.Id);
            if (encounterTypeFromRepo == null)
            {
                return dto;
            }

            dto.WorkPlanName = string.Empty;
            if(encounterTypeFromRepo.EncounterTypeWorkPlans.Count > 0)
            {
                dto.WorkPlanName = encounterTypeFromRepo.EncounterTypeWorkPlans.First().WorkPlan.Description;
            }

            return dto;
        }

    }
}
