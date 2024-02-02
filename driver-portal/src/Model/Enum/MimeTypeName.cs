using EnumsNET;
using System.ComponentModel;
using Winista.Mime;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public enum MimeTypeName
    {
        [Description("image/png")]
        Png,

        [Description("image/jpeg")]
        Jpeg,

        [Description("application/pdf")]
        Pdf
    }

    public static class MimeTypeNameExtensions
    {
        public static bool EqualsMimeType(this MimeTypeName mimeTypeName, MimeType mimeType)
        {
            var mimeTypes = new MimeTypes();
            var mimeTypeFromName = mimeTypes.ForName(mimeTypeName.AsString(EnumFormat.Description));
            return mimeType.Equals(mimeTypeFromName);
        }
    }
}
