using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;


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
            
            MemoryStream imageStream = new MemoryStream(tiffBytes);

            PdfDocument pdfDocument = new PdfDocument();

            int pageCount = getPageCount(imageStream);

            for (int i = 0; i < pageCount; i++)
            {
                PdfPage page = new PdfPage();
                Image img = getTiffImage(imageStream, i);
                XImage imgFrame = XImage.FromGdiPlusImage(img);

                page.Width = imgFrame.PointWidth;
                page.Height = imgFrame.PointHeight;
                pdfDocument.Pages.Add(page);

                var newPage = pdfDocument.Pages[i];

                XGraphics xgr = XGraphics.FromPdfPage(newPage);

                xgr.DrawImage(img, 0, 0);
            }

            // Convert doc to stream or bytes 
            var pdfMemoryStream = new MemoryStream();
            pdfDocument.Save(pdfMemoryStream);

            return pdfMemoryStream.ToArray();

        }

        /// <summary>
        /// Get Tiff Image
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static Image getTiffImage(Stream imageStream, int pageNumber)
        {

            MemoryStream ms = new MemoryStream();
            Image sourceImage = Image.FromStream(imageStream, true, true);

            Guid objGuid = sourceImage.FrameDimensionsList[0];

            FrameDimension objDimension = new FrameDimension(objGuid);

            sourceImage.SelectActiveFrame(objDimension, pageNumber);

            sourceImage.Save(ms, ImageFormat.Tiff);

            return Image.FromStream(ms);

        }

        /// <summary>
        /// Get Page Count
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        public static int getPageCount(Stream imageStream)
        {
            int pageCount = -1;
            try
            {
                Image img = Image.FromStream(imageStream, true, true);
                pageCount = img.GetFrameCount(FrameDimension.Page);
                img.Dispose();

            }
            catch (Exception ex)
            {
                pageCount = 0;
            }

            return pageCount;
        }
    }

}

   
