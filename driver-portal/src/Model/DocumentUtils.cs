using EnumsNET;
using Winista.Mime;

namespace Rsbc.Dmf.DriverPortal.Api
{
    // get mime mappings from here
    // https://github.com/Microsoft/referencesource/blob/master/System.Web/MimeMapping.cs
    public class DocumentUtils
    {
        /// <summary>
        /// GetMimeType
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [Obsolete]
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

        /// <summary>
        /// GetMimeType
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static MimeType GetAllowableMimeTypeOrPdf(string filename)
        {
            var mimeTypes = new MimeTypes();
            var mimeType = mimeTypes.GetMimeType(filename);
            return IsAllowedMimeType(mimeType) 
                ? mimeType 
                : mimeTypes.ForName(MimeTypeName.Pdf.AsString());
        }

        public static bool IsAllowedMimeType(MimeType mimeType)
        {
            return MimeTypeName.Png.EqualsMimeType(mimeType) 
                || MimeTypeName.Jpeg.EqualsMimeType(mimeType) 
                || MimeTypeName.Pdf.EqualsMimeType(mimeType);
        }

        public static byte[] GetByteArray(IFormFile file)
        {
            var ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            return ms.ToArray();
        }
    }
}
