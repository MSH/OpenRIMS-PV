using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PVIMS.API.Helpers;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Models.ValueTypes;
using PVIMS.Core.ValueTypes;
using System;

namespace PVIMS.API.Infrastructure.Services
{
    public class LinkGeneratorService : ILinkGeneratorService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _linkGenerator;

        public LinkGeneratorService(IHttpContextAccessor accessor,
            LinkGenerator linkGenerator)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }


        public string CreateIdResourceUriForWrapper(ResourceUriType type,
            string actionName,
            string orderBy, 
            int pageNumber, 
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName, 
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber - 1,
                          pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber + 1,
                          pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          orderBy,
                          pageNumber,
                          pageSize
                      });
            }
        }

        public string CreateReportResourceUriForWrapper(ResourceUriType type,
            string actionName,
            int pageNumber,
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          pageNumber = pageNumber,
                          pageSize = pageSize
                      });
            }
        }

        public string CreateResourceUri(string resourceName, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"Get{resourceName}ByIdentifier",
              new { id = resourceId });
        }

        public string CreateResourceUriForReportInstance(string actionName, Guid workFlowGuid, long reportInstanceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
              new { workFlowGuid, id = reportInstanceId });
        }

        public string CreateDeleteResourceUri(string resourceName, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"Delete{resourceName}",
              new { id = resourceId });
        }

        public string CreateAdverseEventReportResourceUri(ResourceUriType type,
            int pageNumber,
            int pageSize,
            AdverseEventStratifyCriteria adverseEventStratifyCriteria)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAdverseEventReport",
                      new
                      {
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize,
                          AdverseEventStratifyCriteria = adverseEventStratifyCriteria
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAdverseEventReport",
                      new
                      {
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize,
                          AdverseEventStratifyCriteria = adverseEventStratifyCriteria
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAdverseEventReport",
                      new
                      {
                          pageNumber = pageNumber,
                          pageSize = pageSize,
                          AdverseEventStratifyCriteria = adverseEventStratifyCriteria
                      });
            }
        }

        public string CreateAnalyserTermSetsResourceUri(Guid workFlowGuid, 
            ResourceUriType type,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermsByIdentifier",
                      new
                      {
                          workFlowGuid,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber - 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermsByIdentifier",
                      new
                      {
                          workFlowGuid,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber + 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermsByIdentifier",
                      new
                      {
                          workFlowGuid,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
            }
        }

        public string CreateAnalyserTermPatientsResourceUri(Guid workFlowGuid,
            int termId,
            ResourceUriType type,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermPatients",
                      new
                      {
                          workFlowGuid,
                          id = termId,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber - 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermPatients",
                      new
                      {
                          workFlowGuid,
                          id = termId,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber + 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermPatients",
                      new
                      {
                          workFlowGuid,
                          id = termId,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
            }
        }

        public string CreateAppointmentsResourceUri(ResourceUriType type,
            int pageNumber,
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAppointmentsByDetail",
                      new
                      {
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAppointmentsForSearch",
                      new
                      {
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAppointmentsForSearch",
                      new
                      {
                          pageNumber = pageNumber,
                          pageSize = pageSize
                      });
            }
        }

        public string CreateAuditLogsResourceUri(ResourceUriType type,
            AuditLogResourceParameters auditLogResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAuditLogsByIdentifier",
                      new
                      {
                          orderBy = auditLogResourceParameters.OrderBy,
                          auditType = auditLogResourceParameters.AuditType,
                          searchFrom = auditLogResourceParameters.SearchFrom,
                          searchTo = auditLogResourceParameters.SearchTo,
                          facilityId = auditLogResourceParameters.FacilityId,
                          pageNumber = auditLogResourceParameters.PageNumber - 1,
                          pageSize = auditLogResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAuditLogsByIdentifier",
                      new
                      {
                          orderBy = auditLogResourceParameters.OrderBy,
                          auditType = auditLogResourceParameters.AuditType,
                          searchFrom = auditLogResourceParameters.SearchFrom,
                          searchTo = auditLogResourceParameters.SearchTo,
                          facilityId = auditLogResourceParameters.FacilityId,
                          pageNumber = auditLogResourceParameters.PageNumber + 1,
                          pageSize = auditLogResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAuditLogsByIdentifier",
                      new
                      {
                          orderBy = auditLogResourceParameters.OrderBy,
                          auditType = auditLogResourceParameters.AuditType,
                          searchFrom = auditLogResourceParameters.SearchFrom,
                          searchTo = auditLogResourceParameters.SearchTo,
                          facilityId = auditLogResourceParameters.FacilityId,
                          pageNumber = auditLogResourceParameters.PageNumber,
                          pageSize = auditLogResourceParameters.PageSize
                      });
            }
        }

        public string CreateCausalityReportResourceUri(Guid workFlowGuid, 
            ResourceUriType type,
            int pageNumber,
            int pageSize,
            int facilityId,
            CausalityCriteria causalityCriteria)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize,
                          FacilityId = facilityId,
                          CausalityCriteria = causalityCriteria
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize,
                          FacilityId = facilityId,
                          CausalityCriteria = causalityCriteria
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = pageNumber,
                          pageSize = pageSize,
                          FacilityId = facilityId,
                          CausalityCriteria = causalityCriteria
                      });
            }
        }

        public string CreateConceptsResourceUri(ResourceUriType type,
            string orderBy,
            string searchTerm,
            YesNoBothValueType active,
            int pageNumber,
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConceptsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber - 1,
                          pageSize,
                          searchTerm,
                          active
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConceptsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber + 1,
                          pageSize,
                          searchTerm,
                          active
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConceptsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber,
                          pageSize,
                          searchTerm,
                          active
                      });
            }
        }

        public string CreateConditionsResourceUri(ResourceUriType type,
           ConditionResourceParameters conditionResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber - 1,
                          pageSize = conditionResourceParameters.PageSize,
                          Active = conditionResourceParameters.Active
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber + 1,
                          pageSize = conditionResourceParameters.PageSize,
                          Active = conditionResourceParameters.Active
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber,
                          pageSize = conditionResourceParameters.PageSize,
                          Active = conditionResourceParameters.Active
                      });
            }
        }

        public string CreateCustomAttributesResourceUri(ResourceUriType type,
           CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber - 1,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber + 1,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
            }
        }

        public string CreateDatasetCategoryResourceUri(long datasetid, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetDatasetCategoryByIdentifier",
              new { datasetid, id = resourceId });
        }

        public string CreateDatasetCategoryElementResourceUri(long datasetid, long datasetCategoryId, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetDatasetCategoryElementByIdentifier",
              new { datasetid, datasetCategoryId, id = resourceId });
        }

        public string CreateDownloadActivitySingleAttachmentResourceUri(Guid workFlowGuid, long reportinstanceId, long activityExecutionStatusEventId, long attachmentId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"DownloadActivitySingleAttachment",
              new { workFlowGuid, reportinstanceId, activityExecutionStatusEventId, id = attachmentId });
        }

        public string CreateEncountersResourceUri(ResourceUriType type,
           string orderBy,
           string facilityName,
           int pageNumber,
           int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetEncountersByDetail",
                      new
                      {
                          orderBy,
                          facilityName,
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetEncountersByDetail",
                      new
                      {
                          orderBy,
                          facilityName,
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetEncountersByDetail",
                      new
                      {
                          orderBy,
                          facilityName,
                          pageNumber = pageNumber,
                          pageSize = pageSize
                      });
            }
        }

        public string CreateEncounterForPatientResourceUri(long patientId, long encounterId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetEncounterForPatientByIdentifier",
              new { patientId, id = encounterId });
        }

        public string CreateEnrolmentForPatientResourceUri(long patientId, long enrolmentId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetEnrolmentForPatient",
              new { patientId, id = enrolmentId });
        }

        public string CreateMedicationFormsResourceUri(ResourceUriType type,
            string orderBy,
            int pageNumber,
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetMedicationFormsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber - 1,
                          pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetMedicationFormsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber + 1,
                          pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetMedicationFormsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber,
                          pageSize
                      });
            }
        }

        public string CreateMetaWidgetResourceUri(long metaPageId, long metaWidgetId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetMetaWidgetByIdentifier",
              new { metaPageId, id = metaWidgetId });
        }

        public string CreateNewAppointmentForPatientResourceUri(long patientId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"CreatePatientAppointment",
              new { patientId });
        }

        public string CreateNewEnrolmentForPatientResourceUri(long patientId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"CreatePatientEnrolment",
              new { patientId });
        }

        public string CreateOutstandingVisitReportResourceUri(ResourceUriType type,
            int pageNumber,
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = pageNumber,
                          pageSize = pageSize
                      });
            }
        }

        public string CreatePatientAppointmentResourceUri(long patientId, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetAppointmentByIdentifier",
              new { patientId, id = resourceId });
        }

        public string CreatePatientsResourceUri(ResourceUriType type,
           string orderBy,
           string facilityName,
           int pageNumber,
           int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientsByIdentifier",
                      new
                      {
                          orderBy,
                          facilityName,
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientsByIdentifier",
                      new
                      {
                          orderBy,
                          facilityName,
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientsByIdentifier",
                      new
                      {
                          orderBy,
                          facilityName,
                          pageNumber = pageNumber,
                          pageSize = pageSize
                      });
            }
        }

        public string CreatePatientMedicationReportResourceUri(ResourceUriType type,
           PatientMedicationReportResourceParameters patientMedicationReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber - 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize,
                          SearchTerm = patientMedicationReportResourceParameters.SearchTerm
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber + 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize,
                          SearchTerm = patientMedicationReportResourceParameters.SearchTerm
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber,
                          pageSize = patientMedicationReportResourceParameters.PageSize,
                          SearchTerm = patientMedicationReportResourceParameters.SearchTerm
                      });
            }
        }

        public string CreatePatientTreatmentReportResourceUri(ResourceUriType type,
            int pageNumber,
            int pageSize,
            PatientOnStudyCriteria patientOnStudyCriteria)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = pageNumber - 1,
                          pageSize = pageSize,
                          PatientOnStudyCriteria = patientOnStudyCriteria
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = pageNumber + 1,
                          pageSize = pageSize,
                          PatientOnStudyCriteria = patientOnStudyCriteria
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = pageNumber,
                          pageSize = pageSize,
                          PatientOnStudyCriteria = patientOnStudyCriteria
                      });
            }
        }

        public string CreateProductsResourceUri(ResourceUriType type,
            string orderBy,
            string searchTerm,
            YesNoBothValueType active,
            int pageNumber,
            int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetProductsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber - 1,
                          pageSize,
                          searchTerm,
                          active
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetProductsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber = pageNumber + 1,
                          pageSize,
                          searchTerm,
                          active
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetProductsByIdentifier",
                      new
                      {
                          orderBy,
                          pageNumber,
                          pageSize,
                          searchTerm,
                          active
                      });
            }
        }

        public string CreateReportInstanceResourceUri(Guid workFlowGuid, int reportInstanceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetReportInstanceByIdentifier",
              new { workFlowGuid, id = reportInstanceId });
        }

        public string CreateReportInstancesResourceUri(Guid workFlowGuid, 
           ResourceUriType type,
           string orderBy,
           string qualifiedName,
           DateTime searchFrom,
           DateTime searchTo,
           int pageNumber,
           int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy,
                          qualifiedName,
                          searchFrom,
                          searchTo,
                          pageNumber = pageNumber - 1,
                          pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy,
                          qualifiedName,
                          searchFrom,
                          searchTo,
                          pageNumber = pageNumber + 1,
                          pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy,
                          qualifiedName,
                          searchFrom,
                          searchTo,
                          pageNumber,
                          pageSize
                      });
            }
        }

        public string CreateUpdateDatasetInstanceResourceUri(long datasetId, long datasetInstanceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"UpdateDataset",
              new { datasetId, id = datasetInstanceId });
        }

        public string CreateUpdateDeenrolmentForPatientResourceUri(long patientId, long cohortGroupEnrolmentId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"UpdatePatientDeenrolment",
              new { patientId, cohortGroupEnrolmentId });
        }

        public string CreateUsersResourceUri(ResourceUriType type,
           UserResourceParameters userResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber - 1,
                          pageSize = userResourceParameters.PageSize,
                          SearchTerm = userResourceParameters.SearchTerm
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber + 1,
                          pageSize = userResourceParameters.PageSize,
                          SearchTerm = userResourceParameters.SearchTerm
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber,
                          pageSize = userResourceParameters.PageSize,
                          SearchTerm = userResourceParameters.SearchTerm
                      });
            }
        }
    }
}
