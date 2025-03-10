using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using System.Collections.Immutable;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using TiffLibrary;
using TiffLibrary.PixelFormats;
using TiffLibrary.Compression;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;

﻿namespace Rsbc.Dmf.DriverPortal.Api
{
    // get mime mappings from here
    // https://github.com/Microsoft/referencesource/blob/master/System.Web/MimeMapping.cs
    public class DocumentUtils
    {
        private static string fileError = "This file is corrupted and cannot be opened.";

        /// <summary>
        /// GetMimeType
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetMimeType(string filename)
        {
            string mimetype = "application/pdf";

            if (!string.IsNullOrEmpty(filename))
            {
                string extension = Path.GetExtension(filename);

                if (extension != null && (".tif" == extension.ToLower() || ".tiff" == extension.ToLower()))
                {
                    mimetype = "image/tiff";
                }
            }

            return mimetype;
        }

        public static bool IsAllowedMimeType(string mimeType)
        {
            return new string[] { "application/pdf", "image/png", "image/jpeg" }.Contains(mimeType);
        }

        public static byte[] GetByteArray(IFormFile file)
        {
            var ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            return ms.ToArray();
        }

        public static byte[] convertTiff2Pdf(byte[] tiffBytes)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Create Image Stream
            MemoryStream imageStream = new MemoryStream(tiffBytes);
            imageStream.Position = 0;

            PdfSharpCore.Pdf.PdfDocument pdfDocument = new PdfSharpCore.Pdf.PdfDocument();

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

                TiffImageDecoder decoder = tiff.CreateImageDecoder(ifd);


                TiffRgb24[] pixels = new TiffRgb24[decoder.Width * decoder.Height];

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

                    using (var stream2 = new MemoryStream(encodedFile))
                    {
                        var newPage = pdfDocument.Pages.Add(new PdfSharpCore.Pdf.PdfPage());

                        XGraphics xgr = XGraphics.FromPdfPage(newPage);

                        XImage imgFrame = XImage.FromStream(() => stream2);

                        newPage.Width = imgFrame.PointWidth;
                        newPage.Height = imgFrame.PointHeight;

                        xgr.DrawImage(imgFrame, 0, 0);
                        xgr.Save();
                        xgr.Dispose();
                    }
                }

                ifdOffset = ifd.NextOffset; // get the next page
            }


            // Convert doc to stream or bytes 
            var pdfMemoryStream = new MemoryStream();
            pdfDocument.Save(pdfMemoryStream);
            return pdfMemoryStream.ToArray();

        }
 
         public static string checkTiff2Pdf(string documentId, CaseManager.CaseManagerClient _cmsAdapterClient, DocumentStorageAdapterClient _documentStorageAdapterClient)
        {
            var reply = _cmsAdapterClient.GetLegacyDocument(new LegacyDocumentRequest() { DocumentId = documentId });
            string errorMessage = "";
            // fetch the file from S3
            var downloadFileRequest = new DownloadFileRequest()
            {
                ServerRelativeUrl = reply.Document?.DocumentUrl
            };

            var documentReply = _documentStorageAdapterClient.DownloadFile(downloadFileRequest);
            if (documentReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                byte[] fileContents = documentReply.Data.ToByteArray();
                string fileName = Path.GetFileName(reply.Document.DocumentUrl);
                string mimetype = GetMimeType(fileName);
                if (fileName.EndsWith(".tif") || fileName.EndsWith(".tiff"))
                {

                    try
                    {
                        var tempDoc = convertTiff2Pdf(fileContents);
                    }
                    catch
                    {
                        errorMessage = fileError;
                    }
                }
            }
            else
                errorMessage = fileError;

            return errorMessage;
        }

    }
}
