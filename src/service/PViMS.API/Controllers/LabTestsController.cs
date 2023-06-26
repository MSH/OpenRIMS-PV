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
    [Route("api/labtests")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class LabTestsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IRepositoryInt<LabTest> _labTestRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public LabTestsController(IPropertyMappingService propertyMappingService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<LabTest> labTestRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _labTestRepository = labTestRepository ?? throw new ArgumentNullException(nameof(labTestRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all lab tests using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of LabTestIdentifierDto</returns>
        [HttpGet(Name = "GetLabTestsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<LabTestIdentifierDto>> GetLabTestsByIdentifier(
            [FromQuery] LabTestResourceParameters labTestResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<LabTestIdentifierDto, LabTest>
               (labTestResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedLabTestsWithLinks = GetLabTests<LabTestIdentifierDto>(labTestResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<LabTestIdentifierDto>(mappedLabTestsWithLinks.TotalCount, mappedLabTestsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labTestResourceParameters,
            //    mappedLabTestsWithLinks.HasNext, mappedLabTestsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single lab test using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type LabTestIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetLabTestByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LabTestIdentifierDto>> GetLabTestByIdentifier(long id)
        {
            var mappedLabTest = await GetLabTestAsync<LabTestIdentifierDto>(id);
            if (mappedLabTest == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForLabTest<LabTestIdentifierDto>(mappedLabTest));
        }

        /// <summary>
        /// Create a new lab test
        /// </summary>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateLabTest")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateLabTest(
            [FromBody] LabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(labTestForUpdate.LabTestName, @"[a-zA-Z0-9 ]").Count < labTestForUpdate.LabTestName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<LabTest>().Queryable().
                Where(l => l.Description == labTestForUpdate.LabTestName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newLabTest = new LabTest()
                {
                    Description = labTestForUpdate.LabTestName,
                    Active = true
                };

                _labTestRepository.Save(newLabTest);
                id = newLabTest.Id;

                var mappedLabTest = await GetLabTestAsync<LabTestIdentifierDto>(id);
                if (mappedLabTest == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetLabTestByIdentifier",
                    new
                    {
                        id = mappedLabTest.Id
                    }, CreateLinksForLabTest<LabTestIdentifierDto>(mappedLabTest));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing lab test
        /// </summary>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateLabTest")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateLabTest(long id,
            [FromBody] LabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(labTestForUpdate.LabTestName, @"[a-zA-Z0-9 ]").Count < labTestForUpdate.LabTestName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<LabTest>().Queryable().
                Where(l => l.Description == labTestForUpdate.LabTestName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var labTestFromRepo = await _labTestRepository.GetAsync(f => f.Id == id);
            if (labTestFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                labTestFromRepo.Description = labTestForUpdate.LabTestName;
                labTestFromRepo.Active = (labTestForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

                _labTestRepository.Update(labTestFromRepo);
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
        [HttpDelete("{id}", Name = "DeleteLabTest")]
        public async Task<IActionResult> DeleteLabTest(long id)
        {
            var labTestFromRepo = await _labTestRepository.GetAsync(f => f.Id == id);
            if (labTestFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _labTestRepository.Delete(labTestFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get lab tests from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="labTestResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetLabTests<T>(LabTestResourceParameters labTestResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = labTestResourceParameters.PageNumber,
                PageSize = labTestResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<LabTest>(labTestResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<LabTest>(true);

            if (!String.IsNullOrWhiteSpace(labTestResourceParameters.SearchTerm))
            {
                predicate = predicate.And(f => f.Description.Contains(labTestResourceParameters.SearchTerm.Trim()));
            }
            if (labTestResourceParameters.Active != Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (labTestResourceParameters.Active == Models.ValueTypes.YesNoBothValueType.Yes));
            }

            var pagedLabTestsFromRepo = _labTestRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedLabTestsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabTests = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedLabTestsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedLabTestsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedLabTests.TotalCount,
                    pageSize = mappedLabTests.PageSize,
                    currentPage = mappedLabTests.CurrentPage,
                    totalPages = mappedLabTests.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedLabTests.ForEach(dto => CreateLinksForLabTest(dto));

                return mappedLabTests;
            }

            return null;
        }

        /// <summary>
        /// Get single lab test from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetLabTestAsync<T>(long id) where T : class
        {
            var labTestFromRepo = await _labTestRepository.GetAsync(f => f.Id == id);

            if (labTestFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabTest = _mapper.Map<T>(labTestFromRepo);

                return mappedLabTest;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private LabTestIdentifierDto CreateLinksForLabTest<T>(T dto)
        {
            LabTestIdentifierDto identifier = (LabTestIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("LabTest", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
