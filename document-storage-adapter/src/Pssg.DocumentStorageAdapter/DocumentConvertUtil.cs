using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

using System.Runtime.InteropServices;
using TiffLibrary.ImageEncoder;
using TiffLibrary;
using PdfSharpCore.Pdf.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using TiffLibrary.PixelFormats;
using TiffLibrary.Compression;
using Org.BouncyCastle.Utilities.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

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
           // bool pdfError = false;
            // Create Image Stream
            MemoryStream imageStream = new MemoryStream(tiffBytes);
           
            imageStream.Position = 0;
            

            PdfDocument pdfDocument = new PdfDocument();

            pdfDocument.Options.CompressContentStreams = true;
            pdfDocument.Options.NoCompression = false;
            pdfDocument.Options.FlateEncodeMode = PdfFlateEncodeMode.BestCompression;
            pdfDocument.Options.UseFlateDecoderForJpegImages = PdfUseFlateDecoderForJpegImages.Automatic;
            pdfDocument.Options.EnableCcittCompressionForBilevelImages = true;


            //PdfPage page = new PdfPage();


            var tiff = TiffFileReader.Open(imageStream);
            var fieldReader = tiff.CreateFieldReader();


            TiffStreamOffset ifdOffset = tiff.FirstImageFileDirectoryOffset;
            while (!ifdOffset.IsZero)
            {
                TiffImageFileDirectory ifd = tiff.ReadImageFileDirectory(ifdOffset);

                TiffImageDecoderOptions decoderOptions = new TiffImageDecoderOptions();
                decoderOptions.UndoColorPreMultiplying = false;


                TiffImageDecoder decoder = tiff.CreateImageDecoder(ifd, decoderOptions);
                

                TiffRgb24[] pixels = new TiffRgb24[decoder.Width * decoder.Height];

                try
                {


                    decoder.Decode(TiffPixelBuffer.Wrap(pixels, decoder.Width, decoder.Height));

                    // write it to a memory stream.
                    var sourceBuffer = TiffPixelBuffer.WrapReadOnly(pixels, decoder.Width, decoder.Height);

                    byte[] encodedFile;
                    {
                        var builder = new TiffImageEncoderBuilder();
                        builder.PhotometricInterpretation = TiffPhotometricInterpretation.RGB;
                        builder.Compression = TiffCompression.Jpeg;
                        builder.DeflateCompressionLevel = TiffDeflateCompressionLevel.Optimal;
                        builder.RowsPerStrip = decoder.Height;
                        //builder.JpegOptions = new TiffJpegEncodingOptions { Quality = JpegQuality, UseSharedQuantizationTables = UseSharedQuantizationTables, OptimizeCoding = true };

                        TiffImageEncoder<TiffRgb24> encoder = builder.Build<TiffRgb24>();

                        using var ms = new MemoryStream();
                        using TiffFileWriter writer = TiffFileWriter.OpenAsync(ms, true).GetAwaiter().GetResult();
                        TiffImageFileDirectoryWriter ifdWriter = writer.CreateImageFileDirectory();
                        encoder.EncodeAsync(ifdWriter, sourceBuffer).GetAwaiter().GetResult();
                        writer.SetFirstImageFileDirectoryOffset(ifdWriter.FlushAsync().GetAwaiter().GetResult());
                        writer.FlushAsync().GetAwaiter().GetResult();

                        encodedFile = ms.ToArray();

                        System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();

                        using (System.Drawing.Image image = (System.Drawing.Image)converter.ConvertFrom(encodedFile))
                        {
                            using(MemoryStream stream2 = new MemoryStream())
                            {
                                image.Save(stream2, System.Drawing.Imaging.ImageFormat.Png);
                                stream2.Position = 0;
                                var newPage = pdfDocument.Pages.Add(new PdfPage());

                                XGraphics xgr = XGraphics.FromPdfPage(newPage);

                                XImage imgFrame = XImage.FromStream(() => stream2);

                                newPage.Width = imgFrame.PointWidth;
                                newPage.Height = imgFrame.PointHeight;

                                xgr.DrawImage(imgFrame, 0, 0);
                                xgr.Save();
                                xgr.Dispose();
                            }
                            
                           
                        }
                    }
                }
                catch (Exception e)
                {
                   // pdfError = true;
                    Log.Error(e,"Error occurred decoding page");
                }


                ifdOffset = ifd.NextOffset; // get the next page
            }

            /*if (pdfError)
            {
                PdfPage page = pdfDocument.Pages[0];
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                gfx.DrawString("Document Conversion Error - Please download the document and view on your PC.", font, XBrushes.Black,
               new XRect(0, 0, page.Width, page.Height), new XStringFormat(){ Alignment = XStringAlignment.Center});
            }*/


            // Convert doc to stream or bytes 
            var pdfMemoryStream = new MemoryStream();
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


