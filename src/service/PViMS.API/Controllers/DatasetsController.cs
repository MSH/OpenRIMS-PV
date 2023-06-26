using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Application.Queries.DatasetAggregate;
using PVIMS.Core.Aggregates.DatasetAggregate;
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/datasets")]
    public class DatasetsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetCategory> _datasetCategoryRepository;
        private readonly IRepositoryInt<DatasetCategoryElement> _datasetCategoryElementRepository;
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IRepositoryInt<DatasetElementSub> _datasetElementSubRepository;
        private readonly IRepositoryInt<ContextType> _contextTypeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IWorkFlowService _workflowService;
        private readonly ILogger<DatasetsController> _logger;

        public DatasetsController(
            IMediator mediator, 
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetCategory> datasetCategoryRepository,
            IRepositoryInt<DatasetCategoryElement> datasetCategoryElementRepository,
            IRepositoryInt<DatasetElement> datasetElementRepository,
            IRepositoryInt<DatasetElementSub> datasetElementSubRepository,
            IRepositoryInt<ContextType> contextTypeRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workflowService,
            ILogger<DatasetsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetCategoryRepository = datasetCategoryRepository ?? throw new ArgumentNullException(nameof(datasetCategoryRepository));
            _datasetCategoryElementRepository = datasetCategoryElementRepository ?? throw new ArgumentNullException(nameof(datasetCategoryElementRepository));
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _datasetElementSubRepository = datasetElementSubRepository ?? throw new ArgumentNullException(nameof(datasetElementSubRepository));
            _contextTypeRepository = contextTypeRepository ?? throw new ArgumentNullException(nameof(contextTypeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workflowService = workflowService ?? throw new ArgumentNullException(nameof(workflowService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all datasets using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetIdentifierDto</returns>
        [HttpGet(Name = "GetDatasetsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<DatasetIdentifierDto>> GetDatasetsByIdentifier(
            [FromQuery] IdResourceParameters datasetResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetIdentifierDto>
                (datasetResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedDatasetsWithLinks = GetDatasets<DatasetIdentifierDto>(datasetResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetIdentifierDto>(mappedDatasetsWithLinks.TotalCount, mappedDatasetsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all datasets using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetDetailDto</returns>
        [HttpGet(Name = "GetDatasetsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<DatasetDetailDto>> GetDatasetsByDetail(
            [FromQuery] IdResourceParameters datasetResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetDetailDto>
                (datasetResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedDatasetsWithLinks = GetDatasets<DatasetDetailDto>(datasetResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetDetailDto>(mappedDatasetsWithLinks.TotalCount, mappedDatasetsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all dataset categories using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetCategoryDetailDto</returns>
        [HttpGet("{datasetId}/categories", Name = "GetDatasetCategoriesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<DatasetCategoryDetailDto>>> GetDatasetCategoriesByDetail(long datasetId,
            [FromQuery] IdResourceParameters datasetCategoryResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetCategoryDetailDto>
                (datasetCategoryResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            var mappedDatasetCategoriesWithLinks = GetDatasetCategories<DatasetCategoryDetailDto>(datasetId, datasetCategoryResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetCategoryDetailDto>(mappedDatasetCategoriesWithLinks.TotalCount, mappedDatasetCategoriesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all dataset category elements using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetCategoryElementDetailDto</returns>
        [HttpGet("{datasetId}/categories/{id}/elements", Name = "GetDatasetCategoryElementsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<DatasetCategoryElementDetailDto>>> GetDatasetCategoryElementsByDetail(long datasetId, long id, 
            [FromQuery] IdResourceParameters datasetCategoryElementResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetCategoryElementDetailDto>
                (datasetCategoryElementResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var datasetCategoryFromRepo = await _datasetCategoryRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == id);
            if (datasetCategoryFromRepo == null)
            {
                return NotFound();
            }

            var mappedDatasetCategoryElementsWithLinks = GetDatasetCategoryElements<DatasetCategoryElementDetailDto>(datasetId, id, datasetCategoryElementResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetCategoryElementDetailDto>(mappedDatasetCategoryElementsWithLinks.TotalCount, mappedDatasetCategoryElementsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single dataset using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the dataset</param>
        /// <returns>An ActionResult of type DatasetIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetDatasetByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetIdentifierDto>> GetDatasetByIdentifier(long id)
        {
            var mappedDataset = await GetDatasetAsync<DatasetIdentifierDto>(id);
            if (mappedDataset == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDataset<DatasetIdentifierDto>(mappedDataset));
        }

        /// <summary>
        /// Get a single dataset using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the dataset</param>
        /// <returns>An ActionResult of type DatasetDetailDto</returns>
        [HttpGet("{id}", Name = "GetDatasetByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetDetailDto>> GetDatasetByDetail(long id)
        {
            var mappedDataset = await GetDatasetAsync<DatasetDetailDto>(id);
            if (mappedDataset == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDataset<DatasetDetailDto>(mappedDataset));
        }

        /// <summary>
        /// Get a single dataset using it's unique id and valid media type 
        /// </summary>
        /// <returns>An ActionResult of type DatasetForSpontaneousDto</returns>
        [HttpGet(Name = "GetSpontaneousDataset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.spontaneousdataset.v1+json", "application/vnd.pvims.spontaneousdataset.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.spontaneousdataset.v1+json", "application/vnd.pvims.spontaneousdataset.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetForSpontaneousDto>> GetSpontaneousDataset()
        {
            var mappedDataset = await GetSpontaneousDatasetAsync<DatasetForSpontaneousDto>();
            if (mappedDataset == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDataset<DatasetForSpontaneousDto>(CustomDatasetMap(mappedDataset)));
        }

        /// <summary>
        /// Get a single dataset category using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the dataset category</param>
        /// <returns>An ActionResult of type DatasetCategoryIdentifierDto</returns>
        [HttpGet("{datasetId}/categories/{id}", Name = "GetDatasetCategoryByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetCategoryIdentifierDto>> GetDatasetCategoryByIdentifier(long datasetId, long id)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            var mappedDatasetCategory = await GetDatasetCategoryAsync<DatasetCategoryIdentifierDto>(datasetId, id);
            if (mappedDatasetCategory == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetCategory<DatasetCategoryIdentifierDto>(datasetId, mappedDatasetCategory));
        }

        /// <summary>
        /// Get a single dataset category element using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="datasetCategoryId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the dataset category</param>
        /// <returns>An ActionResult of type DatasetCategoryElementIdentifierDto</returns>
        [HttpGet("{datasetId}/categories/{datasetCategoryId}/elements/{id}", Name = "GetDatasetCategoryElementByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetCategoryElementIdentifierDto>> GetDatasetCategoryElementByIdentifier(long datasetId, long datasetCategoryId, long id)
        {
            var datasetCategoryFromRepo = await _datasetCategoryElementRepository.GetAsync(f => f.DatasetCategory.Dataset.Id == datasetId && f.DatasetCategory.Id == datasetCategoryId && f.Id == id);
            if (datasetCategoryFromRepo == null)
            {
                return NotFound();
            }

            var mappedDatasetCategoryElement = await GetDatasetCategoryElementAsync<DatasetCategoryElementIdentifierDto>(datasetId, datasetCategoryId, id);
            if (mappedDatasetCategoryElement == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetCategoryElement<DatasetCategoryElementIdentifierDto>(datasetId, datasetCategoryId, mappedDatasetCategoryElement));
        }

        /// <summary>
        /// Get a single dataset instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the instance</param>
        /// <returns>An ActionResult of type DatasetInstanceIdentifierDto</returns>
        [HttpGet("{datasetId}/instances/{id}", Name = "GetDatasetInstanceByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetInstanceIdentifierDto>> GetDatasetInstanceByIdentifier(int datasetId, int id)
        {
            var query = new DatasetInstanceIdentifierQuery(datasetId, id);

            _logger.LogInformation(
                "----- Sending query: DatasetInstanceIdentifierQuery - {datasetId}: {id}",
                datasetId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single dataset instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the instance</param>
        /// <returns>An ActionResult of type DatasetInstanceDetailDto</returns>
        [HttpGet("{datasetId}/instances/{id}", Name = "GetDatasetInstanceByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetInstanceDetailDto>> GetDatasetInstanceByDetail(int datasetId, int id)
        {
            var query = new DatasetInstanceDetailQuery(datasetId, id);

            _logger.LogInformation(
                "----- Sending query: DatasetInstanceDetailQuery - {datasetId}: {id}",
                datasetId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new dataset
        /// </summary>
        /// <param name="datasetForUpdate">The dataset payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateDataset")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDataset(
            [FromBody] DatasetForUpdateDto datasetForUpdate)
        {
            if (datasetForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetForUpdate.DatasetName, @"[a-zA-Z ']").Count < datasetForUpdate.DatasetName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(datasetForUpdate.Help))
            {
                if (Regex.Matches(datasetForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            var contextType = await _contextTypeRepository.GetAsync(ct => ct.Description == "Encounter");
            if (contextType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate context type");
            }

            if (_unitOfWork.Repository<Dataset>().Queryable().
                Where(l => l.DatasetName == datasetForUpdate.DatasetName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newDataset = new Dataset(datasetForUpdate.DatasetName, contextType, "", "", datasetForUpdate.Help, "");
                await _datasetRepository.SaveAsync(newDataset);
                id = newDataset.Id;

                var mappedDataset = await GetDatasetAsync<DatasetIdentifierDto>(id);
                if (mappedDataset == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetDatasetByIdentifier",
                    new
                    {
                        id = mappedDataset.Id
                    }, CreateLinksForDataset<DatasetIdentifierDto>(mappedDataset));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new dataset category
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="datasetCategoryForUpdate">The dataset category payload</param>
        /// <returns></returns>
        [HttpPost("{datasetId}/categories", Name = "CreateDatasetCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDatasetCategory(long datasetId, 
            [FromBody] DatasetCategoryForUpdateDto datasetCategoryForUpdate)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            if (datasetCategoryForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetCategoryForUpdate.DatasetCategoryName, @"[a-zA-Z ']").Count < datasetCategoryForUpdate.DatasetCategoryName.Length)
            {
                ModelState.AddModelError("Message", "Category name contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryForUpdate.FriendlyName))
            {
                if (Regex.Matches(datasetCategoryForUpdate.FriendlyName, @"[a-zA-Z0-9. ']").Count < datasetCategoryForUpdate.FriendlyName.Length)
                {
                    ModelState.AddModelError("Message", "Friendly name contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryForUpdate.Help))
            {
                if (Regex.Matches(datasetCategoryForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetCategoryForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<DatasetCategory>().Queryable().
                Where(l => l.Dataset.Id == datasetId && l.DatasetCategoryName == datasetCategoryForUpdate.DatasetCategoryName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newDatasetCategory = new DatasetCategory()
                {
                    CategoryOrder = (short)GetNextCategoryOrder(datasetFromRepo),
                    Dataset = datasetFromRepo,
                    DatasetCategoryName = datasetCategoryForUpdate.DatasetCategoryName,
                    System = false,
                    Acute = (datasetCategoryForUpdate.Acute == Models.ValueTypes.YesNoValueType.Yes),
                    Chronic = (datasetCategoryForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes),
                    Public = false,
                    FriendlyName = datasetCategoryForUpdate.FriendlyName,
                    Help = datasetCategoryForUpdate.Help
                };

                _datasetCategoryRepository.Save(newDatasetCategory);
                id = newDatasetCategory.Id;

                var mappedDatasetCategory = await GetDatasetCategoryAsync<DatasetCategoryIdentifierDto>(datasetId, id);
                if (mappedDatasetCategory == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetDatasetCategoryByIdentifier",
                    new
                    {
                        datasetId,
                        id = mappedDatasetCategory.Id
                    }, CreateLinksForDatasetCategory<DatasetCategoryIdentifierDto>(datasetId, mappedDatasetCategory));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new dataset category element
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="datasetCategoryId">The unique identifier for the dataset category</param>
        /// <param name="datasetCategoryElementForUpdate">The dataset category element payload</param>
        /// <returns></returns>
        [HttpPost("{datasetId}/categories/{datasetCategoryId}/elements", Name = "CreateDatasetCategoryElement")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDatasetCategoryElement(long datasetId, long datasetCategoryId, 
            [FromBody] DatasetCategoryElementForUpdateDto datasetCategoryElementForUpdate)
        {
            var datasetCategoryFromRepo = await _datasetCategoryRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == datasetCategoryId);
            if (datasetCategoryFromRepo == null)
            {
                return NotFound();
            }

            var datasetElementFromRepo = await _datasetElementRepository.GetAsync(f => f.Id == datasetCategoryElementForUpdate.DatasetElementId);
            if (datasetElementFromRepo == null)
            {
                return NotFound();
            }

            if (datasetCategoryElementForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryElementForUpdate.FriendlyName))
            {
                if (Regex.Matches(datasetCategoryElementForUpdate.FriendlyName, @"[a-zA-Z0-9. ']").Count < datasetCategoryElementForUpdate.FriendlyName.Length)
                {
                    ModelState.AddModelError("Message", "Friendly name contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryElementForUpdate.Help))
            {
                if (Regex.Matches(datasetCategoryElementForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetCategoryElementForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<DatasetCategoryElement>().Queryable().
                Where(l => l.DatasetCategory.Id == datasetCategoryId && l.DatasetElement.Id == datasetCategoryElementForUpdate.DatasetElementId)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newDatasetCategoryElement = new DatasetCategoryElement()
                {
                    FieldOrder = (short)GetNextFieldOrder(datasetCategoryFromRepo),
                    DatasetCategory = datasetCategoryFromRepo,
                    DatasetElement = datasetElementFromRepo,
                    System = false,
                    Acute = (datasetCategoryElementForUpdate.Acute == Models.ValueTypes.YesNoValueType.Yes),
                    Chronic = (datasetCategoryElementForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes),
                    Public = false,
                    FriendlyName = datasetCategoryElementForUpdate.FriendlyName,
                    Help = datasetCategoryElementForUpdate.Help
                };

                _datasetCategoryElementRepository.Save(newDatasetCategoryElement);
                id = newDatasetCategoryElement.Id;

                var mappedDatasetCategoryElement = await GetDatasetCategoryElementAsync<DatasetCategoryElementIdentifierDto>(datasetId, datasetCategoryId, id);
                if (mappedDatasetCategoryElement == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetDatasetCategoryElementByIdentifier",
                    new
                    {
                        datasetId,
                        datasetCategoryId,
                        id = mappedDatasetCategoryElement.Id
                    }, CreateLinksForDatasetCategoryElement<DatasetCategoryElementIdentifierDto>(datasetId, datasetCategoryId, mappedDatasetCategoryElement));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing dataset
        /// </summary>
        /// <param name="id">The unique id of the dataset</param>
        /// <param name="datasetForUpdate">The dataset payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateDataset")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDataset(long id,
            [FromBody] DatasetForUpdateDto datasetForUpdate)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == id);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            if (datasetForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetForUpdate.DatasetName, @"[a-zA-Z ']").Count < datasetForUpdate.DatasetName.Length)
            {
                ModelState.AddModelError("Message", "Dataset name contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (!String.IsNullOrWhiteSpace(datasetForUpdate.Help))
            {
                if (Regex.Matches(datasetForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<Dataset>().Queryable().
                Where(l => l.DatasetName == datasetForUpdate.DatasetName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                datasetFromRepo.ChangeDatasetDetails(datasetForUpdate.DatasetName, datasetForUpdate.Help);
                _datasetRepository.Update(datasetFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }
            
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing dataset category
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique id of the dataset</param>
        /// <param name="datasetCategoryForUpdate">The dataset category payload</param>
        /// <returns></returns>
        [HttpPut("{datasetId}/categories/{id}", Name = "UpdateDatasetCategory")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDatasetCategory(long datasetId, long id,
            [FromBody] DatasetCategoryForUpdateDto datasetCategoryForUpdate)
        {
            var datasetCategoryFromRepo = await _datasetCategoryRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == id);
            if (datasetCategoryFromRepo == null)
            {
                return NotFound();
            }

            if (datasetCategoryForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetCategoryForUpdate.DatasetCategoryName, @"[a-zA-Z ']").Count < datasetCategoryForUpdate.DatasetCategoryName.Length)
            {
                ModelState.AddModelError("Message", "Category name contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryForUpdate.FriendlyName))
            {
                if (Regex.Matches(datasetCategoryForUpdate.FriendlyName, @"[a-zA-Z0-9. ']").Count < datasetCategoryForUpdate.FriendlyName.Length)
                {
                    ModelState.AddModelError("Message", "Friendly name contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryForUpdate.Help))
            {
                if (Regex.Matches(datasetCategoryForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetCategoryForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<DatasetCategory>().Queryable().
                Where(l => l.Dataset.Id == datasetId && l.DatasetCategoryName == datasetCategoryForUpdate.DatasetCategoryName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                datasetCategoryFromRepo.DatasetCategoryName = datasetCategoryForUpdate.DatasetCategoryName;
                datasetCategoryFromRepo.Acute = (datasetCategoryForUpdate.Acute == Models.ValueTypes.YesNoValueType.Yes);
                datasetCategoryFromRepo.Chronic = (datasetCategoryForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes);
                datasetCategoryFromRepo.FriendlyName = datasetCategoryForUpdate.FriendlyName;
                datasetCategoryFromRepo.Help = datasetCategoryForUpdate.Help;

                _datasetCategoryRepository.Update(datasetCategoryFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing dataset category element
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="datasetCategoryId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the dataset category</param>
        /// <param name="datasetCategoryElementForUpdate">The dataset category element payload</param>
        /// <returns></returns>
        [HttpPut("{datasetId}/categories/{datasetCategoryId}/elements/{id}", Name = "UpdateDatasetCategoryElement")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDatasetCategoryElement(long datasetId, long datasetCategoryId, long id, 
            [FromBody] DatasetCategoryElementForUpdateDto datasetCategoryElementForUpdate)
        {
            var datasetCategoryElementFromRepo = await _datasetCategoryElementRepository.GetAsync(f => f.DatasetCategory.Dataset.Id == datasetId 
                        && f.DatasetCategory.Id == datasetCategoryId 
                        && f.Id == id);
            if (datasetCategoryElementFromRepo == null)
            {
                return NotFound();
            }

            if (datasetCategoryElementForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryElementForUpdate.FriendlyName))
            {
                if (Regex.Matches(datasetCategoryElementForUpdate.FriendlyName, @"[a-zA-Z0-9. ']").Count < datasetCategoryElementForUpdate.FriendlyName.Length)
                {
                    ModelState.AddModelError("Message", "Friendly name contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryElementForUpdate.Help))
            {
                if (Regex.Matches(datasetCategoryElementForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetCategoryElementForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<DatasetCategoryElement>().Queryable().
                Where(l => l.DatasetCategory.Id == datasetCategoryId && l.DatasetElement.Id == datasetCategoryElementForUpdate.DatasetElementId && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                datasetCategoryElementFromRepo.Acute = (datasetCategoryElementForUpdate.Acute == Models.ValueTypes.YesNoValueType.Yes);
                datasetCategoryElementFromRepo.Chronic = (datasetCategoryElementForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes);
                datasetCategoryElementFromRepo.FriendlyName = datasetCategoryElementForUpdate.FriendlyName;
                datasetCategoryElementFromRepo.Help = datasetCategoryElementForUpdate.Help;

                _datasetCategoryElementRepository.Update(datasetCategoryElementFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing dataset
        /// </summary>
        /// <param name="id">The unique id of the dataset</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteDataset")]
        public async Task<IActionResult> DeleteDataset(long id)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == id);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            //if (datasetFromRepo.WorkPlanDatasets.Count > 0)
            //{
            //    ModelState.AddModelError("Message", "Unable to delete as item is in use.");
            //    return BadRequest(ModelState);
            //}

            if (ModelState.IsValid)
            {
                _datasetRepository.Delete(datasetFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an existing dataset category
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique id of the dataset category</param>
        /// <returns></returns>
        [HttpDelete("{datasetId}/categories/{id}", Name = "DeleteDatasetCategory")]
        public async Task<IActionResult> DeleteDatasetCategory(long datasetId, long id)
        {
            var datasetCategoryFromRepo = await _datasetCategoryRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == id);
            if (datasetCategoryFromRepo == null)
            {
                return NotFound();
            }

            if (datasetCategoryFromRepo.DatasetCategoryElements.Count > 0)
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                _datasetCategoryRepository.Delete(datasetCategoryFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an existing dataset category element
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="datasetCategoryId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the dataset category</param>
        /// <returns></returns>
        [HttpDelete("{datasetId}/categories/{datasetCategoryId}/elements/{id}", Name = "DeleteDatasetCategoryElement")]
        public async Task<IActionResult> DeleteDatasetCategoryElement(long datasetId, long datasetCategoryId, long id)
        {
            var datasetCategoryElementFromRepo = await _datasetCategoryElementRepository.GetAsync(f => f.DatasetCategory.Dataset.Id == datasetId 
                        && f.DatasetCategory.Id == datasetCategoryId 
                        && f.Id == id);
            if (datasetCategoryElementFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _datasetCategoryElementRepository.Delete(datasetCategoryElementFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new dataset instance
        /// </summary>
        /// <param name="datasetId">The unique identifier of the dataset that the instance is being created for</param>
        /// <param name="elementValues">The dataset instance payload</param>
        /// <returns></returns>
        [HttpPut("{datasetId}/instances", Name = "CreateDatasetInstance")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDatasetInstance(long datasetId, [FromBody] Object[] elementValues)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId,
                new string[] {
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.DatasetMappingValues",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.SubMappings.DestinationElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.SubMappings.SourceElement.Field.FieldType"
                });

            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            if (elementValues == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new form");
            }

            if(datasetFromRepo.DatasetName != "Spontaneous Report")
            {
                ModelState.AddModelError("Message", "Can only generate instance of type spontaneous report");
            }

            if (ModelState.IsValid)
            {
                // Prepare new dataset
                var datasetInstance = datasetFromRepo.CreateInstance(1, "", null, null, null);
                datasetInstance.ChangeStatusToComplete();

                // Update dataset instance values
                foreach (var elementValue in elementValues)
                {
                    if (elementValue is JObject)
                    {
                        JObject elementsJObject = JObject.FromObject(elementValue);

                        foreach (var elements in elementsJObject)
                        {
                            string name = elements.Key;
                            JToken value = elements.Value;

                            // Do not process elements with no values
                            if (!String.IsNullOrEmpty(value.ToString()))
                            {
                                var id = Convert.ToInt32(name);
                                var datasetElementFromRepo = _datasetElementRepository.Get(de => de.Id == id,
                                    new string[] {
                                    "Field.FieldType"
                                    });

                                if (datasetElementFromRepo != null)
                                {
                                    if (datasetElementFromRepo.Field.FieldType.Description == "Table")
                                    {
                                        //if (formValues is JArray)
                                        JArray tablesJArray = JArray.FromObject(value);
                                        foreach (JObject arrayContent in tablesJArray.Children<JObject>())
                                        {
                                            var context = Guid.NewGuid();

                                            foreach (var tableRowValue in arrayContent)
                                            {
                                                string subName = tableRowValue.Key;
                                                JToken subValue = tableRowValue.Value;

                                                var sid = Convert.ToInt32(subName);
                                                var datasetElementSubFromRepo = _datasetElementSubRepository.Get(des => des.Id == sid);
                                                try
                                                {
                                                    datasetInstance.SetInstanceSubValue(datasetElementSubFromRepo, subValue.ToString(), context);
                                                }
                                                catch (Exception ex)
                                                {
                                                    ModelState.AddModelError("Message", ex.Message);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            datasetInstance.SetInstanceValue(datasetElementFromRepo, value.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("Message", ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Repository<DatasetInstance>().SaveAsync(datasetInstance);

                    // Instantiate new instance of work flow
                    var patientIdentifier = datasetInstance.GetInstanceValue("Initials");
                    if (String.IsNullOrWhiteSpace(patientIdentifier))
                    {
                        patientIdentifier = datasetInstance.GetInstanceValue("Identification Number");
                    }
                    var sourceIdentifier = datasetInstance.GetInstanceValue("Description of reaction");
                    await _workflowService.CreateWorkFlowInstanceAsync("New Spontaneous Surveilliance Report", datasetInstance.DatasetInstanceGuid, patientIdentifier, sourceIdentifier, "PUBLIC");

                    // Prepare medications
                    List<ReportInstanceMedicationListItem> medications = new List<ReportInstanceMedicationListItem>();
                    var sourceProductElement = _datasetElementRepository.Get(u => u.ElementName == "Product Information");
                    var destinationProductElement = _datasetElementRepository.Get(u => u.ElementName == "Medicinal Products");
                    var sourceContexts = datasetInstance.GetInstanceSubValuesContext("Product Information");
                    foreach (Guid sourceContext in sourceContexts)
                    {
                        var drugItemValues = datasetInstance.GetInstanceSubValues("Product Information", sourceContext);
                        var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                        if (drugName != string.Empty)
                        {
                            var item = new ReportInstanceMedicationListItem()
                            {
                                MedicationIdentifier = drugName,
                                ReportInstanceMedicationGuid = sourceContext
                            };
                            medications.Add(item);
                        }
                    }
                    await _workflowService.AddOrUpdateMedicationsForWorkFlowInstanceAsync(datasetInstance.DatasetInstanceGuid, medications);

                    await _unitOfWork.CompleteAsync();

                    return Ok();
                }

                return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get datasets from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetDatasets<T>(IdResourceParameters datasetResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = datasetResourceParameters.PageNumber,
                PageSize = datasetResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Dataset>(datasetResourceParameters.OrderBy, "asc");

            var pagedDatasetsFromRepo = _datasetRepository.List(pagingInfo, null, orderby, "ContextType");
            if (pagedDatasetsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasets = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedDatasetsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDatasetsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedDatasets.TotalCount,
                    pageSize = mappedDatasets.PageSize,
                    currentPage = mappedDatasets.CurrentPage,
                    totalPages = mappedDatasets.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedDatasets.ForEach(dto => CreateLinksForDataset(dto));

                return mappedDatasets;
            }

            return null;
        }

        /// <summary>
        /// Get dataset categories from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="datasetCategoryResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetDatasetCategories<T>(long datasetId, IdResourceParameters datasetCategoryResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = datasetCategoryResourceParameters.PageNumber,
                PageSize = datasetCategoryResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<DatasetCategory>("CategoryOrder", "asc");

            var predicate = PredicateBuilder.New<DatasetCategory>(true);
            predicate = predicate.And(f => f.Dataset.Id == datasetId);

            var pagedDatasetCategoriesFromRepo = _datasetCategoryRepository.List(pagingInfo, predicate, orderby, new string[] { "DatasetCategoryElements" });
            if (pagedDatasetCategoriesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetCategories = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedDatasetCategoriesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDatasetCategoriesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedDatasetCategories.TotalCount,
                    pageSize = mappedDatasetCategories.PageSize,
                    currentPage = mappedDatasetCategories.CurrentPage,
                    totalPages = mappedDatasetCategories.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedDatasetCategories.ForEach(dto => CreateLinksForDatasetCategory(datasetId, dto));

                return mappedDatasetCategories;
            }

            return null;
        }

        /// <summary>
        /// Get dataset category elements from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="id">Unique id of the dataset category being queried for elements</param>
        /// <param name="datasetCategoryElementResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetDatasetCategoryElements<T>(long datasetId, long id, IdResourceParameters datasetCategoryElementResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = datasetCategoryElementResourceParameters.PageNumber,
                PageSize = datasetCategoryElementResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<DatasetCategoryElement>("FieldOrder", "asc");

            var predicate = PredicateBuilder.New<DatasetCategoryElement>(true);
            predicate = predicate.And(f => f.DatasetCategory.Dataset.Id == datasetId && f.DatasetCategory.Id == id);

            var pagedDatasetCategoryElementsFromRepo = _datasetCategoryElementRepository.List(pagingInfo, predicate, orderby, new string[] { "DatasetElement" } );
            if (pagedDatasetCategoryElementsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetCategoryElements = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedDatasetCategoryElementsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDatasetCategoryElementsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedDatasetCategoryElements.TotalCount,
                    pageSize = mappedDatasetCategoryElements.PageSize,
                    currentPage = mappedDatasetCategoryElements.CurrentPage,
                    totalPages = mappedDatasetCategoryElements.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedDatasetCategoryElements.ForEach(dto => CreateLinksForDatasetCategoryElement(datasetId, id, dto));

                return mappedDatasetCategoryElements;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetAsync<T>(long id) where T : class
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == id,
                new string[] {
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.DatasetMappingValues",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.SubMappings.DestinationElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.SubMappings.SourceElement.Field.FieldType"
                });

            if (datasetFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDataset = _mapper.Map<T>(datasetFromRepo);

                return mappedDataset;
            }

            return null;
        }

        /// <summary>
        /// Get spontaneous dataset from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <returns></returns>
        private async Task<T> GetSpontaneousDatasetAsync<T>() where T : class
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.DatasetName == "Spontaneous Report",
                new string[] {
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldValues",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldValues"
                });

            if (datasetFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDataset = _mapper.Map<T>(datasetFromRepo);

                return mappedDataset;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset category from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetCategoryAsync<T>(long datasetId, long id) where T : class
        {
            var datasetCategoryFromRepo = await _datasetCategoryRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == id);

            if (datasetCategoryFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetCategory = _mapper.Map<T>(datasetCategoryFromRepo);

                return mappedDatasetCategory;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset category element from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="datasetCategoryId">Unique id of the dataset being queried for categories</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetCategoryElementAsync<T>(long datasetId, long datasetCategoryId, long id) where T : class
        {
            var datasetCategoryElementFromRepo = await _datasetCategoryElementRepository.GetAsync(f => f.DatasetCategory.Dataset.Id == datasetId && f.DatasetCategory.Id == datasetCategoryId && f.Id == id);

            if (datasetCategoryElementFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetCategoryElement = _mapper.Map<T>(datasetCategoryElementFromRepo);

                return mappedDatasetCategoryElement;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetIdentifierDto CreateLinksForDataset<T>(T dto)
        {
            DatasetIdentifierDto identifier = (DatasetIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Dataset", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetCategoryIdentifierDto CreateLinksForDatasetCategory<T>(long datasetId, T dto)
        {
            DatasetCategoryIdentifierDto identifier = (DatasetCategoryIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateDatasetCategoryResourceUri(datasetId, identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="datasetCategoryId">Unique id of the dataset category being queried for elements</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetCategoryElementIdentifierDto CreateLinksForDatasetCategoryElement<T>(long datasetId, long datasetCategoryId, T dto)
        {
            DatasetCategoryElementIdentifierDto identifier = (DatasetCategoryElementIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateDatasetCategoryElementResourceUri(datasetId, datasetCategoryId, identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Get the next category order number
        /// </summary>
        /// <param name="dataset">The dataset that is being queried to determine the order number for the category</param>
        /// <returns></returns>
        private int GetNextCategoryOrder(Dataset dataset)
        {
            return dataset.DatasetCategories.Count() == 0 ? 1 : _unitOfWork.Repository<DatasetCategory>().Queryable().Where(dc => dc.Dataset.Id == dataset.Id).OrderByDescending(dc => dc.CategoryOrder).First().CategoryOrder + 1;
        }

        /// <summary>
        ///  Get the next element order number
        /// </summary>
        /// <param name="datasetCategory">The dataset category that is being queried to determine the order number for the element</param>
        /// <returns></returns>
        private int GetNextFieldOrder(DatasetCategory datasetCategory)
        {
            return datasetCategory.DatasetCategoryElements.Count == 0 ? 1 : _unitOfWork.Repository<DatasetCategoryElement>().Queryable().Where(dc => dc.DatasetCategory.Id == datasetCategory.Id).OrderByDescending(dc => dc.FieldOrder).First().FieldOrder + 1;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetForSpontaneousDto CustomDatasetMap(DatasetForSpontaneousDto dto)
        {
            var datasetFromRepo = _datasetRepository.Get(d => d.Id == dto.Id);
            if (datasetFromRepo == null)
            {
                return dto;
            }

            var groupedDatasetCategories = datasetFromRepo.DatasetCategories
                .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                .GroupBy(dce => dce.DatasetCategory)
                .ToList();

            dto.DatasetCategories = groupedDatasetCategories
                .Select(dsc => new DatasetCategoryViewDto
                {
                    DatasetCategoryId = dsc.Key.Id,
                    DatasetCategoryName = dsc.Key.DatasetCategoryName,
                    DatasetCategoryDisplayName = dsc.Key.FriendlyName ?? dsc.Key.DatasetCategoryName,
                    DatasetCategoryHelp = dsc.Key.Help,
                    DatasetCategoryDisplayed = true,
                    DatasetElements = dsc.Select(element => new DatasetElementViewDto
                    {
                        DatasetElementId = element.DatasetElement.Id,
                        DatasetElementName = element.DatasetElement.ElementName,
                        DatasetElementDisplayName = element.FriendlyName ?? element.DatasetElement.ElementName,
                        DatasetElementHelp = element.Help,
                        DatasetElementDisplayed = true,
                        DatasetElementChronic = false,
                        DatasetElementSystem = element.DatasetElement.System,
                        DatasetElementType = element.DatasetElement.Field.FieldType.Description,
                        DatasetElementValue = string.Empty,
                        StringMaxLength = element.DatasetElement.Field.MaxLength,
                        NumericMinValue = element.DatasetElement.Field.MinSize,
                        NumericMaxValue = element.DatasetElement.Field.MaxSize,
                        Required = element.DatasetElement.Field.Mandatory,
                        SelectionDataItems = element.DatasetElement.Field.FieldValues.Select(fv => new SelectionDataItemDto() { SelectionKey = fv.Value, Value = fv.Value }).ToList(),
                        DatasetElementSubs = element.DatasetElement.DatasetElementSubs.Select(elementSub => new DatasetElementSubViewDto
                        {
                            DatasetElementSubId = elementSub.Id,
                            DatasetElementSubName = elementSub.ElementName,
                            DatasetElementSubType = elementSub.Field.FieldType.Description,
                            DatasetElementSubDisplayName = elementSub.FriendlyName ?? elementSub.ElementName,
                            DatasetElementSubHelp = elementSub.Help,
                            DatasetElementSubSystem = elementSub.System,
                            StringMaxLength = elementSub.Field.MaxLength,
                            NumericMinValue = elementSub.Field.MinSize,
                            NumericMaxValue = elementSub.Field.MaxSize,
                            Required = elementSub.Field.Mandatory,
                            SelectionDataItems = elementSub.Field.FieldValues.Select(fv => new SelectionDataItemDto() { SelectionKey = fv.Value, Value = fv.Value }).ToList(),
                        }).ToArray()
                    })
                    .ToArray()
                })
                .ToArray();

            return dto;
        }
    }
}
