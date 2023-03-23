using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rsbc.Dmf.LegacyAdapter
{
    public static class DocumentUtils
    {

        static public DateTimeOffset ParseDpsDate(string data)
        {

            DateTimeOffset result;

            if (!string.IsNullOrEmpty(data))
            {
                if (!DateTimeOffset.TryParse(data, out result))
                {
                    // try an alternate format
                    int tPos = data.ToUpper().IndexOf("T");
                    if (tPos != -1)
                    {
                        data = data.Substring(0, tPos) + "T0" + data.Substring(tPos + 1);
                    }
                    

                    if (!DateTimeOffset.TryParse(data, out result))
                    {
                        result = DateTimeOffset.Now;
                    }

                }
            }
            else
            {
                result = DateTimeOffset.Now;
            }
            
            return result;
        }

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
            //mimetype = "application/pdf";

            return mimetype;
        }
    }
}
