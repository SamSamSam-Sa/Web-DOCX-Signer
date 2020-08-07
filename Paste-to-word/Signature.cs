using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Paste_to_word
{
    class Signature
    {
        public static void PasteToFooter(String documentPath)
        {
            // Replace header in target document with header of source document.
            using (WordprocessingDocument document = WordprocessingDocument.Open(documentPath, true))
            {
                // Get the main document part
                MainDocumentPart mainDocumentPart = document.MainDocumentPart;

                // Delete the existing footer parts
                 mainDocumentPart.DeleteParts(mainDocumentPart.FooterParts);

                // Create a newfooter part
                FooterPart footerPart = mainDocumentPart.AddNewPart<FooterPart>();

                // Get Id of the footer parts
                string footerPartId = mainDocumentPart.GetIdOfPart(footerPart);

                CreateFooterPartContent(footerPart);

                // Get SectionProperties and ReplaceFooterRefernce with new Id
                IEnumerable<SectionProperties> sections = mainDocumentPart.Document.Body.Elements<SectionProperties>();

                foreach (var section in sections)
                {
                    // Delete existing references to footers
                    section.RemoveAllChildren<FooterReference>();

                    // Create the new footer reference node
                    section.PrependChild<FooterReference>(new FooterReference() { Id = footerPartId });
                }
            }
        }

        private static void CreateFooterPartContent(FooterPart part)
        {
            Footer footer = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            footer.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            //Table signatures = CreateSignatures("SamohinaAI");
            Table signatures = CreateSignatures("SamohinaAI", "BessonovVA");

            footer.Append(signatures);

            part.Footer = footer;
        }

        const string PRIMARY_COLOR = "5f487c";
        private static UInt32Value GetBorderSize () => UInt32Value.ToUInt32(25);
        private static EnumValue<BorderValues> GetBorderVal  () => new EnumValue<BorderValues>(BorderValues.Thick);


        public static TableProperties GetTableProperties()
        {            
            return new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val =
                        GetBorderVal(),
                        Size = GetBorderSize(),
                        Color = PRIMARY_COLOR
                    },
                    new BottomBorder()
                    {
                        Val =
                        GetBorderVal(),
                        Size = GetBorderSize(),
                        Color = PRIMARY_COLOR
                    },
                    new LeftBorder()
                    {
                        Val =
                        GetBorderVal(),
                        Size = GetBorderSize(),
                        Color = PRIMARY_COLOR
                    },
                    new RightBorder()
                    {
                        Val =
                        GetBorderVal(),
                        Size = GetBorderSize(),
                        Color = PRIMARY_COLOR
                    }
                ),
                new Justification()
                {
                    Val = JustificationValues.Center
                }
            );
        }
        private static TableCellProperties GetEmptyCellProperties()
        {
            return new TableCellProperties(
                new TableCellWidth()
                {
                    Type = TableWidthUnitValues.Dxa,
                    Width = "3000"
                },
                //new FitText(),
                new TableCellBorders(
                    new TopBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Nil),
                        Size = 0
                    },
                    new BottomBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Nil),
                        Size = 0
                    },
                    new LeftBorder()
                    {
                        Val = GetBorderVal(),
                        Size = GetBorderSize(),
                        Color = PRIMARY_COLOR
                    },
                    new RightBorder()
                    {
                        Val = GetBorderVal(),
                        Size = GetBorderSize(),
                        Color = PRIMARY_COLOR
                    })
            );
        }

        private static TableCellProperties GetFilledCellProperties()
        {
            return new TableCellProperties(
                    new TableCellWidth()
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = "4000"
                    });
        }

        private static Paragraph GetCustomParagraph(string text, int size, bool isBold = false)
        {
            Paragraph paragraph1 = new Paragraph();
            ParagraphProperties pPr = new ParagraphProperties
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines() { After = "80" }
            };
            paragraph1.Append(pPr);
            Run run1 = new Run();
            RunProperties rPr1 = new RunProperties
            {
                Bold = isBold ? new Bold() : null,
                RunFonts = new RunFonts() { HighAnsi = "Times New Roman" },
                FontSize = new FontSize() { Val = size.ToString() },
                Color = new Color() { Val = PRIMARY_COLOR }
            };

            run1.Append(rPr1);

            run1.Append(new Text(text));

            paragraph1.Append(run1);
            return paragraph1;
        }

        // Insert a table into a word processing document.
        public static Table CreateSignatures(string login)
        {
            // Create an empty table.
            Table signature = new Table();

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = GetTableProperties();

            // Append the TableProperties object to the empty table.
            signature.AppendChild<TableProperties>(tblProp);

            // Create a row.
            TableRow tr = new TableRow();

            // Create a cell.
            TableCell tc = new TableCell();

            // Specify the width property of the table cell.
            tc.Append(GetFilledCellProperties());

            Paragraph paragraph1 = GetCustomParagraph("Подписано", 27, false);
            Paragraph paragraph2 = GetCustomParagraph(login, 40, true);

            // specify the table cell content.
            tc.Append(paragraph1);
            tc.Append(paragraph2);

            // Append the table cell to the table row.
            tr.Append(tc);

            // Append the table row to the table.
            signature.Append(tr);

            return signature;
        }

        

        public static Table CreateSignatures(string login1, string login2)
        {
            // Create an empty table.
            Table signatures = new Table();

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = GetTableProperties();

            // Append the TableProperties object to the empty table.
            signatures.AppendChild<TableProperties>(tblProp);

            // Create a row.
            TableRow tr = new TableRow();

            // Create cells.
            TableCell tc1 = new TableCell();
            TableCell tc2 = new TableCell();
            TableCell tc3 = new TableCell();

            // Specify properties of the table cell.
            //TableCellProperties tcPr = GetCellProperties();
            tc1.Append(GetFilledCellProperties());
            tc2.Append(GetEmptyCellProperties());
            tc3.Append(GetFilledCellProperties());

            Paragraph paragraph1 = GetCustomParagraph("Подписано", 27, false);
            Paragraph paragraph2 = GetCustomParagraph(login1, 40, true);
            Paragraph paragraph3 = GetCustomParagraph(login2, 40, true);

            // specify the table cell content.
            tc1.Append(paragraph1);
            tc1.Append(paragraph2);
            tc2.Append(new Paragraph(new Run(new Text(""))));
            tc3.Append((Paragraph)paragraph1.CloneNode(true));
            tc3.Append(paragraph3);

            // Append the table cell to the table row.
            tr.Append(tc1);
            tr.Append(tc2);
            tr.Append(tc3);

            // Append the table row to the table.
            signatures.Append(tr);

            return signatures;
        }

    }
}
