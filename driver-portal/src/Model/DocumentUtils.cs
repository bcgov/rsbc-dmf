namespace Rsbc.Dmf.DriverPortal.Api
{
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
    }
}
