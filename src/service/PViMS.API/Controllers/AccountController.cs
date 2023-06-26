using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.NotificationAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Account;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using PVIMS.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenFactory _tokenFactory;
        private readonly IJwtFactory _jwtFactory;
        private readonly IRepositoryInt<RefreshToken> _refreshTokenRepository;
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator,
            ITokenFactory tokenFactory,
            IJwtFactory jwtFactory,
            IRepositoryInt<RefreshToken> refreshTokenRepository,
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
            _jwtFactory = jwtFactory ?? throw new ArgumentNullException(nameof(jwtFactory));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ping service to check if API endpoint is available
        /// </summary>
        [HttpGet("ping", Name = "Ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Ping()
        {
            return Ok();
        }

        /// <summary>
        /// Ping service to check if API endpoint is available (must be authenticated)
        /// </summary>
        [HttpGet("pingauth", Name = "PingAuth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
        public ActionResult PingAuth()
        {
            return Ok();
        }

        /// <summary>
        /// Authentication provider
        /// </summary>
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes("application/json")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            [FromBody] LoginRequestDto request)
        {
            if (request == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for login request");
                return BadRequest(ModelState);
            }

            var userFromManager = await _userManager.FindByNameAsync(request.UserName);
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == request.UserName, new string[] { "Facilities.Facility" });
            if (userFromManager != null && userFromRepo != null)
            {
                if (await _userManager.CheckPasswordAsync(userFromManager, request.Password))
                {
                    if (userFromRepo.Active)
                    {

                        var audit = new AuditLog()
                        {
                            AuditType = AuditType.UserLogin,
                            User = userFromRepo,
                            ActionDate = DateTime.Now,
                            Details = "User logged in to PViMS"
                        };
                        _auditLogRepository.Save(audit);

                        //var isAdmin = IsAdmin(user);
                        //if (!isAdmin) return RedirectToLocal(returnUrl);
                        //var pendingScriptsExist = AnyPendingScripts();

                        //// Send user to deployment page
                        //if (pendingScriptsExist)
                        //{
                        //    return RedirectToAction("Index", "Deployment");
                        //}

                        var refreshToken = _tokenFactory.GenerateToken();

                        userFromRepo.AddRefreshToken(refreshToken, HttpContext?.Connection?.RemoteIpAddress?.ToString());
                        _userRepository.Update(userFromRepo);
                        await _unitOfWork.CompleteAsync();

                        return Ok(new LoginResponseDto(await _jwtFactory.GenerateEncodedToken(userFromRepo, await _userManager.GetRolesAsync(userFromManager)), refreshToken, userFromRepo.EulaAcceptanceDate == null, userFromRepo.AllowDatasetDownload));
                    }
                    else 
                    {
                        ModelState.AddModelError("Message", "User is not active.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "Invalid password specified.");
                }
            }
            else
            {
                ModelState.AddModelError("Message", "Invalid username specified.");
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Refresh user token
        /// </summary>
        /// <remarks>Expired access tokens are valid. Refresh token can only be used once in order to obtain a new access token.</remarks>
        /// <param name="request">Exchange refresh token request model</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost("refreshtoken", Name = "RefreshToken")]
        public async Task<ActionResult<ExchangeRefreshTokenResponseModel>> RefreshToken([FromBody] ExchangeRefreshTokenRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for refresh token request");
                return BadRequest(ModelState);
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromManager = await _userManager.FindByNameAsync(userName);
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);

            if (userFromManager != null && userFromRepo != null)
            {
                if (userFromManager.Active == false)
                {
                    return StatusCode(401, "User no longer active");
                }

                if (userFromRepo.HasValidRefreshToken(request.RefreshToken))
                {

                    var jwtToken = await _jwtFactory.GenerateEncodedToken(userFromRepo, await _userManager.GetRolesAsync(userFromManager));

                    // delete existing refresh token
                    _refreshTokenRepository.Delete(userFromRepo.RefreshTokens.Single(a => a.Token == request.RefreshToken));

                    // generate refresh token
                    var refreshToken = _tokenFactory.GenerateToken();
                    userFromRepo.AddRefreshToken(refreshToken, HttpContext?.Connection?.RemoteIpAddress?.ToString());

                    _userRepository.Update(userFromRepo);
                    await _unitOfWork.CompleteAsync();

                    return new ExchangeRefreshTokenResponseModel() { AccessToken = jwtToken, RefreshToken = refreshToken };
                }
            }

            return StatusCode(404, "User does not have valid refresh token");
        }

        /// <summary>
        /// Get all notifications using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of NotificationDto</returns>
        [HttpGet("notifications", Name = "GetNotifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            var query = new NotificationsQuery(userFromRepo.Id);

            _logger.LogInformation(
                "----- Sending query: NotificationsQuery - {userId}",
                userFromRepo.Id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);

            //if (await _userManager.IsInRoleAsync(userFromManager, Constants.Role.Clinician))
            //{
            //    notifications.AddRange(PrepareNotificationsForClinician(compareDate));
            //}
            //if (await _userManager.IsInRoleAsync(userFromManager, Constants.Role.PVSpecialist))
            //{
            //    notifications.AddRange(PrepareNotificationsForAnalyst(compareDate));
            //}
        }

        /// <summary>
        ///  Prepare notification for active reports
        /// </summary>
        /// <param name="compDate">The paramterised date to check against</param>
        /// <returns></returns>
        private NotificationDto CreateNotificationForActiveReports(DateTime compDate)
        {
            //var workFlowGuid = new Guid("892F3305-7819-4F18-8A87-11CBA3AEE219");

            //var predicate = PredicateBuilder.New<ReportInstance>(true);

            //predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
            //predicate = predicate.And(f => f.Created >= compDate);

            //var newAnalyserNotificationCount = _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
            //if (newAnalyserNotificationCount > 0)
            //{
            //    return new NotificationDto()
            //    {
            //        Identifier = "ActiveNotification",
            //        Color = "primary",
            //        Icon = "timer",
            //        Message = $"New ACTIVE reports for analysis ({newAnalyserNotificationCount})",
            //        Route = $"/analytical/reportsearch/{workFlowGuid.ToString().ToUpper()}",
            //        Time = DateTime.Now.ToString()
            //    };
            //}
            return null;
        }

        /// <summary>
        ///  Prepare notification for spontaneous reports
        /// </summary>
        /// <param name="compDate">The paramterised date to check against</param>
        /// <returns></returns>
        private NotificationDto CreateNotificationForSpontaneousReports(DateTime compDate)
        {
            //var workFlowGuid = new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986");

            //var predicate = PredicateBuilder.New<ReportInstance>(true);
            //predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
            //predicate = predicate.And(f => f.Created >= compDate);

            //var newAnalyserNotificationCount = _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
            //if (newAnalyserNotificationCount > 0)
            //{
            //    return new NotificationDto()
            //    {
            //        Identifier = "SpontaneousNotification",
            //        Color = "primary",
            //        Icon = "timer",
            //        Message = $"New SPONTANEOUS reports for analysis ({newAnalyserNotificationCount})",
            //        Route = $"/analytical/reportsearch/{workFlowGuid.ToString().ToUpper()}",
            //        Time = DateTime.Now.ToString()
            //    };
            //}
            return null;
        }
    }
}
