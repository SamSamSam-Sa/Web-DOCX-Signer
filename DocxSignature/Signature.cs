using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace DocxSignature
{
    public interface ISignature
    {
        void SignDocument(string documentPath, string firstSignerName, string secondSignerName = null);
        void SignDocument(Stream documentStream, string firstSignerName, string secondSignerName = null);
    }

    public class Signature: ISignature
    {
        public void SignDocument(string documentPath, string firstSignerName, string secondSignerName = null)
        {
            PasteFooterToDocument(documentPath, CreateSignFooter(firstSignerName, secondSignerName));
        }

        public void SignDocument(Stream documentStream, string firstSignerName, string secondSignerName = null)
        {
            PasteFooterToDocument(documentStream, CreateSignFooter(firstSignerName, secondSignerName));
        }

        private void PasteFooterToDocument(string documentPath, Footer footer)
        {
            using (var document = WordprocessingDocument.Open(documentPath, true))
            {
                PasteFooterToDocument(document, footer);
            }
        }
        private void PasteFooterToDocument(Stream documentStream, Footer footer)
        {
            using (var document = WordprocessingDocument.Open(documentStream, true))
            {
                PasteFooterToDocument(document, footer);
            }
        }
        private void PasteFooterToDocument(WordprocessingDocument document, Footer footer)
        {
            var mainDocumentPart = document.MainDocumentPart;
            //mainDocumentPart.DeleteParts(mainDocumentPart.FooterParts);

            var footerPart = mainDocumentPart.AddNewPart<FooterPart>();
            footerPart.Footer = footer;

            var sections = mainDocumentPart.Document.Body.Elements<SectionProperties>();

            foreach (var section in sections)
            {
                //section.RemoveAllChildren<FooterReference>();
                section.PrependChild(new FooterReference() { Id = mainDocumentPart.GetIdOfPart(footerPart) });
            }
        }

        #region XML Properties

        const string SIGN_TEXT = "Подписано";
        const string PRIMARY_COLOR = "5f487c";

        private UInt32Value BorderSize { get; } = UInt32Value.ToUInt32(25);
        private EnumValue<BorderValues> BorderVal { get; } = new EnumValue<BorderValues>(BorderValues.Thick);

        private TableProperties CreateTableProperties()
        {
            return new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val = BorderVal,
                        Size = BorderSize,
                        Color = PRIMARY_COLOR
                    },
                    new BottomBorder()
                    {
                        Val = BorderVal,
                        Size = BorderSize,
                        Color = PRIMARY_COLOR
                    },
                    new LeftBorder()
                    {
                        Val = BorderVal,
                        Size = BorderSize,
                        Color = PRIMARY_COLOR
                    },
                    new RightBorder()
                    {
                        Val = BorderVal,
                        Size = BorderSize,
                        Color = PRIMARY_COLOR
                    }
                ),
                new Justification()
                {
                    Val = JustificationValues.Center
                }
            );
        }
        private TableCellProperties CreateEmptyCellProperties()
        {
            return new TableCellProperties(
                new TableCellWidth()
                {
                    Type = TableWidthUnitValues.Dxa,
                    Width = "3000"
                },
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
                        Val = BorderVal,
                        Size = BorderSize,
                        Color = PRIMARY_COLOR
                    },
                    new RightBorder()
                    {
                        Val = BorderVal,
                        Size = BorderSize,
                        Color = PRIMARY_COLOR
                    })
            );
        }
        private TableCellProperties CreateFilledCellProperties()
        {
            return new TableCellProperties(
                    new TableCellWidth()
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = "4000"
                    });
        }

        #endregion

        #region Create XML elemnts

        private Footer CreateSignFooter(string firstSignerName, string secondSignerName = null)
        {
            var footer = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
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

            var signatureTable = string.IsNullOrEmpty(secondSignerName)
                ? CreateSignatures(firstSignerName)
                : CreateSignatures(firstSignerName, secondSignerName);

            footer.Append(signatureTable);

            return footer;
        }

        private Paragraph CreateCustomParagraph(string text, int size, bool isBold = false)
        {
            return new Paragraph(new ParagraphProperties
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines() { After = "80" }
            },
            new Run(new RunProperties
            {
                Bold = isBold ? new Bold() : null,
                RunFonts = new RunFonts() { HighAnsi = "Times New Roman" },
                FontSize = new FontSize() { Val = size.ToString() },
                Color = new Color() { Val = PRIMARY_COLOR }
            }, new Text(text)));
        }

        private TableCell CreateFilledSignCell(string signerName)
        {
            return new TableCell(CreateFilledCellProperties(),
                CreateCustomParagraph(SIGN_TEXT, 27, false),
                CreateCustomParagraph(signerName, 40, true));
        }

        private TableCell CreateEmptyCell() => new TableCell(CreateEmptyCellProperties(), 
            new Paragraph(new Run(new Text(string.Empty))));

        private Table CreateSignatures(string signerName) => new Table(CreateTableProperties(), 
            new TableRow(CreateFilledSignCell(signerName)));

        private Table CreateSignatures(string firstSignerName, string secondSignerName) => new Table(CreateTableProperties(),
            new TableRow(
                CreateFilledSignCell(firstSignerName),
                CreateEmptyCell(),
                CreateFilledSignCell(secondSignerName)));

        #endregion
    }
}
