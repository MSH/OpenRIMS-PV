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
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/datasetelements")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class DatasetElementsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IRepositoryInt<FieldType> _fieldTypeRepository;
        private readonly IRepositoryInt<DatasetElementType> _datasetElementTypeRepository;
        private readonly IRepositoryInt<DatasetRule> _datasetRuleRepository;
        private readonly IRepositoryInt<Field> _fieldRepository;
        private readonly IRepositoryInt<FieldValue> _fieldValueRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public DatasetElementsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<DatasetElement> datasetElementRepository,
            IRepositoryInt<FieldType> fieldTypeRepository,
            IRepositoryInt<DatasetElementType> datasetElementTypeRepository,
            IRepositoryInt<DatasetRule> datasetRuleRepository,
            IRepositoryInt<Field> fieldRepository,
            IRepositoryInt<FieldValue> fieldValueRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _datasetRuleRepository = datasetRuleRepository ?? throw new ArgumentNullException(nameof(datasetRuleRepository));
            _fieldTypeRepository = fieldTypeRepository ?? throw new ArgumentNullException(nameof(fieldTypeRepository));
            _datasetElementTypeRepository = datasetElementTypeRepository ?? throw new ArgumentNullException(nameof(datasetElementTypeRepository));
            _fieldRepository = fieldRepository ?? throw new ArgumentNullException(nameof(fieldRepository));
            _fieldValueRepository = fieldValueRepository ?? throw new ArgumentNullException(nameof(fieldValueRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all dataset elements using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetElementIdentifierDto</returns>
        [HttpGet(Name = "GetDatasetElementsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<DatasetElementIdentifierDto>> GetDatasetElementsByIdentifier(
            [FromQuery] DatasetElementResourceParameters datasetElementResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetElementIdentifierDto>
                (datasetElementResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedDatasetElementsWithLinks = GetDatasetElements<DatasetElementIdentifierDto>(datasetElementResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetElementIdentifierDto>(mappedDatasetElementsWithLinks.TotalCount, mappedDatasetElementsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetElementResourceParameters,
            //    mappedDatasetElementsWithLinks.HasNext, mappedDatasetElementsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all dataset elements using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetElementDetailDto</returns>
        [HttpGet(Name = "GetDatasetElementsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<DatasetElementDetailDto>> GetDatasetElementsByDetail(
            [FromQuery] DatasetElementResourceParameters datasetElementResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetElementDetailDto>
                (datasetElementResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedDatasetElementsWithLinks = GetDatasetElements<DatasetElementDetailDto>(datasetElementResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetElementIdentifierDto>(mappedDatasetElementsWithLinks.TotalCount, mappedDatasetElementsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetElementResourceParameters,
            //    mappedDatasetElementsWithLinks.HasNext, mappedDatasetElementsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get group values for a dataset element
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of GroupValueDto</returns>
        [HttpGet("{id}", Name = "GetGroupValues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.groupvalue.v1+json", "application/vnd.pvims.groupvalue.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.groupvalue.v1+json", "application/vnd.pvims.groupvalue.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<GroupValueDto>>> GetGroupValues(long id)
        {
            var datasetElementFromRepo = await _datasetElementRepository.GetAsync(f => f.Id == id);
            if (datasetElementFromRepo == null)
            {
                return NotFound();
            }

            var instanceValues = _unitOfWork.Repository<DatasetInstanceValue>()
                .Queryable()
                .Where(div => div.DatasetElement.Id == datasetElementFromRepo.Id)
                .GroupBy(val => val.InstanceValue)
                .Select(group => new GroupValueDto() { GroupValue = group.Key, Count = group.Count() })
                .OrderBy(x => x.GroupValue)
                .ToList();

            return Ok(instanceValues);
        }

        /// <summary>
        /// Get a single dataset element using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the dataset element</param>
        /// <returns>An ActionResult of type DatasetElementIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetDatasetElementByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetElementIdentifierDto>> GetDatasetElementByIdentifier(long id)
        {
            var mappedDatasetElement = await GetDatasetElementAsync<DatasetElementIdentifierDto>(id);
            if (mappedDatasetElement == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetElement<DatasetElementIdentifierDto>(mappedDatasetElement));
        }

        /// <summary>
        /// Get a single dataset element using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique detail for the dataset element</param>
        /// <returns>An ActionResult of type DatasetElementDetailDto</returns>
        [HttpGet("{id}", Name = "GetDatasetElementByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetElementDetailDto>> GetDatasetElementByDetail(long id)
        {
            var mappedDatasetElement = await GetDatasetElementAsync<DatasetElementDetailDto>(id);
            if (mappedDatasetElement == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetElement<DatasetElementDetailDto>(mappedDatasetElement));
        }

        /// <summary>
        /// Get a single dataset element using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique detail for the dataset element</param>
        /// <returns>An ActionResult of type DatasetElementExpandedDto</returns>
        [HttpGet("{id}", Name = "GetDatasetElementByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetElementExpandedDto>> GetDatasetElementByExpanded(long id)
        {
            var mappedDatasetElement = await GetDatasetElementAsync<DatasetElementExpandedDto>(id);
            if (mappedDatasetElement == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetElement<DatasetElementExpandedDto>(CustomElementMap(mappedDatasetElement)));
        }

        /// <summary>
        /// Create a new dataset element
        /// </summary>
        /// <param name="datasetElementForUpdate">The dataset element payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateDatasetElement")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDatasetElement(
            [FromBody] DatasetElementForUpdateDto datasetElementForUpdate)
        {
            if (datasetElementForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to load payload for new request");
            }

            if (Regex.Matches(datasetElementForUpdate.ElementName, @"[a-zA-Z() ']").Count < datasetElementForUpdate.ElementName.Length)
            {
                ModelState.AddModelError("Message", "Element contains invalid characters (Enter A-Z, a-z, open and Close brackets)");
            }

            if (Regex.Matches(datasetElementForUpdate.OID, @"[-a-zA-Z0-9 ']").Count < datasetElementForUpdate.OID.Length)
            {
                ModelState.AddModelError("Message", "OID contains invalid characters (Enter A-Z, a-z, 0-9, hyphen)");
            }

            if (Regex.Matches(datasetElementForUpdate.DefaultValue, @"[-a-zA-Z0-9 ']").Count < datasetElementForUpdate.DefaultValue.Length)
            {
                ModelState.AddModelError("Message", "Default value contains invalid characters (Enter A-Z, a-z, 0-9, hyphen)");
            }

            if (_unitOfWork.Repository<DatasetElement>().Queryable().
                Where(l => l.ElementName == datasetElementForUpdate.ElementName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var fieldType = await _fieldTypeRepository.GetAsync(ft => ft.Description == datasetElementForUpdate.FieldTypeName.ToString());
            if (fieldType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate field type");
            }
            var elementType = await _datasetElementTypeRepository.GetAsync(ft => ft.Description == "Generic");
            if (elementType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate element type");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newDatasetElement = new DatasetElement()
                {
                    DatasetElementType = elementType,
                    ElementName = datasetElementForUpdate.ElementName,
                    Oid = datasetElementForUpdate.OID,
                    DefaultValue = datasetElementForUpdate.DefaultValue,
                    Field = new Field()
                    {
                        Anonymise = (datasetElementForUpdate.Anonymise == Models.ValueTypes.YesNoValueType.Yes),
                        Mandatory = (datasetElementForUpdate.Mandatory == Models.ValueTypes.YesNoValueType.Yes),
                        FieldType = fieldType,
                        MaxLength = datasetElementForUpdate.FieldTypeName == FieldTypes.AlphaNumericTextbox ? datasetElementForUpdate.MaxLength : (short?)null,
                        Decimals = datasetElementForUpdate.FieldTypeName == FieldTypes.NumericTextbox ? datasetElementForUpdate.Decimals : (short?)null,
                        MinSize = datasetElementForUpdate.FieldTypeName == FieldTypes.NumericTextbox ? datasetElementForUpdate.MinSize : (decimal?)null,
                        MaxSize = datasetElementForUpdate.FieldTypeName == FieldTypes.NumericTextbox ? datasetElementForUpdate.MaxSize : (decimal?)null
                    },
                    System = (datasetElementForUpdate.System == Models.ValueTypes.YesNoValueType.Yes)
                };

                var rule = newDatasetElement.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
                rule.RuleActive = (datasetElementForUpdate.SingleDatasetRule == Models.ValueTypes.YesNoValueType.Yes);

                _datasetElementRepository.Save(newDatasetElement);
                id = newDatasetElement.Id;

                var mappedDatasetElement = await GetDatasetElementAsync<DatasetElementIdentifierDto>(id);
                if (mappedDatasetElement == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetDatasetElementByIdentifier",
                    new
                    {
                        id = mappedDatasetElement.Id
                    }, CreateLinksForDatasetElement<DatasetElementIdentifierDto>(mappedDatasetElement));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing dataset element
        /// </summary>
        /// <param name="id">The unique id of the dataset element</param>
        /// <param name="datasetElementForUpdate">The dataset element payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateDatasetElement")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDatasetElement(long id,
            [FromBody] DatasetElementForUpdateDto datasetElementForUpdate)
        {
            if (datasetElementForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(datasetElementForUpdate.ElementName, @"[a-zA-Z() ']").Count < datasetElementForUpdate.ElementName.Length)
            {
                ModelState.AddModelError("Message", "Element contains invalid characters (Enter A-Z, a-z, open and Close brackets)");
                return BadRequest(ModelState);
            }


            if(!String.IsNullOrWhiteSpace(datasetElementForUpdate.OID))
            {
                if (Regex.Matches(datasetElementForUpdate.OID, @"[-a-zA-Z0-9 ']").Count < datasetElementForUpdate.OID.Length)
                {
                    ModelState.AddModelError("Message", "OID contains invalid characters (Enter A-Z, a-z, 0-9, hyphen)");
                    return BadRequest(ModelState);
                }
            }

            if (!String.IsNullOrWhiteSpace(datasetElementForUpdate.DefaultValue))
            {
                if (Regex.Matches(datasetElementForUpdate.DefaultValue, @"[-a-zA-Z0-9 ']").Count < datasetElementForUpdate.DefaultValue.Length)
                {
                    ModelState.AddModelError("Message", "Default value contains invalid characters (Enter A-Z, a-z, 0-9, hyphen)");
                    return BadRequest(ModelState);
                }
            }

            if (_unitOfWork.Repository<DatasetElement>().Queryable().
                Where(l => l.ElementName == datasetElementForUpdate.ElementName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            var fieldType = await _fieldTypeRepository.GetAsync(ft => ft.Description == datasetElementForUpdate.FieldTypeName.ToString());
            if (fieldType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate field type");
                return BadRequest(ModelState);
            }

            var datasetElementFromRepo = await _datasetElementRepository.GetAsync(f => f.Id == id, new string[] { "Field.FieldType" });
            if (datasetElementFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                datasetElementFromRepo.ElementName = datasetElementForUpdate.ElementName;
                datasetElementFromRepo.Oid = datasetElementForUpdate.OID;
                datasetElementFromRepo.DefaultValue = datasetElementForUpdate.DefaultValue;
                datasetElementFromRepo.System = (datasetElementForUpdate.System == Models.ValueTypes.YesNoValueType.Yes);
                datasetElementFromRepo.Field.Mandatory = (datasetElementForUpdate.Mandatory == Models.ValueTypes.YesNoValueType.Yes);
                datasetElementFromRepo.Field.Anonymise = (datasetElementForUpdate.Anonymise == Models.ValueTypes.YesNoValueType.Yes);
                datasetElementFromRepo.Field.MaxLength = datasetElementForUpdate.FieldTypeName == FieldTypes.AlphaNumericTextbox ? datasetElementForUpdate.MaxLength : (short?)null;
                datasetElementFromRepo.Field.Decimals = datasetElementForUpdate.FieldTypeName == FieldTypes.NumericTextbox ? datasetElementForUpdate.Decimals : (short?)null;
                datasetElementFromRepo.Field.MinSize = datasetElementForUpdate.FieldTypeName == FieldTypes.NumericTextbox ? datasetElementForUpdate.MinSize : (decimal?)null;
                datasetElementFromRepo.Field.MaxSize = datasetElementForUpdate.FieldTypeName == FieldTypes.NumericTextbox ? datasetElementForUpdate.MaxSize : (decimal?)null;

                var rule = datasetElementFromRepo.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
                rule.RuleActive = datasetElementForUpdate.SingleDatasetRule == Models.ValueTypes.YesNoValueType.Yes;

                _datasetElementRepository.Update(datasetElementFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing dataset element
        /// </summary>
        /// <param name="id">The unique id of the dataset element</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteDatasetElement")]
        public async Task<IActionResult> DeleteDatasetElement(long id)
        {
            var datasetElementFromRepo = await _datasetElementRepository.GetAsync(f => f.Id == id, new string[] { 
                "DatasetCategoryElements", 
                "DatasetElementSubs", 
                "Field.FieldValues" 
            });
            if (datasetElementFromRepo == null)
            {
                return NotFound();
            }

            if (datasetElementFromRepo.DatasetCategoryElements.Count > 0 ||
                datasetElementFromRepo.DatasetElementSubs.Count > 0 ||
                _unitOfWork.Repository<DatasetInstanceValue>()
                    .Queryable()
                    .Where(div => div.DatasetElement.Id == datasetElementFromRepo.Id)
                    .Any())
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                ICollection<FieldValue> deleteFieldValues = new Collection<FieldValue>();
                foreach(var fieldValue in datasetElementFromRepo.Field.FieldValues)
                {
                    deleteFieldValues.Add(fieldValue);
                }
                foreach (var fieldValue in deleteFieldValues)
                {
                    _fieldValueRepository.Delete(fieldValue);
                }

                ICollection<DatasetRule> deleteDatasetRules = new Collection<DatasetRule>();
                foreach (var datasetRule in datasetElementFromRepo.DatasetRules)
                {
                    deleteDatasetRules.Add(datasetRule);
                }
                foreach (var datasetRule in deleteDatasetRules)
                {
                    _datasetRuleRepository.Delete(datasetRule);
                }

                _datasetElementRepository.Delete(datasetElementFromRepo);
                _fieldRepository.Delete(datasetElementFromRepo.Field);

                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Get dataset elements from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetElementResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetDatasetElements<T>(DatasetElementResourceParameters datasetElementResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = datasetElementResourceParameters.PageNumber,
                PageSize = datasetElementResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<DatasetElement>(datasetElementResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<DatasetElement>(true);

            if (!String.IsNullOrWhiteSpace(datasetElementResourceParameters.ElementName))
            {
                predicate = predicate.And(f => f.ElementName.Contains(datasetElementResourceParameters.ElementName));
            }

            var pagedDatasetElementsFromRepo = _datasetElementRepository.List(pagingInfo, predicate, orderby, new string[] { 
                "Field.FieldType"
            });
            if (pagedDatasetElementsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetElements = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedDatasetElementsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDatasetElementsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedDatasetElements.TotalCount,
                    pageSize = mappedDatasetElements.PageSize,
                    currentPage = mappedDatasetElements.CurrentPage,
                    totalPages = mappedDatasetElements.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedDatasetElements.ForEach(dto => CreateLinksForDatasetElement(dto));

                return mappedDatasetElements;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset element from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetElementAsync<T>(long id) where T : class
        {
            var datasetElementFromRepo = await _datasetElementRepository.GetAsync(f => f.Id == id, new string[] { "Field.FieldType" });

            if (datasetElementFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetElement = _mapper.Map<T>(datasetElementFromRepo);

                return mappedDatasetElement;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetElementIdentifierDto CreateLinksForDatasetElement<T>(T dto)
        {
            DatasetElementIdentifierDto identifier = (DatasetElementIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("DatasetElement", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetElementExpandedDto CustomElementMap(DatasetElementExpandedDto dto)
        {
            var datasetElement = _datasetElementRepository.Get(p => p.Id == dto.Id);
            if (datasetElement == null)
            {
                return dto;
            }

            var rule = datasetElement.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
            dto.SingleDatasetRule = rule.RuleActive ? "Yes" : "No";

            return dto;
        }
    }
}
