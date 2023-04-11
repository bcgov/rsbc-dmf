using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using TiffLibrary;
using TiffLibrary.ImageSharpAdapter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
/*
        public static byte[] ConvertTiffToPdf(byte[] tiffBytes)
        {
            using (var tiffStream = new MemoryStream(tiffBytes))
            using (var pdfStream = new MemoryStream())
            using (var tiff = TiffFile.Open(tiffStream))
            using (var pdf = new PdfEncoder(pdfStream))
            {
                // Set the DPI of the PDF file to match the TIFF file
                var tiffImageFileDirectory = tiff.ImageFileDirectories[0];
                var dpiX = tiffImageFileDirectory.XResolution?.ToDouble() ?? 96;
                var dpiY = tiffImageFileDirectory.YResolution?.ToDouble() ?? 96;
                pdf.SetDpi((int)dpiX, (int)dpiY);

                // Iterate over each page in the TIFF file and add it to the PDF file
                foreach (var entry in tiffImageFileDirectory.Entries)
                {
                    if (entry.Tag == TiffTag.Compression || entry.Tag == TiffTag.StripOffsets || entry.Tag == TiffTag.StripByteCounts)
                    {
                        // Skip tags related to image compression and data organization
                        continue;
                    }

                    // Load the page image from the TIFF file
                    using (var decompressed = TiffFieldDataReader.Create(entry).Decode())
                    using (var image = decompressed.AsImageSharp<Rgba32>())
                    {
                        // Add the page image to the PDF file
                        pdf.AddImage(image);
                    }
                }

                // Return the PDF file bytes
                return pdfStream.ToArray();
            }
        }*/


    }

}

   
