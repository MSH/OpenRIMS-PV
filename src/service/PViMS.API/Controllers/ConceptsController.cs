using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Commands.ConceptAggregate;
using PVIMS.API.Application.Queries.ConceptAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class ConceptsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ILogger<ConceptsController> _logger;

        public ConceptsController(IMediator mediator,
            IPropertyMappingService propertyMappingService, 
            ITypeHelperService typeHelperService,
            ILogger<ConceptsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all concepts using a valid media type 
        /// </summary>
        /// <param name="conceptResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConceptIdentifierDto</returns>
        [HttpGet("concepts", Name = "GetConceptsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ConceptIdentifierDto>>> GetConceptsByIdentifier(
            [FromQuery] ConceptResourceParameters conceptResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConceptIdentifierDto>
                (conceptResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ConceptsIdentifierQuery(
                conceptResourceParameters.OrderBy,
                conceptResourceParameters.SearchTerm,
                conceptResourceParameters.Active,
                conceptResourceParameters.PageNumber,
                conceptResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ConceptsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = conceptResourceParameters.PageSize,
                currentPage = conceptResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all concepts using a valid media type 
        /// </summary>
        /// <param name="conceptResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConceptDetailDto</returns>
        [HttpGet("concepts", Name = "GetConceptsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ConceptDetailDto>>> GetConceptsByDetail(
            [FromQuery] ConceptResourceParameters conceptResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConceptDetailDto>
                (conceptResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ConceptsDetailQuery(
                conceptResourceParameters.OrderBy,
                conceptResourceParameters.SearchTerm,
                conceptResourceParameters.Active,
                conceptResourceParameters.PageNumber,
                conceptResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ConceptsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = conceptResourceParameters.PageSize,
                currentPage = conceptResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all products using a valid media type 
        /// </summary>
        /// <param name="productResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ProductIdentifierDto</returns>
        [HttpGet("products", Name = "GetProductsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ProductIdentifierDto>>> GetProductsByIdentifier(
            [FromQuery] ProductResourceParameters productResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ProductIdentifierDto>
                (productResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ProductsIdentifierQuery(
                productResourceParameters.OrderBy,
                productResourceParameters.SearchTerm,
                productResourceParameters.Active,
                productResourceParameters.PageNumber,
                productResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ProductsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = productResourceParameters.PageSize,
                currentPage = productResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all products using a valid media type 
        /// </summary>
        /// <param name="productResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ProductDetailDto</returns>
        [HttpGet("products", Name = "GetProductsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ProductDetailDto>>> GetProductsByDetail(
            [FromQuery] ProductResourceParameters productResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ProductDetailDto>
                (productResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ProductsDetailQuery(
                productResourceParameters.OrderBy,
                productResourceParameters.SearchTerm,
                productResourceParameters.Active,
                productResourceParameters.PageNumber,
                productResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ProductsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = productResourceParameters.PageSize,
                currentPage = productResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all medication forms using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MedicationFormIdentifierDto</returns>
        [HttpGet("medicationforms", Name = "GetMedicationFormsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>>> GetMedicationFormsByIdentifier(
            [FromQuery] MedicationFormResourceParameters medicationFormResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<MedicationFormIdentifierDto, MedicationForm>
               (medicationFormResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new MedicationFormsIdentifierQuery(
                medicationFormResourceParameters.OrderBy,
                medicationFormResourceParameters.PageNumber,
                medicationFormResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: MedicationFormsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = medicationFormResourceParameters.PageSize,
                currentPage = medicationFormResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single concept using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the concept</param>
        /// <returns>An ActionResult of type ConceptIdentifierDto</returns>
        [HttpGet("concepts/{id}", Name = "GetConceptByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ConceptIdentifierDto>> GetConceptByIdentifier(int id)
        {
            var query = new ConceptIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: ConceptIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single product using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the product</param>
        /// <returns>An ActionResult of type ProductIdentifierDto</returns>
        [HttpGet("products/{id}", Name = "GetProductByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ProductIdentifierDto>> GetProductByIdentifier(int id)
        {
            var query = new ProductIdentifierQuery(id);

            _logger.LogInformation(
                @"----- Sending query: ProductIdentifierQuery - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single product using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the product</param>
        /// <returns>An ActionResult of type ProgramDetailDto</returns>
        [HttpGet("products/{id}", Name = "GetProductByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ProductDetailDto>> GetProductByDetail(int id)
        {
            var query = new ProductDetailQuery(id);

            _logger.LogInformation(
                $"----- Sending query: ProductDetailQuery - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new concept
        /// </summary>
        /// <param name="conceptForUpdate">The concept payload</param>
        /// <returns></returns>
        [HttpPost("concepts", Name = "CreateConcept")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateConcept(
            [FromBody] ConceptForUpdateDto conceptForUpdate)
        {
            if (conceptForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new AddConceptCommand(conceptForUpdate.ConceptName, conceptForUpdate.Strength, conceptForUpdate.MedicationForm);

            _logger.LogInformation(
                $"----- Sending command: AddConceptCommand - {command.ConceptName}");

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetConceptByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing concept
        /// </summary>
        /// <param name="id">The unique id of the concept</param>
        /// <param name="conceptForUpdate">The concept payload</param>
        /// <returns></returns>
        [HttpPut("concepts/{id}", Name = "UpdateConcept")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateConcept(int id,
            [FromBody] ConceptForUpdateDto conceptForUpdate)
        {
            if (conceptForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeConceptDetailsCommand(id, conceptForUpdate.ConceptName, conceptForUpdate.Strength, conceptForUpdate.MedicationForm, conceptForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

            _logger.LogInformation(
                "----- Sending command: ChangeFacilityDetailsCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing concept
        /// </summary>
        /// <param name="id">The unique id of the concept</param>
        /// <returns></returns>
        [HttpDelete("concepts/{id}", Name = "DeleteConcept")]
        public async Task<IActionResult> DeleteConcept(int id)
        {
            var command = new DeleteConceptCommand(id);

            _logger.LogInformation(
                "----- Sending command: DeleteConceptCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="productForUpdate">The product payload</param>
        /// <returns></returns>
        [HttpPost("products", Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct( 
            [FromBody] ProductForUpdateDto productForUpdate)
        {
            if (productForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new AddProductCommand(productForUpdate.ConceptName, productForUpdate.ProductName, productForUpdate.Manufacturer, productForUpdate.Description);

            _logger.LogInformation(
                $"----- Sending command: AddProductCommand - {command.ProductName}");

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetProductByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">The unique id of the product</param>
        /// <param name="productForUpdate">The product payload</param>
        /// <returns></returns>
        [HttpPut("products/{id}", Name = "UpdateProduct")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, 
            [FromBody] ProductForUpdateDto productForUpdate)
        {
            if (productForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeProductDetailsCommand(id, productForUpdate.ConceptName, productForUpdate.ProductName, productForUpdate.Manufacturer, productForUpdate.Description, productForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

            _logger.LogInformation(
                $"----- Sending command: ChangeProductDetailsCommand - {command.ProductId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing product
        /// </summary>
        /// <param name="id">The unique id of the product</param>
        /// <returns></returns>
        [HttpDelete("products/{id}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var command = new DeleteProductCommand(id);

            _logger.LogInformation(
                $"----- Sending command: DeleteProductCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }
    }
}
