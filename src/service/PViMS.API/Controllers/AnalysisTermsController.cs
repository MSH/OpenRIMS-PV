using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;
using PVIMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;


namespace PVIMS.API.Controllers
{
    /// <summary>
    /// Analysis by meddra term
    /// </summary>
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class AnalysisTermsController : ControllerBase
    {
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _termsRepository;
        private readonly IRepositoryInt<RiskFactor> _riskFactorRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly PVIMSDbContext _context;

        public AnalysisTermsController(IRepositoryInt<WorkFlow> workFlowRepository,
                IRepositoryInt<TerminologyMedDra> termsRepository,
                IRepositoryInt<RiskFactor> riskFactorRepository,
                IMapper mapper,
                IUnitOfWorkInt unitOfWork,
                ILinkGeneratorService linkGeneratorService,
                PVIMSDbContext dbContext)
        {
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _termsRepository = termsRepository ?? throw new ArgumentNullException(nameof(termsRepository));
            _riskFactorRepository = riskFactorRepository ?? throw new ArgumentNullException(nameof(riskFactorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Get a list of adverse drug reactions relevant over the analysis period
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="analyserTermSetResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AnalyserTermDto</returns>
        [HttpGet("workflow/{workFlowGuid}/analysisterms", Name = "GetAnalyserTermsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<AnalyserTermIdentifierDto>> GetAnalyserTermsByIdentifier(Guid workFlowGuid,
                        [FromQuery] AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            if (analyserTermSetResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetAnalyserTermSets<AnalyserTermIdentifierDto>(analyserTermSetResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AnalyserTermIdentifierDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForAnalyserTermSets(wrapper, workFlowGuid, analyserTermSetResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single meddra term for analysis using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the meddra term</param>
        /// <param name="analyserTermSetResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AnalyserTermDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/analysisterms/{id}", Name = "GetAnalyserTermByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<AnalyserTermDetailDto>> GetAnalyserTermByDetail(Guid workFlowGuid, int id, 
            [FromQuery] AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            var workFlowFromRepo = _workFlowRepository.Get(r => r.WorkFlowGuid == workFlowGuid);
            if (workFlowFromRepo == null)
            {
                return BadRequest();
            }

            var mappedMeddraTerm = await GetMeddraTermAsync<AnalyserTermDetailDto>(id);
            if (mappedMeddraTerm == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMeddraTerm<AnalyserTermDetailDto>(CustomResultMap(mappedMeddraTerm, analyserTermSetResourceParameters)));
        }

        /// <summary>
        /// Get a list of patients that contributed to the analysis
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the meddra term</param>
        /// <param name="analyserTermSetResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AnalyserPatientDto</returns>
        [HttpGet("workflow/{workFlowGuid}/analysisterms/{id}/patients", Name = "GetAnalyserTermPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.analyserpatientset.v1+json", "application/vnd.pvims.analyserpatientset.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.analyserpatientset.v1+json", "application/vnd.pvims.analyserpatientset.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<AnalyserPatientDto>> GetAnalyserTermPatients(Guid workFlowGuid, int id, 
                        [FromQuery] AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            var workFlowFromRepo = _workFlowRepository.Get(r => r.WorkFlowGuid == workFlowGuid);
            if (workFlowFromRepo == null)
            {
                return BadRequest();
            }

            if (analyserTermSetResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetAnalyserPatientSets<AnalyserPatientDto>(id, analyserTermSetResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AnalyserPatientDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForAnalyserPatients(wrapper, workFlowGuid, id, analyserTermSetResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get single meddra term from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetMeddraTermAsync<T>(long id) where T : class
        {
            var meddraTermFromRepo = await _termsRepository.GetAsync(t => t.Id == id);

            if (meddraTermFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMeddraTerm = _mapper.Map<T>(meddraTermFromRepo);

                return mappedMeddraTerm;
            }

            return null;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="analyserTermSetResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetAnalyserTermSets<T>(AnalyserTermSetResourceParameters analyserTermSetResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = analyserTermSetResourceParameters.PageNumber,
                PageSize = analyserTermSetResourceParameters.PageSize
            };

            // prepare risk factor xml
            var riskFactorXml = "";
            var includeRiskFactor = "False";

            XmlDocument xmlDoc = new XmlDocument();
            if (analyserTermSetResourceParameters.RiskFactorOptionNames?.Count > 0)
            {
                XmlNode rootNode = xmlDoc.CreateElement("Factors", "");

                foreach (var display in analyserTermSetResourceParameters.RiskFactorOptionNames)
                {
                    var riskFactor = _riskFactorRepository.Get(r => r.Options.Any(ro => ro.Display == display));
                    if(riskFactor != null)
                    {
                        XmlNode factorNode = xmlDoc.CreateElement("Factor", "");

                        XmlNode factorChildNode = xmlDoc.CreateElement("Name", "");
                        factorChildNode.InnerText = riskFactor.FactorName;
                        factorNode.AppendChild(factorChildNode);

                        factorChildNode = xmlDoc.CreateElement("Option", "");
                        factorChildNode.InnerText = riskFactor.Options.Single(ro => ro.Display == display)?.Display;
                        factorNode.AppendChild(factorChildNode);

                        rootNode.AppendChild(factorNode);
                        includeRiskFactor = "True";
                    }
                }

                xmlDoc.AppendChild(rootNode);

                riskFactorXml = xmlDoc.InnerXml;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ConditionId", analyserTermSetResourceParameters.ConditionId.ToString()));
            parameters.Add(new SqlParameter("@CohortId", analyserTermSetResourceParameters.CohortGroupId.ToString()));
            parameters.Add(new SqlParameter("@StartDate", analyserTermSetResourceParameters.SearchFrom.ToString()));
            parameters.Add(new SqlParameter("@FinishDate", analyserTermSetResourceParameters.SearchTo.ToString()));
            parameters.Add(new SqlParameter("@TermID", "0"));
            parameters.Add(new SqlParameter("@IncludeRiskFactor", includeRiskFactor));
            parameters.Add(new SqlParameter("@RateByCount", "True"));
            parameters.Add(new SqlParameter("@DebugPatientList", "False"));
            parameters.Add(new SqlParameter("@RiskFactorXml", riskFactorXml));
            parameters.Add(new SqlParameter("@DebugMode", "False"));

            var resultsFromService = _context.ContingencyAnalysisLists
                .FromSqlRaw($"EXECUTE spGenerateAnalysis @ConditionId, @CohortId, @StartDate, @FinishDate, @TermID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @RiskFactorXml, @DebugMode",
                        parameters.ToArray()).ToList();

            if (resultsFromService != null)
            {
                // Map results to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.Count);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="termId">The unique id of the MedDRA term that we are getting results for</param>
        /// <param name="analyserTermSetResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private ICollection<T> GetAnalyserResultSets<T>(int termId, AnalyserTermSetResourceParameters analyserTermSetResourceParameters) where T : class
        {
            // prepare risk factor xml
            var riskFactorXml = "";
            var includeRiskFactor = "False";

            XmlDocument xmlDoc = new XmlDocument();
            if (analyserTermSetResourceParameters.RiskFactorOptionNames?.Count > 0)
            {
                XmlNode rootNode = xmlDoc.CreateElement("Factors", "");

                foreach (var display in analyserTermSetResourceParameters.RiskFactorOptionNames)
                {
                    var riskFactor = _riskFactorRepository.Get(r => r.Options.Any(ro => ro.Display == display));
                    if (riskFactor != null)
                    {
                        XmlNode factorNode = xmlDoc.CreateElement("Factor", "");

                        XmlNode factorChildNode = xmlDoc.CreateElement("Name", "");
                        factorChildNode.InnerText = riskFactor.FactorName;
                        factorNode.AppendChild(factorChildNode);

                        factorChildNode = xmlDoc.CreateElement("Option", "");
                        factorChildNode.InnerText = riskFactor.Options.Single(ro => ro.Display == display)?.Display;
                        factorNode.AppendChild(factorChildNode);

                        rootNode.AppendChild(factorNode);
                        includeRiskFactor = "True";
                    }
                }

                xmlDoc.AppendChild(rootNode);

                riskFactorXml = xmlDoc.InnerXml;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ConditionId", analyserTermSetResourceParameters.ConditionId.ToString()));
            parameters.Add(new SqlParameter("@CohortId", analyserTermSetResourceParameters.CohortGroupId.ToString()));
            parameters.Add(new SqlParameter("@StartDate", analyserTermSetResourceParameters.SearchFrom.ToString()));
            parameters.Add(new SqlParameter("@FinishDate", analyserTermSetResourceParameters.SearchTo.ToString()));
            parameters.Add(new SqlParameter("@TermID", termId.ToString()));
            parameters.Add(new SqlParameter("@IncludeRiskFactor", includeRiskFactor));
            parameters.Add(new SqlParameter("@RateByCount", "True"));
            parameters.Add(new SqlParameter("@DebugPatientList", "False"));
            parameters.Add(new SqlParameter("@RiskFactorXml", riskFactorXml));
            parameters.Add(new SqlParameter("@DebugMode", "False"));

            var resultsFromService = _context.ContingencyAnalysisItems
                .FromSqlRaw($"Exec spGenerateAnalysis @ConditionId, @CohortId, @StartDate, @FinishDate, @TermID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @RiskFactorXml, @DebugMode",
                        parameters.ToArray()).ToList();

            if (resultsFromService != null)
            {
                // Map results to Dto
                return _mapper.Map<ICollection<T>>(resultsFromService);
            }

            return null;
        }

        /// <summary>
        /// Get patient results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="termId">The unique id of the MedDRA term that we are getting results for</param>
        /// <param name="analyserTermSetResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetAnalyserPatientSets<T>(int termId, AnalyserTermSetResourceParameters analyserTermSetResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = analyserTermSetResourceParameters.PageNumber,
                PageSize = analyserTermSetResourceParameters.PageSize
            };

            // prepare risk factor xml
            var riskFactorXml = "";
            var includeRiskFactor = "False";

            XmlDocument xmlDoc = new XmlDocument();
            if (analyserTermSetResourceParameters.RiskFactorOptionNames?.Count > 0)
            {
                XmlNode rootNode = xmlDoc.CreateElement("Factors", "");

                foreach (var display in analyserTermSetResourceParameters.RiskFactorOptionNames)
                {
                    var riskFactor = _riskFactorRepository.Get(r => r.Options.Any(ro => ro.Display == display));
                    if (riskFactor != null)
                    {
                        XmlNode factorNode = xmlDoc.CreateElement("Factor", "");

                        XmlNode factorChildNode = xmlDoc.CreateElement("Name", "");
                        factorChildNode.InnerText = riskFactor.FactorName;
                        factorNode.AppendChild(factorChildNode);

                        factorChildNode = xmlDoc.CreateElement("Option", "");
                        factorChildNode.InnerText = riskFactor.Options.Single(ro => ro.Display == display)?.Display;
                        factorNode.AppendChild(factorChildNode);

                        rootNode.AppendChild(factorNode);
                        includeRiskFactor = "True";
                    }
                }

                xmlDoc.AppendChild(rootNode);

                riskFactorXml = xmlDoc.InnerXml;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ConditionId", analyserTermSetResourceParameters.ConditionId.ToString()));
            parameters.Add(new SqlParameter("@CohortId", analyserTermSetResourceParameters.CohortGroupId.ToString()));
            parameters.Add(new SqlParameter("@StartDate", analyserTermSetResourceParameters.SearchFrom.ToString()));
            parameters.Add(new SqlParameter("@FinishDate", analyserTermSetResourceParameters.SearchTo.ToString()));
            parameters.Add(new SqlParameter("@TermID", termId.ToString()));
            parameters.Add(new SqlParameter("@IncludeRiskFactor", includeRiskFactor));
            parameters.Add(new SqlParameter("@RateByCount", "True"));
            parameters.Add(new SqlParameter("@DebugPatientList", "True"));
            parameters.Add(new SqlParameter("@RiskFactorXml", riskFactorXml));
            parameters.Add(new SqlParameter("@DebugMode", "False"));

            var resultsFromService = PagedCollection<ContingencyAnalysisPatient>.Create(
                _context.ContingencyAnalysisPatients
                    .FromSqlRaw($"Exec spGenerateAnalysis @ConditionId, @CohortId, @StartDate, @FinishDate, @TermID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @RiskFactorXml, @DebugMode",
                            parameters.ToArray()).ToList(), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map results to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="workFlowGuid">The unique identifier of the work flow that we are generating analysis for</param>
        /// <param name="analyserTermSetResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForAnalyserTermSets(
            LinkedResourceBaseDto wrapper,
            Guid workFlowGuid,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAnalyserTermSetsResourceUri(workFlowGuid, ResourceUriType.Current, analyserTermSetResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAnalyserTermSetsResourceUri(workFlowGuid, ResourceUriType.NextPage, analyserTermSetResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAnalyserTermSetsResourceUri(workFlowGuid, ResourceUriType.PreviousPage, analyserTermSetResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="workFlowGuid">The unique identifier of the work flow that we are generating analysis for</param>
        /// <param name="termId">The unique id of the meddra term</param>
        /// <param name="analyserTermSetResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForAnalyserPatients(
            LinkedResourceBaseDto wrapper,
            Guid workFlowGuid,
            int termId,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAnalyserTermPatientsResourceUri(workFlowGuid, termId, ResourceUriType.Current, analyserTermSetResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAnalyserTermPatientsResourceUri(workFlowGuid, termId, ResourceUriType.NextPage, analyserTermSetResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAnalyserTermPatientsResourceUri(workFlowGuid, termId, ResourceUriType.PreviousPage, analyserTermSetResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <param name="analyserTermSetResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private AnalyserTermDetailDto CustomResultMap(AnalyserTermDetailDto dto, AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            dto.Results = GetAnalyserResultSets<AnalyserResultDto>(dto.TerminologyMeddraId, analyserTermSetResourceParameters);

            // Generate series for exposed cases
            var exposedSeriesValueArray = new List<SeriesValueList>();
            var exposedSeriesValueList = new SeriesValueList() { Name = "Exposed Cases" };
            var exposedValues = new List<SeriesValueListItem>();

            var riskSeriesValueArray = new List<SeriesValueList>();
            var riskSeriesValueList = new SeriesValueList() { Name = "Unadjused Relative Risks" };
            var riskValues = new List<SeriesValueListItem>();

            foreach (var result in dto.Results)
            {
                var exposedModelItem = new SeriesValueListItem()
                {
                    Value = result.ExposedIncidenceRate.Cases,
                    //Min = intValue - ((intValue * 20) / 100),
                    //Max = intValue + ((intValue * 20) / 100),
                    Name = result.Medication
                };
                exposedValues.Add(exposedModelItem);

                var riskModelItem = new SeriesValueListItem()
                {
                    Value = result.UnadjustedRelativeRisk,
                    //Min = intValue - ((intValue * 20) / 100),
                    //Max = intValue + ((intValue * 20) / 100),
                    Name = result.Medication
                };
                riskValues.Add(riskModelItem);
            }
            exposedSeriesValueList.Series = exposedValues;
            exposedSeriesValueArray.Add(exposedSeriesValueList);

            riskSeriesValueList.Series = riskValues;
            riskSeriesValueArray.Add(riskSeriesValueList);

            dto.ExposedCaseSeries = exposedSeriesValueArray.ToArray();
            dto.RelativeRiskSeries = riskSeriesValueArray.ToArray();

            return dto;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private AnalyserTermIdentifierDto CreateLinksForMeddraTerm<T>(T dto)
        {
            AnalyserTermIdentifierDto identifier = (AnalyserTermIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MeddraTerm", identifier.TerminologyMeddraId), "self", "GET"));

            return identifier;
        }
    }
}
