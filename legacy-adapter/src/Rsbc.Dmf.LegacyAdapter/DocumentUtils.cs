using System.IO;
using System.Linq;

namespace Rsbc.Dmf.LegacyAdapter
{
    public static class DocumentUtils
    {

        /// <summary>
        /// SanitizeKeyFilename
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public string SanitizeKeyFilename(string data)
        {

            var invalidCharacters = Path.GetInvalidFileNameChars().ToList();
            invalidCharacters.Add(' ');
            invalidCharacters.Add('/');
            invalidCharacters.Add('\\');

            string result = new string(data
                .Where(x => !invalidCharacters.Contains(x))
                .ToArray());

            return result;
        }

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
    }
}
