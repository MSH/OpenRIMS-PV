using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    public class WorkFlowSummaryQueryHandler
        : IRequestHandler<WorkFlowSummaryQuery, WorkFlowSummaryDto>
    {
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IWorkFlowQueries _workFlowQueries;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<WorkFlowSummaryQueryHandler> _logger;

        public WorkFlowSummaryQueryHandler(
            IRepositoryInt<WorkFlow> workFlowRepository,
            IWorkFlowQueries workFlowQueries,
            IHttpContextAccessor httpContextAccessor,
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            ILogger<WorkFlowSummaryQueryHandler> logger)
        {
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _workFlowQueries = workFlowQueries ?? throw new ArgumentNullException(nameof(workFlowQueries));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WorkFlowSummaryDto> Handle(WorkFlowSummaryQuery message, CancellationToken cancellationToken)
        {
            var workFlowFromRepo = await _workFlowRepository.GetAsync(wf => wf.WorkFlowGuid == message.WorkFlowGuid, new string[] { "Activities" });

            if(workFlowFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate work flow");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName, new string[] { "Facilities.Facility" });
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            var summary = await _workFlowQueries.GetWorkFlowFeedbackSummaryAsync(message.WorkFlowGuid, userFromRepo.Facilities.Select(uf => uf.Facility.FacilityCode).ToList());

            return CreateLinks(summary);
        }

        private WorkFlowSummaryDto CreateLinks(WorkFlowSummaryDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("WorkFlow", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
