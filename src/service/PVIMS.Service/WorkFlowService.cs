using Microsoft.AspNetCore.Http;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.Services
{
    public class WorkFlowService : IWorkFlowService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        private readonly IRepositoryInt<Activity> _activityRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }

        public WorkFlowService(IUnitOfWorkInt unitOfWork, 
            ICustomAttributeService attributeService, 
            IPatientService patientService, 
            IRepositoryInt<Activity> activityRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<WorkFlow> workFlowRepository,
            IRepositoryInt<User> userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task AddOrUpdateMedicationsForWorkFlowInstanceAsync(Guid contextGuid, List<ReportInstanceMedicationListItem> medications)
        {
            if(medications == null)
            {
                throw new ArgumentNullException(nameof(medications));
            }
            if (medications.Count == 0) { return; };

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Medications" });
            if (reportInstance == null) {
                return;
            };

            foreach (ReportInstanceMedicationListItem medication in medications)
            {
                if (reportInstance.HasMedication(medication.ReportInstanceMedicationGuid))
                {
                    reportInstance.SetMedicationIdentifier(medication.ReportInstanceMedicationGuid, medication.MedicationIdentifier);
                }
                else
                {
                    reportInstance.AddMedication(medication.MedicationIdentifier, medication.ReportInstanceMedicationGuid);
                }
            }

            _reportInstanceRepository.Update(reportInstance);
            await _unitOfWork.CompleteAsync();
        }

        public async Task CreateWorkFlowInstanceAsync(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier, string facilityIdentifier)
        {
            if (string.IsNullOrWhiteSpace(workFlowName))
            {
                throw new ArgumentException($"'{nameof(workFlowName)}' cannot be null or whitespace.", nameof(workFlowName));
            }

            if (string.IsNullOrWhiteSpace(sourceIdentifier))
            {
                throw new ArgumentException($"'{nameof(sourceIdentifier)}' cannot be null or whitespace.", nameof(sourceIdentifier));
            }

            if (string.IsNullOrWhiteSpace(facilityIdentifier))
            {
                throw new ArgumentException($"'{nameof(facilityIdentifier)}' cannot be null or whitespace.", nameof(facilityIdentifier));
            }

            // Ensure instance does not exist for this context
            var workFlow = await _workFlowRepository.GetAsync(wf => wf.Description == workFlowName, new string[] { "Activities.ExecutionStatuses" });
            if (workFlow == null)
            {
                throw new KeyNotFoundException($"{nameof(workFlowName)} Unable to locate work flow");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await _userRepository.GetAsync(u => u.UserName == userName);
            if (currentUser == null)
            {
                throw new KeyNotFoundException($"Unable to locate current user");
            }

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid);
            if (reportInstance == null)
            {
                reportInstance = new ReportInstance(workFlow, currentUser, contextGuid, patientIdentifier, sourceIdentifier, facilityIdentifier);
                await _reportInstanceRepository.SaveAsync(reportInstance);

                reportInstance.SetSystemIdentifier();

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<ActivityExecutionStatusEvent> ExecuteActivityAsync(Guid contextGuid, string newExecutionStatus, string comments, DateTime? contextDate, string contextCode)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Activities.CurrentStatus", "WorkFlow.Activities.ExecutionStatuses" });
            if (reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate report instance using contextGuid {contextGuid}");
            }

            var newActivityExecutionStatus = await GetExecutionStatusForActivityAsync(reportInstanceFromRepo.CurrentActivity.QualifiedName, reportInstanceFromRepo.WorkFlowId, newExecutionStatus);
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await _userRepository.GetAsync(u => u.UserName == userName);

            var activityExecutionStatusEvent = reportInstanceFromRepo.ExecuteNewEventForActivity(newActivityExecutionStatus, currentUser, comments, contextDate, contextCode);
            _reportInstanceRepository.Update(reportInstanceFromRepo);

            await _unitOfWork.CompleteAsync();

            return activityExecutionStatusEvent;
        }

        public async Task<bool> ValidateExecutionStatusForCurrentActivityAsync(Guid contextGuid, string executionStatusToBeValidated)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Activities" } );
            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate report instance using contextGuid {contextGuid}");
            }    

            var activity = await _activityRepository.GetAsync(a => a.QualifiedName == reportInstanceFromRepo.CurrentActivity.QualifiedName && a.WorkFlow.Id == reportInstanceFromRepo.WorkFlowId, new string[] { "ExecutionStatuses" });
            if (activity == null)
            {
                throw new KeyNotFoundException($"Unable to locate activity using QualifiedName {reportInstanceFromRepo.CurrentActivity.QualifiedName}");
            }

            return activity.ExecutionStatuses.Any(aes => aes.Description == executionStatusToBeValidated);
        }

        public async Task UpdatePatientIdentifierForReportInstanceAsync(Guid contextGuid, string patientIdentifier)
        {
            if (String.IsNullOrWhiteSpace(patientIdentifier))
            {
                throw new ArgumentNullException(nameof(patientIdentifier));
            }

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { 
                "WorkFlow" 
            });
            if(reportInstance == null)
            {
                throw new ArgumentException("reportInstance may not be null");
            }

            reportInstance.SetPatientIdentifier(patientIdentifier);
            _reportInstanceRepository.Update(reportInstance);
        }

        public async Task UpdateSourceIdentifierForReportInstanceAsync(Guid contextGuid, string sourceIdentifier)
        {
            if (String.IsNullOrWhiteSpace(sourceIdentifier))
            {
                throw new ArgumentNullException(nameof(sourceIdentifier));
            }

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { 
                "WorkFlow" 
            });
            if (reportInstance == null)
            {
                throw new ArgumentException("reportInstance may not be null");
            }

            reportInstance.SetSourceIdentifier(sourceIdentifier);
            _reportInstanceRepository.Update(reportInstance);
        }

        private async Task<ActivityExecutionStatus> GetExecutionStatusForActivityAsync(string qualifiedName, int workFlowId, string newExecutionStatus)
        {
            var activity = await _activityRepository.GetAsync(a => a.QualifiedName == qualifiedName && a.WorkFlow.Id == workFlowId, new string[] { "ExecutionStatuses" });
            if (activity == null)
            {
                throw new KeyNotFoundException($"Unable to locate activity using QualifiedName {qualifiedName}");
            }

            return activity.ExecutionStatuses.Single(aes => aes.Description == newExecutionStatus);
        }
    }
}
