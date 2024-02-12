using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using OpenRIMS.PV.Main.Core.Models;
using System.IO;
using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public class WordDocumentService : IWordDocumentService
    {
        ArtefactInfoModel _fileModel;
        private readonly IWebHostEnvironment _environment;

        public WordDocumentService(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public void CreateDocument(ArtefactInfoModel model)
        {
            _fileModel = model;

            using (var document = WordprocessingDocument.Create(Path.Combine(model.Path, model.FileName), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = document.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = new Body();

                SectionProperties sectionProps = new SectionProperties();
                PageMargin pageMargin = new PageMargin() { Top = 404, Right = (UInt32Value)504U, Bottom = 404, Left = (UInt32Value)504U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                sectionProps.Append(pageMargin);
                body.Append(sectionProps);

                mainPart.Document.AppendChild(body);
                //PrepareLogo(document);
            };
        }

        public void AddPageHeader(string header)
        {
            using (var document = WordprocessingDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                Paragraph paragraph = new Paragraph();
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                pprop.ParagraphStyleId = new ParagraphStyleId() { Val = "userheading" };
                paragraph.Append(pprop);

                RunProperties runProperties = new RunProperties();
                runProperties.AppendChild(new Bold());
                FontSize fs = new FontSize();
                fs.Val = "24";
                runProperties.AppendChild(fs);
                Run run = new Run();
                run.AppendChild(runProperties);
                run.AppendChild(new Text(header));
                paragraph.Append(run);

                body.Append(paragraph);

                document.Save();
            }
        }

        public void AddTableHeader(string header)
        {
            using (var document = WordprocessingDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                UInt32Value rowHeight = 20;

                Table mainTable = new Table();

                TableProperties tprops = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                        ),
                    new TableGrid(
                        new GridColumn() { Width = "11352" })
                    );

                mainTable.AppendChild<TableProperties>(tprops);

                var tr = new TableRow();
                TableRowProperties rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell(header, 11352, false, true));
                mainTable.AppendChild<TableRow>(tr);

                body.Append(mainTable);

                document.Save();
            }
        }

        public void AddFourColumnTable(List<KeyValuePair<string, string>> rows)
        {
            using (var document = WordprocessingDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                Table table = new Table();

                var headerWidth = 2500;
                var cellWidth = 3176;
                UInt32Value rowHeight = 12;

                TableProperties tprops = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                        ),
                    new TableGrid(
                        new GridColumn() { Width = headerWidth.ToString() },
                        new GridColumn() { Width = cellWidth.ToString() },
                        new GridColumn() { Width = headerWidth.ToString() },
                        new GridColumn() { Width = cellWidth.ToString() })
                        );

                table.AppendChild<TableProperties>(tprops);

                for (int i = 0; i < rows.Count; i += 2)
                {
                    var tr = new TableRow();
                    TableRowProperties rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareHeaderCell(rows[i].Key, headerWidth));
                    tr.Append(PrepareCell(rows[i].Value, cellWidth));
                    tr.Append(PrepareHeaderCell(rows[i + 1].Key, headerWidth));
                    tr.Append(PrepareCell(rows[i + 1].Value, cellWidth));

                    table.AppendChild<TableRow>(tr);
                }

                body.Append(table);

                document.Save();
            }
        }

        public void AddTwoColumnTable(List<KeyValuePair<string, string>> rows)
        {
            using (var document = WordprocessingDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                Table table = new Table();

                var headerWidth = 2500;
                var cellWidth = 8852;
                UInt32Value rowHeight = 24;

                TableProperties tprops = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                        ),
                    new TableGrid(
                        new GridColumn() { Width = headerWidth.ToString() },
                        new GridColumn() { Width = cellWidth.ToString() }
                        ));

                table.AppendChild<TableProperties>(tprops);

                for (int i = 0; i < rows.Count; i += 2)
                {
                    var tr = new TableRow();
                    TableRowProperties rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareHeaderCell(rows[i].Key, headerWidth));
                    tr.Append(PrepareCell(rows[i].Value, cellWidth));

                    table.AppendChild<TableRow>(tr);
                }

                body.Append(table);

                document.Save();
            }
        }

        public void AddOneColumnTable(List<string> rows)
        {
            using (var document = WordprocessingDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                UInt32Value rowHeight = 24;

                Table table = new Table();

                TableProperties tprops = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                        ),
                    new TableGrid(
                        new GridColumn() { Width = "11352" } )
                    );

                table.AppendChild<TableProperties>(tprops);

                for (int i = 0; i < rows.Count; i++)
                {
                    var tr = new TableRow();
                    TableRowProperties rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareCell(rows[i], 11352));

                    table.AppendChild<TableRow>(tr);
                }

                body.Append(table);

                document.Save();
            }
        }

        public void AddRowTable(List<string[]> rows, int[] cellWidths)
        {
            using (var document = WordprocessingDocument.Open(Path.Combine(_fileModel.Path, _fileModel.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                Table table = new Table();

                UInt32Value rowHeight = 12;

                List<GridColumn> columns = new List<GridColumn>();
                foreach (var cellWidth in cellWidths)
                {
                    columns.Add(new GridColumn() { Width = cellWidth.ToString() });
                }

                TableProperties tprops = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                        ),
                    new TableGrid(columns));

                table.AppendChild<TableProperties>(tprops);

                var i = 0;
                foreach (var row in rows)
                {
                    var tr = new TableRow();
                    TableRowProperties rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    if (i == 0)
                    {
                        var c = 0;
                        foreach (var value in row)
                        {
                            tr.Append(PrepareHeaderCell(value, cellWidths[c], true, false));
                            c++;
                        }
                    }
                    else 
                    {
                        var c = 0;
                        foreach (var value in row)
                        {
                            tr.Append(PrepareCell(value, cellWidths[c], false));
                            c++;
                        }
                    }

                    table.AppendChild<TableRow>(tr);
                    i++;
                }

                body.Append(table);

                document.Save();
            }
        }

        private void PrepareLogo(WordprocessingDocument document)
        {
            ImagePart imagePart = document.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);

            var fileName = Path.Combine(_environment.ContentRootPath, "StaticFiles", "SIAPS_USAID_Small.jpg");
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            AddImageToBody(document, document.MainDocumentPart.GetIdOfPart(imagePart));
        }

        private void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 1757548L, Cy = 253064L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 1757548L, Cy = 253064L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            Paragraph paragraph = new Paragraph();
            ParagraphProperties pprop = new ParagraphProperties();
            Justification centerHeading = new Justification() { Val = JustificationValues.Center };
            pprop.Append(centerHeading);
            pprop.ParagraphStyleId = new ParagraphStyleId() { Val = "bodyimage" };
            paragraph.Append(pprop);

            Run run = new Run();
            run.AppendChild(element);
            paragraph.Append(run);

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(paragraph);
        }

        private TableCell PrepareHeaderCell(string text, int width, bool centre = false, bool bold = false, int mergeCount = 0)
        {
            var tc = new TableCell();

            TableCellProperties props = new TableCellProperties(
                new TableCellWidth { Width = width.ToString() },
                new Shading { Color = "auto", ThemeFillShade = "D9", Fill = "D9D9D9" },
                new TableCellMargin(
                    new BottomMargin { Width = "30" },
                    new TopMargin { Width = "30" },
                    new LeftMargin { Width = "30" },
                    new RightMargin { Width = "30" }
                    ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                );
            if (mergeCount > 0) { props.Append(new GridSpan { Val = mergeCount }); };
            tc.AppendChild<TableCellProperties>(props);

            if (centre)
            {
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                tc.AppendChild<ParagraphProperties>(pprop);
            };

            RunProperties runProperties = new RunProperties();
            if (bold) { runProperties.AppendChild(new Bold()); };
            FontSize fs = new FontSize();
            fs.Val = "20";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(text));

            tc.Append(new Paragraph(run));

            return tc;
        }

        private TableCell PrepareCell(string text, int width, bool centre = true, int mergeCount = 0)
        {
            var tc = new TableCell();

            TableCellProperties props = new TableCellProperties(
                new TableCellWidth { Width = width.ToString() },
                new TableCellMargin(
                    new BottomMargin { Width = "30" },
                    new TopMargin { Width = "30" },
                    new LeftMargin { Width = "30" },
                    new RightMargin { Width = "30" }
                    ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                );
            if (mergeCount > 0) { props.Append(new GridSpan { Val = mergeCount }); };
            tc.AppendChild<TableCellProperties>(props);

            if (centre)
            {
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                tc.AppendChild<ParagraphProperties>(pprop);
            };

            RunProperties runProperties = new RunProperties();
            FontSize fs = new FontSize();
            fs.Val = "20";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(text));

            tc.Append(new Paragraph(run));

            return tc;
        }
    }
}