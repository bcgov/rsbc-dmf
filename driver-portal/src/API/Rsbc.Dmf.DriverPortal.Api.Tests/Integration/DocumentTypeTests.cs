using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class DocumentTypeTests : ApiIntegrationTestBase
    {
        public DocumentTypeTests(IConfiguration configuration) : base(configuration) { }

        [Fact]
        public async Task Get_Driver_DocumentSubTypes()
        {
            var docId = _configuration["DOWNLOAD_DOC_ID"];
            if (string.IsNullOrEmpty(docId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_TYPE_API_BASE}/driver");
            var response = await HttpClientSendRequest<IEnumerable<DocumentSubType>>(request);

            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }
    }
}
