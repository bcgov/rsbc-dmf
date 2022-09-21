using System.IO;

namespace Rsbc.Dmf.LegacyAdapter
{
    public static class MimeUtils
    {

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
    }
}
