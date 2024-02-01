using Google.Protobuf;
﻿namespace Rsbc.Dmf.DriverPortal.Api

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
        static public string GetMimeType(string filename)
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

        public static ByteString GetByteString(IFormFile file)
        {
            var ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            var data = ms.ToArray();
            return ByteString.CopyFrom(data);
        }
    }
}
