using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;


namespace Pssg.DocumentStorageAdapter
{
    public class DocumentConvertUtil
    {
        /// <summary>
        /// convert Tiff2Pdf
        /// </summary>
        /// <param name="tiffBytes"></param>
        /// <returns></returns>
        public static byte[] convertTiff2Pdf(byte[] tiffBytes)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Create Image Stream
            MemoryStream imageStream = new MemoryStream(tiffBytes);
            imageStream.Position = 0;

            PdfDocument pdfDocument = new PdfDocument();
            PdfPage page = new PdfPage();  
            XImage imgFrame = XImage.FromStream(() => imageStream);

            page.Width = imgFrame.PointWidth;
            page.Height = imgFrame.PointHeight;
            pdfDocument.Pages.Add(page);
            var newPage = pdfDocument.Pages[0];

            XGraphics xgr = XGraphics.FromPdfPage(newPage);

                xgr.DrawImage(imgFrame, 0, 0);
            
            // Convert doc to stream or bytes 
            var pdfMemoryStream = new MemoryStream();
           // pdfDocument.Save("../../../TestFiles/test_file_2.pdf");
           // pdfDocument.Close();
           pdfDocument.Save(pdfMemoryStream);
           return pdfMemoryStream.ToArray();

        }

    }

}

   
