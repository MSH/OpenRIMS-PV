using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    public class WorkFlowDetailQueryHandler
        : IRequestHandler<WorkFlowDetailQuery, WorkFlowDetailDto>
    {
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WorkFlowDetailQueryHandler> _logger;

        public WorkFlowDetailQueryHandler(
            IRepositoryInt<WorkFlow> workFlowRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<WorkFlowDetailQueryHandler> logger)
        {
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WorkFlowDetailDto> Handle(WorkFlowDetailQuery message, CancellationToken cancellationToken)
        {
            var workFlowFromRepo = await _workFlowRepository.GetAsync(wf => wf.WorkFlowGuid == message.WorkFlowGuid, new string[] { "Activities" });

            if(workFlowFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate work flow");
            }

            var mappedWorkFlow = _mapper.Map<WorkFlowDetailDto>(workFlowFromRepo);

            await CustomMapAsync(workFlowFromRepo, mappedWorkFlow);

            return CreateLinks(mappedWorkFlow);
        }

        private async Task CustomMapAsync(WorkFlow workFlowFromRepo, WorkFlowDetailDto mappedWorkFlowDto)
        {
            mappedWorkFlowDto.NewReportInstanceCount = await GetNewReportInstancesCountAsync(mappedWorkFlowDto.WorkFlowGuid);

            await PrepareAnalysisActivity(workFlowFromRepo, mappedWorkFlowDto);
            await PrepareFeedbackActivity(workFlowFromRepo, mappedWorkFlowDto);
        }

        private async Task<int> GetNewReportInstancesCountAsync(Guid workFlowGuid)
        {
            var compareDate = await PrepareComparisonForNewReportDateAsync();

            var predicate = PredicateBuilder.New<ReportInstance>(true);
            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
            predicate = predicate.And(f => f.Created >= compareDate);

            var reportInstancesFromRepo = await _reportInstanceRepository.ListAsync(predicate, null, new string[] { "" });

            return reportInstancesFromRepo.Count;
        }

        private async Task<DateTime> PrepareComparisonForNewReportDateAsync()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            if (config == null)
            {
                return DateTime.Now.AddDays(-1);
            }

            if (String.IsNullOrEmpty(config.ConfigValue))
            {
                return DateTime.Now.AddDays(-1);
            }

            var alertCount = Convert.ToInt32(config.ConfigValue);

            return DateTime.Now.AddDays(alertCount * -1);
        }

        private async Task PrepareAnalysisActivity(WorkFlow workFlowFromRepo, WorkFlowDetailDto mappedWorkFlowDto)
        {
            List<ActivitySummaryDto> analysisActivity = new List<ActivitySummaryDto>();

            foreach (var activity in workFlowFromRepo.Activities.OrderBy(a => a.Id))
            {
                analysisActivity.Add(new ActivitySummaryDto()
                {
                    QualifiedName = activity.QualifiedName,
                    ReportInstanceCount = await GetReportInstancesCountForAnalysisAsync(mappedWorkFlowDto.WorkFlowGuid, activity.QualifiedName)
                });
            }

            mappedWorkFlowDto.AnalysisActivity = analysisActivity.ToArray();
        }

        private async Task<int> GetReportInstancesCountForAnalysisAsync(Guid workFlowGuid, string qualifiedName)
        {
            var predicate = PredicateBuilder.New<ReportInstance>(true);
            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);

            switch (qualifiedName)
            {
                case "Confirm Report Data":
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true && a.CurrentStatus.Description != "DELETED"));
                    break;

                case "Extract E2B":
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true && a.CurrentStatus.Description != "E2BSUBMITTED"));
                    break;

                default:
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true));
                    break;
            }

            var reportInstancesFromRepo = await _reportInstanceRepository.ListAsync(predicate, null, new string[] { "" });

            return reportInstancesFromRepo.Count;
        }

        private async Task PrepareFeedbackActivity(WorkFlow workFlowFromRepo, WorkFlowDetailDto mappedWorkFlowDto)
        {
            List<ActivitySummaryDto> feedbackActivity = new List<ActivitySummaryDto>();

            foreach (var activity in workFlowFromRepo.Activities)
            {
                feedbackActivity.Add(new ActivitySummaryDto()
                {
                    QualifiedName = activity.QualifiedName,
                    ReportInstanceCount = await GetReportInstancesCountForFeedbackAsync(mappedWorkFlowDto.WorkFlowGuid, activity.QualifiedName)
                });
            }

            mappedWorkFlowDto.FeedbackActivity = feedbackActivity.ToArray();
        }

        private async Task<int> GetReportInstancesCountForFeedbackAsync(Guid workFlowGuid, string qualifiedName)
        {
            var predicate = PredicateBuilder.New<ReportInstance>(true);
            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);

            predicate = await AssertFilterOnStage(qualifiedName, predicate);
            predicate = await AssertFacilityPermissions(predicate);

            var reportInstancesFromRepo = await _reportInstanceRepository.ListAsync(predicate, null, new string[] { "Activities.CurrentStatus", "Tasks" });

            return reportInstancesFromRepo.Count;
        }

        private async Task<ExpressionStarter<ReportInstance>> AssertFilterOnStage(string qualifiedName, ExpressionStarter<ReportInstance> predicate)
        {
            var compareDate = await PrepareComparisonForFeedbackReportDateAsync();

            switch (qualifiedName)
            {
                case "Confirm Report Data":
                    predicate = predicate.And(ri => ri.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true && a.CurrentStatus.Description != "DELETED") && ri.Tasks.Any(t => t.TaskStatusId != Core.Aggregates.ReportInstanceAggregate.TaskStatus.Cancelled.Id && t.TaskStatusId != Core.Aggregates.ReportInstanceAggregate.TaskStatus.Completed.Id));
                    break;

                case "Set MedDRA and Causality":
                    predicate = predicate.And(ri => ri.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == false && a.CurrentStatus.Description == "CAUSALITYCONFIRMED" && a.LastUpdated >= compareDate));
                    break;

                case "Extract E2B":
                    predicate = predicate.And(ri => ri.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true && a.CurrentStatus.Description == "E2BSUBMITTED" && a.LastUpdated >= compareDate));
                    break;
            }

            return predicate;
        }

        private async Task<ExpressionStarter<ReportInstance>> AssertFacilityPermissions(ExpressionStarter<ReportInstance> predicate)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName, new string[] { "Facilities.Facility" });
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }
            var validfacilities = userFromRepo.Facilities.Select(uf => uf.Facility.FacilityCode).ToArray();
            predicate = predicate.And(ri => validfacilities.Contains(ri.FacilityIdentifier));
            return predicate;
        }

        private async Task<DateTime> PrepareComparisonForFeedbackReportDateAsync()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceFeedbackAlertCount);
            if (config == null)
            {
                return DateTime.Now.AddDays(-1);
            }

            if (String.IsNullOrEmpty(config.ConfigValue))
            {
                return DateTime.Now.AddDays(-1);
            }

            var alertCount = Convert.ToInt32(config.ConfigValue);

            return DateTime.Now.AddDays(alertCount * -1);
        }

        private WorkFlowDetailDto CreateLinks(WorkFlowDetailDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("WorkFlow", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
