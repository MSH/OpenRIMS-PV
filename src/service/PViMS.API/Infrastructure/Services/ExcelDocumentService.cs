using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public class ExcelDocumentService : IExcelDocumentService
    {
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;

        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }

        public ExcelDocumentService(IUnitOfWorkInt unitOfWork,
            ICustomAttributeService attributeService,
            IPatientService patientService,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
        }

        public ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"ActiveDataExtract_{generatedDate}.xlsx";

            //using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            //{
            //    // *************************************
            //    // Create sheet - Patient
            //    // *************************************
            //    var ws = pck.Workbook.Worksheets.Add("Patient");
            //    ws.View.ShowGridLines = true;

            //    var rowCount = 1;
            //    var colCount = 1;

            //    var patientquery = _unitOfWork.Repository<Patient>().Queryable().Where(p => p.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        patientquery = patientquery.Where(p => patientIds.Contains(p.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        patientquery = patientquery.Where(p => p.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }

            //    var patients = patientquery.OrderBy(p => p.Id).ToList();
            //    foreach (Patient patient in patients)
            //    {
            //        ProcessEntity(patient, ref ws, ref rowCount, ref colCount, "Patient");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - PatientMedication
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("PatientMedication");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var medicationquery = _unitOfWork.Repository<PatientMedication>().Queryable().Where(pm => pm.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        medicationquery = medicationquery.Where(pm => patientIds.Contains(pm.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        medicationquery = medicationquery.Where(pm => pm.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var medications = medicationquery.OrderBy(pm => pm.Id).ToList();
            //    foreach (PatientMedication medication in medications)
            //    {
            //        ProcessEntity(medication, ref ws, ref rowCount, ref colCount, "PatientMedication");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - PatientClinicalEvent
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("PatientClinicalEvent");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var eventquery = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Where(pc => pc.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        eventquery = eventquery.Where(pc => patientIds.Contains(pc.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        eventquery = eventquery.Where(pc => pc.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var events = eventquery.OrderBy(pc => pc.Id).ToList();
            //    foreach (PatientClinicalEvent clinicalEvent in events)
            //    {
            //        ProcessEntity(clinicalEvent, ref ws, ref rowCount, ref colCount, "PatientClinicalEvent");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - PatientCondition
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("PatientCondition");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var conditionquery = _unitOfWork.Repository<PatientCondition>().Queryable().Where(pc => pc.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        conditionquery = conditionquery.Where(pc => patientIds.Contains(pc.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        conditionquery = conditionquery.Where(pc => pc.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var conditions = conditionquery.OrderBy(pc => pc.Id).ToList();
            //    foreach (PatientCondition condition in conditions)
            //    {
            //        ProcessEntity(condition, ref ws, ref rowCount, ref colCount, "PatientCondition");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - PatientLabTest
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("PatientLabTest");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var labtestquery = _unitOfWork.Repository<PatientLabTest>().Queryable().Where(pl => pl.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        labtestquery = labtestquery.Where(pl => patientIds.Contains(pl.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        labtestquery = labtestquery.Where(pl => pl.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var labTests = labtestquery.OrderBy(pl => pl.Id).ToList();
            //    foreach (PatientLabTest labTest in labTests)
            //    {
            //        ProcessEntity(labTest, ref ws, ref rowCount, ref colCount, "PatientLabTest");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - Encounter
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("Encounter");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var encounterquery = _unitOfWork.Repository<Encounter>().Queryable().Where(e => e.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        encounterquery = encounterquery.Where(e => patientIds.Contains(e.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        encounterquery = encounterquery.Where(e => e.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var encounters = encounterquery.OrderBy(e => e.Id).ToList();
            //    foreach (Encounter encounter in encounters)
            //    {
            //        ProcessEntity(encounter, ref ws, ref rowCount, ref colCount, "Encounter");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - CohortGroupEnrolment
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("CohortGroupEnrolment");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var enrolmentquery = _unitOfWork.Repository<CohortGroupEnrolment>().Queryable().Where(e => e.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        enrolmentquery = enrolmentquery.Where(e => patientIds.Contains(e.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        enrolmentquery = enrolmentquery.Where(e => e.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var enrolments = enrolmentquery.OrderBy(e => e.Id).ToList();
            //    foreach (CohortGroupEnrolment enrolment in enrolments)
            //    {
            //        ProcessEntity(enrolment, ref ws, ref rowCount, ref colCount, "CohortGroupEnrolment");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - PatientFacility
            //    // *************************************
            //    ws = pck.Workbook.Worksheets.Add("PatientFacility");
            //    ws.View.ShowGridLines = true;

            //    rowCount = 1;
            //    colCount = 1;

            //    var facilityquery = _unitOfWork.Repository<PatientFacility>().Queryable().Where(pf => pf.Archived == false);
            //    if (patientIds.Length > 0)
            //    {
            //        facilityquery = facilityquery.Where(pf => patientIds.Contains(pf.Patient.Id));
            //    }
            //    if (cohortGroupId > 0)
            //    {
            //        facilityquery = facilityquery.Where(pf => pf.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
            //    }
            //    var facilities = facilityquery.OrderBy(pf => pf.Id).ToList();
            //    foreach (PatientFacility facility in facilities)
            //    {
            //        ProcessEntity(facility, ref ws, ref rowCount, ref colCount, "PatientFacility");
            //    }
            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    pck.Save();
            //}

            return model;
        }

        public ArtefactInfoModel CreateSpontaneousDatasetForDownload()
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"SpontaneousDataExtract_{generatedDate}.xlsx";

            //using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            //{
            //    // *************************************
            //    // Create sheet - Main Spontaneous
            //    // *************************************
            //    var ws = pck.Workbook.Worksheets.Add("Spontaneous");
            //    ws.View.ShowGridLines = true;

            //    var rowCount = 1;
            //    var colCount = 2;
            //    var maxColCount = 1;

            //    List<string> columns = new List<string>();

            //    // Header
            //    ws.Cells["A1"].Value = "Unique Identifier";

            //    var dataset = _unitOfWork.Repository<Dataset>().Queryable().Single(ds => ds.DatasetName == "Spontaneous Report");
            //    foreach (DatasetCategory category in dataset.DatasetCategories)
            //    {
            //        foreach (DatasetCategoryElement element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description != "Table"))
            //        {
            //            ws.Cells[GetExcelColumnName(colCount) + "1"].Value = element.DatasetElement.ElementName;
            //            columns.Add(element.DatasetElement.ElementName);
            //            colCount += 1;
            //        }
            //    }
            //    maxColCount = colCount - 1;

            //    //Set the header and format it
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + "1"])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
            //        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            //        r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
            //    }

            //    // Data
            //    foreach (ReportInstance reportInstance in _unitOfWork.Repository<ReportInstance>()
            //        .Queryable()
            //        .Where(ri => ri.WorkFlow.WorkFlowGuid.ToString() == "4096D0A3-45F7-4702-BDA1-76AEDE41B986" && ri.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED")))
            //    {
            //        DatasetInstance datasetInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);

            //        rowCount += 1;
            //        ws.Cells["A" + rowCount].Value = datasetInstance.DatasetInstanceGuid.ToString();

            //        foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description != "Table").OrderBy(div2 => div2.Id))
            //        {
            //            colCount = columns.IndexOf(value.DatasetElement.ElementName) + 2;
            //            ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = value.InstanceValue;
            //        };
            //    }

            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + rowCount])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    // *************************************
            //    // Create sheet - Sub tables
            //    // *************************************
            //    foreach (DatasetCategory category in dataset.DatasetCategories)
            //    {
            //        foreach (DatasetCategoryElement element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description == "Table"))
            //        {
            //            ws = pck.Workbook.Worksheets.Add(element.DatasetElement.ElementName);
            //            ws.View.ShowGridLines = true;

            //            // Write headers
            //            colCount = 2;
            //            rowCount = 1;

            //            ws.Cells["A1"].Value = "Unique Identifier";

            //            foreach (var subElement in element.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
            //            {
            //                ws.Cells[GetExcelColumnName(colCount) + "1"].Value = subElement.ElementName;
            //                colCount += 1;
            //            }
            //            maxColCount = colCount - 1;

            //            //Set the header and format it
            //            using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + "1"])
            //            {
            //                r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
            //                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
            //                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            //                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
            //            }

            //            // Data
            //            foreach (var value in _unitOfWork.Repository<DatasetInstanceValue>()
            //                .Queryable()
            //                .Where(div1 => div1.DatasetElement.Id == element.DatasetElement.Id && div1.DatasetInstanceSubValues.Count > 0 && div1.DatasetInstance.Status == Core.ValueTypes.DatasetInstanceStatus.COMPLETE).OrderBy(div2 => div2.Id))
            //            {
            //                // Get report and ensure it is not deleted
            //                ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == value.DatasetInstance.DatasetInstanceGuid);

            //                if (reportInstance != null)
            //                {
            //                    if (reportInstance.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED"))
            //                    {
            //                        // Get unique contexts
            //                        var contexts = value.DatasetInstance.GetInstanceSubValuesContext(value.DatasetElement.ElementName);
            //                        foreach (var context in contexts)
            //                        {
            //                            rowCount += 1;
            //                            ws.Cells["A" + rowCount].Value = value.DatasetInstance.DatasetInstanceGuid.ToString();

            //                            foreach (var subvalue in value.DatasetInstance.GetInstanceSubValues(value.DatasetElement.ElementName, context))
            //                            {
            //                                if (subvalue.DatasetElementSub.System == false)
            //                                {
            //                                    colCount = value.DatasetElement.DatasetElementSubs.ToList().IndexOf(subvalue.DatasetElementSub) + 2;
            //                                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = subvalue.InstanceValue;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            //format row
            //            using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + rowCount])
            //            {
            //                r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //                r.AutoFitColumns();
            //            }
            //        }
            //    }

            //    pck.Save();
            //}

            return model;
        }

        public async Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"InstanceDataExtract_{generatedDate}.xlsx";

            //using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            //{
            //    // Create XLS
            //    var ws = pck.Workbook.Worksheets.Add("Spontaneous ID " + datasetInstanceId);
            //    ws.View.ShowGridLines = true;

            //    // Write headers
            //    ws.Cells["A1"].Value = "Dataset Name";
            //    ws.Cells["B1"].Value = "Dataset Category";
            //    ws.Cells["C1"].Value = "Element Name";
            //    ws.Cells["D1"].Value = "Field Type";
            //    ws.Cells["E1"].Value = "Value";

            //    //Set the first header and format it
            //    using (var r = ws.Cells["A1:E1"])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
            //        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            //        r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
            //    }

            //    // Write content
            //    var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.Id == datasetInstanceId);
            //    if (datasetInstance == null)
            //    {
            //        throw new KeyNotFoundException(nameof(datasetInstance));
            //    }

            //    var count = 1;
            //    // Loop through and render main table
            //    foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description != "Table").OrderBy(div2 => div2.Id))
            //    {
            //        count += 1;
            //        ws.Cells["A" + count].Value = datasetInstance.Dataset.DatasetName;
            //        ws.Cells["B" + count].Value = value.DatasetElement.DatasetCategoryElements.Single(dce => dce.DatasetCategory.Dataset.Id == datasetInstance.Dataset.Id).DatasetCategory.DatasetCategoryName;
            //        ws.Cells["C" + count].Value = value.DatasetElement.ElementName;
            //        ws.Cells["D" + count].Value = value.DatasetElement.Field.FieldType.Description;
            //        ws.Cells["E" + count].Value = value.InstanceValue;
            //    };

            //    // Loop through and render sub tables
            //    var maxcount = 5;
            //    var subcount = 1;
            //    foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description == "Table").OrderBy(div2 => div2.Id))
            //    {
            //        count += 2;
            //        ws.Cells["A" + count].Value = datasetInstance.Dataset.DatasetName;
            //        ws.Cells["B" + count].Value = value.DatasetElement.DatasetCategoryElements.Single(dce => dce.DatasetCategory.Dataset.Id == datasetInstance.Dataset.Id).DatasetCategory.DatasetCategoryName;
            //        ws.Cells["C" + count].Value = value.DatasetElement.ElementName;
            //        ws.Cells["D" + count].Value = value.DatasetElement.Field.FieldType.Description;
            //        ws.Cells["E" + count].Value = string.Empty;

            //        if (value.DatasetInstanceSubValues.Count > 0)
            //        {
            //            // Write headers
            //            count += 1;
            //            foreach (var subElement in value.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
            //            {
            //                ws.Cells[GetExcelColumnName(subcount) + count].Value = subElement.ElementName;
            //                subcount++;
            //                maxcount = subcount > maxcount ? subcount : maxcount;
            //            }

            //            //Set the sub header and format it
            //            using (var r = ws.Cells["A" + count + ":" + GetExcelColumnName(subcount) + count])
            //            {
            //                r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
            //                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
            //                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            //                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
            //            }

            //            // Get unique contexts
            //            var contexts = datasetInstance.GetInstanceSubValuesContext(value.DatasetElement.ElementName);
            //            foreach (var context in contexts)
            //            {
            //                count += 1;
            //                subcount = 1;
            //                foreach (var subvalue in datasetInstance.GetInstanceSubValues(value.DatasetElement.ElementName, context))
            //                {
            //                    subcount = value.DatasetElement.DatasetElementSubs.ToList().IndexOf(subvalue.DatasetElementSub) + 1;
            //                    ws.Cells[GetExcelColumnName(subcount) + count].Value = subvalue.InstanceValue;
            //                }
            //            }
            //        }
            //    };

            //    //format row
            //    using (var r = ws.Cells["A1:" + GetExcelColumnName(maxcount) + count])
            //    {
            //        r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            //        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        r.AutoFitColumns();
            //    }

            //    pck.Save();
            //}

            return model;
        }

        #region "Excel Processing"

        //private void ProcessEntity(Object obj, ref ExcelWorksheet ws, ref int rowCount, ref int colCount, string entityName)
        //{
        //    DateTime tempdt;

        //    // Write headers
        //    Type type = obj.GetType();
        //    PropertyInfo[] properties = type.GetProperties();

        //    var attributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == entityName).OrderBy(c => c.Id);

        //    IQueryable elements = null;
        //    if (entityName == "Encounter")
        //    {
        //        elements = _unitOfWork.Repository<DatasetCategoryElement>()
        //            .Queryable()
        //            .Where(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment")
        //            .OrderBy(dce => dce.DatasetCategory.CategoryOrder)
        //            .ThenBy(dce => dce.FieldOrder);
        //    }

        //    if (rowCount == 1)
        //    {
        //        foreach (PropertyInfo property in properties)
        //        {
        //            if (property.Name != "CustomAttributesXmlSerialised")
        //            {
        //                if (property.PropertyType == typeof(DateTime?)
        //                    || property.PropertyType == typeof(DateTime)
        //                    || property.PropertyType == typeof(string)
        //                    || property.PropertyType == typeof(int)
        //                    || property.PropertyType == typeof(decimal)
        //                    || property.PropertyType == typeof(User)
        //                    || property.PropertyType == typeof(Concept)
        //                    || property.PropertyType == typeof(Product)
        //                    || property.PropertyType == typeof(Patient)
        //                    || property.PropertyType == typeof(Encounter)
        //                    || property.PropertyType == typeof(TerminologyMedDra)
        //                    || property.PropertyType == typeof(Outcome)
        //                    || property.PropertyType == typeof(LabTest)
        //                    || property.PropertyType == typeof(LabTestUnit)
        //                    || property.PropertyType == typeof(CohortGroup)
        //                    || property.PropertyType == typeof(Facility)
        //                    || property.PropertyType == typeof(Priority)
        //                    || property.PropertyType == typeof(EncounterType)
        //                    || property.PropertyType == typeof(TreatmentOutcome)
        //                    || property.PropertyType == typeof(Guid)
        //                    || property.PropertyType == typeof(bool)
        //                    )
        //                {
        //                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = property.Name;
        //                    colCount += 1;
        //                }
        //            }
        //        }

        //        // Now process attributes
        //        foreach (CustomAttributeConfiguration attribute in attributes)
        //        {
        //            ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = attribute.AttributeKey;
        //            colCount += 1;
        //        }

        //        // Process instance headers
        //        if (elements != null)
        //        {
        //            foreach (DatasetCategoryElement dce in elements)
        //            {
        //                ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = dce.DatasetElement.ElementName;
        //                colCount += 1;
        //            }
        //        }

        //        //Set the first header and format it
        //        using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount - 1) + "1"])
        //        {
        //            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
        //            r.Style.Font.Color.SetColor(System.Drawing.Color.White);
        //            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
        //        }
        //    }

        //    rowCount += 1;
        //    colCount = 1;

        //    var subOutput = "**IGNORE**";

        //    // Write values
        //    foreach (PropertyInfo property in properties)
        //    {
        //        if (property.Name != "CustomAttributesXmlSerialised")
        //        {
        //            subOutput = "**IGNORE**";
        //            if (property.PropertyType == typeof(DateTime?)
        //                || property.PropertyType == typeof(DateTime))
        //            {
        //                var dt = property.GetValue(obj, null) != null ? Convert.ToDateTime(property.GetValue(obj, null)).ToString("yyyy-MM-dd") : "";
        //                subOutput = dt;
        //            }
        //            if (property.PropertyType == typeof(string)
        //                || property.PropertyType == typeof(int)
        //                || property.PropertyType == typeof(decimal)
        //                || property.PropertyType == typeof(Guid)
        //                || property.PropertyType == typeof(bool)
        //                )
        //            {
        //                subOutput = property.GetValue(obj, null) != null ? property.GetValue(obj, null).ToString() : "";
        //            }
        //            if (property.PropertyType == typeof(User))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var user = (User)property.GetValue(obj, null);
        //                    subOutput = user.UserName;
        //                }
        //            }
        //            if (property.PropertyType == typeof(Patient))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var patient = (Patient)property.GetValue(obj, null);
        //                    subOutput = patient.PatientGuid.ToString();
        //                }
        //            }
        //            if (property.PropertyType == typeof(Concept))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var medication = (Concept)property.GetValue(obj, null);
        //                    subOutput = medication.ConceptName;
        //                }
        //            }
        //            if (property.PropertyType == typeof(Product))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var medication = (Product)property.GetValue(obj, null);
        //                    subOutput = medication.ProductName;
        //                }
        //            }
        //            if (property.PropertyType == typeof(Encounter))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var encounter = (Encounter)property.GetValue(obj, null);
        //                    subOutput = encounter.EncounterGuid.ToString();
        //                }
        //            }
        //            if (property.PropertyType == typeof(TerminologyMedDra))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var meddra = (TerminologyMedDra)property.GetValue(obj, null);
        //                    subOutput = meddra.DisplayName;
        //                }
        //            }
        //            if (property.PropertyType == typeof(Outcome))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var outcome = (Outcome)property.GetValue(obj, null);
        //                    subOutput = outcome.Description;
        //                }
        //            }
        //            if (property.PropertyType == typeof(TreatmentOutcome))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var txOutcome = (TreatmentOutcome)property.GetValue(obj, null);
        //                    subOutput = txOutcome.Description;
        //                }
        //            }
        //            if (property.PropertyType == typeof(LabTest))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var labTest = (LabTest)property.GetValue(obj, null);
        //                    subOutput = labTest.Description;
        //                }
        //            }
        //            if (property.PropertyType == typeof(LabTestUnit))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var unit = (LabTestUnit)property.GetValue(obj, null);
        //                    subOutput = unit.Description;
        //                }
        //            }
        //            if (property.PropertyType == typeof(CohortGroup))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var group = (CohortGroup)property.GetValue(obj, null);
        //                    subOutput = group.DisplayName;
        //                }
        //            }
        //            if (property.PropertyType == typeof(Facility))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var facility = (Facility)property.GetValue(obj, null);
        //                    subOutput = facility.FacilityName;
        //                }
        //            }
        //            if (property.PropertyType == typeof(Priority))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var priority = (Priority)property.GetValue(obj, null);
        //                    subOutput = priority.Description;
        //                }
        //            }
        //            if (property.PropertyType == typeof(EncounterType))
        //            {
        //                subOutput = "";
        //                if (property.GetValue(obj, null) != null)
        //                {
        //                    var et = (EncounterType)property.GetValue(obj, null);
        //                    subOutput = et.Description;
        //                }
        //            }
        //            if (subOutput != "**IGNORE**")
        //            {
        //                ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = subOutput;
        //                colCount += 1;
        //            }
        //        }
        //    }

        //    IExtendable extended = null;
        //    switch (entityName)
        //    {
        //        case "Patient":
        //            extended = (Patient)obj;
        //            break;

        //        case "PatientMedication":
        //            extended = (PatientMedication)obj;
        //            break;

        //        case "PatientClinicalEvent":
        //            extended = (PatientClinicalEvent)obj;
        //            break;

        //        case "PatientCondition":
        //            extended = (PatientCondition)obj;
        //            break;

        //        case "PatientLabTest":
        //            extended = (PatientLabTest)obj;
        //            break;

        //        default:
        //            break;
        //    }

        //    if (extended != null)
        //    {
        //        foreach (CustomAttributeConfiguration attribute in attributes)
        //        {
        //            var output = "";
        //            var val = extended.GetAttributeValue(attribute.AttributeKey);
        //            if (val != null)
        //            {
        //                if (attribute.CustomAttributeType == CustomAttributeType.Selection)
        //                {
        //                    var tempSDI = _unitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(u => u.AttributeKey == attribute.AttributeKey && u.SelectionKey == val.ToString());
        //                    if (tempSDI != null)
        //                        output = tempSDI.Value;
        //                }
        //                else if (attribute.CustomAttributeType == CustomAttributeType.DateTime)
        //                {
        //                    if (attribute != null && DateTime.TryParse(val.ToString(), out tempdt))
        //                    {
        //                        output = tempdt.ToString("yyyy-MM-dd");
        //                    }
        //                    else
        //                    {
        //                        output = val.ToString();
        //                    }
        //                }
        //                else
        //                {
        //                    output = val.ToString();
        //                }
        //            }

        //            ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = output;
        //            colCount += 1;
        //        }
        //    }

        //    if (elements != null)
        //    {
        //        var id = Convert.ToInt32(((Encounter)obj).Id);
        //        var instance = _unitOfWork.Repository<DatasetInstance>()
        //            .Queryable()
        //            .SingleOrDefault(di => di.Dataset.DatasetName == "Chronic Treatment" && di.ContextId == id);
        //        foreach (DatasetCategoryElement dce in elements)
        //        {
        //            var eleOutput = "";
        //            if (instance != null)
        //            {
        //                eleOutput = instance.GetInstanceValue(dce.DatasetElement.ElementName);
        //            }

        //            ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = eleOutput;
        //            colCount += 1;
        //        }
        //    }
        //}

        //private string GetExcelColumnName(int columnNumber)
        //{
        //    int dividend = columnNumber;
        //    string columnName = String.Empty;
        //    int modulo;

        //    while (dividend > 0)
        //    {
        //        modulo = (dividend - 1) % 26;
        //        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        //        dividend = (int)((dividend - modulo) / 26);
        //    }

        //    return columnName;
        //}

        #endregion
    }
}
