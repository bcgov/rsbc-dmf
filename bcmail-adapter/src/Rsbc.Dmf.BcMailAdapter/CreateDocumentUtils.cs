using DocumentFormat.OpenXml.Packaging;
using MariGold.OpenXHTML;
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

namespace Rsbc.Dmf.BcMailAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDocumentUtils
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


            using (MemoryStream mem = new MemoryStream())
            {
                WordDocument doc = new WordDocument(mem);

               
                // decode body
                byte[] bodyData = Convert.FromBase64String(body);
                string decodedbody = Encoding.UTF8.GetString(bodyData);

                // Decode Header
                byte[] headerData = Convert.FromBase64String(header);
                string decodedHeader = Encoding.UTF8.GetString(headerData);

                // Decode Footer
                byte[] footerData = Convert.FromBase64String(footer);
                string decodedFooter = Encoding.UTF8.GetString(footerData);

                /* var headerElement = new Header(
                    new Paragraph(new Run(header)));*/

                var parseBody = new HtmlParser(decodedbody);

                doc.Process(parseBody);
                /*  doc.Process(new HtmlParser(decodedFooter));
                  doc.Process(new HtmlParser(decodedHeader));
  */
                // Approach 1: Working adding footer with plain text
                ApplyFooter(doc);

                // Apporach 2: Add Headers parts using  MemoryStream and OpenXML AddAlternativeFormatImportPart
                ApplyHeader2(doc.WordprocessingDocument);

                doc.Save();
                result = mem.ToArray();
            }

            return result;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="decodedHeader"></param>
        public static void ApplyHeader1(WordDocument doc)
        {
            // Get the main document part.
            MainDocumentPart mainDocPart = doc.MainDocumentPart;

            HeaderPart headerPart1 = mainDocPart.AddNewPart<HeaderPart>("r97");

            Header header1 = new Header();

            //create a memory stream with the HTML required
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("<html><p>test<br/>test2<ul><li>item1</li><li>item2<li2></p><html>"));

            //Create an alternative format import part on the MainDocumentPart
            AlternativeFormatImportPart altformatImportPart = headerPart1.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html);

            //Add the HTML data into the alternative format import part
            altformatImportPart.FeedData(ms);

            //create a new altChunk and link it to the id of the AlternativeFormatImportPart
            AltChunk altChunk = new AltChunk();
            altChunk.Id = headerPart1.GetIdOfPart(altformatImportPart);

            Paragraph paragraph = new Paragraph();

            paragraph.Append(altChunk);

            header1.Append(paragraph);

            headerPart1.Header = header1;

            SectionProperties sectionProperties1 = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
            if (sectionProperties1 == null)
            {
                sectionProperties1 = new SectionProperties() { };
                mainDocPart.Document.Body.Append(sectionProperties1);
            }
            HeaderReference headerReference1 = new HeaderReference() { Type = HeaderFooterValues.Default, Id = "r97" };


            sectionProperties1.InsertAt(headerReference1, 0);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        public static void ApplyHeader2(WordprocessingDocument doc)
        {
            // Get the main document part.
            MainDocumentPart mainDocPart = doc.MainDocumentPart;

            HeaderPart headerPart1 = mainDocPart.AddNewPart<HeaderPart>("r97");

            Header header1 = new Header();

            string html =
            @"<p>Continuation Sheet</p>";

            string altChunkId = "AltChunkId2";

            AlternativeFormatImportPart chunk = mainDocPart.AddAlternativeFormatImportPart("application/xhtml+xml", altChunkId);
            using (Stream chunkStream = chunk.GetStream())
            {
                using (StreamWriter stringWriter = new StreamWriter(chunkStream, Encoding.UTF8)) //Encoding.UTF8 is important to remove special characters
                {
                    stringWriter.Write(html);
                }
            }
            AltChunk altChunk = new AltChunk();
            altChunk.Id = altChunkId;

            // first header
            Paragraph paragraph1 = new Paragraph() { };

            paragraph1.InsertAt(altChunk, 0);
            header1.Append(paragraph1);
            headerPart1.Header = header1;


            SectionProperties sectionProperties1 = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
            if (sectionProperties1 == null)
            {
                sectionProperties1 = new SectionProperties() { };
                mainDocPart.Document.Body.Append(sectionProperties1);
            }
            HeaderReference headerReference1 = new HeaderReference() { Type = HeaderFooterValues.Default, Id = "r97" };

            sectionProperties1.InsertAt(headerReference1, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        public static void ApplyFooter(WordDocument doc)
        {
            // Get the main document part.
            MainDocumentPart mainDocPart = doc.MainDocumentPart;

            FooterPart footerPart1 = mainDocPart.AddNewPart<FooterPart>("r98");

            Footer footer1 = new Footer();

            Paragraph paragraph1 = new Paragraph() { };

            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = "<p>Footer stuff<p>";

            run1.Append(text1);

            paragraph1.Append(run1);


            footer1.Append(paragraph1);

            footerPart1.Footer = footer1;


            SectionProperties sectionProperties1 = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
            if (sectionProperties1 == null)
            {
                sectionProperties1 = new SectionProperties() { };
                mainDocPart.Document.Body.Append(sectionProperties1);
            }
            FooterReference footerReference1 = new FooterReference() { Type = DocumentFormat.OpenXml.Wordprocessing.HeaderFooterValues.Default, Id = "r98" };


            sectionProperties1.InsertAt(footerReference1, 0);

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepathFrom"></param>
        /// <param name="filepathTo"></param>
        public static void AddHeaderFromTo(string filepathFrom, string filepathTo)
        {
            // Replace header in target document with header of source document.
            using (WordprocessingDocument
                wdDoc = WordprocessingDocument.Open(filepathTo, true))
            {
                MainDocumentPart mainPart = wdDoc.MainDocumentPart;

                // Delete the existing header part.
                mainPart.DeleteParts(mainPart.HeaderParts);

                // Create a new header part.
                DocumentFormat.OpenXml.Packaging.HeaderPart headerPart =
            mainPart.AddNewPart<HeaderPart>();

                // Get Id of the headerPart.
                string rId = mainPart.GetIdOfPart(headerPart);

                // Feed target headerPart with source headerPart.
                using (WordprocessingDocument wdDocSource =
                    WordprocessingDocument.Open(filepathFrom, true))
                {
                    DocumentFormat.OpenXml.Packaging.HeaderPart firstHeader =
            wdDocSource.MainDocumentPart.HeaderParts.FirstOrDefault();

                    wdDocSource.MainDocumentPart.HeaderParts.FirstOrDefault();

                    if (firstHeader != null)
                    {
                        headerPart.FeedData(firstHeader.GetStream());
                    }
                }

                // Get SectionProperties and Replace HeaderReference with new Id.
                IEnumerable<DocumentFormat.OpenXml.Wordprocessing.SectionProperties> sectPrs =
            mainPart.Document.Body.Elements<SectionProperties>();
                foreach (var sectPr in sectPrs)
                {
                    // Delete existing references to headers.
                    sectPr.RemoveAllChildren<HeaderReference>();

                    // Create the new header reference node.
                    sectPr.PrependChild<HeaderReference>(new HeaderReference() { Id = rId });
                }
            }
        }
    }
}
