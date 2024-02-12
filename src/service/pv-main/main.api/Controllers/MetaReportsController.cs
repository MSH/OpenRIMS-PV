using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System.Xml;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Linq;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api/metareports")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class MetaReportsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IRepositoryInt<MetaColumn> _metaColumnRepository;
        private readonly IRepositoryInt<MetaDependency> _metaDependencyRepository;
        private readonly IRepositoryInt<MetaReport> _metaReportRepository;
        private readonly IRepositoryInt<MetaTable> _metaTableRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;

        public MetaReportsController(ITypeHelperService typeHelperService,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            IRepositoryInt<MetaColumn> metaColumnTypeRepository,
            IRepositoryInt<MetaDependency> metaDependencyRepository,
            IRepositoryInt<MetaReport> metaReportRepository,
            IRepositoryInt<MetaTable> metaTableRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _metaColumnRepository = metaColumnTypeRepository ?? throw new ArgumentNullException(nameof(metaColumnTypeRepository));
            _metaDependencyRepository = metaDependencyRepository ?? throw new ArgumentNullException(nameof(metaDependencyRepository));
            _metaReportRepository = metaReportRepository ?? throw new ArgumentNullException(nameof(metaReportRepository));
            _metaTableRepository = metaTableRepository ?? throw new ArgumentNullException(nameof(metaTableRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all meta reports using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaReportIdentifierDto</returns>
        [HttpGet(Name = "GetMetaReportsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaReportIdentifierDto>> GetMetaReportsByIdentifier(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaReportIdentifierDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaReportsWithLinks = GetMetaReports<MetaReportIdentifierDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaReportIdentifierDto>(mappedMetaReportsWithLinks.TotalCount, mappedMetaReportsWithLinks);
            var wrapperWithLinks = CreateLinksForReports(wrapper, metaResourceParameters,
                mappedMetaReportsWithLinks.HasNext, mappedMetaReportsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all meta reports using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaReportDetailDto</returns>
        [HttpGet(Name = "GetMetaReportsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaReportDetailDto>> GetMetaReportsByDetail(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaReportDetailDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaReportsWithLinks = GetMetaReports<MetaReportDetailDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaReportDetailDto>(mappedMetaReportsWithLinks.TotalCount, mappedMetaReportsWithLinks);
            var wrapperWithLinks = CreateLinksForReports(wrapper, metaResourceParameters,
                mappedMetaReportsWithLinks.HasNext, mappedMetaReportsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single meta report using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta report</param>
        /// <returns>An ActionResult of type MetaReportIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetMetaReportByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<MetaReportIdentifierDto>> GetMetaReportByIdentifier(long id)
        {
            var mappedMetaReport = await GetMetaReportAsync<MetaReportIdentifierDto>(id);
            if (mappedMetaReport == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaReport<MetaReportIdentifierDto>(mappedMetaReport));
        }

        /// <summary>
        /// Get a single meta report using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta report</param>
        /// <returns>An ActionResult of type MetaReportDetailDto</returns>
        [HttpGet("{id}", Name = "GetMetaReportByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaReportDetailDto>> GetMetaReportByDetail(long id)
        {
            var mappedMetaReport = await GetMetaReportAsync<MetaReportDetailDto>(id);
            if (mappedMetaReport == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaReport<MetaReportDetailDto>(CustomReportMap(mappedMetaReport)));
        }

        /// <summary>
        /// Get a single meta report using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta report</param>
        /// <returns>An ActionResult of type MetaReportExpandedDto</returns>
        [HttpGet("{id}", Name = "GetMetaReportByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.expanded.v1+json", "application/vnd.main.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.expanded.v1+json", "application/vnd.main.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaReportExpandedDto>> GetMetaReportByExpanded(long id)
        {
            var mappedMetaReport = await GetMetaReportAsync<MetaReportExpandedDto>(id);
            if (mappedMetaReport == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaReport<MetaReportExpandedDto>(CustomReportMap(mappedMetaReport)));
        }

        /// <summary>
        /// Create a new meta report
        /// </summary>
        /// <param name="metaReportForUpdate">The meta report payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateMetaReport")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "ReporterAdmin")]
        public async Task<IActionResult> CreateMetaReport(
            [FromBody] MetaReportForUpdateDto metaReportForUpdate)
        {
            if (metaReportForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(metaReportForUpdate.ReportName, @"[a-zA-Z0-9 ]").Count < metaReportForUpdate.ReportName.Length)
            {
                ModelState.AddModelError("Message", "Report name contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            var coreEntityFromRepo = await _metaTableRepository.GetAsync(f => f.TableName == metaReportForUpdate.CoreEntity);
            if (coreEntityFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate core entity");
            }

            if (!string.IsNullOrWhiteSpace(metaReportForUpdate.ReportDefinition))
            {
                if (Regex.Matches(metaReportForUpdate.ReportDefinition, @"[-a-zA-Z0-9 .,]").Count < metaReportForUpdate.ReportDefinition.Length)
                {
                    ModelState.AddModelError("Message", "Report definition contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                }
            }

            if (!string.IsNullOrWhiteSpace(metaReportForUpdate.Breadcrumb))
            {
                if (Regex.Matches(metaReportForUpdate.Breadcrumb, @"[-a-zA-Z0-9 .,]").Count < metaReportForUpdate.Breadcrumb.Length)
                {
                    ModelState.AddModelError("Message", "Bread crumb contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                }
            }

            if (_unitOfWork.Repository<MetaReport>().Queryable().
                Where(l => l.ReportName == metaReportForUpdate.ReportName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var newMetaReport = new MetaReport()
                {
                    ReportName = metaReportForUpdate.ReportName,
                    ReportDefinition = metaReportForUpdate.ReportDefinition,
                    Breadcrumb = metaReportForUpdate.Breadcrumb,
                    IsSystem = false,
                    MetaReportGuid = Guid.NewGuid(),
                    ReportStatus = metaReportForUpdate.ReportStatus,
                    MetaDefinition = PrepareMetaDefinition(metaReportForUpdate)
                };

                _metaReportRepository.Save(newMetaReport);

                var mappedMetaReport = await GetMetaReportAsync<MetaReportIdentifierDto>(newMetaReport.Id);
                if (mappedMetaReport == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetMetaReportByIdentifier",
                    new
                    {
                        id = mappedMetaReport.Id
                    }, CreateLinksForMetaReport<MetaReportIdentifierDto>(mappedMetaReport));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing meta report
        /// </summary>
        /// <param name="id">The unique id of the meta report</param>
        /// <param name="metaReportForUpdate">The meta report payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateMetaReport")]
        [Consumes("application/json")]
        [Authorize(Roles = "ReporterAdmin")]
        public async Task<IActionResult> UpdateMetaReport(long id,
            [FromBody] MetaReportForUpdateDto metaReportForUpdate)
        {
            if (metaReportForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(metaReportForUpdate.ReportName, @"[a-zA-Z0-9 ]").Count < metaReportForUpdate.ReportName.Length)
            {
                ModelState.AddModelError("Message", "Report name contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            var coreEntityFromRepo = await _metaTableRepository.GetAsync(f => f.TableName == metaReportForUpdate.CoreEntity);
            if (coreEntityFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate core entity");
            }

            if (!string.IsNullOrWhiteSpace(metaReportForUpdate.ReportDefinition))
            {
                if (Regex.Matches(metaReportForUpdate.ReportDefinition, @"[-a-zA-Z0-9 .,]").Count < metaReportForUpdate.ReportDefinition.Length)
                {
                    ModelState.AddModelError("Message", "Report definition contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                }
            }

            if (!string.IsNullOrWhiteSpace(metaReportForUpdate.Breadcrumb))
            {
                if (Regex.Matches(metaReportForUpdate.Breadcrumb, @"[-a-zA-Z0-9 .,]").Count < metaReportForUpdate.Breadcrumb.Length)
                {
                    ModelState.AddModelError("Message", "Bread crumb contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                }
            }

            if (_unitOfWork.Repository<MetaReport>().Queryable().
                Where(l => l.ReportName == metaReportForUpdate.ReportName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var metaReportFromRepo = await _metaReportRepository.GetAsync(f => f.Id == id);
            if (metaReportFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                metaReportFromRepo.ReportName = metaReportForUpdate.ReportName;
                metaReportFromRepo.ReportDefinition = metaReportForUpdate.ReportDefinition;
                metaReportFromRepo.Breadcrumb = metaReportForUpdate.Breadcrumb;
                metaReportFromRepo.ReportStatus = metaReportForUpdate.ReportStatus;
                metaReportFromRepo.MetaDefinition = PrepareMetaDefinition(metaReportForUpdate);

                _metaReportRepository.Update(metaReportFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing meta report
        /// </summary>
        /// <param name="id">The unique id of the meta report</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteMetaReport")]
        [Authorize(Roles = "ReporterAdmin")]
        public async Task<IActionResult> DeleteMetaReport(long id)
        {
            var metaReportFromRepo = await _metaReportRepository.GetAsync(f => f.Id == id);
            if (metaReportFromRepo == null)
            {
                return NotFound();
            }

            if (metaReportFromRepo.IsSystem)
            {
                ModelState.AddModelError("Message", "Unable to delete a system report");
            }

            if (ModelState.IsValid)
            {
                _metaReportRepository.Delete(metaReportFromRepo);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing meta report attributes
        /// </summary>
        /// <param name="id">The unique id of the meta report</param>
        /// <param name="metaReportForAttributeUpdate">The meta widget payload</param>
        /// <returns></returns>
        [HttpPut("{id}/attributes", Name = "UpdateMetaReportAttributes")]
        [Consumes("application/json")]
        [Authorize(Roles = "ReporterAdmin")]
        public async Task<IActionResult> UpdateMetaReportAttributes(long id,
            [FromBody] MetaReportForAttributeUpdateDto metaReportForAttributeUpdate)
        {
            if (metaReportForAttributeUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var metaReportFromRepo = await _metaReportRepository.GetAsync(f => f.Id == id);
            if (metaReportFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                PrepareMetaDefinitionForAttribute(metaReportForAttributeUpdate, metaReportFromRepo);

                _metaReportRepository.Update(metaReportFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get meta tables from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        PagedCollection<T> GetMetaReports<T>(IdResourceParameters metaResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = metaResourceParameters.PageNumber,
                PageSize = metaResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaReport>(metaResourceParameters.OrderBy, "asc");

            var reportdMetaReportsFromRepo = _metaReportRepository.List(pagingInfo, null, orderby, "");
            if (reportdMetaReportsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaReports = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(reportdMetaReportsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    reportdMetaReportsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedMetaReports.TotalCount,
                    reportSize = mappedMetaReports.PageSize,
                    currentReport = mappedMetaReports.CurrentPage,
                    totalReports = mappedMetaReports.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMetaTables.ForEach(dto => CreateLinksForMetaTable(dto));

                return mappedMetaReports;
            }

            return null;
        }

        /// <summary>
        /// Get single meta report from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetMetaReportAsync<T>(long id) where T : class
        {
            var metaReportFromRepo = await _metaReportRepository.GetAsync(f => f.Id == id, new string[] { "" });

            if (metaReportFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaReport = _mapper.Map<T>(metaReportFromRepo);

                return mappedMetaReport;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaReportIdentifierDto CreateLinksForMetaReport<T>(T dto)
        {
            MetaReportIdentifierDto identifier = (MetaReportIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaReport", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare an XML definition containing details of the report
        /// </summary>
        /// <param name="metaReportForUpdate">The meta report payload</param>
        /// <returns></returns>
        private string PrepareMetaDefinition(MetaReportForUpdateDto metaReportForUpdate)
        {
            // Prepare XML
            XmlDocument meta = new XmlDocument();

            var ns = ""; // urn:pvims-org:v3

            XmlNode rootNode = null;
            XmlAttribute attrib;

            XmlDeclaration xmlDeclaration = meta.CreateXmlDeclaration("1.0", "UTF-8", null);
            meta.AppendChild(xmlDeclaration);

            rootNode = meta.CreateElement("MetaReport", ns);

            attrib = meta.CreateAttribute("Type");
            attrib.InnerText = metaReportForUpdate.ReportType.ToString();
            rootNode.Attributes.Append(attrib);

            attrib = meta.CreateAttribute("CoreEntity");
            attrib.InnerText = metaReportForUpdate.CoreEntity;
            rootNode.Attributes.Append(attrib);

            meta.AppendChild(rootNode);

            return meta.InnerXml;
        }

        /// <summary>
        ///  Prepare an XML definition containing details of the report (including attributes)
        /// </summary>
        /// <param name="metaReportForAttributeUpdate">The meta report attribute payload</param>
        /// <param name="metaReport">The meta report being updated</param>
        /// <returns></returns>
        private void PrepareMetaDefinitionForAttribute(MetaReportForAttributeUpdateDto metaReportForAttributeUpdate,
            MetaReport metaReport)
        {
            // Prepare XML
            XmlDocument meta = new XmlDocument();

            var ns = ""; // urn:pvims-org:v3

            XmlNode rootNode = null;
            XmlNode mainNode = null;
            XmlNode subNode = null;
            XmlAttribute attrib;

            XmlDeclaration xmlDeclaration = meta.CreateXmlDeclaration("1.0", "UTF-8", null);
            meta.AppendChild(xmlDeclaration);

            rootNode = meta.CreateElement("MetaReport", ns);

            attrib = meta.CreateAttribute("Type");
            attrib.InnerText = metaReportForAttributeUpdate.ReportType.ToString();
            rootNode.Attributes.Append(attrib);

            attrib = meta.CreateAttribute("CoreEntity");
            attrib.InnerText = metaReportForAttributeUpdate.CoreEntity.ToString();
            rootNode.Attributes.Append(attrib);

            if (metaReportForAttributeUpdate.ReportType == MetaReportTypes.Summary)
            {
                mainNode = meta.CreateElement("Summary", ns);

                foreach (var attribute in metaReportForAttributeUpdate.Attributes)
                {
                    subNode = meta.CreateElement("SummaryItem", ns);
                    attrib = meta.CreateAttribute("DisplayName");
                    attrib.InnerText = attribute.DisplayName;
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("Aggregate");
                    attrib.InnerText = attribute.Aggregate;
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("AttributeName");
                    attrib.InnerText = attribute.AttributeName;
                    subNode.Attributes.Append(attrib);

                    mainNode.AppendChild(subNode);
                }

                rootNode.AppendChild(mainNode);
            }
            else
            {
                mainNode = meta.CreateElement("List", ns);

                foreach (var attribute in metaReportForAttributeUpdate.Attributes)
                {
                    subNode = meta.CreateElement("ListItem", ns);
                    attrib = meta.CreateAttribute("DisplayName");
                    attrib.InnerText = attribute.DisplayName;
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("Aggregate");
                    attrib.InnerText = string.Empty;
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("AttributeName");
                    attrib.InnerText = attribute.AttributeName;
                    subNode.Attributes.Append(attrib);

                    mainNode.AppendChild(subNode);
                }

                rootNode.AppendChild(mainNode);
            }

            mainNode = meta.CreateElement("Filter", ns);

            foreach (var filter in metaReportForAttributeUpdate.Filters)
            {
                subNode = meta.CreateElement("FilterItem", ns);
                attrib = meta.CreateAttribute("AttributeName");
                attrib.InnerText = filter.AttributeName;
                subNode.Attributes.Append(attrib);

                attrib = meta.CreateAttribute("Operator");
                attrib.InnerText = filter.Operator;
                subNode.Attributes.Append(attrib);

                attrib = meta.CreateAttribute("Relation");
                attrib.InnerText = filter.Relation;
                subNode.Attributes.Append(attrib);

                mainNode.AppendChild(subNode);
            }

            rootNode.AppendChild(mainNode);
            meta.AppendChild(rootNode);

            metaReport.MetaDefinition = meta.InnerXml;

            string sql = string.Empty;
            if (metaReportForAttributeUpdate.ReportType == MetaReportTypes.Summary)
            {
                sql = PrepareSummaryQueryForPublication(metaReportForAttributeUpdate);
            }
            else
            {
                sql = PrepareListQueryForPublication(metaReportForAttributeUpdate);
            }
            metaReport.SqlDefinition = sql;
        }

        /// <summary>
        ///  Prepare the sql query needed to produce the list report
        /// </summary>
        /// <param name="metaReportForAttributeUpdate">The meta report attribute payload</param>
        /// <returns></returns>
        private string PrepareListQueryForPublication(MetaReportForAttributeUpdateDto metaReportForAttributeUpdate)
        {
            string sql = "";

            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var metaTable = _metaTableRepository.Get(mt => mt.TableName == metaReportForAttributeUpdate.CoreEntity, new string[] { "TableType" });

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                default:
                    break;
            }

            // JOINS
            MetaDependency metaDependency;
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Child:
                case MetaTableTypes.Core:
                    // do nothing
                    break;

                case MetaTableTypes.History:
                case MetaTableTypes.CoreChild:
                    // get parent
                    metaDependency = _metaDependencyRepository.Get(md => md.ReferenceTable.TableName == metaReportForAttributeUpdate.CoreEntity, new string[] { "ParentTable" });

                    jcriteria += String.Format(" LEFT JOIN [Meta{0}] P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            var fc = 0;
            foreach (var list in metaReportForAttributeUpdate.Attributes)
            {
                fc += 1;

                scriteria += "cast(" + list.AttributeName + " as varchar)" + " as 'Col" + fc.ToString() + "', ";
                ocriteria += list.AttributeName + ", ";
            }
            scriteria = !String.IsNullOrWhiteSpace(scriteria) ? scriteria.Substring(0, scriteria.Length - 2) : "";
            ocriteria = !String.IsNullOrWhiteSpace(ocriteria) ? ocriteria.Substring(0, ocriteria.Length - 2) : "";

            // FILTERS
            fc = 0;
            foreach (var filter in metaReportForAttributeUpdate.Filters)
            {
                fc += 1;
                wcriteria += String.Format("{0} ({1} {2} %{3})", filter.Relation, filter.AttributeName, filter.Operator, fc.ToString());
            }

            sql = String.Format(@"
                select {0} 
                    from {3} 
                            {1}
                    where 1 = 1 {4}
                            ORDER BY {2}
                ", scriteria, jcriteria, ocriteria, fcriteria, wcriteria);

            return sql;
        }

        /// <summary>
        ///  Prepare the sql query needed to produce the summary report
        /// </summary>
        /// <param name="metaReportForAttributeUpdate">The meta report attribute payload</param>
        /// <returns></returns>
        private string PrepareSummaryQueryForPublication(MetaReportForAttributeUpdateDto metaReportForAttributeUpdate)
        {
            string sql = "";

            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string gcriteria = ""; // groups
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var metaTable = _unitOfWork.Repository<MetaTable>()
                .Queryable()
                .SingleOrDefault(mt => mt.TableName == metaReportForAttributeUpdate.CoreEntity);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                default:
                    break;
            }

            // JOINS
            MetaDependency metaDependency;
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Child:
                case MetaTableTypes.Core:
                    // do nothing
                    break;

                case MetaTableTypes.History:
                case MetaTableTypes.CoreChild:
                    // get parent
                    metaDependency = _unitOfWork.Repository<MetaDependency>()
                        .Queryable()
                        .SingleOrDefault(md => md.ReferenceTable.TableName == metaReportForAttributeUpdate.CoreEntity);

                    jcriteria += String.Format(" LEFT JOIN [Meta{0}] P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            var fc = 0;
            foreach (var strat in metaReportForAttributeUpdate.Attributes)
            {
                fc += 1;

                scriteria += "cast(" + strat.AttributeName + " as varchar)" + " as 'Col" + fc.ToString() + "', ";
                gcriteria += strat.AttributeName + ", ";
                ocriteria += strat.AttributeName + ", ";
            }

            scriteria = !String.IsNullOrWhiteSpace(scriteria) ? scriteria.Substring(0, scriteria.Length - 2) : "";
            gcriteria = !String.IsNullOrWhiteSpace(gcriteria) ? gcriteria.Substring(0, ocriteria.Length - 2) : "";
            ocriteria = !String.IsNullOrWhiteSpace(ocriteria) ? ocriteria.Substring(0, ocriteria.Length - 2) : "";

            // FILTERS
            var filc = 0;
            foreach (var filter in metaReportForAttributeUpdate.Filters)
            {
                filc += 1;
                wcriteria += String.Format("{0} ({1} {2} %{3})", filter.Relation, filter.AttributeName, filter.Operator, filc.ToString());
            }

            sql = String.Format(@"
                select {0}, CAST(COUNT(*) as varchar) AS Col{6}
                    from {4} 
                            {1}
                    where 1 = 1 {5}
                            GROUP BY {2}
                            ORDER BY {3}
                ", scriteria, jcriteria, gcriteria, ocriteria, fcriteria, wcriteria, (fc + 1).ToString());

            return sql;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaReportDetailDto CustomReportMap(MetaReportDetailDto dto)
        {
            var metaReportFromRepo = _metaReportRepository.Get(p => p.Id == dto.Id);
            if (metaReportFromRepo == null)
            {
                return dto;
            }

            // Map report definition attributes
            XmlDocument meta = new XmlDocument();
            meta.LoadXml(metaReportFromRepo.MetaDefinition);

            XmlNode rootNode = meta.SelectSingleNode("//MetaReport");
            XmlAttribute typeAttr = rootNode.Attributes["Type"];
            XmlAttribute entityAttr = rootNode.Attributes["CoreEntity"];

            dto.CoreEntity = entityAttr.Value;
            dto.ReportType = typeAttr.Value;

            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaReportExpandedDto CustomReportMap(MetaReportExpandedDto dto)
        {
            var metaReportFromRepo = _metaReportRepository.Get(p => p.Id == dto.Id);
            if (metaReportFromRepo == null)
            {
                return dto;
            }

            // Map report definition attributes
            XmlDocument meta = new XmlDocument();
            meta.LoadXml(metaReportFromRepo.MetaDefinition);

            XmlNode rootNode = meta.SelectSingleNode("//MetaReport");
            XmlAttribute typeAttr = rootNode.Attributes["Type"];
            XmlAttribute entityAttr = rootNode.Attributes["CoreEntity"];

            dto.CoreEntity = entityAttr.Value;
            dto.ReportType = typeAttr.Value;

            // Map attributes
            var rootNodeName = dto.ReportType == "List" ? "//List" : "//Summary";

            XmlNode mainNode = rootNode.SelectSingleNode(rootNodeName);
            if (mainNode != null)
            {
                foreach (XmlNode subNode in mainNode.ChildNodes)
                {
                    var attribute = new MetaAttributeDto()
                    {
                        AttributeName = subNode.Attributes.GetNamedItem("AttributeName").Value,
                        Aggregate = subNode.Attributes.GetNamedItem("Aggregate").Value,
                        DisplayName = subNode.Attributes.GetNamedItem("DisplayName").Value,
                        Index = dto.Attributes.Count + 1
                    };
                    dto.Attributes.Add(attribute);
                }
            }

            // Map filters
            mainNode = rootNode.SelectSingleNode("//Filter");
            if (mainNode != null)
            {
                foreach (XmlNode subNode in mainNode.ChildNodes)
                {
                    var attributeName = subNode.Attributes.GetNamedItem("AttributeName").Value;
                    var filter = new MetaFilterDto()
                    {
                        AttributeName = attributeName,
                        ColumnType = _metaColumnRepository.Get(mc => mc.ColumnName == attributeName)?.ColumnType.Description,
                        Operator = subNode.Attributes.GetNamedItem("Operator").Value,
                        Relation = subNode.Attributes.GetNamedItem("Relation").Value,
                        Index = dto.Attributes.Count + 1
                    };
                    dto.Filters.Add(filter);
                }
            }

            // Map core entity
            var metaTableFromRepo = _metaTableRepository.Get(p => p.TableName == dto.CoreEntity, new string[] { "TableType", "Columns.ColumnType" });
            if (metaTableFromRepo == null)
            {
                return dto;
            }

            // Map EF entity to Dto
            dto.CoreMetaTable = _mapper.Map<MetaTableExpandedDto>(metaTableFromRepo);
            dto.CoreMetaTable.Columns = dto.CoreMetaTable.Columns.OrderBy(c => c.ColumnName).ToList();

            dto.CoreMetaTable.Columns.ForEach(column => CustomColumnMap(column));

            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaColumnExpandedDto CustomColumnMap(MetaColumnExpandedDto dto)
        {
            var metaColumnFromRepo = _metaColumnRepository.Get(c => c.Id == dto.Id);
            if (metaColumnFromRepo == null)
            {
                return dto;
            }

            switch ((MetaColumnTypes)metaColumnFromRepo.ColumnType.Id)
            {
                case MetaColumnTypes.tbigint:
                case MetaColumnTypes.tint:
                case MetaColumnTypes.tdecimal:
                case MetaColumnTypes.tsmallint:
                case MetaColumnTypes.ttinyint:
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Equals", OperatorValue = "=" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Not Equals", OperatorValue = "<>" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Greater Than", OperatorValue = ">" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Less Than", OperatorValue = "<" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "GreaterEqual Than", OperatorValue = ">=" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "LessEqual Than", OperatorValue = "<=" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Between", OperatorValue = "between" });

                    break;

                case MetaColumnTypes.tchar:
                case MetaColumnTypes.tnchar:
                case MetaColumnTypes.tnvarchar:
                case MetaColumnTypes.tvarchar:
                    if (String.IsNullOrEmpty(metaColumnFromRepo.Range))
                    {
                        dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Equals", OperatorValue = "=" });
                        dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Not Equals", OperatorValue = "<>" });
                    }
                    else
                    {
                        dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Equals", OperatorValue = "=" });
                        dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Not Equals", OperatorValue = "<>" });
                        dto.Operators.Add(new MetaOperatorDto() { OperatorName = "In", OperatorValue = "in" });
                    }

                    break;

                case MetaColumnTypes.tdate:
                case MetaColumnTypes.tdatetime:
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Equals", OperatorValue = "=" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Not Equals", OperatorValue = "<>" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Greater Than", OperatorValue = ">" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Less Than", OperatorValue = "<" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "GreaterEqual Than", OperatorValue = ">=" });
                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "LessEqual Than", OperatorValue = "<=" });

                    dto.Operators.Add(new MetaOperatorDto() { OperatorName = "Between", OperatorValue = "between" });
                    break;

                default:
                    break;
            }

            return dto;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForReports(
            LinkedResourceBaseDto wrapper,
            IdResourceParameters metaResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetMetaReportsByIdentifier", metaResourceParameters.OrderBy, metaResourceParameters.PageNumber, metaResourceParameters.PageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetMetaReportsByIdentifier", metaResourceParameters.OrderBy, metaResourceParameters.PageNumber, metaResourceParameters.PageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetMetaReportsByIdentifier", metaResourceParameters.OrderBy, metaResourceParameters.PageNumber, metaResourceParameters.PageSize),
                       "previousPage", "GET"));
            }

            return wrapper;
        }
    }
}
