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
using System.Linq;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class LabResultsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IRepositoryInt<LabResult> _labResultRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public LabResultsController(IPropertyMappingService propertyMappingService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<LabResult> labResultRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _labResultRepository = labResultRepository ?? throw new ArgumentNullException(nameof(labResultRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all lab results using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of LabResultIdentifierDto</returns>
        [HttpGet("labresults", Name = "GetLabResultsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<LabResultIdentifierDto>> GetLabResultsByIdentifier(
            [FromQuery] LabResultResourceParameters labResultResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<LabResultIdentifierDto, LabResult>
               (labResultResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedLabResultsWithLinks = GetLabResults<LabResultIdentifierDto>(labResultResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<LabResultIdentifierDto>(mappedLabResultsWithLinks.TotalCount, mappedLabResultsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labResultResourceParameters,
            //    mappedLabResultsWithLinks.HasNext, mappedLabResultsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single lab result using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab result</param>
        /// <returns>An ActionResult of type LabResultIdentifierDto</returns>
        [HttpGet("labresults/{id}", Name = "GetLabResultByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<LabResultIdentifierDto>> GetLabResultByIdentifier(long id)
        {
            var mappedLabResult = await GetLabResultAsync<LabResultIdentifierDto>(id);
            if (mappedLabResult == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForLabResult<LabResultIdentifierDto>(mappedLabResult));
        }

        /// <summary>
        /// Create a new lab result
        /// </summary>
        /// <param name="labResultForUpdate">The lab result payload</param>
        /// <returns></returns>
        [HttpPost("labresults", Name = "CreateLabResult")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateLabResult(
            [FromBody] LabResultForUpdateDto labResultForUpdate)
        {
            if (labResultForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<LabResult>().Queryable().
                Where(l => l.Description == labResultForUpdate.LabResultName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newLabResult = new LabResult()
                {
                    Description = labResultForUpdate.LabResultName,
                    Active = true
                };

                _labResultRepository.Save(newLabResult);
                id = newLabResult.Id;
            }

            var mappedLabResult = await GetLabResultAsync<LabResultIdentifierDto>(id);
            if (mappedLabResult == null)
            {
                return StatusCode(500, "Unable to locate newly added item");
            }

            return CreatedAtAction("GetLabResultByIdentifier",
                new
                {
                    id = mappedLabResult.Id
                }, CreateLinksForLabResult<LabResultIdentifierDto>(mappedLabResult));
        }

        /// <summary>
        /// Update an existing lab result
        /// </summary>
        /// <param name="id">The unique id of the lab result</param>
        /// <param name="labResultForUpdate">The lab result payload</param>
        /// <returns></returns>
        [HttpPut("labresults/{id}", Name = "UpdateLabResult")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateLabResult(long id,
            [FromBody] LabResultForUpdateDto labResultForUpdate)
        {
            if (labResultForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<LabResult>().Queryable().
                Where(l => l.Description == labResultForUpdate.LabResultName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            var labResultFromRepo = await _labResultRepository.GetAsync(f => f.Id == id);
            if (labResultFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                labResultFromRepo.Description = labResultForUpdate.LabResultName;
                labResultFromRepo.Active = (labResultForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

                _labResultRepository.Update(labResultFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing lab result
        /// </summary>
        /// <param name="id">The unique id of the lab result</param>
        /// <returns></returns>
        [HttpDelete("labresults/{id}", Name = "DeleteLabResult")]
        public async Task<IActionResult> DeleteLabResult(long id)
        {
            var labResultFromRepo = await _labResultRepository.GetAsync(f => f.Id == id);
            if (labResultFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _labResultRepository.Delete(labResultFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get lab results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="labResultResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetLabResults<T>(LabResultResourceParameters labResultResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = labResultResourceParameters.PageNumber,
                PageSize = labResultResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<LabResult>(labResultResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<LabResult>(true);

            if (!String.IsNullOrWhiteSpace(labResultResourceParameters.SearchTerm))
            {
                predicate = predicate.And(f => f.Description.Contains(labResultResourceParameters.SearchTerm.Trim()));
            }
            if (labResultResourceParameters.Active != Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (labResultResourceParameters.Active == Models.ValueTypes.YesNoBothValueType.Yes));
            }

            var pagedLabResultsFromRepo = _labResultRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedLabResultsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedLabResultsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedLabResultsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedLabResults.TotalCount,
                    pageSize = mappedLabResults.PageSize,
                    currentPage = mappedLabResults.CurrentPage,
                    totalPages = mappedLabResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedLabResults.ForEach(dto => CreateLinksForLabResult(dto));

                return mappedLabResults;
            }

            return null;
        }

        /// <summary>
        /// Get single lab result from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetLabResultAsync<T>(long id) where T : class
        {
            var labResultFromRepo = await _labResultRepository.GetAsync(f => f.Id == id);

            if (labResultFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabResult = _mapper.Map<T>(labResultFromRepo);

                return mappedLabResult;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private LabResultIdentifierDto CreateLinksForLabResult<T>(T dto)
        {
            LabResultIdentifierDto identifier = (LabResultIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("LabResult", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
