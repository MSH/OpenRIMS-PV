using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System.Linq;

namespace PVIMS.API.Infrastructure.Services
{
    public class ArtefactService : IArtefactService
    {
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly ICustomAttributeService _attributeService;
        private readonly IPatientService _patientService;
        private readonly IWordDocumentService _wordDocumentService;

        public ArtefactService(
            IRepositoryInt<DatasetElement> datasetElementRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            ICustomAttributeService attributeService,
            IPatientService patientService,
            IWordDocumentService wordDocumentService)
        {
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _wordDocumentService = wordDocumentService ?? throw new ArgumentNullException(nameof(wordDocumentService));
        }

        public async Task<ArtefactInfoModel> CreatePatientSummaryForActiveReportAsync(Guid contextGuid)
        {
            var patientClinicalEvent = await GetPatientClinicalEventAsync(contextGuid);
            var reportInstance = await GetReportInstanceForPatientClinicalEventAsync(contextGuid);

            var isSerious = await CheckIfSeriousAsync(patientClinicalEvent);
            var model = PrepareFileModelForActive(patientClinicalEvent, isSerious);

            _wordDocumentService.CreateDocument(model);
            _wordDocumentService.AddPageHeader(isSerious ? "SERIOUS ADVERSE EVENT" : "PATIENT SUMMARY");

            _wordDocumentService.AddTableHeader("A. BASIC PATIENT INFORMATION");
            _wordDocumentService.AddFourColumnTable(await PrepareBasicInformationForActiveReportAsync(patientClinicalEvent));
            _wordDocumentService.AddTwoColumnTable(PrepareNotes(patientClinicalEvent.Patient.Notes));
            _wordDocumentService.AddTableHeader("B. PRE-EXISITING CONDITIONS");
            _wordDocumentService.AddTwoColumnTable(PrepareConditionsForActiveReport(patientClinicalEvent));
            _wordDocumentService.AddTableHeader("C. ADVERSE EVENT INFORMATION");
            _wordDocumentService.AddFourColumnTable(await PrepareAdverseEventForActiveReportAsync(patientClinicalEvent, reportInstance, isSerious));
            _wordDocumentService.AddTwoColumnTable(PrepareNotes(""));
            _wordDocumentService.AddTableHeader("D. MEDICATIONS");
            await PrepareMedicationsForActiveReportAsync(reportInstance);
            _wordDocumentService.AddTableHeader("E. CLINICAL EVALUATIONS");
            PrepareEvaluationsForActiveReport(patientClinicalEvent);
            _wordDocumentService.AddTwoColumnTable(PrepareNotes(""));
            
            //_wordDocumentService.AddTableHeader("F. WEIGHT HISTORY");
            //PrepareWeightForActiveReport(patientClinicalEvent);

            //_wordDocumentService.AddTwoColumnTable(PrepareNotes(""));

            return model;
        }

        public async Task<ArtefactInfoModel> CreatePatientSummaryForSpontaneousReportAsync(Guid contextGuid)
        {
            var datasetInstance = await GetDatasetInstanceAsync(contextGuid);

            var isSerious = !String.IsNullOrWhiteSpace(datasetInstance.GetInstanceValue("Reaction serious details"));
            var model = PrepareFileModelForSpontaneous(datasetInstance, isSerious);

            _wordDocumentService.CreateDocument(model);
            _wordDocumentService.AddPageHeader(isSerious ? "SERIOUS ADVERSE EVENT" : "PATIENT SUMMARY");

            _wordDocumentService.AddTableHeader("A. BASIC PATIENT INFORMATION");
            _wordDocumentService.AddFourColumnTable(PrepareBasicInformationForSpontaneousReport(datasetInstance));
            _wordDocumentService.AddTwoColumnTable(PrepareNotes(""));
            _wordDocumentService.AddTableHeader("B. ADVERSE EVENT INFORMATION");
            _wordDocumentService.AddFourColumnTable(await PrepareAdverseEventForSpontaneousReportAsync(datasetInstance, isSerious));
            _wordDocumentService.AddTwoColumnTable(PrepareNotes(""));
            _wordDocumentService.AddTableHeader("C. MEDICATIONS");

            await PrepareMedicationsForSpontaneousReportAsync(datasetInstance);

            _wordDocumentService.AddTableHeader("D. CLINICAL EVALUATIONS");

            await PrepareEvaluationsForSpontaneousReport(datasetInstance);

            _wordDocumentService.AddTwoColumnTable(PrepareNotes(""));

            return model;
        }

