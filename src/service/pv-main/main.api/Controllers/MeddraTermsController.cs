using AutoMapper;
using Ionic.Zip;
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
using OpenRIMS.PV.Main.Core.Services;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/meddraterms")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class MeddraTermsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<TerminologyMedDra> _termsRepository;
        private readonly IMedDraService _medDraService;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public MeddraTermsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMedDraService medDraService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<TerminologyMedDra> termsRepository)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _medDraService = medDraService ?? throw new ArgumentNullException(nameof(medDraService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _termsRepository = termsRepository ?? throw new ArgumentNullException(nameof(termsRepository));
        }

        /// <summary>
        /// Get a single meddra term using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meddra term</param>
        /// <returns>An ActionResult of type MeddraTermIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetMeddraTermByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MeddraTermIdentifierDto>> GetMeddraTermByIdentifier(long id)
        {
            var mappedMeddraTerm = await GetMeddraTermAsync<MeddraTermIdentifierDto>(id);
            if (mappedMeddraTerm == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMeddraTerm<MeddraTermIdentifierDto>(mappedMeddraTerm));
        }

        /// <summary>
        /// Get a single meddra term using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meddra term</param>
        /// <returns>An ActionResult of type MeddraTermDetailDto</returns>
        [HttpGet("{id}", Name = "GetMeddraTermByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MeddraTermDetailDto>> GetMeddraTermByDetail(long id)
        {
            var mappedMeddraTerm = await GetMeddraTermAsync<MeddraTermDetailDto>(id);
            if (mappedMeddraTerm == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMeddraTerm<MeddraTermDetailDto>(mappedMeddraTerm));
        }

        /// <summary>
        /// Get all meddra terms using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MeddraTermIdentifierDto</returns>
        [HttpGet(Name = "GetMeddraTermsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MeddraTermIdentifierDto>> GetMeddraTermsByIdentifier(
            [FromQuery] MeddraTermResourceParameters meddraTermResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MeddraTermIdentifierDto>
                (meddraTermResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedTermsWithLinks = GetMeddraTerms<MeddraTermIdentifierDto>(meddraTermResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MeddraTermIdentifierDto>(mappedTermsWithLinks.TotalCount, mappedTermsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labTestResourceParameters,
            //    mappedLabTestsWithLinks.HasNext, mappedLabTestsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all common meddra terms using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MeddraTermIdentifierDto</returns>
        [HttpGet(Name = "GetCommonMeddraTerms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.commonmeddra.v1+json", "application/vnd.main.commonmeddra.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.commonmeddra.v1+json", "application/vnd.main.commonmeddra.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<MeddraTermIdentifierDto>> GetCommonMeddraTerms(
            [FromQuery] MeddraTermResourceParameters meddraTermResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MeddraTermIdentifierDto>
                (meddraTermResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedTermsWithLinks = GetCommonMeddraTerms<MeddraTermIdentifierDto>(meddraTermResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MeddraTermIdentifierDto>(mappedTermsWithLinks.TotalCount, mappedTermsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labTestResourceParameters,
            //    mappedLabTestsWithLinks.HasNext, mappedLabTestsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all meddra terms using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MeddraTermDetailDto</returns>
        [HttpGet(Name = "GetMeddraTermsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<MeddraTermDetailDto>> GetMeddraTermsByDetail(
            [FromQuery] MeddraTermResourceParameters meddraTermResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MeddraTermDetailDto>
                (meddraTermResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedTermsWithLinks = GetMeddraTerms<MeddraTermDetailDto>(meddraTermResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MeddraTermDetailDto>(mappedTermsWithLinks.TotalCount, mappedTermsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labTestResourceParameters,
            //    mappedLabTestsWithLinks.HasNext, mappedLabTestsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Import a new meddra file
        /// </summary>
        /// <param name="meddraFileForImportDto">The attachment payload</param>
        /// <returns></returns>
        [HttpPost(Name = "Import")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Import([FromForm] MeddraFileForImportDto meddraFileForImportDto)
        {
            if (meddraFileForImportDto == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new import");
            }

            if (meddraFileForImportDto.Source.Length > 0)
            {
                if(Path.GetExtension(meddraFileForImportDto.Source.FileName).Replace(".", "").ToLower() != "zip")
                {
                    ModelState.AddModelError("Message", "File is not of type ZIP");
                }
                if (Path.GetFileNameWithoutExtension(meddraFileForImportDto.Source.FileName).ToLower() != "medascii")
                {
                    ModelState.AddModelError("Message", "File name incorrect");
                }

                if (ModelState.IsValid)
                {
                    var generatedDate = DateTime.Now.ToString("med_yyyyMMddhhmmss");

                    // Store zip file in temp folder for extraction
                    var fileNameAndPath = $"{Path.GetTempPath()}{meddraFileForImportDto.Source.FileName}";
                    using (var fileStream = new FileStream(fileNameAndPath, FileMode.Create))
                    {
                        await meddraFileForImportDto.Source.CopyToAsync(fileStream);
                    }

                    var subDirectory = $"{Path.GetTempPath()}\\{generatedDate}";
                    // create a sub directory for zip decompression
                    Directory.CreateDirectory(subDirectory);

                    // uncompress into sub-directory
                    using (var zip = new ZipFile(fileNameAndPath))
                    {
                        zip.ExtractAll($"{Path.GetTempPath()}\\{generatedDate}");
                    }

                    var response = _medDraService.ValidateSourceData(meddraFileForImportDto.Source.FileName, subDirectory);
                    response += _medDraService.ImportSourceData(meddraFileForImportDto.Source.FileName, subDirectory);

                    return Ok();
                }

                return BadRequest(ModelState);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get single meddra term from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetMeddraTermAsync<T>(long id) where T : class
        {
            var meddraTermFromRepo = await _termsRepository.GetAsync(t => t.Id == id, new string[] { 
                "Children",
                "Parent"
            });

            if (meddraTermFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMeddraTerm = _mapper.Map<T>(meddraTermFromRepo);

                return mappedMeddraTerm;
            }

            return null;
        }

        /// <summary>
        /// Get meddra terms from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="meddraTermResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMeddraTerms<T>(MeddraTermResourceParameters meddraTermResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = meddraTermResourceParameters.PageNumber,
                PageSize = meddraTermResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<TerminologyMedDra>(meddraTermResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<TerminologyMedDra>(true);
            if(!String.IsNullOrWhiteSpace(meddraTermResourceParameters.TermType))
            {
                predicate = predicate.And(mt => mt.MedDraTermType == meddraTermResourceParameters.TermType);
            }
            if (!String.IsNullOrWhiteSpace(meddraTermResourceParameters.ParentSearchTerm))
            {
                predicate = predicate.And(mt => mt.Parent.MedDraTerm.Contains(meddraTermResourceParameters.ParentSearchTerm));
            }
            if (!String.IsNullOrWhiteSpace(meddraTermResourceParameters.SearchTerm))
            {
                predicate = predicate.And(mt => mt.MedDraTerm.Contains(meddraTermResourceParameters.SearchTerm));
            }
            if (!String.IsNullOrWhiteSpace(meddraTermResourceParameters.SearchCode))
            {
                predicate = predicate.And(mt => mt.MedDraCode.Contains(meddraTermResourceParameters.SearchCode));
            }

            var pagedTermsFromRepo = _termsRepository.List(pagingInfo, predicate, orderby, new string[] {
                "Children",
                "Parent"
            });
            if (pagedTermsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedTerms = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedTermsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedTermsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedTerms.TotalCount,
                    pageSize = mappedTerms.PageSize,
                    currentPage = mappedTerms.CurrentPage,
                    totalPages = mappedTerms.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedLabTests.ForEach(dto => CreateLinksForFacility(dto));

                return mappedTerms;
            }

            return null;
        }

        /// <summary>
        /// Get common meddra terms from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="meddraTermResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetCommonMeddraTerms<T>(MeddraTermResourceParameters meddraTermResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = meddraTermResourceParameters.PageNumber,
                PageSize = meddraTermResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<TerminologyMedDra>(meddraTermResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<TerminologyMedDra>(true);
            predicate = predicate.And(mt => mt.Common == true);

            var pagedTermsFromRepo = _termsRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedTermsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedTerms = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedTermsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedTermsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedTerms.TotalCount,
                    pageSize = mappedTerms.PageSize,
                    currentPage = mappedTerms.CurrentPage,
                    totalPages = mappedTerms.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedLabTests.ForEach(dto => CreateLinksForFacility(dto));

                return mappedTerms;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MeddraTermIdentifierDto CreateLinksForMeddraTerm<T>(T dto)
        {
            MeddraTermIdentifierDto identifier = (MeddraTermIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MeddraTerm", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
