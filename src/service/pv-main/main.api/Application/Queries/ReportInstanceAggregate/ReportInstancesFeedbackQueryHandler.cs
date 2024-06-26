﻿using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    public class ReportInstancesFeedbackQueryHandler
        : IRequestHandler<ReportInstancesFeedbackQuery, LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>
    {
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReportInstancesFeedbackQueryHandler> _logger;

        public ReportInstancesFeedbackQueryHandler(
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReportInstancesFeedbackQueryHandler> logger)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>> Handle(ReportInstancesFeedbackQuery message, CancellationToken cancellationToken)
        {
            return await GetReportInstancesAsync(message.WorkFlowGuid, message.PageNumber, message.PageSize, message.QualifiedName);
        }

        private async Task<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>> GetReportInstancesAsync(
            Guid workFlowGuid, 
            int pageNumber, 
            int pageSize,
            string qualifiedName)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var orderby = Extensions.GetOrderBy<ReportInstance>("Created", "desc");
            var predicate = await PreparePredicateForFilter(workFlowGuid, qualifiedName);

            var pagedReportsFromRepo = _reportInstanceRepository.List(pagingInfo, predicate, orderby, new string[] { "WorkFlow", "Medications", "TerminologyMedDra", "Activities.CurrentStatus", "Activities.ExecutionEvents.ExecutionStatus", "Tasks" });
            if (pagedReportsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedReportsWithLinks = new List<ReportInstanceDetailDto>();

                foreach (var pagedReport in pagedReportsFromRepo)
                {
                    var mappedReport = _mapper.Map<ReportInstanceDetailDto>(pagedReport);

                    await CustomMapAsync(pagedReport, mappedReport);
                    await CreateLinksAsync(pagedReport, mappedReport);

                    mappedReportsWithLinks.Add(mappedReport);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>(pagedReportsFromRepo.TotalCount, mappedReportsWithLinks, pagedReportsFromRepo.TotalPages);

                CreateLinksForReportInstances(workFlowGuid, wrapper, "Created", "", DateTime.MinValue, DateTime.MaxValue, pageNumber, pageSize,
                    pagedReportsFromRepo.HasNext, pagedReportsFromRepo.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private async Task<ExpressionStarter<ReportInstance>> PreparePredicateForFilter(Guid workFlowGuid, string qualifiedName)
        {
            var predicate = PredicateBuilder.New<ReportInstance>(true);
            predicate = predicate.And(r => r.WorkFlow.WorkFlowGuid == workFlowGuid);

            predicate = await AssertFilterOnStage(qualifiedName, predicate);
            predicate = await AssertFacilityPermissions(predicate);

            return predicate;
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

        private async Task CustomMapAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto mappedReportInstanceDto)
        {
            if (reportInstanceFromRepo == null)
            {
                throw new ArgumentNullException(nameof(reportInstanceFromRepo));
            }

            await MapIdsForReportInstanceAsync(reportInstanceFromRepo, mappedReportInstanceDto);
            await MapE2BActivitiesForReportInstanceAsync(reportInstanceFromRepo, mappedReportInstanceDto);

            if (reportInstanceFromRepo.WorkFlow.Description == "New Spontaneous Surveilliance Report")
            {
                await MapSpontaneousInstanceForReportInstanceAsync(reportInstanceFromRepo.ContextGuid, mappedReportInstanceDto);
            }

            foreach (var medication in mappedReportInstanceDto.Medications)
            {
                await CustomReportInstanceMedicationMapAsync(reportInstanceFromRepo, medication);
            }
        }

        private async Task MapIdsForReportInstanceAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto dto)
        {
            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(p => p.PatientClinicalEventGuid == reportInstanceFromRepo.ContextGuid, new string[] { "Patient" });
            dto.PatientId = patientClinicalEvent != null ? patientClinicalEvent.Patient.Id : 0;
            dto.PatientClinicalEventId = patientClinicalEvent != null ? patientClinicalEvent.Id : 0;
        }

        private async Task MapE2BActivitiesForReportInstanceAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto dto)
        {
            switch (reportInstanceFromRepo.CurrentActivity.QualifiedName)
            {
                case "Extract E2B":
                    if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "E2BINITIATED")
                    {
                        await MapE2BInstanceForReportInstanceAsync(reportInstanceFromRepo, dto);
                    }

                    if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "E2BGENERATED")
                    {
                        MapE2BAttachmentForReportInstance(reportInstanceFromRepo, dto);
                    }

                    break;

                default:
                    break;
            }
        }

        private async Task MapE2BInstanceForReportInstanceAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto dto)
        {
            var latestExecutionEvent = reportInstanceFromRepo.CurrentActivity.GetLatestEvent();
            if (latestExecutionEvent != null)
            {
                var tag = reportInstanceFromRepo.WorkFlow.Description == "New Active Surveilliance Report" ? "Active" : "Spontaneous";
                var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.Tag == tag && di.ContextId == latestExecutionEvent.Id, new string[] { "Dataset" });
                if (datasetInstance != null)
                {
                    dto.E2BInstance = new DatasetInstanceDto()
                    {
                        DatasetId = datasetInstance.Dataset.Id,
                        DatasetInstanceId = datasetInstance.Id
                    };
                }
            }
        }

        private void MapE2BAttachmentForReportInstance(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto dto)
        {
            var latestE2BGeneratedExecutionEvent = reportInstanceFromRepo.CurrentActivity.GetLatestE2BGeneratedEvent();
            if (latestE2BGeneratedExecutionEvent != null)
            {
                var e2bAttachment = latestE2BGeneratedExecutionEvent.Attachments.SingleOrDefault(att => att.Description == "E2b");
                if (e2bAttachment != null)
                {
                    dto.ActivityExecutionStatusEventId = latestE2BGeneratedExecutionEvent.Id;
                    dto.AttachmentId = e2bAttachment.Id;
                }
            }
        }

        private async Task MapSpontaneousInstanceForReportInstanceAsync(Guid contextGuid, ReportInstanceDetailDto dto)
        {
            var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == contextGuid, new string[] { "Dataset" });
            if (datasetInstanceFromRepo != null)
            {
                dto.SpontaneousInstance = new DatasetInstanceDto()
                {
                    DatasetId = datasetInstanceFromRepo.Dataset.Id,
                    DatasetInstanceId = datasetInstanceFromRepo.Id
                };
            }
        }

        private async Task CustomReportInstanceMedicationMapAsync(ReportInstance reportInstanceFromRepo, ReportInstanceMedicationDetailDto dto)
        {
            if (reportInstanceFromRepo.WorkFlow.Description == "New Spontaneous Surveilliance Report")
            {
                var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == reportInstanceFromRepo.ContextGuid);
                if (datasetInstanceFromRepo != null)
                {
                    var drugItemValues = datasetInstanceFromRepo.GetInstanceSubValues("Product Information", dto.ReportInstanceMedicationGuid);
                    var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product")?.InstanceValue;
                    DateTime tempdt;

                    var startElement = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                    var stopElement = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");

                    dto.StartDate = startElement != null ? DateTime.TryParse(startElement.InstanceValue, out tempdt) ? Convert.ToDateTime(startElement.InstanceValue).ToString("yyyy-MM-dd") : "" : "";
                    dto.EndDate = stopElement != null ? DateTime.TryParse(stopElement.InstanceValue, out tempdt) ? Convert.ToDateTime(stopElement.InstanceValue).ToString("yyyy-MM-dd") : "" : "";
                }
            }
            else
            {
                var medication = await _patientMedicationRepository.GetAsync(p => p.PatientMedicationGuid == dto.ReportInstanceMedicationGuid);
                if (medication == null)
                {
                    return;
                }

                dto.StartDate = medication.StartDate.ToString("yyyy-MM-dd");
                dto.EndDate = medication.EndDate.HasValue ? medication.EndDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }

        private async Task CreateLinksAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto mappedReportInstance)
        {
            if (reportInstanceFromRepo == null)
            {
                throw new ArgumentNullException(nameof(reportInstanceFromRepo));
            }

            mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateReportInstanceResourceUri(reportInstanceFromRepo.WorkFlow.WorkFlowGuid, mappedReportInstance.Id), "self", "GET"));

            switch (reportInstanceFromRepo.CurrentActivity.QualifiedName)
            {
                case "Confirm Report Data":
                    CreateLinksForConfirmationStep(reportInstanceFromRepo, mappedReportInstance);
                    break;

                case "Set MedDRA and Causality":
                    await CreateLinksForTerminologyStepAsync(reportInstanceFromRepo, mappedReportInstance);
                    break;

                case "Extract E2B":
                    await CreateLinksForE2BStepAsync(reportInstanceFromRepo, mappedReportInstance);
                    break;

                default:
                    break;
            }

            var validRoles = new string[] { "RegClerk", "DataCap", "Clinician" };
            if (reportInstanceFromRepo.WorkFlow.Description == "New Active Surveilliance Report")
            {
                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", reportInstanceFromRepo.Id), "viewpatient", "GET"));
            }

            if (reportInstanceFromRepo.WorkFlow.Description == "New Spontaneous Surveilliance Report")
            {
                var datasetInstance = _datasetInstanceRepository.Get(di => di.DatasetInstanceGuid == reportInstanceFromRepo.ContextGuid);
                if (datasetInstance != null)
                {
                    mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDatasetInstanceResourceUri(
                        datasetInstance.Dataset.Id, datasetInstance.Id), "updatespont", "PUT"));
                }
            }
        }

        private void CreateLinksForConfirmationStep(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto mappedReportInstance)
        {
            if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "UNCONFIRMED")
            {
                if(!reportInstanceFromRepo.HasActiveTasks())
                {
                    mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                        reportInstanceFromRepo.WorkFlow.WorkFlowGuid, mappedReportInstance.Id), "confirm", "PUT"));
                }
                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                    reportInstanceFromRepo.WorkFlow.WorkFlowGuid, mappedReportInstance.Id), "delete", "PUT"));
            }
        }

        private async Task CreateLinksForTerminologyStepAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto mappedReportInstance)
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.AssessmentScale);
            if (config == null)
            {
                return;
            };

            mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceTerminology",
                reportInstanceFromRepo.WorkFlow.WorkFlowGuid, mappedReportInstance.Id), "setmeddra", "PUT"));

            if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description != "NOTSET")
            {
                // ConfigType.AssessmentScale

                if (config.ConfigValue == "Both Scales" || config.ConfigValue == "WHO Scale")
                {
                    mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                        reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id), "whocausalityset", "PUT"));
                }

                if (config.ConfigValue == "Both Scales" || config.ConfigValue == "Naranjo Scale")
                {
                    mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                        reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id), "naranjocausalityset", "PUT"));
                }

                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                    reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id), "causalityset", "PUT"));
            }
        }

        private async Task CreateLinksForE2BStepAsync(ReportInstance reportInstanceFromRepo, ReportInstanceDetailDto mappedReportInstance)
        {
            if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "NOTGENERATED"
                || reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "E2BSUBMITTED")
            {
                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("CreateE2BInstance",
                    reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id), "createe2b", "PUT"));
            }

            if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "E2BINITIATED")
            {
                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                    reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id), "preparereporte2b", "PUT"));

                var evt = reportInstanceFromRepo.CurrentActivity.ExecutionEvents
                    .OrderByDescending(ee => ee.EventDateTime)
                    .First(ee => ee.ExecutionStatus.Id == reportInstanceFromRepo.CurrentActivity.CurrentStatus.Id);
                var tag = (reportInstanceFromRepo.WorkFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

                var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.Tag == tag && di.ContextId == evt.Id);
                if (datasetInstance != null)
                {
                    mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDatasetInstanceResourceUri(datasetInstance.Dataset.Id, datasetInstance.Id), "updatee2b", "PUT"));
                }
            }

            if (reportInstanceFromRepo.CurrentActivity.CurrentStatus.Description == "E2BGENERATED")
            {
                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                    reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id), "confirmsubmissione2b", "PUT"));

                var executionEvent = reportInstanceFromRepo.CurrentActivity.ExecutionEvents
                    .OrderByDescending(ee => ee.EventDateTime)
                    .First(ee => ee.ExecutionStatus.Description == "E2BGENERATED");
                if (executionEvent != null)
                {
                    var e2bAttachment = executionEvent.Attachments.SingleOrDefault(att => att.Description == "E2b");
                    if (e2bAttachment != null)
                    {
                        mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateDownloadActivitySingleAttachmentResourceUri(
                            reportInstanceFromRepo.WorkFlow.WorkFlowGuid, reportInstanceFromRepo.Id, executionEvent.Id, e2bAttachment.Id), "downloadxml", "GET"));
                    }
                }
            }
        }

        private void CreateLinksForReportInstances(Guid workFlowGuid,
            LinkedResourceBaseDto wrapper, string orderBy, string qualifiedName, DateTime searchFrom, DateTime searchTo, int pageNumber, int pageSize, bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateReportInstancesResourceUri(workFlowGuid, ResourceUriType.Current, orderBy, qualifiedName, searchFrom, searchTo, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportInstancesResourceUri(workFlowGuid, ResourceUriType.NextPage, orderBy, qualifiedName, searchFrom, searchTo, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportInstancesResourceUri(workFlowGuid, ResourceUriType.PreviousPage, orderBy, qualifiedName, searchFrom, searchTo, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
