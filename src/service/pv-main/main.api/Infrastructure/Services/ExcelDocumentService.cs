using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public class ExcelDocumentService : IExcelDocumentService
    {
        private ArtifactDto _fileModel;

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

        public void CreateDocument(ArtifactDto model)
        {
            _fileModel = model;

            using (var document = SpreadsheetDocument.Create(Path.Combine(model.Path, model.FileName), SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document 
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                document.WorkbookPart.Workbook.Sheets = new Sheets();

                var stylePart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                // add styles to sheet
                stylePart.Stylesheet = CreateStylesheet();
                stylePart.Stylesheet.Save();

                document.Close();
            };
        }

        public void AddSheet(string sheetName, List<List<string>> data)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                throw new ArgumentException($"'{nameof(sheetName)}' cannot be null or whitespace.", nameof(sheetName));
            }

            using (var document = SpreadsheetDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                WorksheetPart newWorksheetPart = PrepareBlankWorksheetPart(document);
                PrepareNewSheet(document, newWorksheetPart, sheetName);
                SheetData sheetData = PrepareSheetData(data);
                Columns columns = AutoSize(sheetData);

                newWorksheetPart.Worksheet.Append(columns);
                newWorksheetPart.Worksheet.Append(sheetData);

                ProtectSheetPart(newWorksheetPart);

                document.WorkbookPart.Workbook.Save();
                document.Close();
            }
        }

        private void ProtectSheetPart(WorksheetPart newWorksheetPart)
        {
            var hexConvertedPassword = HexPasswordConversion("rcAHtHx&pN6cE3kL");
            SheetProtection sheetProtection = new SheetProtection() { Sheet = true, Objects = true, Scenarios = true, Password = hexConvertedPassword };
            newWorksheetPart.Worksheet.InsertAfter(sheetProtection, newWorksheetPart.Worksheet.Descendants<SheetData>().LastOrDefault());
            newWorksheetPart.Worksheet.Save();
        }

        private SheetData PrepareSheetData(List<List<string>> data)
        {
            UInt32Value i = 1;

            var sheetData = new SheetData();

            foreach (var dataRow in data)
            {
                var xlRow = new Row() { RowIndex = i };

                foreach (var dataValue in dataRow)
                {
                    Cell xlCell = new Cell() { DataType = CellValues.String, CellValue = new CellValue(dataValue), StyleIndex = i == 1 ? (UInt32Value)1U : (UInt32Value)2U };
                    xlRow.AppendChild(xlCell);
                }

                sheetData.AppendChild(xlRow);
                i++;
            }

            return sheetData;
        }

        private void PrepareNewSheet(SpreadsheetDocument document, WorksheetPart newWorksheetPart, string sheetName)
        {
            Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = document.WorkbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName.Length > 28 ? $"{sheetName.Substring(0, 27)}..." : sheetName };
            sheets.Append(sheet);
        }

        private WorksheetPart PrepareBlankWorksheetPart(SpreadsheetDocument document)
        {
            WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet();
            return newWorksheetPart;
        }

        private Columns AutoSize(SheetData sheetData)
        {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            Columns columns = new Columns();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                columns.Append(col);
            }

            return columns;
        }

        private Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
        {
            //iterate over all cells getting a max char value for each column
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            UInt32[] numberStyles = new UInt32[] { 5, 6, 7, 8 }; //styles that will add extra chars
            UInt32[] boldStyles = new UInt32[] { 1, 2, 3, 4, 6, 7, 8 }; //styles that will bold
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();

                //using cell index as my column
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    var cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    var cellTextLength = cellValue.Length;

                    if (cell.StyleIndex != null && numberStyles.Contains(cell.StyleIndex))
                    {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);

                        //add 3 for '.00' 
                        cellTextLength += (3 + thousandCount);
                    }

                    if (cell.StyleIndex != null && boldStyles.Contains(cell.StyleIndex))
                    {
                        //add an extra char for bold - not 100% acurate but good enough for what i need.
                        cellTextLength += 1;
                    }

                    if (maxColWidth.ContainsKey(i))
                    {
                        var current = maxColWidth[i];
                        if (cellTextLength > current)
                        {
                            maxColWidth[i] = cellTextLength;
                        }
                    }
                    else
                    {
                        maxColWidth.Add(i, cellTextLength);
                    }
                }
            }

            return maxColWidth;
        }

        private Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 10D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            fonts1.Append(font1);

            stylesheet1.Fonts = fonts1;

            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);

            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };
            fill2.Append(patternFill2);

            // FillId = 2, Header
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "E8E8E8" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3, Normal row
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "FFFFFF" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4,YELLO
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);

            stylesheet1.Fills = fills1;

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder() { Style = BorderStyleValues.Thick };
            RightBorder rightBorder1 = new RightBorder() { Style = BorderStyleValues.Thick };
            TopBorder topBorder1 = new TopBorder() { Style = BorderStyleValues.Thick };
            BottomBorder bottomBorder1 = new BottomBorder() { Style = BorderStyleValues.Thick };
            DiagonalBorder diagonalBorder1 = new DiagonalBorder() { Style = BorderStyleValues.Thick };

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            stylesheet1.Borders = borders1;

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            stylesheet1.CellStyleFormats = cellStyleFormats1;

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);

            stylesheet1.CellFormats = cellFormats1;

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            stylesheet1.CellStyles = cellStyles1;

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };

            stylesheet1.DifferentialFormats = differentialFormats1;
            stylesheet1.TableStyles = tableStyles1;

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();
            stylesheet1.StylesheetExtensionList = stylesheetExtensionList1;

            return stylesheet1;
        }

        private string HexPasswordConversion(string password)
        {
            byte[] passwordCharacters = System.Text.Encoding.ASCII.GetBytes(password);
            int hash = 0;
            if (passwordCharacters.Length > 0)
            {
                int charIndex = passwordCharacters.Length;

                while (charIndex-- > 0)
                {
                    hash = ((hash >> 14) & 0x01) | ((hash << 1) & 0x7fff);
                    hash ^= passwordCharacters[charIndex];
                }
                // Main difference from spec, also hash with charcount
                hash = ((hash >> 14) & 0x01) | ((hash << 1) & 0x7fff);
                hash ^= passwordCharacters.Length;
                hash ^= (0x8000 | ('N' << 8) | 'K');
            }

            return Convert.ToString(hash, 16).ToUpperInvariant();
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
    }
}