        private async Task<bool> CheckIfSeriousAsync(PatientClinicalEvent patientClinicalEvent)
        {
            var extendable = (IExtendable)patientClinicalEvent;
            var extendableValue = await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Is the adverse event serious?", extendable);
            return extendableValue == "Yes";
        }

        private ArtefactInfoModel PrepareFileModelForActive(PatientClinicalEvent patientClinicalEvent, bool isSerious)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            var fileNamePrefix = isSerious ? "SAEReport_Active" : "PatientSummary_Active";
            model.FileName = $"{fileNamePrefix}{patientClinicalEvent.Patient.Id}_{generatedDate}.docx";
            return model;
        }

        private ArtefactInfoModel PrepareFileModelForSpontaneous(DatasetInstance datasetInstance, bool isSerious)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            var fileNamePrefix = isSerious ? "SAEReport_Spontaneous" : "PatientSummary_Spontaneous";
            model.FileName = $"{fileNamePrefix}{datasetInstance.Id}_{generatedDate}.docx";
            return model;
        }

        private async Task<PatientClinicalEvent> GetPatientClinicalEventAsync(Guid contextGuid)
        {
            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(pce => pce.PatientClinicalEventGuid == contextGuid,
                new string[] { "SourceTerminologyMedDra",
                    "Patient.PatientFacilities.Facility",
                    "Patient.PatientConditions.TerminologyMedDra", "Patient.PatientConditions.Outcome", "Patient.PatientConditions.TreatmentOutcome",
                    "Patient.PatientMedications.Concept.MedicationForm", "Patient.PatientMedications.Product",
                    "Patient.PatientLabTests.LabTest", "Patient.PatientLabTests.TestUnit",
                    "Patient.PatientFacilities.Facility"});
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEvent));
            }

            return patientClinicalEvent;
        }

        private async Task<ReportInstance> GetReportInstanceForPatientClinicalEventAsync(Guid contextGuid)
        {
            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Medications", 
                "TerminologyMedDra" });

            if (reportInstance == null)
            {
                throw new KeyNotFoundException(nameof(reportInstance));
            }

            return reportInstance;
        }

        private async Task<DatasetInstance> GetDatasetInstanceAsync(Guid contextGuid)
        {
            var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == contextGuid, new string[] { "DatasetInstanceValues.DatasetElement", "DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub" });
            if (datasetInstance == null)
            {
                throw new KeyNotFoundException(nameof(datasetInstance));
            }

            return datasetInstance;
        }

        private async Task<List<KeyValuePair<string, string>>> PrepareBasicInformationForActiveReportAsync(PatientClinicalEvent patientClinicalEvent)
        {
            var extendable = (IExtendable)patientClinicalEvent;
            var patientExtendable = (IExtendable)patientClinicalEvent.Patient;
            List<KeyValuePair<string, string>> rows = new();

            rows.Add(new KeyValuePair<string, string>("Patient Name", patientClinicalEvent.Patient.FullName));
            rows.Add(new KeyValuePair<string, string>("Date of Birth (yyyy-mm-dd)", patientClinicalEvent.Patient.DateOfBirth.HasValue ? patientClinicalEvent.Patient.DateOfBirth.Value.ToString("yyyy-MM-dd") : ""));
            rows.Add(new KeyValuePair<string, string>("Age Group", patientClinicalEvent.Patient.AgeGroup));

            if (patientClinicalEvent.OnsetDate.HasValue && patientClinicalEvent.Patient.DateOfBirth.HasValue)
            {
                rows.Add(new KeyValuePair<string, string>("Age at time of onset", $"{CalculateAge(patientClinicalEvent.Patient.DateOfBirth.Value, patientClinicalEvent.OnsetDate.Value).ToString()} years"));
            }
            else
            {
                rows.Add(new KeyValuePair<string, string>("Age at time of onset", string.Empty));
            }

            rows.Add(new KeyValuePair<string, string>("Gender", await _attributeService.GetCustomAttributeValueAsync("Patient", "Gender", patientExtendable)));
            rows.Add(new KeyValuePair<string, string>("Ethnic Group", await _attributeService.GetCustomAttributeValueAsync("Patient", "Ethnic Group", patientExtendable)));
            rows.Add(new KeyValuePair<string, string>("Facility", patientClinicalEvent.Patient.CurrentFacility.Facility.FacilityName));
            rows.Add(new KeyValuePair<string, string>("Region", ""));
            //rows.Add(new KeyValuePair<string, string>("Medical Record Number", await _attributeService.GetCustomAttributeValueAsync("Patient", "Medical Record Number", extendable)));
            //rows.Add(new KeyValuePair<string, string>("Identity Number", await _attributeService.GetCustomAttributeValueAsync("Patient", "Patient Identity Number", extendable)));

            rows.Add(new KeyValuePair<string, string>("Weight (kg)", await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Weight (kg)", extendable)));
            rows.Add(new KeyValuePair<string, string>("Height (cm)", await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Height (cm)", extendable)));

            return rows;
        }

        private List<KeyValuePair<string, string>> PrepareBasicInformationForSpontaneousReport(DatasetInstance datasetInstance)
        {
            List<KeyValuePair<string, string>> rows = new();

            rows.Add(new KeyValuePair<string, string>("Patient Name", datasetInstance.GetInstanceValue("Initials")));
            rows.Add(new KeyValuePair<string, string>("Date of Birth", DateTime.TryParse(datasetInstance.GetInstanceValue("Date of Birth"), out var dateOfBirthValue) ? dateOfBirthValue.ToString("yyyy-MM-dd") : ""));
            rows.Add(new KeyValuePair<string, string>("Age Group", string.Empty));
            rows.Add(new KeyValuePair<string, string>("Age at time of onset", $"{datasetInstance.GetInstanceValue("Age")} {datasetInstance.GetInstanceValue("Age Unit")}"));
            rows.Add(new KeyValuePair<string, string>("Gender", datasetInstance.GetInstanceValue("Sex")));
            rows.Add(new KeyValuePair<string, string>("Facility", string.Empty));
            rows.Add(new KeyValuePair<string, string>("Medical Record Number", string.Empty));
            rows.Add(new KeyValuePair<string, string>("Identity Number", datasetInstance.GetInstanceValue("Identification Number")));
            rows.Add(new KeyValuePair<string, string>("Weight (kg)", datasetInstance.GetInstanceValue("Weight (kg)")));
            rows.Add(new KeyValuePair<string, string>("Height (cm)", string.Empty));

            return rows;
        }

        private List<KeyValuePair<string, string>> PrepareNotes(string notes)
        {
            List<KeyValuePair<string, string>> rows = new();
            rows.Add(new KeyValuePair<string, string>("Notes", notes));
            return rows;
        }

        private List<KeyValuePair<string, string>> PrepareConditionsForActiveReport(PatientClinicalEvent patientClinicalEvent)
        {
            List<KeyValuePair<string, string>> rows = new();

            var i = 0;
            foreach (PatientCondition patientCondition in patientClinicalEvent.Patient.PatientConditions.Where(pc => pc.OnsetDate <= patientClinicalEvent.OnsetDate).OrderByDescending(c => c.OnsetDate))
            {
                i += 1;
                rows.Add(new KeyValuePair<string, string>("Condition", patientCondition.TerminologyMedDra != null ? patientCondition.TerminologyMedDra.MedDraTerm : patientCondition.ConditionSource));
                rows.Add(new KeyValuePair<string, string>("Start Date (yyyy-mm-dd)", patientCondition.OnsetDate.ToString("yyyy-MM-dd")));
                rows.Add(new KeyValuePair<string, string>("End Date (yyyy-mm-dd)", patientCondition.OutcomeDate.HasValue ? patientCondition.OutcomeDate.Value.ToString("yyyy-MM-dd") : ""));
            }

            return rows;
        }

        private async Task<List<KeyValuePair<string, string>>> PrepareAdverseEventForActiveReportAsync(PatientClinicalEvent patientClinicalEvent, ReportInstance reportInstance, bool isSerious)
        {
            var extendable = (IExtendable)patientClinicalEvent;
            List<KeyValuePair<string, string>> rows = new();

            rows.Add(new KeyValuePair<string, string>("Source Description", patientClinicalEvent.SourceDescription));
            rows.Add(new KeyValuePair<string, string>("MedDRA Term", reportInstance.TerminologyMedDra?.MedDraTerm));
            rows.Add(new KeyValuePair<string, string>("Onset Date (yyyy-mm-dd)", patientClinicalEvent.OnsetDate.HasValue ? patientClinicalEvent.OnsetDate.Value.ToString("yyyy-MM-dd") : ""));
            rows.Add(new KeyValuePair<string, string>("Resolution Date (yyyy-mm-dd)", patientClinicalEvent.ResolutionDate.HasValue ? patientClinicalEvent.ResolutionDate.Value.ToString("yyyy-MM-dd") : ""));

            if (patientClinicalEvent.OnsetDate.HasValue && patientClinicalEvent.ResolutionDate.HasValue)
            {
                rows.Add(new KeyValuePair<string, string>("Duration", $"${(patientClinicalEvent.ResolutionDate.Value - patientClinicalEvent.OnsetDate.Value).Days} days"));
            }
            else
            {
                rows.Add(new KeyValuePair<string, string>("Duration", string.Empty));
            }

            rows.Add(new KeyValuePair<string, string>("Outcome", await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Outcome", extendable)));

            if (isSerious)
            {
                rows.Add(new KeyValuePair<string, string>("Seriousness", await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Seriousness", extendable)));
                rows.Add(new KeyValuePair<string, string>("Classification", ReportClassification.From(reportInstance.ReportClassificationId).Name));
                rows.Add(new KeyValuePair<string, string>("Severity Grade", await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Severity Grade", extendable)));
                rows.Add(new KeyValuePair<string, string>("SAE Number", await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "FDA SAE Number", extendable)));
            }

            return rows;
        }

        private async Task<List<KeyValuePair<string, string>>> PrepareAdverseEventForSpontaneousReportAsync(DatasetInstance datasetInstance, bool isSerious)
        {
            List<KeyValuePair<string, string>> rows = new();

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid, new string[] { "TerminologyMedDra" });

            DateTime.TryParse(datasetInstance.GetInstanceValue("Reaction known start date"), out var onsetDateValue);
            DateTime.TryParse(datasetInstance.GetInstanceValue("Reaction date of recovery"), out var resolutionDateValue);

            rows.Add(new KeyValuePair<string, string>("Source Description", datasetInstance.GetInstanceValue("Description of reaction")));
            rows.Add(new KeyValuePair<string, string>("MedDRA Term", reportInstance.TerminologyMedDra?.MedDraTerm));
            rows.Add(new KeyValuePair<string, string>("Onset Date", onsetDateValue > DateTime.MinValue ? onsetDateValue.ToString("yyyy-MM-dd") : ""));
            rows.Add(new KeyValuePair<string, string>("Resolution Date", resolutionDateValue > DateTime.MinValue ? resolutionDateValue.ToString("yyyy-MM-dd") : ""));

            if (onsetDateValue > DateTime.MinValue && resolutionDateValue > DateTime.MinValue)
            {
                rows.Add(new KeyValuePair<string, string>("Duration", $"${(resolutionDateValue - onsetDateValue).Days} days"));
            }
            else
            {
                rows.Add(new KeyValuePair<string, string>("Duration", string.Empty));
            }

            rows.Add(new KeyValuePair<string, string>("Outcome", datasetInstance.GetInstanceValue("Outcome of reaction")));

            if (isSerious)
            {
                rows.Add(new KeyValuePair<string, string>("Seriousness", datasetInstance.GetInstanceValue("Reaction serious details")));
                rows.Add(new KeyValuePair<string, string>("Grading Scale", string.Empty));
                rows.Add(new KeyValuePair<string, string>("Severity Grade", string.Empty));
                rows.Add(new KeyValuePair<string, string>("SAE Number", string.Empty));
            }

            return rows;
        }

        private async Task PrepareMedicationsForActiveReportAsync(ReportInstance reportInstance)
        {
            var i = 0;
            foreach (ReportInstanceMedication reportMedication in reportInstance.Medications)
            {
                var patientMedication = await _patientMedicationRepository.GetAsync(pm => pm.PatientMedicationGuid == reportMedication.ReportInstanceMedicationGuid);
                if (patientMedication != null)
                {
                    i += 1;

                    List<string[]> rows = new();
                    List<string> cells = new();

                    cells.Add("Drug");
                    cells.Add("Start Date (yyyy-mm-dd)");
                    cells.Add("End Date (yyyy-mm-dd)");
                    cells.Add("Dose");
                    cells.Add("Route");
                    cells.Add("Causality");

                    rows.Add(cells.ToArray());

                    cells = new();

                    IExtendable extendable = patientMedication;
                    cells.Add(patientMedication.DisplayName);
                    cells.Add(patientMedication.StartDate.ToString("yyyy-MM-dd"));
                    cells.Add(patientMedication.EndDate.HasValue ? patientMedication.EndDate.Value.ToString("yyyy-MM-dd") : "");
                    cells.Add($"{patientMedication.Dose} {patientMedication.DoseUnit}");
                    cells.Add(await _attributeService.GetCustomAttributeValueAsync("PatientMedication", "Route", extendable));
                    cells.Add($"{reportMedication.WhoCausality} {reportMedication.NaranjoCausality}");

                    rows.Add(cells.ToArray());

                    _wordDocumentService.AddRowTable(rows, new int[] { 2500, 1250, 1250, 1250, 1250, 3852 });
                    _wordDocumentService.AddTableHeader("D.1 CLINICIAN ACTION TAKEN WITH REGARD TO MEDICINE");
                    _wordDocumentService.AddOneColumnTable(new List<string>() { "" });
                    _wordDocumentService.AddTableHeader("D.2 EFFECT OF DECHALLENGE/RECHALLENGE");
                    _wordDocumentService.AddOneColumnTable(new List<string>() { "" });
                    _wordDocumentService.AddTableHeader("D.3 NOTES");
                    _wordDocumentService.AddOneColumnTable(new List<string>() { "" });
                }
            }
        }

        private async Task PrepareMedicationsForSpontaneousReportAsync(DatasetInstance datasetInstance)
        {
            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid, new string[] { "Medications" });
            if (reportInstance == null) { return; };

            var sourceProductElement = await _datasetElementRepository.GetAsync(dse => dse.ElementName == "Product Information");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceProductElement.ElementName);

            var i = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                i += 1;

                List<string[]> rows = new();
                List<string> cells = new();

                cells.Add($"Drug {i}");
                cells.Add("Start Date");
                cells.Add("End Date");
                cells.Add("Dose");
                cells.Add("Route");
                cells.Add("Indication");

                rows.Add(cells.ToArray());

                cells = new();

                var drugItemValues = datasetInstance.GetInstanceSubValues(sourceProductElement.ElementName, med.ReportInstanceMedicationGuid);

                var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");
                var dose = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose number");
                var route = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug route of administration");
                var indication = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Indication");

                cells.Add(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue);
                cells.Add(startValue != null ? Convert.ToDateTime(startValue.InstanceValue).ToString("yyyy-MM-dd") : string.Empty);
                cells.Add(endValue != null ? Convert.ToDateTime(endValue.InstanceValue).ToString("yyyy-MM-dd") : string.Empty);
                cells.Add(dose != null ? dose.InstanceValue : string.Empty);
                cells.Add(route != null ? route.InstanceValue : string.Empty);
                cells.Add(indication != null ? indication.InstanceValue : string.Empty);

                rows.Add(cells.ToArray());

                _wordDocumentService.AddRowTable(rows, new int[] { 2500, 1250, 1250, 1250, 1250, 3852 });
                _wordDocumentService.AddTableHeader("C.1 CLINICIAN ACTION TAKEN WITH REGARD TO MEDICINE");
                _wordDocumentService.AddOneColumnTable(new List<string>() { "" });
                _wordDocumentService.AddTableHeader("C.2 EFFECT OF DECHALLENGE/ RECHALLENGE");
                _wordDocumentService.AddOneColumnTable(new List<string>() { "" });
                _wordDocumentService.AddTableHeader("C.3 NOTES");
                _wordDocumentService.AddOneColumnTable(new List<string>() { "" });
            }
        }

        private void PrepareEvaluationsForActiveReport(PatientClinicalEvent patientClinicalEvent)
        {
            List<string[]> rows = new();
            List<string> cells = new();

            cells.Add("Test");
            cells.Add("Test Date (yyyy-mm-dd)");
            cells.Add("Test Result");

            rows.Add(cells.ToArray());

            foreach (PatientLabTest labTest in patientClinicalEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientClinicalEvent.OnsetDate).OrderByDescending(lt => lt.TestDate))
            {
                cells = new();

                cells.Add(labTest.LabTest.Description);
                cells.Add(labTest.TestDate.ToString("yyyy-MM-dd"));
                cells.Add(labTest.TestResult);

                rows.Add(cells.ToArray());
            }

            _wordDocumentService.AddRowTable(rows, new int[] { 2500, 2500, 6352 });
        }

        private async Task PrepareEvaluationsForSpontaneousReport(DatasetInstance datasetInstance)
        {
            List<string[]> rows = new();
            List<string> cells = new();

            cells.Add("Test");
            cells.Add("Test Date");
            cells.Add("Test Result");

            rows.Add(cells.ToArray());

            var sourceLabElement = await _datasetElementRepository.GetAsync(dse => dse.ElementName == "Test Results");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceLabElement.ElementName);

            foreach (Guid sourceContext in sourceContexts)
            {
                var labItemValues = datasetInstance.GetInstanceSubValues(sourceLabElement.ElementName, sourceContext);

                var testDate = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Date");
                var testResult = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Result");

                cells = new();

                cells.Add(labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Name").InstanceValue);
                cells.Add(testDate != null ? Convert.ToDateTime(testDate.InstanceValue).ToString("yyyy-MM-dd") : string.Empty);
                cells.Add(testResult != null ? testResult.InstanceValue : string.Empty);

                rows.Add(cells.ToArray());
            }

            _wordDocumentService.AddRowTable(rows, new int[] { 2500, 2500, 6352 });
        }

        private void PrepareWeightForActiveReport(PatientClinicalEvent patientClinicalEvent)
        {
            List<string[]> rows = new();
            List<string> cells = new();

            cells.Add("Weight Date");
            cells.Add("Weight");

            rows.Add(cells.ToArray());

            var weightSeries = _patientService.GetElementValues(patientClinicalEvent.Patient.Id, "Weight(kg)", 10);

            if (weightSeries.Length > 0)
            {
                foreach (var weight in weightSeries[0].Series)
                {
                    cells = new();

                    cells.Add(weight.Name);
                    cells.Add(weight.Value.ToString());

                    rows.Add(cells.ToArray());
                }
            }

            _wordDocumentService.AddRowTable(rows, new int[] { 2500, 8852 });
        }

        private int CalculateAge(DateTime birthDate, DateTime onsetDate)
        {
            var age = onsetDate.Year - birthDate.Year;
            if (onsetDate > birthDate.AddYears(-age)) age--;
            return age;
        }
    }
}