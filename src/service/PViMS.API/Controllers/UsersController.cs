using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.UserAggregate;
using MediatR;
using PVIMS.API.Application.Queries.UserAggregate;
using PVIMS.Core.Aggregates.UserAggregate;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator,
            ITypeHelperService typeHelperService,
            IRepositoryInt<User> userRepository,
            ILogger<UsersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all users using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of UserIdentifierDto</returns>
        [HttpGet(Name = "GetUsersByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<UserIdentifierDto>>> GetUsersByIdentifier(
            [FromQuery] UserResourceParameters userResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<UserIdentifierDto>
                (userResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new UsersIdentifierQuery(
                userResourceParameters.OrderBy,
                userResourceParameters.PageNumber,
                userResourceParameters.PageSize,
                userResourceParameters.SearchTerm);

            _logger.LogInformation(
                "----- Sending query: UsersIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = userResourceParameters.PageSize,
                currentPage = userResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single user using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the user</param>
        /// <param name="userRoleResourceParameter">The unique identifier for the user</param>
        /// <returns>An ActionResult of type UserIdentifierDto</returns>
        [HttpGet("{id}/roles", Name = "GetUserRolesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<UserRoleDto[]>> GetUserRolesByIdentifier(int id,
            [FromQuery] IdResourceParameters userRoleResourceParameter)
        {
            if (!_typeHelperService.TypeHasProperties<UserRoleDto>
                (userRoleResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            //return Ok(GetUserRoles(id));
            return Ok();
        }

        /// <summary>
        /// Get a single user using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the user</param>
        /// <returns>An ActionResult of type UserIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetUserByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<UserIdentifierDto>> GetUserByIdentifier(int id)
        {
            var query = new UserIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: UserIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single user using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the user</param>
        /// <returns>An ActionResult of type UserDetailDto</returns>
        [HttpGet("{id}", Name = "GetUserByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UserDetailDto>> GetUserByDetail(int id)
        {
            var query = new UserDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: UserDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userForCreation">The user payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateUser(
            [FromBody] UserForCreationDto userForCreation)
        {
            if (userForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new AddUserCommand(userForCreation.FirstName, userForCreation.LastName, userForCreation.Email, userForCreation.UserName, userForCreation.Password, userForCreation.ConfirmPassword, userForCreation.Roles, userForCreation.Facilities);

            _logger.LogInformation(
                "----- Sending command: AddUserCommand - {userName}",
                command.UserName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetUserByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing user's details
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="userDetailForUpdate">The user payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateUserDetail")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateUserDetail(int id,
            [FromBody] UserDetailForUpdateDto userDetailForUpdate)
        {
            if (userDetailForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeUserDetailCommand( id, userDetailForUpdate.FirstName, userDetailForUpdate.LastName, userDetailForUpdate.Email, userDetailForUpdate.UserName, userDetailForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes, userDetailForUpdate.AllowDatasetDownload == Models.ValueTypes.YesNoValueType.Yes);

            _logger.LogInformation(
                "----- Sending command: ChangeUserDetailCommand - {userId}",
                command.UserId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Add a role to a user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="userRolesForUpdate">The user payload</param>
        /// <returns></returns>
        [HttpPost("{id}/roles", Name = "CreateUserRole")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateUserRole(int id,
            [FromBody] UserRolesForUpdateDto userRolesForUpdate)
        {
            if (userRolesForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new AddRoleToUserCommand(id, userRolesForUpdate.Role);

            _logger.LogInformation(
                $"----- Sending command: AddRoleToUserCommand - {command.UserId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Remove a role from a user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="role">The role to remove</param>
        /// <returns></returns>
        [HttpDelete("{id}/roles/{role}", Name = "RemoveUserRole")]
        [Consumes("application/json")]
        public async Task<IActionResult> RemoveUserRole(int id, string role)
        {
            if (String.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new RemoveRoleFromUserCommand(id, role);

            _logger.LogInformation(
                $"----- Sending command: RemoveRoleFromUserCommand - {command.UserId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Add a facility to a user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="userFacilitiesForUpdate">The user payload</param>
        /// <returns></returns>
        [HttpPost("{id}/facilities", Name = "CreateUserFacility")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateUserFacility(int id,
            [FromBody] UserFacilitiesForUpdateDto userFacilitiesForUpdate)
        {
            if (userFacilitiesForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new AddFacilityToUserCommand(id, userFacilitiesForUpdate.FacilityId);

            _logger.LogInformation(
                $"----- Sending command: AddFacilityToUserCommand - {command.UserId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Remove a facility from a user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="facilityId">The facility to remove</param>
        /// <returns></returns>
        [HttpDelete("{id}/facilities/{facilityId}", Name = "RemoveUserFacility")]
        [Consumes("application/json")]
        public async Task<IActionResult> RemoveUserFacility(int id, int facilityId)
        {
            var command = new RemoveFacilityFromUserCommand(id, facilityId);

            _logger.LogInformation(
                $"----- Sending command: RemoveFacilityFromUserCommand - {command.UserId}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Reset the password for a user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="userForPasswordUpdate">The password payload</param>
        /// <returns></returns>
        [HttpPut("{id}/password", Name = "UpdateUserPassword")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateUserPassword(int id,
            [FromBody] UserForPasswordUpdateDto userForPasswordUpdate)
        {
            if (userForPasswordUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeUserPasswordCommand(id, userForPasswordUpdate.Password);

            _logger.LogInformation(
                "----- Sending command: ChangeUserPasswordCommand - {userId}",
                command.UserId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Confirm that the user has accepted the EULA
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <returns></returns>
        [HttpPut("{id}/accepteula", Name = "AcceptEula")]
        [Consumes("application/json")]
        public async Task<IActionResult> AcceptEula(int id)
        {
            var command = new AcceptEulaCommand(id);

            _logger.LogInformation(
                "----- Sending command: AcceptEulaCommand - {userId}",
                command.UserId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand(id);

            _logger.LogInformation(
                "----- Sending command: DeleteUserCommand - {userId}",
                command.UserId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }
    }
}