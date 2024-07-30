using Microsoft.Extensions.Configuration;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class DocumentTests : ApiIntegrationTestBase
{
    public DocumentTests(IConfiguration configuration) : base(configuration) { }

    //[Fact]
    public async Task Get_Documents_By_Type_For_User()
    {
        var loginIds = _configuration["TEST_LOGIN_IDS"];
        if (string.IsNullOrEmpty(loginIds))
            return;

        var documentTypeCode = "001";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_API_BASE}/Type/{documentTypeCode}");
        var clientResult = await HttpClientSendRequest<IEnumerable<CaseDocument>>(request);

        Assert.NotNull(clientResult);
    }

    [Fact]
    public async Task Claim_Dmer()
    {
        var documentId = _configuration["TEST_DMER_DOCUMENT_ID"];
        if (string.IsNullOrEmpty(documentId))
            return;

        // unclaim dmer, so we know we are starting from a known state of LoginId being null
        var request = new HttpRequestMessage(HttpMethod.Post, $"{DOCUMENT_API_BASE}/UnclaimDmer?documentId={documentId}");
        var clientResult = await HttpClientSendRequest<DmerDocument>(request);

        Assert.Null(clientResult.LoginId);

        // claim dmer
        request = new HttpRequestMessage(HttpMethod.Post, $"{DOCUMENT_API_BASE}/ClaimDmer?documentId={documentId}");
        clientResult = await HttpClientSendRequest<DmerDocument>(request);

        Assert.NotNull(clientResult.LoginId);

        // unclaim dmer
        request = new HttpRequestMessage(HttpMethod.Post, $"{DOCUMENT_API_BASE}/UnclaimDmer?documentId={documentId}");
        clientResult = await HttpClientSendRequest<DmerDocument>(request);

        Assert.Null(clientResult.LoginId);
    }
}
