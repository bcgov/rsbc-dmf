using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace RSBC.DMF.MedicalPortal.API.Services
{
    public class PdfService
    {
        private readonly IConverter Converter;

        public PdfService(IConverter converter)
        {
            Converter = converter;
        }

        // NOTE this could be a shared service by moving the dependency Haukcode.WkHtmlToPdfDotNet. Currently, used only by ChefsController.PutSubmission
        // Write the results to file with `File.WriteAllBytes(fileName, pdfData);`
        public byte[] GeneratePdf(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                throw new ArgumentNullException(nameof(html));
            }

            var margins = new MarginSettings() { Top = 3, Bottom = 3, Left = 0.5, Right = 0.5, Unit = Unit.Inches };          
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.Letter,
                    DPI = 72,
                    Margins = margins
                },
                Objects = {
                    new ObjectSettings() {
                        LoadSettings = { BlockLocalFileAccess = false ,  LoadErrorHandling = ContentErrorHandling.Abort },
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8",},
                    }
                }
            };

            var pdfData = Converter.Convert(doc);
            return pdfData;
        }
    }

    public static class PdfServiceExtensions
    {
        public static IServiceCollection AddPdfService(this IServiceCollection services)
        {
            services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
            services.AddTransient<PdfService>();
            return services;
        }
    }
}
