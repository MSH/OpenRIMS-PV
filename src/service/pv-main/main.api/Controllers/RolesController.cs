using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.API.Models.Parameters;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;

namespace OpenRIMS.PV.Main.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class RolesController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RolesController(ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        /// <summary>
        /// Get all roles using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of RoleIdentifierDto</returns>
        [HttpGet("roles", Name = "GetRolesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<RoleIdentifierDto>> GetRolesByIdentifier(
            [FromQuery] IdResourceParameters baseResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<RoleIdentifierDto>
                (baseResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedRolesWithLinks = GetRoles<RoleIdentifierDto>(baseResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<RoleIdentifierDto>(mappedRolesWithLinks.TotalCount, mappedRolesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, roleResourceParameters,
            //    mappedRolesWithLinks.HasNext, mappedRolesWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single role using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab result</param>
        /// <returns>An ActionResult of type RoleIdentifierDto</returns>
        [HttpGet("roles/{id}", Name = "GetRoleByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.main.identifier.v1+json", "application/vnd.main.identifier.v1+xml")]
        public async Task<ActionResult<RoleIdentifierDto>> GetRoleByIdentifier(long id)
        {
            var mappedRole = await GetRoleAsync<RoleIdentifierDto>(id);
            if (mappedRole == null)
            {
                return NotFound();
            }

            return Ok(mappedRole);
        }

        /// <summary>
        /// Get roles from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="baseResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetRoles<T>(IdResourceParameters baseResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = baseResourceParameters.PageNumber,
                PageSize = baseResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<IdentityRole<Guid>>(baseResourceParameters.OrderBy, "asc");

            var rolesFromManager = _roleManager.Roles.ToList();
            if (rolesFromManager != null)
            {
                // Map EF entity to Dto
                var mappedRoles = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(rolesFromManager),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    rolesFromManager.Count);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedRoles.TotalCount,
                    pageSize = mappedRoles.PageSize,
                    currentPage = mappedRoles.CurrentPage,
                    totalPages = mappedRoles.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedRoles;
            }

            return null;
        }

        /// <summary>
        /// Get single role from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetRoleAsync<T>(long id) where T : class
        {
            var roleFromManager = await _roleManager.FindByIdAsync(id.ToString());

            if (roleFromManager != null)
            {
                // Map EF entity to Dto
                var mappedRole = _mapper.Map<T>(roleFromManager);

                return mappedRole;
            }

            return null;
        }

    }
}
