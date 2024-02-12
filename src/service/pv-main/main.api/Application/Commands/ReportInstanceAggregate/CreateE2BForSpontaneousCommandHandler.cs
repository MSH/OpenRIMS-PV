using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ContactAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    public class CreateE2BForSpontaneousCommandHandler
        : IRequestHandler<CreateE2BForSpontaneousCommand, bool>
    {
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<SiteContactDetail> _siteContactDetailRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateE2BForSpontaneousCommandHandler> _logger;

        public CreateE2BForSpontaneousCommandHandler(
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetElement> datasetElementRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<SiteContactDetail> siteContactDetailRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateE2BForSpontaneousCommandHandler> logger)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _siteContactDetailRepository = siteContactDetailRepository ?? throw new ArgumentNullException(nameof(siteContactDetailRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateE2BForSpontaneousCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId, new string[] { "CreatedBy", "WorkFlow" });

            if (reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var spontaneousReport = await _datasetInstanceRepository.GetAsync(ds => ds.DatasetInstanceGuid == reportInstanceFromRepo.ContextGuid,
                new string[] { "DatasetInstanceValues.DatasetElement.Field.FieldType"
                    , "DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType"
                });
            if (spontaneousReport == null)
            {
                throw new KeyNotFoundException("Unable to locate spontaneous report");
            }

            var dataset = await GetDatasetForInstance();
            if (dataset == null)
            {
                throw new KeyNotFoundException("Unable to locate E2B dataset");
            }

            var newActivityExecutionStatusEvent = await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "E2BINITIATED", "AUTOMATION: E2B dataset created", null, "");

            var e2bInstance = dataset.CreateInstance(newActivityExecutionStatusEvent.Id, "Spontaneous", null, spontaneousReport, null);
            await _datasetInstanceRepository.SaveAsync(e2bInstance);

            await UpdateValuesUsingSourceAsync(e2bInstance, spontaneousReport, dataset);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- E2B instance created for spontaneous report {message.ReportInstanceId} created");

            return true;
        }

        private async Task UpdateValuesUsingSourceAsync(DatasetInstance e2bInstance, DatasetInstance spontaneousReport, Dataset dataset)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);

            if (dataset.DatasetName.Contains("(R2)"))
            {
                await SetInstanceValuesForSpontaneousRelease2Async(e2bInstance, spontaneousReport, user);
            }
            if (dataset.DatasetName.Contains("(R3)"))
            {
                SetInstanceValuesForSpontaneousRelease3(e2bInstance, spontaneousReport, user);
            }
        }

        private async Task<Dataset> GetDatasetForInstance()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.E2BVersion);
            if (config == null)
            {
                throw new KeyNotFoundException("Unable to locate E2BVersion configuration");
            }
            var datasetName = config.ConfigValue;
            return await _datasetRepository.GetAsync(d => d.DatasetName == datasetName,
                new string[] {
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.DatasetMappingValues",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.SubMappings.DestinationElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DestinationMappings.SubMappings.SourceElement.Field.FieldType"
                });
        }

        private async Task SetInstanceValuesForSpontaneousRelease2Async(DatasetInstance e2bInstance, DatasetInstance spontaneousReport, User currentUser)
        {
            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == spontaneousReport.DatasetInstanceGuid, new string[] { "Medications", "TerminologyMedDra" });

            // ************************************* ichicsrmessageheader
            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Message Header").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Message Number").DatasetElement, e2bInstance.Id.ToString("D8"));
            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Message Header").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Message Date").DatasetElement, DateTime.Today.ToString("yyyyMMddhhmmss"));

            MapSafetyReportRelatedFields(e2bInstance, spontaneousReport, reportInstance);
            MapPrimarySourceRelatedFields(e2bInstance, spontaneousReport);
            MapSenderAndReceivedRelatedFields(e2bInstance);

            DateTime? onset, recovery;
            MapPatientRelatedFields(e2bInstance, spontaneousReport, out onset, out recovery);

            await MapReactionRelatedFieldsAsync(e2bInstance, reportInstance, onset, recovery);
            MapTestRelatedFields(e2bInstance, spontaneousReport);
            await MapDrugRelatedFieldsAsync(e2bInstance, spontaneousReport, reportInstance);
        }

        private void MapSafetyReportRelatedFields(DatasetInstance e2bInstance, DatasetInstance spontaneousReport, ReportInstance reportInstance)
        {
            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Safety Report ID").DatasetElement, string.Format("RW.FDA.{0}", reportInstance.Id.ToString("D6")));
            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Transmission Date").DatasetElement, DateTime.Today.ToString("yyyyMMdd"));
            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Report Type").DatasetElement, "1");

            var seriousReason = spontaneousReport.GetInstanceValue("Reaction serious details");
            if (!String.IsNullOrWhiteSpace(seriousReason))
            {
                var sd = "2=No";
                var slt = "2=No";
                var sh = "2=No";
                var sdi = "2=No";
                var sca = "2=No";
                var so = "2=No";

                switch (seriousReason)
                {
                    case "Resulted in death":
                        sd = "1=Yes";
                        break;

                    case "Is life-threatening":
                        slt = "1=Yes";
                        break;

                    case "Is a congenital anomaly/birth defect":
                        sca = "1=Yes";
                        break;

                    case "Requires hospitalization or longer stay in hospital":
                        sh = "1=Yes";
                        break;

                    case "Results in persistent or significant disability/incapacity (as per reporter's opinion)":
                        sdi = "1=Yes";
                        break;

                    case "Other medically important condition":
                        so = "1=Yes";
                        break;

                    default:
                        break;
                }

                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA"), sd); //Seriousness Death
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "26C6F08E-B80B-411E-BFDC-0506FE102253"), slt); //Seriousness Life Threatening
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "837154A9-D088-41C6-A9E2-8A0231128496"), sh); //Seriousness Hospitalization
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DDEBDEC0-2A90-49C7-970E-B7855CFDF19D"), sdi); //Seriousness Disabling
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DF89C98B-1D2A-4C8E-A753-02E265841F4F"), sca); //Seriousness Congenital Anomaly
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "33A75547-EF1B-42FB-8768-CD6EC52B24F8"), so); //Seriousness Other
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), "1"); //Serious
            }
            else
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), "2"); //Serious
            }

            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date report was first received").DatasetElement, spontaneousReport.Created.ToString("yyyyMMdd"));
            e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date of most recent info").DatasetElement, spontaneousReport.Created.ToString("yyyyMMdd"));
        }

        private void MapPrimarySourceRelatedFields(DatasetInstance e2bInstance, DatasetInstance spontaneousReport)
        {
            var fullName = spontaneousReport.GetInstanceValue("Reporter Name");
            if (!String.IsNullOrWhiteSpace(fullName))
            {
                if (fullName.Contains(" "))
                {
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Given Name").DatasetElement, fullName.Substring(0, fullName.IndexOf(" ")));
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Family Name").DatasetElement, fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1)));
                }
                else
                {
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Given Name").DatasetElement, fullName);
                }
            }

            var profession = spontaneousReport.GetInstanceValue("Profession");
            switch (profession)
            {
                case "Other health professional":
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Qualification").DatasetElement, "3=Other Health Professional");
                    break;

                case "Physician":
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Qualification").DatasetElement, "1=Physician");
                    break;

                case "Consumer or other non-health professional":
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Qualification").DatasetElement, "5=Consumer or other non health professional");
                    break;

                case "Pharmacist":
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Qualification").DatasetElement, "2=Pharmacist");
                    break;

                case "Lawyer":
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Qualification").DatasetElement, "4=Lawyer");
                    break;

                default:
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Qualification").DatasetElement, "5=Consumer or other non health professional");
                    break;
            }
        }

        private void MapSenderAndReceivedRelatedFields(DatasetInstance e2bInstance)
        {
            var sendingAuthority = _siteContactDetailRepository.Get(cd => cd.ContactType == ContactType.SendingAuthority);
            if (sendingAuthority != null)
            {
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Type"), ((int)sendingAuthority.OrganisationType).ToString());
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Organization"), sendingAuthority.OrganisationName);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Department"), sendingAuthority.DepartmentName);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Given Name"), sendingAuthority.ContactFirstName);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Family Name"), sendingAuthority.ContactSurname);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Street Address"), sendingAuthority.StreetAddress);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender City"), sendingAuthority.City);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender State"), sendingAuthority.State);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Postcode"), sendingAuthority.PostCode);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Tel Number"), sendingAuthority.ContactNumber);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Tel Country Code"), sendingAuthority.CountryCode);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Sender Email Address"), sendingAuthority.ContactEmail);
            }

            var receivingAuthority = _siteContactDetailRepository.Get(cd => cd.ContactType == ContactType.ReceivingAuthority);
            if (receivingAuthority != null)
            {
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Type"), ((int)receivingAuthority.OrganisationType).ToString());
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Organization"), receivingAuthority.OrganisationName);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Department"), receivingAuthority.DepartmentName);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Given Name"), receivingAuthority.ContactFirstName);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Family Name"), receivingAuthority.ContactSurname);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Street Address"), receivingAuthority.StreetAddress);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver City"), receivingAuthority.City);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver State"), receivingAuthority.State);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Postcode"), receivingAuthority.PostCode);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Tel"), receivingAuthority.ContactNumber);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Tel Country Code"), receivingAuthority.CountryCode);
                e2bInstance.SetInstanceValue(_datasetElementRepository.Get(dse => dse.ElementName == "Receiver Email Address"), receivingAuthority.ContactEmail);
            }
        }

        private void MapPatientRelatedFields(DatasetInstance e2bInstance, DatasetInstance spontaneousReport, out DateTime? onset, out DateTime? recovery)
        {
            var init = spontaneousReport.GetInstanceValue("Initials");
            if (!String.IsNullOrWhiteSpace(init)) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A0BEAB3A-0B0A-457E-B190-1B66FE60CA73"), init); }; //Patient Initial

            DateTime? dob = null;
            if(!String.IsNullOrWhiteSpace(spontaneousReport.GetInstanceValue("Date of Birth")))
            {
                dob = Convert.ToDateTime(spontaneousReport.GetInstanceValue("Date of Birth"));
            }
            onset = null;
            if (!String.IsNullOrWhiteSpace(spontaneousReport.GetInstanceValue("Reaction known start date")))
            {
                onset = Convert.ToDateTime(spontaneousReport.GetInstanceValue("Reaction known start date"));
            }
            recovery = null;
            if (!String.IsNullOrWhiteSpace(spontaneousReport.GetInstanceValue("Reaction date of recovery")))
            {
                recovery = Convert.ToDateTime(spontaneousReport.GetInstanceValue("Reaction date of recovery"));
            }

            if (dob.HasValue)
            {
                e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Birthdate").DatasetElement, dob.Value.ToString("yyyyMMdd"));

                if (onset.HasValue)
                {
                    var age = onset.Value.Year - dob.Value.Year;
                    if (dob.Value > onset.Value.AddYears(-age)) age--;
                    if(age < 1) { age = 1; };

                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Onset Age").DatasetElement, age.ToString());
                    e2bInstance.SetInstanceValue(e2bInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Onset Age Unit").DatasetElement, "801=Year");
                }
            }
        }

        private async Task MapReactionRelatedFieldsAsync(DatasetInstance e2bInstance, ReportInstance reportInstance, DateTime? onset, DateTime? recovery)
        {
            var terminologyMedDra = reportInstance.TerminologyMedDra;
            var term = terminologyMedDra != null ? terminologyMedDra.DisplayName : "";
            if (!String.IsNullOrWhiteSpace(term))
            {
                var datasetElement = await _datasetElementRepository.GetAsync(dse => dse.DatasetElementGuid.ToString() == "C8DD9A5E-BD9A-488D-8ABF-171271F5D370");
                e2bInstance.SetInstanceValue(datasetElement, term);
            }; //Reaction MedDRA LLT

            if (onset.HasValue)
            {
                var datasetElement = await _datasetElementRepository.GetAsync(dse => dse.DatasetElementGuid.ToString() == "1EAD9E11-60E6-4B27-9A4D-4B296B169E90");
                e2bInstance.SetInstanceValue(datasetElement, onset.Value.ToString("yyyyMMdd"));
            }; //Reaction Start Date

            if (recovery.HasValue)
            {
                var datasetElement = await _datasetElementRepository.GetAsync(dse => dse.DatasetElementGuid.ToString() == "3A0F240E-8B36-48F6-9527-77E55F6E7CF1");
                e2bInstance.SetInstanceValue(datasetElement, recovery.Value.ToString("yyyyMMdd"));
            }; // Reaction End Date

            if (onset.HasValue && recovery.HasValue)
            {
                var rduration = (recovery.Value - onset.Value).Days;
                var datasetElement = await _datasetElementRepository.GetAsync(dse => dse.DatasetElementGuid.ToString() == "0712C664-2ADD-44C0-B8D5-B6E83FB01F42");
                e2bInstance.SetInstanceValue(datasetElement, rduration.ToString()); //Reaction Duration

                datasetElement = await _datasetElementRepository.GetAsync(dse => dse.DatasetElementGuid.ToString() == "F96E702D-DCC5-455A-AB45-CAEFF25BF82A");
                e2bInstance.SetInstanceValue(datasetElement, "804=Day"); //Reaction Duration Unit
            }
        }

        private void MapTestRelatedFields(DatasetInstance e2bInstance, DatasetInstance spontaneousReport)
        {
            //var destinationTestElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "693A2E8C-B041-46E7-8687-0A42E6B3C82E"); // Test History
            //foreach (PatientLabTest labTest in activeReport.Patient.PatientLabTests.Where(lt => lt.TestDate >= activeReport.OnsetDate).OrderByDescending(lt => lt.TestDate))
            //{
            //    var newContext = Guid.NewGuid();

            //    e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Date"), labTest.TestDate.ToString("yyyyMMdd"), (Guid)newContext);
            //    e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Name"), labTest.LabTest.Description, (Guid)newContext);

            //    var testResult = !String.IsNullOrWhiteSpace(labTest.LabValue) ? labTest.LabValue : !String.IsNullOrWhiteSpace(labTest.TestResult) ? labTest.TestResult : "";
            //    e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Result"), testResult, (Guid)newContext);

            //    var testUnit = labTest.TestUnit != null ? labTest.TestUnit.Description : "";
            //    if (!String.IsNullOrWhiteSpace(testUnit)) { e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Unit"), testUnit, (Guid)newContext); };

            //    var lowRange = labTest.ReferenceLower;
            //    if (!String.IsNullOrWhiteSpace(lowRange)) { e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Low Test Range"), lowRange, (Guid)newContext); };

            //    var highRange = labTest.ReferenceUpper;
            //    if (!String.IsNullOrWhiteSpace(highRange)) { e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "High Test Range"), highRange, (Guid)newContext); };
            //}
        }

        private async Task MapDrugRelatedFieldsAsync(DatasetInstance e2bInstance, DatasetInstance spontaneousReport, ReportInstance reportInstance)
        {
            string[] validNaranjoCriteria = { "Possible", "Probable", "Definite" };
            string[] validWHOCriteria = { "Possible", "Probable", "Certain" };

            var destinationProductElement = await _datasetElementRepository.GetAsync(de => de.DatasetElementGuid.ToString() == "E033BDE8-EDC8-43FF-A6B0-DEA6D6FA581C", new string[] { "DatasetElementSubs" }); // Medicinal Products
            var sourceContexts = spontaneousReport.GetInstanceSubValuesContext("Product Information");
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                var newContext = Guid.NewGuid();

                var drugItemValues = spontaneousReport.GetInstanceSubValues("Product Information", med.ReportInstanceMedicationGuid);
                var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                var character = "";
                character = (validNaranjoCriteria.Contains(med.NaranjoCausality) || validWHOCriteria.Contains(med.WhoCausality)) ? "1=Suspect" : "2=Concomitant";
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Characterization"), character, (Guid)newContext);

                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Medicinal Product"), drugName, (Guid)newContext);

                var batchNumber = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product Batch Number")?.InstanceValue;
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Batch Number"), batchNumber, (Guid)newContext);

                var doseFrequency = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product Frequency")?.InstanceValue;
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Text"), doseFrequency, (Guid)newContext);

                var startdate = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                if (startdate != null)
                {
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Start Date"), Convert.ToDateTime(startdate.InstanceValue).ToString("yyyyMMdd"), (Guid)newContext);
                }   

                var enddate = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");
                if (enddate != null)
                {
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug End Date"), Convert.ToDateTime(enddate.InstanceValue).ToString("yyyyMMdd"), (Guid)newContext);
                }

                if (startdate != null && enddate != null)
                {
                    var rduration = (Convert.ToDateTime(enddate.InstanceValue) - Convert.ToDateTime(startdate.InstanceValue)).Days;
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration"), rduration.ToString(), (Guid)newContext);
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration Unit"), "804=Day", (Guid)newContext);
                }

                // Dosage
                if (drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Strength") != null && drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose Number") != null)
                {
                    decimal strength = ConvertValueToDecimal(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Strength").InstanceValue);
                    decimal dosage = ConvertValueToDecimal(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose Number").InstanceValue);

                    decimal dosageCalc = strength * dosage;
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Structured Dosage"), dosageCalc.ToString(), (Guid)newContext);
                }

                if (med.WhoCausality != null)
                {
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), reportInstance.TerminologyMedDra.DisplayName, (Guid)newContext);
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "WHO Causality Scale", (Guid)newContext);
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.WhoCausality.ToLowerInvariant() == "ignored" ? "" : med.WhoCausality, (Guid)newContext);
                }
                else
                {
                    if (med.NaranjoCausality != null)
                    {
                        e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), reportInstance.TerminologyMedDra.DisplayName, (Guid)newContext);
                        e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "Naranjo Causality Scale", (Guid)newContext);
                        e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.NaranjoCausality.ToLowerInvariant() == "ignored" ? "" : med.NaranjoCausality, (Guid)newContext);
                    }
                }
            } // foreach (ReportInstanceMedication med in reportInstance.Medications)
        }

        private void SetInstanceValuesForSpontaneousRelease3(DatasetInstance e2bInstance, DatasetInstance spontaneousReport, User currentUser)
        {
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.2 Batch Number"), "MSH.PViMS-B01000" + spontaneousReport.ToString());
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.5 Date of Batch Transmission"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.1 Message Identifier"), "MSH.PViMS-B01000" + spontaneousReport.ToString() + "-" + DateTime.Now.ToString("mmsss"));
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.4 Date of Message Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.1 Sender’s (case) Safety Report Unique Identifier"), String.Format("US-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), spontaneousReport.Id.ToString()));
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.2 Date of Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.8.1 Worldwide Unique Case Identification Number"), String.Format("US-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), spontaneousReport.Id.ToString()));

            // Default remaining fields
            // C1 Identification
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.3 Type of Report"), "1=Spontaneous report");
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.4 Date Report Was First Received from Source"), spontaneousReport.Created.ToString("yyyy-MM-dd"));

            // C2 Primary Source
            var fullName = spontaneousReport.GetInstanceValue("Reporter Name");
            if (fullName != string.Empty)
            {
                if (fullName.Contains(" "))
                {
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.2 Reporter’s Given Name"), fullName.Substring(0, fullName.IndexOf(" ")));
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.4 Reporter’s Family Name"), fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1)));
                }
                else
                {
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.2 Reporter’s Given Name"), fullName);
                }
            }
            var place = spontaneousReport.GetInstanceValue("Reporter Place of Practise");
            if (place != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.1 Reporter’s Organisation"), place);
            }
            var address = spontaneousReport.GetInstanceValue("Reporter Address");
            if (address != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.3 Reporter’s Street"), address.Substring(0, 99));
            }
            var telNo = spontaneousReport.GetInstanceValue("Reporter Telephone Number");
            if (telNo != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.7 Reporter’s Telephone"), telNo);
            }

            // C3 Sender
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.3.3 Sender’s Given Name"), currentUser.FirstName);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.3.5 Sender’s Family Name"), currentUser.LastName);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.4.8 Sender’s E-mail Address"), currentUser.Email);

            // D Patient
            var dob = spontaneousReport.GetInstanceValue("Date of Birth");
            var onset = spontaneousReport.GetInstanceValue("Date of Onset");
            if (dob != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.1 Date of Birth"), dob);

                if (onset != string.Empty)
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.2a Age at Time of Onset of Reaction / Event"), age.ToString());
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.2bAge at Time of Onset of Reaction / Event (unit)"), "Day");
                }
            }
            var weight = spontaneousReport.GetInstanceValue("Weight (kg)");
            if (weight != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.3 Body Weight (kg)"), weight);
            }
            var sex = spontaneousReport.GetInstanceValue("Sex");
            if (sex != string.Empty)
            {
                if (sex == "Male")
                {
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.5 Sex"), "1=Male");
                }
                if (sex == "Female")
                {
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.5 Sex"), "2=Female");
                }
            }
            var death = spontaneousReport.GetInstanceValue("Reaction date of death");
            if (death != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.9.1 Date of Death"), death);
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.3.2a Results in Death"), "Yes");
            }

            // E Reaction
            var evnt = spontaneousReport.GetInstanceValue("Description of reaction");
            if (evnt != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.1.1a Reaction / Event as Reported by the Primary Source in Native Language"), evnt);
            }

            var term = spontaneousReport.GetInstanceValue("TerminologyMedDra");
            var termOut = "NOT SET";
            if (term != string.Empty)
            {
                var termid = Convert.ToInt32(term);
                termOut = _unitOfWork.Repository<TerminologyMedDra>().Queryable().Single(u => u.Id == termid).DisplayName;
            };
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.2.1b MedDRA Code for Reaction / Event"), termOut);

            if (onset != string.Empty)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.4 Date of Start of Reaction / Event"), onset);
            }

            var outcome = spontaneousReport.GetInstanceValue("Outcome of reaction");
            if (outcome != string.Empty)
            {
                switch (outcome)
                {
                    case "Died - Drug may be contributory":
                    case "Died - Due to adverse reaction":
                    case "Died - Unrelated to drug":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "5=fatal");
                        break;

                    case "Not yet recovered":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "3=not recovered/not resolved/ongoing");
                        break;

                    case "Recovered":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "1=recovered/resolved");
                        break;

                    case "Uncertain outcome":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "0=unknown");
                        break;

                    default:
                        break;
                }
            }

            for (int i = 1; i <= 6; i++)
            {
                var drugId = 0;
                var elementName = "";
                var drugName = "";
                var tempi = 0;

                if (i < 4)
                {
                    drugId = i;
                    elementName = string.Format("Suspected Drug {0}", drugId);
                    drugName = spontaneousReport.GetInstanceValue(elementName);

                    if (drugName != string.Empty)
                    {
                        // Create a new context
                        var context = Guid.NewGuid();

                        e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.1 Characterisation of Drug Role"), "1=Suspect", context);
                        e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.2.2 Medicinal Product Name as Reported by the Primary Source"), drugName, context);

                        elementName = string.Format("Suspected Drug {0} Dosage", drugId);
                        var dosage = spontaneousReport.GetInstanceValue(elementName);
                        if (dosage != string.Empty)
                        {
                            if (Int32.TryParse(dosage, out tempi))
                            {
                                e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1a Dose (number)"), dosage, context);
                            }
                        }
                        elementName = string.Format("Suspected Drug {0} Dosage Unit", drugId);
                        var dosageUnit = spontaneousReport.GetInstanceValue(elementName);
                        if (dosageUnit != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1b Dose (unit)"), dosageUnit, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Date Started", drugId);
                        var dateStarted = spontaneousReport.GetInstanceValue(elementName);
                        if (dateStarted != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.4 Date of Start of Drug"), dateStarted, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Date Stopped", drugId);
                        var dateStopped = spontaneousReport.GetInstanceValue(elementName);
                        if (dateStopped != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.5 Date of Last Administration"), dateStopped, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Batch Number", drugId);
                        var batch = spontaneousReport.GetInstanceValue(elementName);
                        if (batch != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.7 Batch / Lot Number"), batch, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Route", drugId);
                        var route = spontaneousReport.GetInstanceValue(elementName);
                        if (route != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.10.1 Route of Administration"), route, context);
                        }
                    }
                }
                else
                {
                    drugId = i - 3;
                    elementName = string.Format("Concomitant Drug {0}", drugId);
                    drugName = spontaneousReport.GetInstanceValue(elementName);

                    if (drugName != string.Empty)
                    {
                        // Create a new context
                        var context = Guid.NewGuid();

                        e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.1 Characterisation of Drug Role"), "1=Suspect", context);
                        e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.2.2 Medicinal Product Name as Reported by the Primary Source"), drugName, context);

                        elementName = string.Format("Concomitant Drug {0} Dosage", drugId);
                        var dosage = spontaneousReport.GetInstanceValue(elementName);
                        if (dosage != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1a Dose (number)"), dosage, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Dosage Unit", drugId);
                        var dosageUnit = spontaneousReport.GetInstanceValue(elementName);
                        if (dosageUnit != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1b Dose (unit)"), dosageUnit, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Date Started", drugId);
                        var dateStarted = spontaneousReport.GetInstanceValue(elementName);
                        if (dateStarted != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.4 Date of Start of Drug"), dateStarted, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Date Stopped", drugId);
                        var dateStopped = spontaneousReport.GetInstanceValue(elementName);
                        if (dateStopped != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.5 Date of Last Administration"), dateStopped, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Batch Number", drugId);
                        var batch = spontaneousReport.GetInstanceValue(elementName);
                        if (batch != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.7 Batch / Lot Number"), batch, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Route", drugId);
                        var route = spontaneousReport.GetInstanceValue(elementName);
                        if (route != string.Empty)
                        {
                            e2bInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.10.1 Route of Administration"), route, context);
                        }
                    }
                }
            }
        }

        private decimal ConvertValueToDecimal(string value)
        {
            return Decimal.TryParse(value, out decimal tempdec) ? tempdec : decimal.MinValue;
        }
    }
}
