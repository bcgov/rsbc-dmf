using EnumsNET;
using Rsbc.Dmf.DriverPortal.Api;
using Winista.Mime;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class MimeTypeTests
    {
        private readonly MimeTypes _mimeTypes = new MimeTypes();

        [Fact]
        public void GetAllowableMimeTypeOrPdf_Check_Pdf()
        {
            var fileName = "test.pdf";
            var mimeType = DocumentUtils.GetAllowableMimeTypeOrPdf(fileName);

            Assert.Equal("application/pdf", mimeType.Name);
        }

        [Fact]
        public void Mime()
        {
            var mimeType = _mimeTypes.GetMimeType("test.png");

            Assert.True(MimeTypeName.Png.EqualsMimeType(mimeType));
        }
    }
}
