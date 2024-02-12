using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Models;

namespace OpenRIMS.PV.Main.Core.Services
{
    public interface IWorkFlowService
    {
        Task AddOrUpdateMedicationsForWorkFlowInstanceAsync(Guid contextGuid, List<ReportInstanceMedicationListItem> medications);

        Task CreateWorkFlowInstanceAsync(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier, string facilityIdentifier);

        Task<ActivityExecutionStatusEvent> ExecuteActivityAsync(Guid contextGuid, string newExecutionStatus, string comments, DateTime? contextDate, string contextCode);

        Task<bool> ValidateExecutionStatusForCurrentActivityAsync(Guid contextGuid, string executionStatusToBeValidated);

        Task UpdatePatientIdentifierForReportInstanceAsync(Guid contextGuid, string patientIdentifier);

        Task UpdateSourceIdentifierForReportInstanceAsync(Guid contextGuid, string sourceIdentifier);
    }
}
