using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    public class WorkFlowDownloadActiveDatasetQueryHandler
        : IRequestHandler<WorkFlowDownloadActiveDatasetQuery, ArtifactDto>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeConfigurationRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<ReportInstanceMedication> _reportInstanceMedicationRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IExcelDocumentService _excelDocumentService;
        private readonly ILogger<WorkFlowDownloadActiveDatasetQueryHandler> _logger;

        public WorkFlowDownloadActiveDatasetQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeConfigurationRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<ReportInstanceMedication> reportInstanceMedicationRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IExcelDocumentService excelDocumentService,
            ILogger<WorkFlowDownloadActiveDatasetQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _customAttributeConfigurationRepository = customAttributeConfigurationRepository ?? throw new ArgumentNullException(nameof(customAttributeConfigurationRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _reportInstanceMedicationRepository = reportInstanceMedicationRepository ?? throw new ArgumentNullException(nameof(reportInstanceMedicationRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _excelDocumentService = excelDocumentService ?? throw new ArgumentNullException(nameof(excelDocumentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ArtifactDto> Handle(WorkFlowDownloadActiveDatasetQuery message, CancellationToken cancellationToken)
        {
            var model = PrepareFileModel();

            _excelDocumentService.CreateDocument(model);

            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == message.CohortGroupId);
            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate cohort group {message.CohortGroupId} for active dataset download");
            }

            var orderby = Extensions.GetOrderBy<CohortGroupEnrolment>("Id", "asc");

            var predicate = PredicateBuilder.New<CohortGroupEnrolment>(true);
            predicate = predicate.And(cge => cge.CohortGroup.Id == message.CohortGroupId);

            var pagedCohortGroupEnrolmentsFromRepo = await _cohortGroupEnrolmentRepository.ListAsync(predicate, orderby, new string[] {
                "CohortGroup",
                "Patient" });

            List<int> patientIds = pagedCohortGroupEnrolmentsFromRepo.Select(enrolment => enrolment.PatientId).ToList();

            await PrepareDataForPatientSheetAsync(patientIds.ToArray());
            await PrepareDataForMedicationSheetAsync(patientIds.ToArray());
            await PrepareDataForClinicalEventSheetAsync(patientIds.ToArray());
            await PrepareDataForConditionSheetAsync(patientIds.ToArray());
            await PrepareDataForLabTestSheetAsync(patientIds.ToArray());
            await PrepareDataForEncounterSheetAsync(patientIds.ToArray());

            return model;
        }

        private async Task PrepareDataForPatientSheetAsync(int[] patientIds)
        {
            var data = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<Patient>("Id", "asc");
            var patientsFromRepo = await _patientRepository.ListAsync(p => patientIds.Contains(p.Id) && p.Archived == false, orderby, new string[] { 
                "Encounters",
                "PatientFacilities.Facility.OrgUnit",
                "CreatedBy", "UpdatedBy"
            });
            if (patientsFromRepo != null)
            {
                var headers = await PrepareEntityHeaderAsync(new Patient(), "Patient");
                data.Add(headers);

                foreach (var patient in patientsFromRepo)
                {
                    var row = await PrepareEntityRowAsync(patient, "Patient");
                    data.Add(row);
                }
            }

            _excelDocumentService.AddSheet("Patients", data);
        }

        private async Task PrepareDataForMedicationSheetAsync(int[] patientIds)
        {
            var medicationData = new List<List<string>>();
            var causalityData = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<PatientMedication>("Id", "asc");
            var medicationsFromRepo = await _patientMedicationRepository.ListAsync(pm => patientIds.Contains(pm.Patient.Id) && pm.Archived == false, orderby, new string[] { 
                "Concept.MedicationForm", 
                "Product" });
            if (medicationsFromRepo != null)
            {
                var medicationHeaders = await PrepareEntityHeaderAsync(new PatientMedication(), "PatientMedication");
                medicationData.Add(medicationHeaders);

                var causalityHeaders = await PrepareEntityHeaderAsync(new ReportInstanceMedication(), "ReportInstanceMedication");
                causalityData.Add(causalityHeaders);

                foreach (var medication in medicationsFromRepo)
                {
                    var medicationRow = await PrepareEntityRowAsync(medication, "PatientMedication");
                    medicationData.Add(medicationRow);

                    var reportInstanceMedications = await _reportInstanceMedicationRepository.ListAsync(rm => rm.ReportInstanceMedicationGuid == medication.PatientMedicationGuid);
                    foreach (var reportInstanceMedication in reportInstanceMedications)
                    {
                        var causalityRow = await PrepareEntityRowAsync(reportInstanceMedication, "ReportInstanceMedication");
                        causalityData.Add(causalityRow);
                    }
                }
            }

            _excelDocumentService.AddSheet("Medications", medicationData);
            _excelDocumentService.AddSheet("Medication Causality", causalityData);
        }

        private async Task PrepareDataForClinicalEventSheetAsync(int[] patientIds)
        {
            var clinicalData = new List<List<string>>();
            var reportInstanceData = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<PatientClinicalEvent>("Id", "asc");
            var eventsFromRepo = await _patientClinicalEventRepository.ListAsync(pce => patientIds.Contains(pce.Patient.Id) && pce.Archived == false, orderby, new string[] { 
                "SourceTerminologyMedDra" 
            });
            if (eventsFromRepo != null)
            {
                var clinicalHeaders = await PrepareEntityHeaderAsync(new PatientClinicalEvent(), "PatientClinicalEvent");
                clinicalData.Add(clinicalHeaders);

                var reportInstanceHeaders = await PrepareEntityHeaderAsync(new ReportInstance(), "ReportInstance");
                reportInstanceData.Add(reportInstanceHeaders);

                foreach (var clinicalEvent in eventsFromRepo)
                {
                    var clinicalRow = await PrepareEntityRowAsync(clinicalEvent, "PatientClinicalEvent");
                    clinicalData.Add(clinicalRow);

                    var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == clinicalEvent.PatientClinicalEventGuid, new string[] {
                        "Activities.ExecutionEvents.ExecutionStatus", 
                        "Tasks.CreatedBy", 
                        "CreatedBy", 
                        "TerminologyMedDra"
                    });
                    if(reportInstance != null)
                    {
                        var reportInstanceRow = await PrepareEntityRowAsync(reportInstance, "ReportInstance");
                        reportInstanceData.Add(reportInstanceRow);
                    }
                }
            }

            _excelDocumentService.AddSheet("Adverse Events", clinicalData);
            _excelDocumentService.AddSheet("Adverse Event Terminology", reportInstanceData);
        }

        private async Task PrepareDataForConditionSheetAsync(int[] patientIds)
        {
            var data = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<PatientCondition>("Id", "asc");
            var conditionsFromRepo = await _patientConditionRepository.ListAsync(pc => patientIds.Contains(pc.Patient.Id) && pc.Archived == false, orderby, new string[] { 
                "TerminologyMedDra", 
                "Outcome", 
                "TreatmentOutcome" 
            });
            if (conditionsFromRepo != null)
            {
                var headers = await PrepareEntityHeaderAsync(new PatientCondition(), "PatientCondition");
                data.Add(headers);

                foreach (var condition in conditionsFromRepo)
                {
                    var row = await PrepareEntityRowAsync(condition, "PatientCondition");
                    data.Add(row);
                }
            }

            _excelDocumentService.AddSheet("Conditions", data);
        }

        private async Task PrepareDataForLabTestSheetAsync(int[] patientIds)
        {
            var data = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<PatientLabTest>("Id", "asc");
            var labTestsFromRepo = await _patientLabTestRepository.ListAsync(plt => patientIds.Contains(plt.Patient.Id) && plt.Archived == false, orderby, new string[] { 
                "LabTest", 
                "TestUnit" 
            });
            if (labTestsFromRepo != null)
            {
                var headers = await PrepareEntityHeaderAsync(new PatientLabTest(), "PatientLabTest");
                data.Add(headers);

                foreach (var labTest in labTestsFromRepo)
                {
                    var row = await PrepareEntityRowAsync(labTest, "PatientLabTest");
                    data.Add(row);
                }
            }

            _excelDocumentService.AddSheet("Lab Tests", data);
        }

        private async Task PrepareDataForEncounterSheetAsync(int[] patientIds)
        {
            var data = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<Encounter>("Id", "asc");
            var encountersFromRepo = await _encounterRepository.ListAsync(plt => patientIds.Contains(plt.Patient.Id) && plt.Archived == false, orderby, new string[] {
                "EncounterType",
                "Priority"
            });
            if (encountersFromRepo != null)
            {
                var headers = await PrepareEntityHeaderAsync(new Encounter(), "Encounter");
                data.Add(headers);

                foreach (var encounter in encountersFromRepo)
                {
                    var row = await PrepareEntityRowAsync(encounter, "Encounter");
                    data.Add(row);
                }
            }

            _excelDocumentService.AddSheet("Encounters", data);
        }

        private ArtifactDto PrepareFileModel()
        {
            var model = new ArtifactDto();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"{Path.GetRandomFileName()}_{generatedDate}.xlsx";
            return model;
        }

        private async Task<List<string>> PrepareEntityHeaderAsync(Object obj, string entityName)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            var headers = new List<string>();

            var propertiesToIgnore = new List<string>() { 
                "CustomAttributesXmlSerialised", 
                "Archived", "ArchivedDate", "ArchivedReason", "AuditUser",
                "Patient"
            };

            foreach (PropertyInfo property in properties)
            {
                if (!propertiesToIgnore.Contains(property.Name))
                {
                    if (property.PropertyType == typeof(DateTime?)
                        || property.PropertyType == typeof(DateTime)
                        || property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(int)
                        || property.PropertyType == typeof(decimal)
                        || property.PropertyType == typeof(User)
                        || property.PropertyType == typeof(Concept)
                        || property.PropertyType == typeof(Product)
                        || property.PropertyType == typeof(Patient)
                        || property.PropertyType == typeof(Encounter)
                        || property.PropertyType == typeof(TerminologyMedDra)
                        || property.PropertyType == typeof(Outcome)
                        || property.PropertyType == typeof(LabTest)
                        || property.PropertyType == typeof(LabTestUnit)
                        || property.PropertyType == typeof(CohortGroup)
                        || property.PropertyType == typeof(Facility)
                        || property.PropertyType == typeof(Priority)
                        || property.PropertyType == typeof(EncounterType)
                        || property.PropertyType == typeof(TreatmentOutcome)
                        || property.PropertyType == typeof(Guid)
                        || property.PropertyType == typeof(bool)
                        )
                    {
                        headers.Add(property.Name);
                    }
                }
            }

            // Now process attributes
            var attributes = await _customAttributeConfigurationRepository.ListAsync(c => c.ExtendableTypeName == entityName);
            foreach (var attribute in attributes)
            {
                headers.Add(attribute.AttributeKey);
            }

            return headers;
        }

        private async Task<List<string>> PrepareEntityRowAsync(Object obj, string entityName)
        {
            DateTime tempdt;

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            var row = new List<string>();

            var propertiesToIgnore = new List<string>() {
                "CustomAttributesXmlSerialised",
                "Archived", "ArchivedDate", "ArchivedReason", "AuditUser",
                "Patient", 
                "DomainEvents"
            };

            var subOutput = "**IGNORE**";
            foreach (PropertyInfo property in properties)
            {
                if (!propertiesToIgnore.Contains(property.Name))
                {
                    subOutput = "**IGNORE**";
                    if (property.PropertyType == typeof(DateTime?)
                        || property.PropertyType == typeof(DateTime))
                    {
                        var dt = property.GetValue(obj, null) != null ? Convert.ToDateTime(property.GetValue(obj, null)).ToString("yyyy-MM-dd") : "";
                        subOutput = dt;
                    }
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(int)
                        || property.PropertyType == typeof(decimal)
                        || property.PropertyType == typeof(Guid)
                        || property.PropertyType == typeof(bool)
                        )
                    {
                        subOutput = property.GetValue(obj, null) != null ? property.GetValue(obj, null).ToString() : "";
                    }
                    if (property.PropertyType == typeof(User))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var user = (User)property.GetValue(obj, null);
                            subOutput = user.UserName;
                        }
                    }
                    if (property.PropertyType == typeof(Patient))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var patient = (Patient)property.GetValue(obj, null);
                            subOutput = patient.PatientGuid.ToString();
                        }
                    }
                    if (property.PropertyType == typeof(Concept))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var medication = (Concept)property.GetValue(obj, null);
                            subOutput = medication.ConceptName;
                        }
                    }
                    if (property.PropertyType == typeof(Product))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var medication = (Product)property.GetValue(obj, null);
                            subOutput = medication.ProductName;
                        }
                    }
                    if (property.PropertyType == typeof(Encounter))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var encounter = (Encounter)property.GetValue(obj, null);
                            subOutput = encounter.EncounterGuid.ToString();
                        }
                    }
                    if (property.PropertyType == typeof(TerminologyMedDra))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var meddra = (TerminologyMedDra)property.GetValue(obj, null);
                            subOutput = meddra.DisplayName;
                        }
                    }
                    if (property.PropertyType == typeof(Outcome))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var outcome = (Outcome)property.GetValue(obj, null);
                            subOutput = outcome.Description;
                        }
                    }
                    if (property.PropertyType == typeof(TreatmentOutcome))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var txOutcome = (TreatmentOutcome)property.GetValue(obj, null);
                            subOutput = txOutcome.Description;
                        }
                    }
                    if (property.PropertyType == typeof(LabTest))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var labTest = (LabTest)property.GetValue(obj, null);
                            subOutput = labTest.Description;
                        }
                    }
                    if (property.PropertyType == typeof(LabTestUnit))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var unit = (LabTestUnit)property.GetValue(obj, null);
                            subOutput = unit.Description;
                        }
                    }
                    if (property.PropertyType == typeof(CohortGroup))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var group = (CohortGroup)property.GetValue(obj, null);
                            subOutput = group.DisplayName;
                        }
                    }
                    if (property.PropertyType == typeof(Facility))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var facility = (Facility)property.GetValue(obj, null);
                            subOutput = facility.FacilityName;
                        }
                    }
                    if (property.PropertyType == typeof(Priority))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var priority = (Priority)property.GetValue(obj, null);
                            subOutput = priority.Description;
                        }
                    }
                    if (property.PropertyType == typeof(EncounterType))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var et = (EncounterType)property.GetValue(obj, null);
                            subOutput = et.Description;
                        }
                    }
                    if (subOutput != "**IGNORE**")
                    {
                        row.Add(subOutput);
                    }
                }
            }

            IExtendable extended = null;
            switch (entityName)
            {
                case "Patient":
                    extended = (Patient)obj;
                    break;

                case "PatientMedication":
                    extended = (PatientMedication)obj;
                    break;

                case "PatientClinicalEvent":
                    extended = (PatientClinicalEvent)obj;
                    break;

                case "PatientCondition":
                    extended = (PatientCondition)obj;
                    break;

                case "PatientLabTest":
                    extended = (PatientLabTest)obj;
                    break;

                default:
                    break;
            }

            if (extended != null)
            {
                var attributes = await _customAttributeConfigurationRepository.ListAsync(c => c.ExtendableTypeName == entityName);
                foreach (var attribute in attributes)
                {
                    var output = "";
                    var val = extended.GetAttributeValue(attribute.AttributeKey);
                    if (val != null)
                    {
                        if (attribute.CustomAttributeType == CustomAttributeType.Selection)
                        {
                            var tempSDI = await _selectionDataItemRepository.GetAsync(u => u.AttributeKey == attribute.AttributeKey && u.SelectionKey == val.ToString());
                            if (tempSDI != null)
                                output = tempSDI.Value;
                        }
                        else if (attribute.CustomAttributeType == CustomAttributeType.DateTime)
                        {
                            if (attribute != null && DateTime.TryParse(val.ToString(), out tempdt))
                            {
                                output = tempdt.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                output = val.ToString();
                            }
                        }
                        else
                        {
                            output = val.ToString();
                        }
                    }

                    row.Add(output);
                }
            }

            return row;
        }
    }
}
