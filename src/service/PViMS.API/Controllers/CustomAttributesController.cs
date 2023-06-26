using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Commands.CustomAttributeAggregate;
using PVIMS.API.Application.Queries.CustomAttributeAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    /// <summary>
    /// A representation of all custom attributes.
    /// A custom attribute is configured to represent core entity additional values
    /// </summary>
    [Route("api/customattributes")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class CustomAttributesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<CustomAttributesController> _logger;

        public CustomAttributesController(
            IMediator mediator, 
            ITypeHelperService typeHelperService,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ILogger<CustomAttributesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all custom attributes using a valid media type 
        /// </summary>
        /// <param name="customAttributeResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CustomAttributeIdentifierDto</returns>
        [HttpGet(Name = "GetCustomAttributesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>>> GetCustomAttributesByIdentifier(
            [FromQuery] CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CustomAttributeIdentifierDto>
                (customAttributeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CustomAttributesIdentifierQuery(
                customAttributeResourceParameters.OrderBy,
                customAttributeResourceParameters.ExtendableTypeName,
                customAttributeResourceParameters.CustomAttributeType,
                customAttributeResourceParameters.IsSearchable,
                customAttributeResourceParameters.PageNumber,
                customAttributeResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CustomAttributesIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = customAttributeResourceParameters.PageSize,
                currentPage = customAttributeResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all custom attributes using a valid media type 
        /// </summary>
        /// <param name="customAttributeResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CustomAttributeDetailDto</returns>
        [HttpGet(Name = "GetCustomAttributesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>>> GetCustomAttributesByDetail(
            [FromQuery] CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CustomAttributeIdentifierDto>
                (customAttributeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CustomAttributesDetailQuery(
                customAttributeResourceParameters.OrderBy,
                customAttributeResourceParameters.ExtendableTypeName,
                customAttributeResourceParameters.CustomAttributeType,
                customAttributeResourceParameters.IsSearchable,
                customAttributeResourceParameters.PageNumber,
                customAttributeResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CustomAttributesDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = customAttributeResourceParameters.PageSize,
                currentPage = customAttributeResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single custom attribute using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the custom attribute</param>
        /// <returns>An ActionResult of type CustomAttributeIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetCustomAttributeByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<CustomAttributeIdentifierDto>> GetCustomAttributeByIdentifier(int id)
        {
            var mappedCustomAttribute = await GetCustomAttributeAsync<CustomAttributeIdentifierDto>(id);
            if (mappedCustomAttribute == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCustomAttribute<CustomAttributeIdentifierDto>(mappedCustomAttribute));
        }

        /// <summary>
        /// Get a single custom attribute using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the custom attribute</param>
        /// <returns>An ActionResult of type CustomAttributeDetailDto</returns>
        [HttpGet("{id}", Name = "GetCustomAttributeByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CustomAttributeDetailDto>> GetCustomAttributeByDetail(int id)
        {
            var query = new CustomAttributeDetailQuery(id);

            _logger.LogInformation($"----- Sending query: CustomAttributeDetailQuery - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new custom attribute
        /// </summary>
        /// <param name="customAttributeForCreation">The custom attribute payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateCustomAttribute")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCustomAttribute(
            [FromBody] CustomAttributeForCreationDto customAttributeForCreation)
        {
            if (customAttributeForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new attribute");
                return BadRequest(ModelState);
            }

            var command = new AddCustomAttributeCommand(
                customAttributeForCreation.ExtendableTypeName.ToString(),
                MapCustomAttributeType(customAttributeForCreation.CustomAttributeType),
                customAttributeForCreation.Category,
                customAttributeForCreation.AttributeKey,
                customAttributeForCreation.AttributeDetail,
                customAttributeForCreation.IsRequired == Models.ValueTypes.YesNoValueType.Yes,
                customAttributeForCreation.MaxLength,
                customAttributeForCreation.MinValue,
                customAttributeForCreation.MaxValue,
                customAttributeForCreation.FutureDateOnly == Models.ValueTypes.YesNoValueType.Yes,
                customAttributeForCreation.PastDateOnly == Models.ValueTypes.YesNoValueType.Yes,
                customAttributeForCreation.IsSearchable == Models.ValueTypes.YesNoValueType.Yes
            );

            _logger.LogInformation($"----- Sending command: AddCustomAttributeCommand - {customAttributeForCreation.AttributeKey}");

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetCustomAttributeByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing custom attribute
        /// </summary>
        /// <param name="id">The unique id of the custom attribute</param>
        /// <param name="customAttributeForUpdate">The custom attribute payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateCustomAttribute")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCustomAttribute(int id,
            [FromBody] CustomAttributeForUpdateDto customAttributeForUpdate)
        {
            if (customAttributeForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for custom attribute");
                return BadRequest(ModelState);
            }

            var command = new ChangeCustomAttributeDetailsCommand(
                id,
                customAttributeForUpdate.ExtendableTypeName,
                customAttributeForUpdate.Category,
                customAttributeForUpdate.AttributeKey,
                customAttributeForUpdate.AttributeDetail,
                customAttributeForUpdate.IsRequired == Models.ValueTypes.YesNoValueType.Yes,
                customAttributeForUpdate.StringMaxLength,
                customAttributeForUpdate.NumericMinValue,
                customAttributeForUpdate.NumericMaxValue,
                customAttributeForUpdate.FutureDateOnly == Models.ValueTypes.YesNoValueType.Yes,
                customAttributeForUpdate.PastDateOnly == Models.ValueTypes.YesNoValueType.Yes,
                customAttributeForUpdate.IsSearchable == Models.ValueTypes.YesNoValueType.Yes
             );

            _logger.LogInformation($"----- Sending command: ChangeCustomAttributeDetailsCommand - {command.Id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing custom attribute
        /// </summary>
        /// <param name="id">The unique id of the custom attribute</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteCustomAttribute")]
        public async Task<IActionResult> DeleteCustomAttribute(int id)
        {
            var command = new DeleteCustomAttributeCommand(id);

            _logger.LogInformation($"----- Sending command: DeleteCustomAttributeCommand - {command.Id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new selection value
        /// </summary>
        /// <param name="customAttributeId">The unique id of the custom attribute</param>
        /// <param name="selectionDataItemForCreation">The selection data item payload</param>
        /// <returns></returns>
        [HttpPost("{customAttributeId}/selection", Name = "CreateSelectionDataItem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSelectionDataItem(int customAttributeId,
            [FromBody] SelectionDataItemForCreationDto selectionDataItemForCreation)
        {
            if (selectionDataItemForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new value");
                return BadRequest(ModelState);
            }

            var command = new AddSelectionValueCommand(
                selectionDataItemForCreation.AttributeKey,
                selectionDataItemForCreation.SelectionKey,
                selectionDataItemForCreation.DataItemValue
            );

            _logger.LogInformation($"----- Sending command: AddSelectionValueCommand - {selectionDataItemForCreation.AttributeKey}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing selection data item
        /// </summary>
        /// <param name="customAttributeId">The unique id of the custom attribute</param>
        /// <param name="key">The unique id of the selection data item key</param>
        /// <returns></returns>
        [HttpDelete("{customAttributeId}/selection/{key}", Name = "DeleteSelectionDataItem")]
        public async Task<IActionResult> DeleteSelectionDataItem(int customAttributeId, string key)
        {
            var command = new DeleteSelectionValueCommand(customAttributeId, key);

            _logger.LogInformation($"----- Sending command: DeleteSelectionValueCommand - {key}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }

        /// <summary>
        /// Get single custom attribute from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource guid to search by</param>
        /// <returns></returns>
        private async Task<T> GetCustomAttributeAsync<T>(long id) where T : class
        {
            var customAttributeFromRepo = await _customAttributeRepository.GetAsync(f => f.Id == id);

            if (customAttributeFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCustomAttribute = _mapper.Map<T>(customAttributeFromRepo);

                return mappedCustomAttribute;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CustomAttributeIdentifierDto CreateLinksForCustomAttribute<T>(T dto)
        {
            CustomAttributeIdentifierDto identifier = (CustomAttributeIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CustomAttribute", identifier.Id), "self", "GET"));

            return identifier;
        }

        private CustomAttributeType MapCustomAttributeType(CustomAttributeTypes sourceAttributeType)
        {
            switch (sourceAttributeType)
            {
                case CustomAttributeTypes.All:
                    return CustomAttributeType.String;
                case CustomAttributeTypes.None:
                    return CustomAttributeType.String;
                case CustomAttributeTypes.Numeric:
                    return CustomAttributeType.Numeric;
                case CustomAttributeTypes.String:
                    return CustomAttributeType.String;
                case CustomAttributeTypes.Selection:
                    return CustomAttributeType.Selection;
                case CustomAttributeTypes.DateTime:
                    return CustomAttributeType.DateTime;
                case CustomAttributeTypes.FirstClassProperty:
                    return CustomAttributeType.FirstClassProperty;
            }
            return CustomAttributeType.String;
        }
    }
}
