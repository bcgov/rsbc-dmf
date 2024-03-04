using System.Collections.Immutable;

﻿namespace Rsbc.Dmf.DriverPortal.Api
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
    }
}
