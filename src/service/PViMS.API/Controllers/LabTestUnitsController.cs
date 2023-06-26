using AutoMapper;
using LinqKit;
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/labtestunits")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class LabTestUnitsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<LabTestUnit> _labTestUnitRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public LabTestUnitsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<LabTestUnit> labTestUnitRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _labTestUnitRepository = labTestUnitRepository ?? throw new ArgumentNullException(nameof(labTestUnitRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all lab test units using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of LabTestUnitIdentifierDto</returns>
        [HttpGet(Name = "GetLabTestUnitsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<LabTestUnitIdentifierDto>> GetLabTestUnitsByIdentifier(
            [FromQuery] LabTestUnitResourceParameters labTestUnitResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<LabTestUnitIdentifierDto, LabTestUnit>
               (labTestUnitResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedLabTestUnitsWithLinks = GetLabTestUnits<LabTestUnitIdentifierDto>(labTestUnitResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<LabTestUnitIdentifierDto>(mappedLabTestUnitsWithLinks.TotalCount, mappedLabTestUnitsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labTestUnitResourceParameters,
            //    mappedLabTestUnitsWithLinks.HasNext, mappedLabTestUnitsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single lab test using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type LabTestUnitIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetLabTestUnitByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LabTestUnitIdentifierDto>> GetLabTestUnitByIdentifier(long id)
        {
            var mappedLabTestUnit = await GetLabTestUnitAsync<LabTestUnitIdentifierDto>(id);
            if (mappedLabTestUnit == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForLabTestUnit<LabTestUnitIdentifierDto>(mappedLabTestUnit));
        }

        /// <summary>
        /// Create a new lab test
        /// </summary>
        /// <param name="labTestUnitForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateLabTestUnit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateLabTestUnit(
            [FromBody] LabTestUnitForUpdateDto labTestUnitForUpdate)
        {
            if (labTestUnitForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(labTestUnitForUpdate.LabTestUnitName, @"[a-zA-Z0-9 ]").Count < labTestUnitForUpdate.LabTestUnitName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<LabTestUnit>().Queryable().
                Where(l => l.Description == labTestUnitForUpdate.LabTestUnitName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newLabTestUnit = new LabTestUnit()
                {
                    Description = labTestUnitForUpdate.LabTestUnitName
                };

                _labTestUnitRepository.Save(newLabTestUnit);
                id = newLabTestUnit.Id;

                var mappedLabTestUnit = await GetLabTestUnitAsync<LabTestUnitIdentifierDto>(id);
                if (mappedLabTestUnit == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetLabTestUnitByIdentifier",
                    new
                    {
                        id = mappedLabTestUnit.Id
                    }, CreateLinksForLabTestUnit<LabTestUnitIdentifierDto>(mappedLabTestUnit));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing lab test
        /// </summary>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestUnitForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateLabTestUnit")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateLabTestUnit(long id,
            [FromBody] LabTestUnitForUpdateDto labTestUnitForUpdate)
        {
            if (labTestUnitForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(labTestUnitForUpdate.LabTestUnitName, @"[a-zA-Z0-9 ]").Count < labTestUnitForUpdate.LabTestUnitName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<LabTestUnit>().Queryable().
                Where(l => l.Description == labTestUnitForUpdate.LabTestUnitName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var labTestUnitFromRepo = await _labTestUnitRepository.GetAsync(f => f.Id == id);
            if (labTestUnitFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                labTestUnitFromRepo.Description = labTestUnitForUpdate.LabTestUnitName;

                _labTestUnitRepository.Update(labTestUnitFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing lab test
        /// </summary>
        /// <param name="id">The unique id of the lab test</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteLabTestUnit")]
        public async Task<IActionResult> DeleteLabTestUnit(long id)
        {
            var labTestUnitFromRepo = await _labTestUnitRepository.GetAsync(f => f.Id == id);
            if (labTestUnitFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _labTestUnitRepository.Delete(labTestUnitFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get lab tests from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="labTestUnitResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetLabTestUnits<T>(LabTestUnitResourceParameters labTestUnitResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = labTestUnitResourceParameters.PageNumber,
                PageSize = labTestUnitResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<LabTestUnit>(labTestUnitResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<LabTestUnit>(true);

            if (!String.IsNullOrWhiteSpace(labTestUnitResourceParameters.SearchTerm))
            {
                predicate = predicate.And(f => f.Description.Contains(labTestUnitResourceParameters.SearchTerm.Trim()));
            }

            var pagedLabTestUnitsFromRepo = _labTestUnitRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedLabTestUnitsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabTestUnits = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedLabTestUnitsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedLabTestUnitsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedLabTestUnits.TotalCount,
                    pageSize = mappedLabTestUnits.PageSize,
                    currentPage = mappedLabTestUnits.CurrentPage,
                    totalPages = mappedLabTestUnits.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedLabTestUnits.ForEach(dto => CreateLinksForLabTestUnit(dto));

                return mappedLabTestUnits;
            }

            return null;
        }

        /// <summary>
        /// Get single lab test from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetLabTestUnitAsync<T>(long id) where T : class
        {
            var labTestUnitFromRepo = await _labTestUnitRepository.GetAsync(f => f.Id == id);

            if (labTestUnitFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabTestUnit = _mapper.Map<T>(labTestUnitFromRepo);

                return mappedLabTestUnit;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private LabTestUnitIdentifierDto CreateLinksForLabTestUnit<T>(T dto)
        {
            LabTestUnitIdentifierDto identifier = (LabTestUnitIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("LabTestUnit", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
