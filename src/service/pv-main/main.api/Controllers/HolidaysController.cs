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
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class HolidaysController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Holiday> _holidayRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public HolidaysController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Holiday> holidayRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _holidayRepository = holidayRepository ?? throw new ArgumentNullException(nameof(holidayRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all holidays using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of HolidayIdentifierDto</returns>
        [HttpGet("holidays", Name = "GetHolidaysByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<HolidayIdentifierDto>> GetHolidaysByIdentifier(
            [FromQuery] HolidayResourceParameters holidayResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<HolidayIdentifierDto>
                (holidayResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedHolidaysWithLinks = GetHolidays<HolidayIdentifierDto>(holidayResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<HolidayIdentifierDto>(mappedHolidaysWithLinks.TotalCount, mappedHolidaysWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, holidayResourceParameters,
            //    mappedHolidaysWithLinks.HasNext, mappedHolidaysWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single holiday using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type HolidayIdentifierDto</returns>
        [HttpGet("holidays/{id}", Name = "GetHolidayByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<HolidayIdentifierDto>> GetHolidayByIdentifier(long id)
        {
            var mappedHoliday = await GetHolidayAsync<HolidayIdentifierDto>(id);
            if (mappedHoliday == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForHoliday<HolidayIdentifierDto>(mappedHoliday));
        }

        /// <summary>
        /// Create a new holiday
        /// </summary>
        /// <param name="holidayForUpdate">The holiday payload</param>
        /// <returns></returns>
        [HttpPost("holidays", Name = "CreateHoliday")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateHoliday(
            [FromBody] HolidayForUpdateDto holidayForUpdate)
        {
            if (holidayForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<Holiday>().Queryable().
                Where(l => l.HolidayDate == holidayForUpdate.HolidayDate)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "A holiday has already been loaded for this date");
                return BadRequest(ModelState);
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newHoliday = new Holiday()
                {
                    HolidayDate = holidayForUpdate.HolidayDate,
                    Description = holidayForUpdate.Description
                };

                _holidayRepository.Save(newHoliday);
                id = newHoliday.Id;
            }

            var mappedHoliday = await GetHolidayAsync<HolidayIdentifierDto>(id);
            if (mappedHoliday == null)
            {
                return StatusCode(500, "Unable to locate newly added item");
            }

            return CreatedAtAction("GetHolidayByIdentifier",
                new
                {
                    id = mappedHoliday.Id
                }, CreateLinksForHoliday<HolidayIdentifierDto>(mappedHoliday));
        }

        /// <summary>
        /// Update an existing holiday
        /// </summary>
        /// <param name="id">The unique id of the holiday</param>
        /// <param name="holidayForUpdate">The holiday payload</param>
        /// <returns></returns>
        [HttpPut("holidays/{id}", Name = "UpdateHoliday")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateHoliday(long id,
            [FromBody] HolidayForUpdateDto holidayForUpdate)
        {
            if (holidayForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<Holiday>().Queryable().
                Where(l => l.HolidayDate == holidayForUpdate.HolidayDate && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "A holiday has already been loaded for this date");
                return BadRequest(ModelState);
            }

            var holidayFromRepo = await _holidayRepository.GetAsync(f => f.Id == id);
            if (holidayFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                holidayFromRepo.HolidayDate = holidayForUpdate.HolidayDate;
                holidayFromRepo.Description = holidayForUpdate.Description;

                _holidayRepository.Update(holidayFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing holiday
        /// </summary>
        /// <param name="id">The unique id of the holiday</param>
        /// <returns></returns>
        [HttpDelete("holidays/{id}", Name = "DeleteHoliday")]
        public async Task<IActionResult> DeleteHoliday(long id)
        {
            var holidayFromRepo = await _holidayRepository.GetAsync(f => f.Id == id);
            if (holidayFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _holidayRepository.Delete(holidayFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get holidays from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="holidayResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetHolidays<T>(HolidayResourceParameters holidayResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = holidayResourceParameters.PageNumber,
                PageSize = holidayResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Holiday>(holidayResourceParameters.OrderBy, "asc");

            var pagedHolidaysFromRepo = _holidayRepository.List(pagingInfo, null, orderby, "");
            if (pagedHolidaysFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedHolidays = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedHolidaysFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedHolidaysFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedHolidays.TotalCount,
                    pageSize = mappedHolidays.PageSize,
                    currentPage = mappedHolidays.CurrentPage,
                    totalPages = mappedHolidays.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedHolidays.ForEach(dto => CreateLinksForHoliday(dto));

                return mappedHolidays;
            }

            return null;
        }

        /// <summary>
        /// Get single lab test from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetHolidayAsync<T>(long id) where T : class
        {
            var holidayFromRepo = await _holidayRepository.GetAsync(f => f.Id == id);

            if (holidayFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedHoliday = _mapper.Map<T>(holidayFromRepo);

                return mappedHoliday;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private HolidayIdentifierDto CreateLinksForHoliday<T>(T dto)
        {
            HolidayIdentifierDto identifier = (HolidayIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Holiday", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
