using DocumentFormat.OpenXml.Packaging;

using System.IO;
using System.Text;
using System;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System.Data.Common;
using Grpc.Core;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using SectionProperties = DocumentFormat.OpenXml.Wordprocessing.SectionProperties;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml;
using HtmlToOpenXml;

namespace Rsbc.Dmf.BcMailAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="header"></param>
        /// <param name="footer"></param>

        static public byte[] CreateDocument(string body, string header, string footer)
        {
            byte[] result;


            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    HtmlConverter converter = new HtmlConverter(mainPart);
                    converter.ParseHtml(body);
                    ApplyHeader(package, header);
                    ApplyFooter(package, footer);

                    package.ChangeDocumentType(WordprocessingDocumentType.Document);
                    package.Save();
                }

                return generatedDocument.ToArray();
            }
        }


        // This code was from https://github.com/bmalz/Service.HTML2DOCXConverter

        private static IList<OpenXmlCompositeElement> ConvertHtmlToOpenXml(string input)
        {
            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    HtmlConverter converter = new HtmlConverter(mainPart);
                    return converter.Parse(input);
                }
            }
        }

        private static void ApplyHeader(WordprocessingDocument doc, string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                MainDocumentPart mainDocPart = doc.MainDocumentPart;
                HeaderPart headerPart = mainDocPart.AddNewPart<HeaderPart>("r97");

                Header header = new Header();
                Paragraph paragraph = new Paragraph() { };
                Run run = new Run();
                run.Append(ConvertHtmlToOpenXml(input));
                paragraph.Append(run);
                header.Append(paragraph);
                headerPart.Header = header;

                SectionProperties sectionProperties = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (sectionProperties == null)
                {
                    sectionProperties = new SectionProperties() { };
                    mainDocPart.Document.Body.Append(sectionProperties);
                }

                HeaderReference headerReference = new HeaderReference() { Type = DocumentFormat.OpenXml.Wordprocessing.HeaderFooterValues.Default, Id = "r97" };
                sectionProperties.InsertAt(headerReference, 0);
            }
        }

        private static void ApplyFooter(WordprocessingDocument doc, string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                MainDocumentPart mainDocPart = doc.MainDocumentPart;
                FooterPart footerPart = mainDocPart.AddNewPart<FooterPart>("r98");

                Footer footer = new Footer();
                Paragraph paragraph = new Paragraph() { };
                Run run = new Run();
                run.Append(ConvertHtmlToOpenXml(input));
                paragraph.Append(run);
                footer.Append(paragraph);
                footerPart.Footer = footer;

                SectionProperties sectionProperties = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (sectionProperties == null)
                {
                    sectionProperties = new SectionProperties() { };
                    mainDocPart.Document.Body.Append(sectionProperties);
                }

                FooterReference footerReference = new FooterReference() { Type = DocumentFormat.OpenXml.Wordprocessing.HeaderFooterValues.Default, Id = "r98" };
                sectionProperties.InsertAt(footerReference, 0);
            }

        }
    }
}
