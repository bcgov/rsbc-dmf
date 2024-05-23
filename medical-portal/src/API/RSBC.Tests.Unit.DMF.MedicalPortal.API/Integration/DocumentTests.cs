using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RSBC.Tests.Unit.DMF.MedicalPortal.API.Integration
{
    public class DocumentTests : ApiIntegrationTestBase
    {
        public DocumentTests(IConfiguration configuration) : base(configuration) { }

        //[Fact]
        public async Task Get_Documents_By_Type_For_User()
        {
            var loginIds = _configuration["Test:LoginIds"];
            if (string.IsNullOrEmpty(loginIds))
                return;

            var documentTypeCode = "001";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_API_BASE}/Type/{documentTypeCode}");
            var clientResult = await HttpClientSendRequest<IEnumerable<CaseDocument>>(request);

            Assert.NotNull(clientResult);
        }
    }
}
