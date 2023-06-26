using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using System;
using System.Linq;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System.Threading.Tasks;
using PVIMS.API.Helpers;
using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PVIMS.API.Infrastructure.Auth;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/metapages")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class MetaPagesController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<MetaPage> _metaPageRepository;
        private readonly IRepositoryInt<MetaWidget> _metaWidgetRepository;
        private readonly IRepositoryInt<MetaWidgetType> _metaWidgetTypeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public MetaPagesController(ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<MetaPage> metaPageRepository,
            IRepositoryInt<MetaWidget> metaWidgetRepository,
            IRepositoryInt<MetaWidgetType> metaWidgetTypeRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _metaPageRepository = metaPageRepository ?? throw new ArgumentNullException(nameof(metaPageRepository));
            _metaWidgetRepository = metaWidgetRepository ?? throw new ArgumentNullException(nameof(metaWidgetRepository));
            _metaWidgetTypeRepository = metaWidgetTypeRepository ?? throw new ArgumentNullException(nameof(metaWidgetTypeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all meta pages using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MetaPageDetailDto</returns>
        [HttpGet(Name = "GetMetaPagesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MetaPageDetailDto>> GetMetaPagesByDetail(
            [FromQuery] IdResourceParameters metaResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<MetaPageDetailDto>
                (metaResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMetaPagesWithLinks = GetMetaPages<MetaPageDetailDto>(metaResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MetaPageDetailDto>(mappedMetaPagesWithLinks.TotalCount, mappedMetaPagesWithLinks);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single meta page using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta page</param>
        /// <returns>An ActionResult of type MetaPageIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetMetaPageByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<MetaPageIdentifierDto>> GetMetaPageByIdentifier(long id)
        {
            var mappedMetaPage = await GetMetaPageAsync<MetaPageIdentifierDto>(id);
            if (mappedMetaPage == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaPage<MetaPageIdentifierDto>(mappedMetaPage));
        }

        /// <summary>
        /// Get a single meta page using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta page</param>
        /// <returns>An ActionResult of type MetaPageDetailDto</returns>
        [HttpGet("{id}", Name = "GetMetaPageByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaPageDetailDto>> GetMetaPageByDetail(long id)
        {
            var mappedMetaPage = await GetMetaPageAsync<MetaPageDetailDto>(id);
            if (mappedMetaPage == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaPage<MetaPageDetailDto>(mappedMetaPage));
        }

        /// <summary>
        /// Get a single meta page using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the meta page</param>
        /// <returns>An ActionResult of type MetaPageExpandedDto</returns>
        [HttpGet("{id}", Name = "GetMetaPageByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaPageExpandedDto>> GetMetaPageByExpanded(long id)
        {
            var mappedMetaPage = await GetMetaPageAsync<MetaPageExpandedDto>(id);
            if (mappedMetaPage == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaPage<MetaPageExpandedDto>(CustomPageMap(mappedMetaPage)));
        }

        /// <summary>
        /// Get a single meta page widget using it's unique id and valid media type 
        /// </summary>
        /// <param name="metaPageId">The unique identifier for the meta page that owns the widget</param>
        /// <param name="id">The unique identifier for the widget</param>
        /// <returns>An ActionResult of type MetaPageIdentifierDto</returns>
        [HttpGet("{metaPageId}/widgets/{id}", Name = "GetMetaWidgetByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<MetaWidgetIdentifierDto>> GetMetaWidgetByIdentifier(long metaPageId, long id)
        {
            var mappedMetaWidget = await GetMetaWidgetAsync<MetaWidgetIdentifierDto>(metaPageId, id);
            if (mappedMetaWidget == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaWidget<MetaWidgetIdentifierDto>(metaPageId, mappedMetaWidget));
        }

        /// <summary>
        /// Get a single meta page widget using it's unique id and valid media type 
        /// </summary>
        /// <param name="metaPageId">The unique identifier for the meta page that owns the widget</param>
        /// <param name="id">The unique identifier for the widget</param>
        /// <returns>An ActionResult of type MetaWidgetDetailDto</returns>
        [HttpGet("{metaPageId}/widgets/{id}", Name = "GetMetaWidgetByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MetaWidgetDetailDto>> GetMetaWidgetByDetail(long metaPageId, long id)
        {
            var mappedMetaWidget = await GetMetaWidgetAsync<MetaWidgetDetailDto>(metaPageId, id);
            if (mappedMetaWidget == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForMetaWidget<MetaWidgetDetailDto>(metaPageId, CustomWidgetMap(mappedMetaWidget)));
        }

        /// <summary>
        /// Create a new meta page
        /// </summary>
        /// <param name="metaPageForUpdate">The meta page payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateMetaPage")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> CreateMetaPage(
            [FromBody] MetaPageForUpdateDto metaPageForUpdate)
        {
            if (metaPageForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(metaPageForUpdate.PageName, @"[a-zA-Z0-9 ]").Count < metaPageForUpdate.PageName.Length)
            {
                ModelState.AddModelError("Message", "Page name contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if(!string.IsNullOrWhiteSpace(metaPageForUpdate.PageDefinition))
            {
                if (Regex.Matches(metaPageForUpdate.PageDefinition, @"[-a-zA-Z0-9 .,]").Count < metaPageForUpdate.PageDefinition.Length)
                {
                    ModelState.AddModelError("Message", "Page definition contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                    return BadRequest(ModelState);
                }
            }

            if (!string.IsNullOrWhiteSpace(metaPageForUpdate.Breadcrumb))
            {
                if (Regex.Matches(metaPageForUpdate.Breadcrumb, @"[-a-zA-Z0-9 .,]").Count < metaPageForUpdate.Breadcrumb.Length)
                {
                    ModelState.AddModelError("Message", "Bread crumb contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                    return BadRequest(ModelState);
                }
            }

            if (_unitOfWork.Repository<MetaPage>().Queryable().
                Where(l => l.PageName == metaPageForUpdate.PageName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var newMetaPage = new MetaPage()
                {
                    PageName = metaPageForUpdate.PageName,
                    PageDefinition = metaPageForUpdate.PageDefinition,
                    Breadcrumb = metaPageForUpdate.Breadcrumb,
                    IsSystem = false,
                    MetaDefinition = string.Empty,
                    MetaPageGuid = Guid.NewGuid(),
                    IsVisible = (metaPageForUpdate.Visible == Models.ValueTypes.YesNoValueType.Yes)
                };

                _metaPageRepository.Save(newMetaPage);

                var mappedMetaPage = await GetMetaPageAsync<MetaPageIdentifierDto>(newMetaPage.Id);
                if (mappedMetaPage == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetMetaPageByIdentifier",
                    new
                    {
                        id = mappedMetaPage.Id
                    }, CreateLinksForMetaPage<MetaPageIdentifierDto>(mappedMetaPage));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing meta page
        /// </summary>
        /// <param name="id">The unique id of the meta page</param>
        /// <param name="metaPageForUpdate">The meta page payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateMetaPage")]
        [Consumes("application/json")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> UpdateMetaPage(long id,
            [FromBody] MetaPageForUpdateDto metaPageForUpdate)
        {
            if (metaPageForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(metaPageForUpdate.PageName, @"[a-zA-Z0-9 ]").Count < metaPageForUpdate.PageName.Length)
            {
                ModelState.AddModelError("Message", "Page name contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrWhiteSpace(metaPageForUpdate.PageDefinition))
            {
                if (Regex.Matches(metaPageForUpdate.PageDefinition, @"[-a-zA-Z0-9 .,]").Count < metaPageForUpdate.PageDefinition.Length)
                {
                    ModelState.AddModelError("Message", "Page definition contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                    return BadRequest(ModelState);
                }
            }

            if (!string.IsNullOrWhiteSpace(metaPageForUpdate.Breadcrumb))
            {
                if (Regex.Matches(metaPageForUpdate.Breadcrumb, @"[-a-zA-Z0-9 .,]").Count < metaPageForUpdate.Breadcrumb.Length)
                {
                    ModelState.AddModelError("Message", "Bread crumb contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                    return BadRequest(ModelState);
                }
            }

            if (_unitOfWork.Repository<MetaPage>().Queryable().
                Where(l => l.PageName == metaPageForUpdate.PageName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var metaPageFromRepo = await _metaPageRepository.GetAsync(f => f.Id == id);
            if (metaPageFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                metaPageFromRepo.PageName = metaPageForUpdate.PageName;
                metaPageFromRepo.PageDefinition = metaPageForUpdate.PageDefinition;
                metaPageFromRepo.Breadcrumb = metaPageForUpdate.Breadcrumb;
                metaPageFromRepo.IsVisible = (metaPageForUpdate.Visible == Models.ValueTypes.YesNoValueType.Yes);

                _metaPageRepository.Update(metaPageFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing meta page
        /// </summary>
        /// <param name="id">The unique id of the meta page</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteMetaPage")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> DeleteMetaPage(long id)
        {
            var metaPageFromRepo = await _metaPageRepository.GetAsync(f => f.Id == id);
            if (metaPageFromRepo == null)
            {
                return NotFound();
            }

            var pageInUse = _unitOfWork.Repository<MetaWidget>().Queryable()
                .Any(w => w.MetaPage.Id == id);

            if (pageInUse)
            {
                ModelState.AddModelError("Message", "Unable to delete the page as it is currently in use.");
            }
            if (metaPageFromRepo.IsSystem)
            {
                ModelState.AddModelError("Message", "Unable to delete a system page");
            }

            if (ModelState.IsValid)
            {
                _metaPageRepository.Delete(metaPageFromRepo);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new meta widget
        /// </summary>
        /// <param name="metaPageId">The unique identifier of the meta page</param>
        /// <param name="metaWidgetForCreation">The meta widget payload</param>
        /// <returns></returns>
        [HttpPost("{metaPageId}/widgets", Name = "CreateMetaWidget")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> CreateMetaWidget(long metaPageId, 
            [FromBody] MetaWidgetForCreationDto metaWidgetForCreation)
        {
            if (metaWidgetForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var metaPageFromRepo = await _metaPageRepository.GetAsync(f => f.Id == metaPageId);
            if (metaPageFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(metaWidgetForCreation.WidgetName, @"[a-zA-Z0-9 ]").Count < metaWidgetForCreation.WidgetName.Length)
            {
                ModelState.AddModelError("Message", "Widget name contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            if (Regex.Matches(metaWidgetForCreation.WidgetType, @"[a-zA-Z]").Count < metaWidgetForCreation.WidgetType.Length)
            {
                ModelState.AddModelError("Message", "Widget type contains invalid characters (Enter A-Z, a-z)");
            }

            if (!string.IsNullOrWhiteSpace(metaWidgetForCreation.WidgetDefinition))
            {
                if (Regex.Matches(metaWidgetForCreation.WidgetDefinition, @"[-a-zA-Z0-9 .,]").Count < metaWidgetForCreation.WidgetDefinition.Length)
                {
                    ModelState.AddModelError("Message", "Widget definition contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                }
            }

            if (Regex.Matches(metaWidgetForCreation.Icon, @"[a-zA-Z_]").Count < metaWidgetForCreation.Icon.Length)
            {
                ModelState.AddModelError("Message", "Icon contains invalid characters (Enter A-Z, a-z, underscore)");
            }

            var metaWidgetTypeFromRepo = await _metaWidgetTypeRepository.GetAsync(f => f.Description == metaWidgetForCreation.WidgetType);
            if (metaWidgetTypeFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate widget type");
            }

            if (_unitOfWork.Repository<MetaWidget>().Queryable().
                Where(l => l.MetaPage.Id == metaPageId && l.WidgetName == metaWidgetForCreation.WidgetName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var widgetTypeE = (MetaWidgetTypes)metaWidgetTypeFromRepo.Id;
                var content = string.Empty;

                switch (widgetTypeE)
                {
                    case MetaWidgetTypes.General:
                        content = "** PLEASE ENTER YOUR CONTENT HERE **";
                        break;

                    case MetaWidgetTypes.SubItem:
                    case MetaWidgetTypes.ItemList:
                        content = GetBaseTemplate(widgetTypeE, metaPageId);
                        break;

                    default:
                        break;
                }

                var newMetaWidget = new MetaWidget()
                {
                    Content = content,
                    MetaPage = metaPageFromRepo,
                    WidgetDefinition = metaWidgetForCreation.WidgetDefinition,
                    WidgetType = metaWidgetTypeFromRepo,
                    WidgetLocation = MetaWidgetLocation.Unassigned,
                    WidgetName = metaWidgetForCreation.WidgetName,
                    WidgetStatus = MetaWidgetStatus.Unpublished,
                    Icon = metaWidgetForCreation.Icon
                };

                _metaWidgetRepository.Save(newMetaWidget);

                var mappedMetaWidget = await GetMetaWidgetAsync<MetaWidgetIdentifierDto>(metaPageId, newMetaWidget.Id);
                if (mappedMetaWidget == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtAction("GetMetaWidgetByIdentifier",
                    new
                    {
                        metaPageId,
                        id = mappedMetaWidget.Id
                    }, CreateLinksForMetaWidget<MetaWidgetIdentifierDto>(metaPageId, mappedMetaWidget));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing meta widget
        /// </summary>
        /// <param name="metaPageId">The unique identifier of the meta page</param>
        /// <param name="id">The unique id of the meta widget</param>
        /// <param name="metaWidgetForUpdate">The meta widget payload</param>
        /// <returns></returns>
        [HttpPut("{metaPageId}/widgets/{id}", Name = "UpdateMetaWidget")]
        [Consumes("application/json")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> UpdateMetaWidget(long metaPageId, long id,
            [FromBody] MetaWidgetForUpdateDto metaWidgetForUpdate)
        {
            if (metaWidgetForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var metaPageFromRepo = await _metaPageRepository.GetAsync(f => f.Id == metaPageId, new string[] { "Widgets" });
            if (metaPageFromRepo == null)
            {
                return NotFound();
            }

            var metaWidgetFromRepo = await _metaWidgetRepository.GetAsync(f => f.MetaPage.Id == metaPageId && f.Id == id, new string[] { "WidgetType" });
            if (metaWidgetFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(metaWidgetForUpdate.WidgetName, @"[a-zA-Z0-9 ]").Count < metaWidgetForUpdate.WidgetName.Length)
            {
                ModelState.AddModelError("Message", "Widget name contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            if (!string.IsNullOrWhiteSpace(metaWidgetForUpdate.WidgetDefinition))
            {
                if (Regex.Matches(metaWidgetForUpdate.WidgetDefinition, @"[-a-zA-Z0-9 .,]").Count < metaWidgetForUpdate.WidgetDefinition.Length)
                {
                    ModelState.AddModelError("Message", "Widget definition contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma)");
                }
            }

            if (Regex.Matches(metaWidgetForUpdate.Icon, @"[a-zA-Z_]").Count < metaWidgetForUpdate.Icon.Length)
            {
                ModelState.AddModelError("Message", "Icon contains invalid characters (Enter A-Z, a-z, underscore)");
            }

            if (_unitOfWork.Repository<MetaWidget>().Queryable().
                Where(l => l.MetaPage.Id == metaPageId && l.WidgetName == metaWidgetForUpdate.WidgetName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var widgetTypeE = (MetaWidgetTypes)metaWidgetFromRepo.WidgetType.Id;
                var content = string.Empty;

                switch (widgetTypeE)
                {
                    case MetaWidgetTypes.General:
                        content = metaWidgetForUpdate.GeneralContent;
                        break;

                    case MetaWidgetTypes.SubItem:
                    case MetaWidgetTypes.ItemList:
                        content = GetContentFromWidget(widgetTypeE, metaWidgetForUpdate);
                        break;

                    default:
                        break;
                }

                if(metaWidgetForUpdate.WidgetLocation != MetaWidgetLocation.Unassigned)
                {
                    // If meta page already contains a widget in this location, mark it as unpublished
                    var metaWidgetFromPage = metaPageFromRepo.Widgets.SingleOrDefault(w => w.WidgetLocation == metaWidgetForUpdate.WidgetLocation && w.Id != metaWidgetFromRepo.Id);
                    if (metaWidgetFromPage != null)
                    {
                        metaWidgetFromPage.WidgetLocation = MetaWidgetLocation.Unassigned;
                        metaWidgetFromPage.WidgetStatus = MetaWidgetStatus.Unpublished;

                        _metaWidgetRepository.Update(metaWidgetFromPage);
                    }
                }

                metaWidgetFromRepo.WidgetName = metaWidgetForUpdate.WidgetName;
                metaWidgetFromRepo.WidgetDefinition = metaWidgetForUpdate.WidgetDefinition;
                metaWidgetFromRepo.WidgetLocation = metaWidgetForUpdate.WidgetLocation;
                metaWidgetFromRepo.WidgetStatus = metaWidgetForUpdate.WidgetStatus;
                metaWidgetFromRepo.Content = content;
                metaWidgetFromRepo.Icon = metaWidgetForUpdate.Icon;

                _metaWidgetRepository.Update(metaWidgetFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Move a meta widget to a new page
        /// </summary>
        /// <param name="metaPageId">The unique identifier of the meta page</param>
        /// <param name="id">The unique id of the meta page</param>
        /// <param name="metaWidgetForMove">The move meta widget payload</param>
        /// <returns></returns>
        [HttpPut("{metaPageId}/widgets/{id}/move", Name = "MoveMetaWidget")]
        [Consumes("application/json")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> MoveMetaWidget(long metaPageId, long id,
            [FromBody] MetaWidgetForMoveDto metaWidgetForMove)
        {
            if (metaWidgetForMove == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var metaWidgetFromRepo = await _metaWidgetRepository.GetAsync(f => f.MetaPage.Id == metaPageId && f.Id == id);
            if (metaWidgetFromRepo == null)
            {
                return NotFound();
            }

            var metaDestinationPageFromRepo = await _metaPageRepository.GetAsync(f => f.Id == metaWidgetForMove.DestinationMetaPageId);
            if (metaDestinationPageFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate destination page");
            }

            if (ModelState.IsValid)
            {
                metaWidgetFromRepo.MetaPage = metaDestinationPageFromRepo;

                _metaWidgetRepository.Update(metaWidgetFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing meta widget
        /// </summary>
        /// <param name="metaPageId">The unique identifier of the meta page</param>
        /// <param name="id">The unique id of the meta page</param>
        /// <returns></returns>
        [HttpDelete("{metaPageId}/widgets/{id}", Name = "DeleteMetaWidget")]
        [Authorize(Roles = "PublisherAdmin")]
        public async Task<IActionResult> DeleteMetaWidget(long metaPageId, long id)
        {
            var metaWidgetFromRepo = await _metaWidgetRepository.GetAsync(f => f.MetaPage.Id == metaPageId && f.Id == id);
            if (metaWidgetFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _metaWidgetRepository.Delete(metaWidgetFromRepo);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get meta tables from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMetaPages<T>(IdResourceParameters metaResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = metaResourceParameters.PageNumber,
                PageSize = metaResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaPage>(metaResourceParameters.OrderBy, "asc");

            var pagedMetaPagesFromRepo = _metaPageRepository.List(pagingInfo, null, orderby, "");
            if (pagedMetaPagesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaPages = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedMetaPagesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMetaPagesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedMetaPages.TotalCount,
                    pageSize = mappedMetaPages.PageSize,
                    currentPage = mappedMetaPages.CurrentPage,
                    totalPages = mappedMetaPages.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMetaTables.ForEach(dto => CreateLinksForMetaTable(dto));

                return mappedMetaPages;
            }

            return null;
        }

        /// <summary>
        /// Get single meta page from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetMetaPageAsync<T>(long id) where T : class
        {
            var metaPageFromRepo = await _metaPageRepository.GetAsync(f => f.Id == id, new string[] { "Widgets.WidgetType" });

            if (metaPageFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaPage = _mapper.Map<T>(metaPageFromRepo);

                return mappedMetaPage;
            }

            return null;
        }

        /// <summary>
        /// Get single meta widget from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="metaPageId">Parent id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetMetaWidgetAsync<T>(long metaPageId, long id) where T : class
        {
            var metaWidgetFromRepo = await _metaWidgetRepository.GetAsync(f => f.MetaPage.Id == metaPageId && f.Id == id, new string[] { "WidgetType" });

            if (metaWidgetFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaWidget = _mapper.Map<T>(metaWidgetFromRepo);

                return mappedMetaWidget;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaPageIdentifierDto CreateLinksForMetaPage<T>(T dto)
        {
            MetaPageIdentifierDto identifier = (MetaPageIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaPage", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="metaPageId">The unique identifier for the meta page that owns the widget</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaWidgetIdentifierDto CreateLinksForMetaWidget<T>(long metaPageId, T dto)
        {
            MetaWidgetIdentifierDto identifier = (MetaWidgetIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateMetaWidgetResourceUri(metaPageId, identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaPageExpandedDto CustomPageMap(MetaPageExpandedDto dto)
        {
            dto.Widgets.ForEach(widget => CustomWidgetMap(widget));
            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private MetaWidgetDetailDto CustomWidgetMap(MetaWidgetDetailDto dto)
        {
            var metaWidget = _metaWidgetRepository.Get(p => p.Id == dto.Id);

            if (metaWidget == null)
            {
                return dto;
            }

            var widgetType = (MetaWidgetTypes)metaWidget.WidgetType.Id;
            if (widgetType == MetaWidgetTypes.SubItem )
            {
                XmlDocument wdoc = new XmlDocument();
                wdoc.LoadXml(metaWidget.Content);
                XmlNode wroot = wdoc.DocumentElement;

                // Loop through each listitem
                foreach (XmlNode node in wroot.ChildNodes)
                {
                    var listItem = new WidgetistItemDto()
                    {
                        Title = node.ChildNodes[0].InnerText,
                        SubTitle = node.ChildNodes[1].InnerText,
                        ContentPage = node.ChildNodes[2].InnerText,
                        Modified = node.ChildNodes[3] != null ? node.ChildNodes[3].InnerText : DateTime.Today.ToString("yyyy-MM-dd")
                    };
                    dto.ContentItems.Add(listItem);
                }

            }
            if (widgetType == MetaWidgetTypes.ItemList)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(metaWidget.Content);
                XmlNode root = doc.DocumentElement;

                // Loop through each listitem
                foreach (XmlNode node in root.ChildNodes)
                {
                    var listItem = new WidgetistItemDto()
                    {
                        Title = node.ChildNodes[0].InnerText,
                        Content = node.ChildNodes[1].InnerText,
                    };
                    dto.ContentItems.Add(listItem);
                }
            }

            return dto;
        }

        /// <summary>
        ///  Prepare base content for widget
        /// </summary>
        /// <param name="metaPageId">The unique identifier of the meta page</param>
        /// <param name="widgetTypeE">The type of widget to prepare content for</param>
        /// <returns></returns>
        private string GetBaseTemplate(MetaWidgetTypes widgetTypeE, long metaPageId)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode parentNode = xmlDoc.CreateElement("WidgetList", "");
            XmlNode childNode = xmlDoc.CreateElement("ListItem", "");
            if (widgetTypeE == MetaWidgetTypes.ItemList)
            {
                XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                titleNode.InnerText = "** PLEASE ADD TITLE HERE **";
                XmlNode contentNode = xmlDoc.CreateElement("Content", "");
                contentNode.InnerText = "** PLEASE ADD CONTENT HERE **";

                childNode.AppendChild(titleNode);
                childNode.AppendChild(contentNode);
                parentNode.AppendChild(childNode);
            }
            if (widgetTypeE == MetaWidgetTypes.SubItem)
            {
                XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                titleNode.InnerText = "** PLEASE ADD TITLE HERE **";
                XmlNode subTitleNode = xmlDoc.CreateElement("SubTitle", "");
                subTitleNode.InnerText = "** PLEASE ADD SUB-TITLE HERE **";
                XmlNode contentPageNode = xmlDoc.CreateElement("ContentPage", "");
                contentPageNode.InnerText = metaPageId.ToString();
                XmlNode modifiedNode = xmlDoc.CreateElement("ModifiedDate", "");
                modifiedNode.InnerText = DateTime.Today.ToString("yyyy-MM-dd");

                childNode.AppendChild(titleNode);
                childNode.AppendChild(subTitleNode);
                childNode.AppendChild(contentPageNode);
                childNode.AppendChild(modifiedNode);
                parentNode.AppendChild(childNode);
            }

            xmlDoc.AppendChild(parentNode);
            return xmlDoc.InnerXml;
        }

        /// <summary>
        ///  Prepare new content for widget
        /// </summary>
        /// <param name="widgetTypeE">The type of widget to prepare content for</param>
        /// <param name="metaWidgetForUpdate">The meta widget payload</param>
        /// <returns></returns>
        private string GetContentFromWidget(MetaWidgetTypes widgetTypeE, MetaWidgetForUpdateDto metaWidgetForUpdate)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode parentNode = xmlDoc.CreateElement("WidgetList", "");

            if (widgetTypeE == MetaWidgetTypes.ItemList)
            {
                foreach(var listItem in metaWidgetForUpdate.ListItems)
                {
                    XmlNode childNode = xmlDoc.CreateElement("ListItem", "");
                    XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                    titleNode.InnerText = listItem.Title;
                    XmlNode contentNode = xmlDoc.CreateElement("Content", "");
                    contentNode.InnerText = listItem.Content;

                    childNode.AppendChild(titleNode);
                    childNode.AppendChild(contentNode);
                    parentNode.AppendChild(childNode);
                }
            }
            if (widgetTypeE == MetaWidgetTypes.SubItem)
            {
                foreach (var subItem in metaWidgetForUpdate.SubItems)
                {
                    XmlNode childNode = xmlDoc.CreateElement("ListItem", "");
                    XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                    titleNode.InnerText = subItem.Title;
                    XmlNode subTitleNode = xmlDoc.CreateElement("SubTitle", "");
                    subTitleNode.InnerText = subItem.SubTitle;
                    XmlNode contentPageNode = xmlDoc.CreateElement("ContentPage", "");
                    contentPageNode.InnerText = subItem.ContentPage;
                    XmlNode modifiedNode = xmlDoc.CreateElement("ModifiedDate", "");
                    modifiedNode.InnerText = DateTime.Today.ToString("yyyy-MM-dd");

                    childNode.AppendChild(titleNode);
                    childNode.AppendChild(subTitleNode);
                    childNode.AppendChild(contentPageNode);
                    childNode.AppendChild(modifiedNode);
                    parentNode.AppendChild(childNode);

                }
            }

            xmlDoc.AppendChild(parentNode);
            return xmlDoc.InnerXml;
        }
    }
}
