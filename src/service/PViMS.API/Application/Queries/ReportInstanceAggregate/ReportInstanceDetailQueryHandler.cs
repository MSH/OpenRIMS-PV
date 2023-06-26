using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    public class ReportInstanceDetailQueryHandler
        : IRequestHandler<ReportInstanceDetailQuery, ReportInstanceDetailDto>
    {
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportInstanceDetailQueryHandler> _logger;

        public ReportInstanceDetailQueryHandler(
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ReportInstanceDetailQueryHandler> logger)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ReportInstanceDetailDto> Handle(ReportInstanceDetailQuery message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                && f.Id == message.ReportInstanceId, 
                new string[] { "WorkFlow", "TerminologyMedDra", "Activities.CurrentStatus", "Activities.ExecutionEvents.ExecutionStatus", "Activities.ExecutionEvents.Attachments", "Tasks", "Medications" });

            if (reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var mappedReport = _mapper.Map<ReportInstanceDetailDto>(reportInstanceFromRepo);

            await CustomMapAsync(reportInstanceFromRepo, mappedReport);
            await CreateLinksAsync(reportInstanceFromRepo, mappedReport);

            return mappedReport;
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
                mappedReportInstance.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus",
                    reportInstanceFromRepo.WorkFlow.WorkFlowGuid, mappedReportInstance.Id), "confirm", "PUT"));
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
    }
}
