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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/careevents")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class CareEventsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<CareEvent> _careEventRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public CareEventsController(
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<CareEvent> careEventRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _careEventRepository = careEventRepository ?? throw new ArgumentNullException(nameof(careEventRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all care events using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CareEventIdentifierDto</returns>
        [HttpGet(Name = "GetCareEventsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<CareEventIdentifierDto>> GetCareEventsByIdentifier(
            [FromQuery] IdResourceParameters caseEventResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CareEventIdentifierDto>
                (caseEventResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedCareEventsWithLinks = GetCareEvents<CareEventIdentifierDto>(caseEventResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<CareEventIdentifierDto>(mappedCareEventsWithLinks.TotalCount, mappedCareEventsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, careEventResourceParameters,
            //    mappedCareEventsWithLinks.HasNext, mappedCareEventsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single care event using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the care event</param>
        /// <returns>An ActionResult of type CareEventIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetCareEventByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<CareEventIdentifierDto>> GetCareEventByIdentifier(long id)
        {
            var mappedCareEvent = await GetCareEventAsync<CareEventIdentifierDto>(id);
            if (mappedCareEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCareEvent<CareEventIdentifierDto>(mappedCareEvent));
        }

        /// <summary>
        /// Create a new care event
        /// </summary>
        /// <param name="careEventForUpdate">The care event payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateCareEvent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCareEvent(
            [FromBody] CareEventForUpdateDto careEventForUpdate)
        {
            if (careEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(careEventForUpdate.CareEventName, @"[a-zA-Z ']").Count < careEventForUpdate.CareEventName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<CareEvent>().Queryable().
                Where(l => l.Description == careEventForUpdate.CareEventName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newCareEvent = new CareEvent()
                {
                    Description = careEventForUpdate.CareEventName
                };

                _careEventRepository.Save(newCareEvent);
                id = newCareEvent.Id;
            }

            var mappedCareEvent = await GetCareEventAsync<CareEventIdentifierDto>(id);
            if (mappedCareEvent == null)
            {
                return StatusCode(500, "Unable to locate newly added item");
            }

            return CreatedAtAction("GetCareEventByIdentifier",
                new
                {
                    id = mappedCareEvent.Id
                }, CreateLinksForCareEvent<CareEventIdentifierDto>(mappedCareEvent));
        }

        /// <summary>
        /// Update an existing care event
        /// </summary>
        /// <param name="id">The unique id of the care event</param>
        /// <param name="careEventForUpdate">The care event payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateCareEvent")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCareEvent(long id,
            [FromBody] CareEventForUpdateDto careEventForUpdate)
        {
            if (careEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(careEventForUpdate.CareEventName, @"[a-zA-Z ']").Count < careEventForUpdate.CareEventName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<CareEvent>().Queryable().
                Where(l => l.Description == careEventForUpdate.CareEventName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            var careEventFromRepo = await _careEventRepository.GetAsync(f => f.Id == id);
            if (careEventFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                careEventFromRepo.Description = careEventForUpdate.CareEventName;

                _careEventRepository.Update(careEventFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing care event
        /// </summary>
        /// <param name="id">The unique id of the care event</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteCareEvent")]
        public async Task<IActionResult> DeleteCareEvent(long id)
        {
            var careEventFromRepo = await _careEventRepository.GetAsync(f => f.Id == id);
            if (careEventFromRepo == null)
            {
                return NotFound();
            }

            if (careEventFromRepo.WorkPlanCareEvents.Count > 0)
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                _careEventRepository.Delete(careEventFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get care events from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="careEventResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetCareEvents<T>(IdResourceParameters careEventResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = careEventResourceParameters.PageNumber,
                PageSize = careEventResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<CareEvent>(careEventResourceParameters.OrderBy, "asc");

            var pagedCareEventsFromRepo = _careEventRepository.List(pagingInfo, null, orderby, "");
            if (pagedCareEventsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCareEvents = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedCareEventsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCareEventsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedCareEvents.TotalCount,
                    pageSize = mappedCareEvents.PageSize,
                    currentPage = mappedCareEvents.CurrentPage,
                    totalPages = mappedCareEvents.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedCareEvents.ForEach(dto => CreateLinksForCareEvent(dto));

                return mappedCareEvents;
            }

            return null;
        }

        /// <summary>
        /// Get single care event from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetCareEventAsync<T>(long id) where T : class
        {
            var careEventFromRepo = await _careEventRepository.GetAsync(f => f.Id == id);

            if (careEventFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCareEvent = _mapper.Map<T>(careEventFromRepo);

                return mappedCareEvent;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CareEventIdentifierDto CreateLinksForCareEvent<T>(T dto)
        {
            CareEventIdentifierDto identifier = (CareEventIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CareEvent", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
