using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using OpenRIMS.PV.Main.Core.ValueTypes;
using OpenRIMS.PV.Main.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class MetaController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IInfrastructureService _infrastructureService;
        private readonly IRepositoryInt<DatasetCategoryElement> _datasetCategoryElementRepository;
        private readonly IRepositoryInt<MetaTable> _metaTableRepository;
        private readonly IRepositoryInt<MetaTableType> _metaTableTypeRepository;
        private readonly IRepositoryInt<MetaColumn> _metaColumnRepository;
        private readonly IRepositoryInt<MetaColumnType> _metaColumnTypeRepository;
        private readonly IRepositoryInt<MetaDependency> _metaDependencyRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly MainDbContext _context;

        private List<String> _entities = new List<String>() { 
            "Patient", 
            "PatientClinicalEvent", 
            "PatientCondition", 
            "PatientFacility", 
            "PatientLabTest", 
            "PatientMedication", 
            "Encounter", 
            "CohortGroupEnrolment" 
        };

        public MetaController(
            ITypeHelperService typeHelperService,
            IInfrastructureService infrastructureService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<DatasetCategoryElement> datasetCategoryElementRepository,
            IRepositoryInt<MetaTable> metaTableRepository,
            IRepositoryInt<MetaTableType> metaTableTypeRepository,
            IRepositoryInt<MetaColumn> metaColumnRepository,
            IRepositoryInt<MetaColumnType> metaColumnTypeRepository,
            IRepositoryInt<MetaDependency> metaDependencyRepository,
            IUnitOfWorkInt unitOfWork,
            MainDbContext dbContext)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _infrastructureService = infrastructureService ?? throw new ArgumentNullException(nameof(infrastructureService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _datasetCategoryElementRepository = datasetCategoryElementRepository ?? throw new ArgumentNullException(nameof(datasetCategoryElementRepository));
            _metaTableRepository = metaTableRepository ?? throw new ArgumentNullException(nameof(metaTableRepository));
            _metaTableTypeRepository = metaTableTypeRepository ?? throw new ArgumentNullException(nameof(metaTableTypeRepository));
            _metaColumnRepository = metaColumnRepository ?? throw new ArgumentNullException(nameof(metaColumnRepository));
            _metaColumnTypeRepository = metaColumnTypeRepository ?? throw new ArgumentNullException(nameof(metaColumnTypeRepository));
            _metaDependencyRepository = metaDependencyRepository ?? throw new ArgumentNullException(nameof(metaDependencyRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Get a summary of meta data used for reporting
        /// </summary>
        /// <returns>An ActionResult of type MetaSummaryDto</returns>
        [HttpGet("meta", Name = "GetMetaSummary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        public ActionResult<MetaSummaryDto> GetMetaSummary()
        {
            var metaSummary = new MetaSummaryDto();

            string sql = string.Format(@"
                SELECT Id FROM MetaPatient;
                ");

            var resultsFromService = _context.MetaPatientLists
                .FromSqlRaw("SELECT Id FROM MetaPatient");

            metaSummary.PatientCount = resultsFromService.Count();

            var config = _infrastructureService.GetOrCreateConfig(ConfigType.MetaDataLastUpdated);
            metaSummary.LatestRefreshDate = config.ConfigValue;

            return Ok(metaSummary);
        }

        /// <summary>
        /// Get all meta tables using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaTableIdentifierDto</returns>
        [HttpGet("metatables", Name = "GetMetaTablesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaTableIdentifierDto>> GetMetaTablesByIdentifier(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaTableIdentifierDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaTablesWithLinks = GetMetaTables<MetaTableIdentifierDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaTableIdentifierDto>(mappedMetaTablesWithLinks.TotalCount, mappedMetaTablesWithLinks);
            var wrapperWithLinks = CreateLinksForTables(wrapper, metaResourceParameters,
                mappedMetaTablesWithLinks.HasNext, mappedMetaTablesWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all meta tables using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaTableDetailDto</returns>
        [HttpGet("metatables", Name = "GetMetaTablesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaTableDetailDto>> GetMetaTablesByDetail(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaTableDetailDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaTablesWithLinks = GetMetaTables<MetaTableDetailDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaTableDetailDto>(mappedMetaTablesWithLinks.TotalCount, mappedMetaTablesWithLinks);
            var wrapperWithLinks = CreateLinksForTables(wrapper, metaResourceParameters,
                mappedMetaTablesWithLinks.HasNext, mappedMetaTablesWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all meta columns using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaColumnDetailDto</returns>
        [HttpGet("metacolumns", Name = "GetMetaColumnsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaColumnDetailDto>> GetMetaColumnsByDetail(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaColumnDetailDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaColumnsWithLinks = GetMetaColumns<MetaColumnDetailDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaColumnDetailDto>(mappedMetaColumnsWithLinks.TotalCount, mappedMetaColumnsWithLinks);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all meta dependencies using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaDependencyDetailDto</returns>
        [HttpGet("metadependencies", Name = "GetMetaDependenciesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.detail.v1+json", "application/vnd.main.detail.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaDependencyDetailDto>> GetMetaDependenciesByDetail(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaDependencyDetailDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaDependenciesWithLinks = GetMetaDependencies<MetaDependencyDetailDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaDependencyDetailDto>(mappedMetaDependenciesWithLinks.TotalCount, mappedMetaDependenciesWithLinks);

            return Ok(wrapper);
        }

        /// <summary>
        /// Refresh meta data
        /// </summary>
        /// <returns></returns>
        [HttpPost("meta", Name = "RefreshMeta")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RefreshMetaAsync()
        {
            // Ensure all meta definitions exist
            CheckEntitiesExist();
            CheckDependenciesExist();
            CheckColumnsExist();

            // Ensure all meta tables prepared
            DropMetaTables();
            CreateMetaTables();

            // Populate meta tables
            PopulateMetaTables();
            UpdateCustomAttributes();
            CreateMetaDependencies();

            await _infrastructureService.SetConfigValueAsync(ConfigType.MetaDataLastUpdated, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            return NoContent();
        }

        /// <summary>
        /// Get meta tables from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMetaTables<T>(IdResourceParameters metaResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = metaResourceParameters.PageNumber,
                PageSize = metaResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaTable>(metaResourceParameters.OrderBy, "asc");

            var pagedMetaTablesFromRepo = _metaTableRepository.List(pagingInfo, null, orderby, new string[] { "TableType" });
            if (pagedMetaTablesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaTables = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedMetaTablesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMetaTablesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedMetaTables.TotalCount,
                    pageSize = mappedMetaTables.PageSize,
                    currentPage = mappedMetaTables.CurrentPage,
                    totalPages = mappedMetaTables.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMetaTables.ForEach(dto => CreateLinksForMetaTable(dto));

                return mappedMetaTables;
            }

            return null;
        }

        /// <summary>
        /// Get meta columns from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMetaColumns<T>(IdResourceParameters metaResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = metaResourceParameters.PageNumber,
                PageSize = metaResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaColumn>(metaResourceParameters.OrderBy, "asc");

            var pagedMetaColumnsFromRepo = _metaColumnRepository.List(pagingInfo, null, orderby, new string[] { "Table", "ColumnType" });
            if (pagedMetaColumnsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaColumns = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedMetaColumnsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMetaColumnsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedMetaColumns.TotalCount,
                    pageSize = mappedMetaColumns.PageSize,
                    currentPage = mappedMetaColumns.CurrentPage,
                    totalPages = mappedMetaColumns.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMetaTables.ForEach(dto => CreateLinksForMetaTable(dto));

                return mappedMetaColumns;
            }

            return null;
        }

        /// <summary>
        /// Get meta dependencies from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMetaDependencies<T>(IdResourceParameters metaResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = metaResourceParameters.PageNumber,
                PageSize = metaResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaDependency>(metaResourceParameters.OrderBy, "asc");

            var pagedMetaDependenciesFromRepo = _metaDependencyRepository.List(pagingInfo, null, orderby, new string[] { "ParentTable", "ReferenceTable" });
            if (pagedMetaDependenciesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaDependencies = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedMetaDependenciesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMetaDependenciesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedMetaDependencies.TotalCount,
                    pageSize = mappedMetaDependencies.PageSize,
                    currentPage = mappedMetaDependencies.CurrentPage,
                    totalPages = mappedMetaDependencies.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMetaDependencies.ForEach(dto => CreateLinksForMetaDependency(dto));

                return mappedMetaDependencies;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForTables(
            LinkedResourceBaseDto wrapper,
            IdResourceParameters metaResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetMetaTablesByIdentifier", metaResourceParameters.OrderBy, metaResourceParameters.PageNumber, metaResourceParameters.PageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetMetaTablesByIdentifier", metaResourceParameters.OrderBy, metaResourceParameters.PageNumber, metaResourceParameters.PageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetMetaTablesByIdentifier", metaResourceParameters.OrderBy, metaResourceParameters.PageNumber, metaResourceParameters.PageSize),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        #region "Prepare Meta Definitions"

        private void CheckEntitiesExist()
        {
            foreach (String entity in _entities)
            {
                var metaTable = _metaTableRepository.Get(mt => mt.TableName == entity);
                if (metaTable == null)
                {
                    switch (entity)
                    {
                        case "Patient":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Core patient table",
                                FriendlyName = "Patient",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "Core")
                            };
                            break;

                        case "PatientClinicalEvent":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient adverse event history",
                                FriendlyName = "Patient Adverse Events",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "PatientCondition":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient condition history",
                                FriendlyName = "Patient Conditions",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType =  _metaTableTypeRepository.Get(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "PatientFacility":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Current facility",
                                FriendlyName = "Current Facility",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "History")
                            };
                            break;

                        case "PatientLabTest":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient evaluation history",
                                FriendlyName = "Patien Lab Tests",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "PatientMedication":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient medication history",
                                FriendlyName = "Patient Medications",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "Encounter":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient encounter history",
                                FriendlyName = "Patient Encounters",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "Child")
                            };
                            break;

                        case "CohortGroupEnrolment":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient cohort enrolments",
                                FriendlyName = "Cohort Enrolment",
                                MetaTableGuid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = _metaTableTypeRepository.Get(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        default:
                            break;
                    }
                    _metaTableRepository.Save(metaTable);
                }
            }
        }

        private void CheckDependenciesExist()
        {
            var parentTable = _metaTableRepository.Get(mt => mt.TableName == "Patient");

            // Dependency Patient --> PatientClinicalEvent
            var referenceTable = _metaTableRepository.Get(mt => mt.TableName == "PatientClinicalEvent");
            var metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.Save(metaDependency);
            }

            // Dependency Patient --> PatientCondition
            referenceTable = _metaTableRepository.Get(mt => mt.TableName == "PatientCondition");
            metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.Save(metaDependency);
            }

            // Dependency Patient --> PatientFacility
            referenceTable = _metaTableRepository.Get(mt => mt.TableName == "PatientFacility");
            metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.Save(metaDependency);
            }

            // Dependency Patient --> PatientLabTest
            referenceTable = _metaTableRepository.Get(mt => mt.TableName == "PatientLabTest");
            metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.Save(metaDependency);
            }

            // Dependency Patient --> PatientMedication
            referenceTable = _metaTableRepository.Get(mt => mt.TableName == "PatientMedication");
            metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.Save(metaDependency);
            }

            // Dependency Patient --> Encounter
            referenceTable = _metaTableRepository.Get(mt => mt.TableName == "Encounter");
            metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.SaveAsync(metaDependency);
            }

            // Dependency  --> CohortGroupEnrolment
            referenceTable = _metaTableRepository.Get(mt => mt.TableName == "CohortGroupEnrolment");
            metaDependency = _metaDependencyRepository.Get(md => md.ParentTable.MetaTableGuid == parentTable.MetaTableGuid && md.ReferenceTable.MetaTableGuid == referenceTable.MetaTableGuid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    MetaDependencyGuid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                _metaDependencyRepository.SaveAsync(metaDependency);
            }
        }

        private void CheckColumnsExist()
        {
            ProcessEntity(typeof(Patient), "Patient");
            ProcessEntity(typeof(PatientMedication), "PatientMedication");
            ProcessEntity(typeof(PatientClinicalEvent), "PatientClinicalEvent");
            ProcessEntity(typeof(PatientCondition), "PatientCondition");
            ProcessEntity(typeof(PatientLabTest), "PatientLabTest");
            ProcessEntity(typeof(Encounter), "Encounter");
            ProcessEntity(typeof(CohortGroupEnrolment), "CohortGroupEnrolment");
            ProcessEntity(typeof(PatientFacility), "PatientFacility");
        }

        private void ProcessEntity(Type type, string entityName)
        {
            PropertyInfo[] properties = type.GetProperties();
            var invalidProperties = new[] { "CustomAttributesXmlSerialised", "Archived", "ArchivedReason", "ArchivedDate", "AuditUser", "Age", "FullName", "DisplayName", "CurrentFacilityName", "LatestEncounterDate" };

            var metaTable = _metaTableRepository.Get(mt => mt.TableName == entityName);
            var attributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == entityName).OrderBy(c => c.Id).ToList();

            ICollection<DatasetCategoryElement> elements = null;
            if (entityName == "Encounter")
            {
                elements = _datasetCategoryElementRepository.List(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment", null, new string[] { "DatasetElement.Field.FieldType" });
            }

            MetaColumn metaColumn;
            MetaColumnType metaColumnType;
            String range = "";

            foreach (PropertyInfo property in properties)
            {
                if (!invalidProperties.Contains(property.Name))
                {
                    var columnName = property.Name;
                    if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter))
                    {
                        columnName = property.Name + "_Id";
                    };

                    metaColumn = _metaColumnRepository.Get(mc => mc.Table.TableName == entityName && mc.ColumnName == columnName);
                    if (metaColumn == null)
                    {
                        metaColumnType = null;
                        range = "";

                        if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "datetime"); };
                        if (property.PropertyType == typeof(string)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar"); };
                        if (property.PropertyType == typeof(int)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "int"); };
                        if (property.PropertyType == typeof(decimal)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "decimal"); };
                        if (property.PropertyType == typeof(Guid)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "uniqueidentifier"); };
                        if (property.PropertyType == typeof(bool)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "bit"); };
                        if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter)) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "int"); };

                        if (property.PropertyType == typeof(EncounterType))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:EncounterType.Description";
                        }
                        if (property.PropertyType == typeof(Facility))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:Facility.FacilityName";
                        }
                        if (property.PropertyType == typeof(CohortGroup))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:CohortGroup.CohortName";
                        }
                        if (property.PropertyType == typeof(LabTestUnit))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:LabTestUnit.Description";
                        }
                        if (property.PropertyType == typeof(LabTest))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:LabTest.Description";
                        }
                        if (property.PropertyType == typeof(Outcome))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:Outcome.Description";
                        }
                        if (property.PropertyType == typeof(TerminologyMedDra))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                        }
                        if (property.PropertyType == typeof(User))
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "SOURCE:User.UserName";
                        }

                        if (metaColumnType != null)
                        {
                            metaColumn = new MetaColumn()
                            {
                                ColumnName = columnName,
                                ColumnType = metaColumnType,
                                IsIdentity = (property.Name == "Id"),
                                //IsNullable = Nullable.GetUnderlyingType(property.PropertyType) != null,
                                IsNullable = property.Name == "Id" ? false : true,
                                MetaColumnGuid = Guid.NewGuid(),
                                Table = metaTable,
                                Range = range
                            };
                            _metaColumnRepository.Save(metaColumn);
                        }
                    }
                }
            }

            // Now process attributes
            foreach (CustomAttributeConfiguration attribute in attributes)
            {
                metaColumn = _metaColumnRepository.Get(mc => mc.Table.TableName == entityName && mc.ColumnName == attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""));
                if (metaColumn == null)
                {
                    metaColumnType = null;
                    range = "";

                    if (attribute.CustomAttributeType == CustomAttributeType.DateTime) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "datetime"); };
                    if (attribute.CustomAttributeType == CustomAttributeType.Numeric) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "int"); };
                    if (attribute.CustomAttributeType == CustomAttributeType.String) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar"); };
                    if (attribute.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                        var selectionItems = _unitOfWork.Repository<SelectionDataItem>().Queryable().Where(sd => sd.AttributeKey == attribute.AttributeKey).Select(s => s.Value).ToList();
                        range = string.Join(",", selectionItems);
                    };

                    if (metaColumnType != null)
                    {
                        metaColumn = new MetaColumn()
                        {
                            ColumnName = attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "").Trim().Length > 100 ? attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "").Trim().Substring(0, 100) : attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "").Trim(),
                            ColumnType = metaColumnType,
                            IsIdentity = false,
                            IsNullable = true,
                            MetaColumnGuid = Guid.NewGuid(),
                            Table = metaTable,
                            Range = range
                        };
                        _metaColumnRepository.Save(metaColumn);
                    }
                }
            }

            // Process instance headers
            if (elements != null)
            {
                foreach (DatasetCategoryElement dce in elements)
                {
                    metaColumn = _metaColumnRepository.Get(mc => mc.Table.TableName == entityName && mc.ColumnName == dce.DatasetElement.ElementName.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""));
                    if (metaColumn == null)
                    {
                        metaColumnType = null;
                        range = "";

                        if (dce.DatasetElement.Field.FieldType.Description == "Date") { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "date"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox") { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "NumericTextbox" && dce.DatasetElement.Field.Decimals == 0) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "int"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "NumericTextbox" && dce.DatasetElement.Field.Decimals > 0) { metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "decimal"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "DropDownList")
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            var fieldValues = _unitOfWork.Repository<FieldValue>().Queryable().Where(fv => fv.Field.Id == dce.DatasetElement.Field.Id).Select(s => s.Value).ToList();
                            range = string.Join(",", fieldValues);
                        };
                        if (dce.DatasetElement.Field.FieldType.Description == "YesNo")
                        {
                            metaColumnType = _metaColumnTypeRepository.Get(mct => mct.Description == "varchar");
                            range = "Yes, No";
                        };

                        if (metaColumnType != null)
                        {
                            metaColumn = new MetaColumn()
                            {
                                ColumnName = dce.DatasetElement.ElementName.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""),
                                ColumnType = metaColumnType,
                                IsIdentity = false,
                                IsNullable = true,
                                MetaColumnGuid = Guid.NewGuid(),
                                Table = metaTable,
                                Range = range
                            };
                            _metaColumnRepository.Save(metaColumn);
                        }
                    }
                }
            }
        }

        #endregion

        #region "Handle Meta Tables"

        private void DropMetaTables()
        {
            StringBuilder sb;

            var metaTables = _unitOfWork.Repository<MetaTable>().Queryable().OrderByDescending(mt => mt.Id).ToList();
            foreach (MetaTable metaTable in metaTables)
            {
                sb = new StringBuilder();

                sb.AppendFormat("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meta{0}]') AND type in (N'U')) DROP TABLE [Meta{0}]", metaTable.TableName);

                _unitOfWork.Repository<Patient>().ExecuteSqlCommand(sb.ToString(), new SqlParameter[0]);
            }
        }

        private void CreateMetaTables()
        {
            StringBuilder sb;
            var valid = new[] { "varchar", "nvarchar" };

            var metaTables = _metaTableRepository.List(null, null, new string[] { "Columns.ColumnType" });
            foreach (MetaTable metaTable in metaTables)
            {

                sb = new StringBuilder();

                sb.AppendFormat("CREATE TABLE [Meta{0}] (", metaTable.TableName);

                // Add each column
                foreach (MetaColumn metaColumn in metaTable.Columns)
                {
                    sb.AppendFormat("[{0}] [{1}] {2} {3},", metaColumn.ColumnName, metaColumn.ColumnType.Description, valid.Contains(metaColumn.ColumnType.Description) ? "(max)" : "", metaColumn.IsNullable ? "NULL" : "NOT NULL");
                }

                // Create constraint
                sb.AppendFormat("CONSTRAINT [{0}_PK] PRIMARY KEY CLUSTERED  ([Id] ASC))", metaTable.TableName);

                _unitOfWork.Repository<Patient>().ExecuteSqlCommand(sb.ToString(), new SqlParameter[0]);
            }
        }

        private void CreateMetaDependencies()
        {
            StringBuilder sb;

            var metaDependencies = _unitOfWork.Repository<MetaDependency>().Queryable().OrderBy(md => md.ParentTable.Id).ToList();
            foreach (MetaDependency metaDependency in metaDependencies)
            {
                sb = new StringBuilder();

                sb.AppendFormat("ALTER TABLE [Meta{0}] WITH NOCHECK ADD  CONSTRAINT [FK_Meta{0}_Meta{1}] FOREIGN KEY([{2}]) REFERENCES [Meta{1}] ([{3}])", metaDependency.ReferenceTable.TableName, metaDependency.ParentTable.TableName, metaDependency.ReferenceColumnName, metaDependency.ParentColumnName);

                _unitOfWork.Repository<Patient>().ExecuteSqlCommand(sb.ToString(), new SqlParameter[0]);
            }
        }

        private void PopulateMetaTables()
        {
            ProcessInsertEntity(typeof(Patient), "Patient");
            ProcessInsertEntity(typeof(PatientMedication), "PatientMedication");
            ProcessInsertEntity(typeof(PatientClinicalEvent), "PatientClinicalEvent");
            ProcessInsertEntity(typeof(PatientCondition), "PatientCondition");
            ProcessInsertEntity(typeof(PatientLabTest), "PatientLabTest");
            ProcessInsertEntity(typeof(Encounter), "Encounter");
            ProcessInsertEntity(typeof(CohortGroupEnrolment), "CohortGroupEnrolment");
            ProcessInsertEntity(typeof(PatientFacility), "PatientFacility");
        }

        private void UpdateCustomAttributes()
        {
            ProcessUpdateAttribute("Patient", "mp");
            ProcessUpdateAttribute("PatientMedication", "mpm");
            ProcessUpdateAttribute("PatientClinicalEvent", "mpce");
            ProcessUpdateAttribute("PatientCondition", "mpc");
            ProcessUpdateAttribute("PatientLabTest", "mplt");

            ProcessUpdateSelection("Patient", "mp");
            ProcessUpdateSelection("PatientMedication", "mpm");
            ProcessUpdateSelection("PatientClinicalEvent", "mpce");
            ProcessUpdateSelection("PatientCondition", "mpc");
            ProcessUpdateSelection("PatientLabTest", "mplt");
        }

        private void ProcessInsertEntity(Type type, string entityName)
        {
            PropertyInfo[] properties = type.GetProperties();
            var invalidProperties = new[] { "CustomAttributesXmlSerialised", "Archived", "ArchivedReason", "ArchivedDate", "AuditUser", "Age", "FullName", "AgeGroup", "DisplayName", "CurrentFacilityName", "CurrentFacilityCode", "CurrentFacilityOrganisationUnit", "LatestEncounterDate", "CreatedById" , "ConceptId", "PatientId", "LabTestId", "PriorityId", "EncounterTypeId", "CohortGroupId", "FacilityId" };

            var metaTable = _metaTableRepository.Get(mt => mt.TableName == entityName);

            StringBuilder sbMain = new StringBuilder(); ;
            sbMain.AppendFormat("INSERT INTO [Meta{0}] (", metaTable.TableName);
            StringBuilder sbJoins = new StringBuilder(); ;
            StringBuilder sbIntoFields = new StringBuilder(); ;
            StringBuilder sbSelectFields = new StringBuilder(); ;

            var userCount = 0;
            var terminologyCount = 0;

            foreach (PropertyInfo property in properties)
            {
                if (!invalidProperties.Contains(property.Name))
                {
                    var columnName = property.Name;
                    if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter))
                    {
                        columnName = property.Name + "_Id";
                    };

                    if (property.PropertyType == typeof(EncounterType))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("et.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [EncounterType] et ON tbl.{0}_Id = et.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(Facility))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("f.[FacilityName], ");
                        sbJoins.AppendFormat("LEFT JOIN [Facility] f ON tbl.{0}_Id = f.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(CohortGroup))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("cg.[CohortName], ");
                        sbJoins.AppendFormat("LEFT JOIN [CohortGroup] cg ON tbl.{0}_Id = cg.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(LabTestUnit))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("ltu.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [LabTestUnit] ltu ON tbl.{0}_Id = ltu.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(LabTest))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("lt.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [LabTest] lt ON tbl.{0}_Id = lt.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(Outcome))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("o.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [Outcome] o ON tbl.{0}_Id = o.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(TerminologyMedDra))
                    {
                        terminologyCount += 1;
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("tm{0}.MedDraTerm, ", terminologyCount.ToString());
                        sbJoins.AppendFormat("LEFT JOIN [TerminologyMedDra] tm{1} ON tbl.{0}_Id = tm{1}.Id ", columnName, terminologyCount.ToString());
                    }
                    if (property.PropertyType == typeof(User))
                    {
                        userCount += 1;
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("u{0}.UserName, ", userCount.ToString());
                        sbJoins.AppendFormat("LEFT JOIN [User] u{1} ON tbl.{0}_Id = u{1}.Id ", columnName, userCount.ToString());
                    }
                    if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter) || property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(string) || property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(Guid) || property.PropertyType == typeof(bool))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("tbl.[{0}], ", columnName);
                    };
                }
            }
            if (sbIntoFields.Length > 0)
            {
                sbIntoFields.Remove(sbIntoFields.Length - 2, 2);
            }
            if (sbSelectFields.Length > 0)
            {
                sbSelectFields.Remove(sbSelectFields.Length - 2, 2);
            }

            sbMain.AppendFormat("{0}) SELECT {1} FROM {2} tbl {3} WHERE Archived = 0", sbIntoFields.ToString(), sbSelectFields.ToString(), metaTable.TableName, sbJoins.ToString());
            _unitOfWork.Repository<Patient>().ExecuteSqlCommand(sbMain.ToString(), new SqlParameter[0]);
        }

        private void ProcessUpdateAttribute(string entityName, string alias)
        {
            StringBuilder sbMain = new StringBuilder();

            var attributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable()
                .Where(c => c.ExtendableTypeName == entityName)
                .OrderBy(c => c.Id).ToList();
            var attributeType = "";

            foreach (CustomAttributeConfiguration attribute in attributes)
            {
                sbMain.Clear();

                switch (attribute.CustomAttributeType)
                {
                    case CustomAttributeType.Numeric:
                    case CustomAttributeType.String:
                        attributeType = "CustomStringAttribute";
                        break;

                    case CustomAttributeType.Selection:
                    case CustomAttributeType.DateTime:
                        attributeType = "CustomSelectionAttribute";
                        break;
                }

                if (!String.IsNullOrWhiteSpace(attributeType))
                {
                    sbMain.AppendFormat(@"UPDATE {0} SET {0}.[{1}] = CustomAttributesXmlSerialised.value('(/CustomAttributeSet/{2}[Key=""{3}"" ]/Value)[1]', 'nvarchar(max)') from {4} as h inner join Meta{4} as {0} on h.Id = {0}.Id", alias, attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""), attributeType, attribute.AttributeKey.Replace("&", "&amp;"), entityName);
                    _unitOfWork.Repository<Patient>().ExecuteSqlCommand(sbMain.ToString(), new SqlParameter[0]);
                }
            }
        }

        private void ProcessUpdateSelection(string entityName, string alias)
        {
            StringBuilder sbMain = new StringBuilder();

            var attributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable()
                .Where(c => c.ExtendableTypeName == entityName)
                .OrderBy(c => c.Id).ToList();

            foreach (CustomAttributeConfiguration attribute in attributes.Where(ca => ca.CustomAttributeType == CustomAttributeType.Selection))
            {
                sbMain.Clear();
                sbMain.AppendFormat(@"UPDATE {0} SET {0}.[{1}] = sdi.Value from Meta{2} as {0} inner join SelectionDataItem as sdi on sdi.AttributeKey = '{3}' and SelectionKey collate Latin1_General_CI_AS = {0}.[{1}] collate Latin1_General_CI_AS", alias, attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""), entityName, attribute.AttributeKey);
                _unitOfWork.Repository<Patient>().ExecuteSqlCommand(sbMain.ToString(), new SqlParameter[0]);
            }
        }

        #endregion

    }
}
