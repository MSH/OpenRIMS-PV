using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Entities.Keyless;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Core.ValueTypes;
using OpenRIMS.PV.Main.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.Services
{
    public class PatientService : IPatientService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientStatus> _patientStatusRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<EncounterTypeWorkPlan> _encounterTypeWorkPlanRepository;
        private readonly IRepositoryInt<LabTest> _labTestRepository;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<Priority> _priorityRepository;
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ITypeExtensionHandler _typeExtensionHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MainDbContext _context;

        public PatientService(IUnitOfWorkInt unitOfWork,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientStatus> patientStatusRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<EncounterTypeWorkPlan> encounterTypeWorkPlanRepository,
            IRepositoryInt<LabTest> labTestRepository,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<Priority> priorityRepository,
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<User> userRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            IHttpContextAccessor httpContextAccessor,
            MainDbContext dbContext)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientStatusRepository = patientStatusRepository ?? throw new ArgumentNullException(nameof(patientStatusRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _encounterTypeWorkPlanRepository = encounterTypeWorkPlanRepository ?? throw new ArgumentNullException(nameof(encounterTypeWorkPlanRepository));
            _labTestRepository = labTestRepository ?? throw new ArgumentNullException(nameof(labTestRepository));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _priorityRepository = priorityRepository ?? throw new ArgumentNullException(nameof(priorityRepository));
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _typeExtensionHandler = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public SeriesValueList[] GetElementValues(long patientId, string elementName, int records)
        {
            var patientFromRepo = _patientRepository.Get(p => p.Id == patientId);
            if (patientFromRepo == null)
            {
                throw new ArgumentException(nameof(patientId));
            }
            if (string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentException(nameof(elementName));
            }

            var encounters = _encounterRepository.List(e => e.Patient.Id == patientId)
                .OrderBy(e => e.EncounterDate);

            var seriesValueArray = new List<SeriesValueList>();
            var seriesValueList = new SeriesValueList()
            {
                Name = elementName
            };

            var values = new List<SeriesValueListItem>();
            foreach (Encounter encounter in encounters)
            {
                var datasetInstance = _datasetInstanceRepository.Get(di => di.ContextId == encounter.Id && di.Dataset.ContextType.Id == (int)ContextTypes.Encounter);
                if(datasetInstance != null)
                {
                    var value = datasetInstance.GetInstanceValue(elementName);
                    var decimalValue = 0M;
                    Decimal.TryParse(value, out decimalValue);
                    if(!String.IsNullOrWhiteSpace(value))
                    {
                        var modelItem = new SeriesValueListItem()
                        {
                            Value = decimalValue,
                            //Min = intValue - ((intValue * 20) / 100),
                            //Max = intValue + ((intValue * 20) / 100),
                            Name = encounter.EncounterDate.ToString("yyyy-MM-dd")
                        };
                        values.Add(modelItem);
                        if(values.Count >= records) { break; }
                    }
                }
            }
            seriesValueList.Series = values;
            seriesValueArray.Add(seriesValueList);
            return seriesValueArray.ToArray();
        }

        public async Task<SeriesValueListItem> GetCurrentElementValueForPatientAsync(long patientId, string elementName)
        {
            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == patientId, new string[] { "Encounters" });
            if (patientFromRepo == null)
            {
                throw new ArgumentException(nameof(patientId));
            }
            if(string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentException(nameof(elementName));
            }

            var encounter = patientFromRepo.GetCurrentEncounter();
            if (encounter == null) return null;

            var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.ContextId == encounter.Id && di.Dataset.ContextType.Id == (int)ContextTypes.Encounter, new string[] { 
                "DatasetInstanceValues.DatasetElement" 
            });
            if (datasetInstance == null) return null;

            var value = datasetInstance.GetInstanceValue(elementName);
            var tempDecimal = 0.00M;
            if (decimal.TryParse(value, out tempDecimal))
            {
                return new SeriesValueListItem()
                {
                    Value = tempDecimal,
                    Name = encounter.EncounterDate.ToString("yyyy-MM-dd")
                };
            }

            return null;
        }

        /// <summary>
        /// Add a new patient to the repository
        /// </summary>
        /// <param name="patientDetail">The details of the patient to add</param>
        public async Task<int> AddPatientAsync(PatientDetailForCreation patientDetail)
        {
            var facility = _facilityRepository.Get(f => f.FacilityName == patientDetail.CurrentFacilityName);
            if(facility == null)
            {
                throw new ArgumentException(nameof(patientDetail.CurrentFacilityName));
            }
            var patientStatus = _patientStatusRepository.Get(f => f.Description == "Active");

            var newPatient = new Patient
            {
                FirstName = patientDetail.FirstName,
                MiddleName = patientDetail.MiddleName,
                Surname = patientDetail.Surname,
                DateOfBirth = patientDetail.DateOfBirth,
            };
            newPatient.ChangePatientFacility(facility);
            newPatient.SetPatientStatus(patientStatus);

            // Custom Property handling
            _typeExtensionHandler.UpdateExtendable(newPatient, patientDetail.CustomAttributes, "Admin");

            // Clinical data
            AddConditions(newPatient, patientDetail.Conditions);
            await AddLabTestsAsync(newPatient, patientDetail.LabTests);
            await AddMedicationsAsync(newPatient, patientDetail.Medications);
            AddClinicalEvents(newPatient, patientDetail.ClinicalEvents);

            // Other data
            AddAttachments(newPatient, patientDetail.Attachments);

            if(patientDetail.CohortGroupId > 0)
            {
                AddCohortEnrollment(newPatient, patientDetail);
            }

            await _patientRepository.SaveAsync(newPatient);

            // Register encounter
            if(patientDetail.EncounterTypeId > 0)
            {
                await AddEncounterAsync(newPatient, new EncounterDetail() { 
                    EncounterDate = patientDetail.EncounterDate, 
                    EncounterTypeId = patientDetail.EncounterTypeId, 
                    PatientId = newPatient.Id, 
                    PriorityId = patientDetail.PriorityId });
            }

            return newPatient.Id;
        }

        /// <summary>
        /// Update an existing patient in the repository
        /// </summary>
        /// <param name="patientDetail">The details of the patient to add</param>
        public async Task UpdatePatientAsync(PatientDetailForUpdate patientDetail)
        {
            var identifier_asm = patientDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == "Medical Record Number")?.Value.ToString();
            var identifierChanged = false;

            if(String.IsNullOrWhiteSpace(identifier_asm))
            {
                throw new Exception("Unable to locate patient in repo for update");
            }

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });

            var patientFromRepo = GetPatientUsingAttributes(parameters);
            if (patientFromRepo == null)
            {
                throw new ArgumentException(nameof(patientDetail));
            }

            if(!String.IsNullOrWhiteSpace(patientDetail.FirstName))
            {
                if(!patientFromRepo.FirstName.Equals(patientDetail.FirstName, StringComparison.OrdinalIgnoreCase))
                {
                    identifierChanged = true;
                }
                patientFromRepo.FirstName = patientDetail.FirstName;
            }

            if (!String.IsNullOrWhiteSpace(patientDetail.Surname))
            {
                if (!patientFromRepo.Surname.Equals(patientDetail.Surname, StringComparison.OrdinalIgnoreCase))
                {
                    identifierChanged = true;
                }
                patientFromRepo.Surname = patientDetail.Surname;
            }

            if (!String.IsNullOrWhiteSpace(patientDetail.MiddleName))
            {
                patientFromRepo.MiddleName = patientDetail.MiddleName;
            }

            if(patientDetail.DateOfBirth.HasValue)
            {
                if (!patientFromRepo.DateOfBirth.Equals(patientDetail.DateOfBirth))
                {
                    identifierChanged = true;
                }
                patientFromRepo.DateOfBirth = patientDetail.DateOfBirth;
            }

            // Custom Property handling
            _typeExtensionHandler.UpdateExtendable(patientFromRepo, patientDetail.CustomAttributes, "Admin");

            // Clinical data
            AddConditions(patientFromRepo, patientDetail.Conditions);
            await AddLabTestsAsync(patientFromRepo, patientDetail.LabTests);
            await AddMedicationsAsync(patientFromRepo, patientDetail.Medications);
            AddClinicalEvents(patientFromRepo, patientDetail.ClinicalEvents);

            // Other data
            AddAttachments(patientFromRepo, patientDetail.Attachments);

            _patientRepository.Update(patientFromRepo);

            // Register encounter
            if (patientDetail.EncounterTypeId > 0)
            {
                await AddEncounterAsync(patientFromRepo, new EncounterDetail() { 
                    EncounterDate = patientDetail.EncounterDate, 
                    EncounterTypeId = patientDetail.EncounterTypeId, 
                    PatientId = patientFromRepo.Id, 
                    PriorityId = patientDetail.PriorityId 
                });
            }

            if(identifierChanged)
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);

                var auditLog = new AuditLog()
                {
                    AuditType = AuditType.DataValidation,
                    User = userFromRepo,
                    ActionDate = DateTime.Now,
                    Details = $"Identifier (name or date of birth) changed for patient {identifier_asm}"
                };
                await _auditLogRepository.SaveAsync(auditLog);
            }
        }

        /// <summary>
        /// Register patient encounter
        /// </summary>
        public async Task<int> AddEncounterAsync(Patient patient, EncounterDetail encounterDetail)
        {
            var encounterType = _encounterTypeRepository.Get(et => et.Id == encounterDetail.EncounterTypeId);
            if (encounterType == null)
            {
                throw new ArgumentException(nameof(encounterDetail.EncounterTypeId));
            }

            var priority = _priorityRepository.Get(p => p.Id == encounterDetail.PriorityId);
            if (priority == null)
            {
                throw new ArgumentException(nameof(encounterDetail.PriorityId));
            }

            var newEncounter = new Encounter(patient)
            {
                EncounterType = encounterType,
                Priority = priority,
                EncounterDate = encounterDetail.EncounterDate,
                Notes = encounterDetail.Notes
            };

            //newEncounter.AuditStamp(user);
            await _encounterRepository.SaveAsync(newEncounter);

            var encounterTypeWorkPlan = _encounterTypeWorkPlanRepository.Get(et => et.EncounterType.Id == encounterType.Id, new string[] { "WorkPlan.Dataset" });
            if (encounterTypeWorkPlan != null)
            {
                // Create a new instance
                var dataset = _datasetRepository.Get(d => d.Id == encounterTypeWorkPlan.WorkPlan.Dataset.Id, new string[] { "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType" });
                if (dataset != null)
                {
                    var datasetInstance = dataset.CreateInstance(newEncounter.Id, "", encounterTypeWorkPlan, null, null);
                    await _datasetInstanceRepository.SaveAsync(datasetInstance);
                }
            }

            return newEncounter.Id;
        }

        /// <summary>
        /// Check patient's custom attributes to ensure they are unique within the repository
        /// </summary>
        /// <param name="parameters">The list of custom attributes that should be checked</param>
        public bool isUnique(List<CustomAttributeParameter> parameters, int patientId = 0)
        {
            if(parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if(parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            string sql = "SELECT Id FROM Patient p CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y) WHERE ";
            sql += $"p.Id != {patientId} AND (";

            foreach(var parameter in parameters)
            {
                sql += $"X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeKey.Trim()}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeValue.Trim()}' ";
                sql += "OR ";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += ")";

            var result = _context.PatientIdLists
                .FromSqlRaw<PatientIdList>(sql).ToList();
            return result.Count == 0;
        }

        /// <summary>
        /// Check patient's custom attributes to ensure they exist within the repository
        /// </summary>
        /// <param name="parameters">The list of custom attributes that should be checked</param>
        public bool Exists(List<CustomAttributeParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            //string sql = "SELECT Id FROM Patient p CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y) WHERE ";

            //foreach (var parameter in parameters)
            //{
            //    sql += $"X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeKey.Trim()}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeValue.Trim()}' ";
            //    sql += "OR ";
            //}
            //sql = sql.Substring(0, sql.Length - 3);

            //var result = _context.PatientLists
            //    .FromSqlInterpolated($"Exec spPatientExists {parameters}").ToList();
            //return result.Count > 0;
            return false;
        }

        /// <summary>
        /// Fetch a patient record using the supplied attributes
        /// </summary>
        /// <param name="parameters">The list of custom attributes that should be checked</param>
        public Patient GetPatientUsingAttributes(List<CustomAttributeParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            string sql = "SELECT Id FROM Patient p CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y) WHERE ";

            foreach (var parameter in parameters)
            {
                sql += $"X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeKey.Trim()}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeValue.Trim()}' ";
                sql += "OR ";
            }
            sql = sql.Substring(0, sql.Length - 3);

            //var result = _context.PatientLists
            //    .FromSqlInterpolated($"Exec spPatients {parameters}").ToList();

            //if(result.Count == 0)
            //{
            //    return null;
            //}
            //return _patientRepository.Get(p => p.Id == result.First().PatientId);
            return _patientRepository.Get(p => p.Id == 1);
        }

        /// <summary>
        /// Prepare patient record with associated conditions
        /// </summary>
        private void AddConditions(Patient patient, List<ConditionDetail> conditions)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }
            if (conditions.Count == 0)
            {
                return;
            }

            foreach (var condition in conditions)
            {
                var treatmentOutcome = !String.IsNullOrWhiteSpace(condition.TreatmentOutcome) ? _unitOfWork.Repository<TreatmentOutcome>().Get(to => to.Description == condition.TreatmentOutcome) : null;
                var terminologyMedDra = condition.MeddraTermId != null ? _unitOfWork.Repository<TerminologyMedDra>().Get(tm => tm.Id == condition.MeddraTermId) : null;

                var patientCondition = new PatientCondition
                {
                    Patient = patient,
                    ConditionSource = condition.ConditionSource,
                    TerminologyMedDra = terminologyMedDra,
                    OnsetDate = Convert.ToDateTime(condition.OnsetDate),
                    TreatmentOutcome = treatmentOutcome,
                    CaseNumber = condition.CaseNumber,
                    Comments = condition.Comments
                };

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(patientCondition, condition.CustomAttributes, "Admin");

                patient.PatientConditions.Add(patientCondition);
            }
        }

        /// <summary>
        /// Prepare patient record with associated lab tests
        /// </summary>
        private async Task AddLabTestsAsync(Patient patient, List<LabTestDetail> labTests) 
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (labTests == null)
            {
                throw new ArgumentNullException(nameof(labTests));
            }
            if (labTests.Count == 0)
            {
                return;
            }

            foreach (var labTest in labTests)
            {
                var labTestFromRepo = await _labTestRepository.GetAsync(lt => lt.Description == labTest.LabTestSource);
                var newLabTest = patient.AddLabTest(labTest.TestDate, labTest.TestResult, labTestFromRepo, null, string.Empty, string.Empty, string.Empty);

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(newLabTest, labTest.CustomAttributes, "Admin");

                patient.PatientLabTests.Add(newLabTest);
            }
        }

        /// <summary>
        /// Prepare patient record with associated medications
        /// </summary>
        private async Task AddMedicationsAsync(Patient patient, List<MedicationDetail> medications)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (medications == null)
            {
                throw new ArgumentNullException(nameof(medications));
            }
            if (medications.Count == 0)
            {
                return;
            }

            var concept = await _conceptRepository.GetAsync(c => c.ConceptName == "NOT ASSIGNED");
            foreach (var medication in medications)
            {
                var patientMedication = patient.AddMedication(concept, medication.DateStart, medication.DateEnd, medication.Dose, medication.DoseFrequency, "", null, medication.MedicationSource);
                _typeExtensionHandler.UpdateExtendable(patientMedication, medication.CustomAttributes, "Admin");
            }
        }

        /// <summary>
        /// Prepare patient record with associated clinical events
        /// </summary>
        private void AddClinicalEvents(Patient patient, List<ClinicalEventDetail> clinicalEvents)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (clinicalEvents == null)
            {
                throw new ArgumentNullException(nameof(clinicalEvents));
            }
            if (clinicalEvents.Count == 0)
            {
                return;
            }

            foreach (var clinicalEvent in clinicalEvents)
            {
                var newClinicalEvent = patient.AddClinicalEvent(clinicalEvent.OnsetDate, clinicalEvent.ResolutionDate, null, clinicalEvent.SourceDescription);

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(newClinicalEvent, clinicalEvent.CustomAttributes, "Admin");
            }
        }

        /// <summary>
        /// Prepare patient record with associated attachments
        /// </summary>
        private void AddAttachments(Patient patient, List<AttachmentDetail> attachments)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }
            if (attachments.Count == 0)
            {
                return;
            }

            // Handle attachments
            foreach (var sourceAttachment in attachments)
            {
                var attachmentType = _unitOfWork.Repository<AttachmentType>()
                    .Queryable()
                    .SingleOrDefault(u => u.Key == "jpg");

                //var byt = Encoding.ASCII.GetBytes(sourceAttachment.ImageSource);
                //string hash;
                //using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                //{
                //    hash = Convert.ToBase64String(sha1.ComputeHash(byt));
                //}

                // Create the attachment
                var attachment = new Attachment(Convert.FromBase64String(sourceAttachment.ImageSource.Replace("data:image/jpeg;base64,", "")), sourceAttachment.Description, $"{Guid.NewGuid()}.jpg", 0, attachmentType, null, patient, null);
                patient.Attachments.Add(attachment);
            }
        }

        /// <summary>
        /// Enrol patient into cohort
        /// </summary>
        private void AddCohortEnrollment(Patient patient, PatientDetailForCreation patientDetail)
        {
            var cohortGroup = _cohortGroupRepository.Get(cg => cg.Id == patientDetail.CohortGroupId);
            var enrolment = new CohortGroupEnrolment
            {
                Patient = patient,
                EnroledDate = Convert.ToDateTime(patientDetail.EnroledDate),
                CohortGroup = cohortGroup
            };

            patient.CohortEnrolments.Add(enrolment);
        }
    }
}
