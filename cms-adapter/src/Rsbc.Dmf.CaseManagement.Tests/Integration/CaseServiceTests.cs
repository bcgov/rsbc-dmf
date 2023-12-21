using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using Rsbc.Dmf.CaseManagement.Service;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CaseServiceTests : WebAppTestBase
    {
        private readonly CaseService caseService;


        public CaseServiceTests(ITestOutputHelper output) : base(output)
        {
            var logger = services.GetRequiredService<ILogger<CaseService>>();
            var caseManager = services.GetRequiredService<ICaseManager>();
            caseService =
                new CaseService(logger, caseManager, configuration); 
        }
        

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByStatusPending()
        {
            var expectedClinicId = "a5a45383-8ff4-eb11-b82b-00505683fbf4";
            var request = new SearchRequest()
            {
                ClinicId = expectedClinicId
            };
            request.Statuses.Add("Pending");

            var queryResults = (await caseService.Search(request, null)).Items;

            queryResults.ShouldNotBeEmpty();
            
        }

        /*
        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetComment()
        {
            var id = "";
            var request = new CommentIdRequest()
            {
                CommentId = id
            };
            var queryResults = await caseService.GetComment(request, null);
        }
        */

        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetFlags()
        {
            var request = new EmptyRequest();

            var queryResults = (await caseService.GetAllFlags(request, null)).Flags;

            queryResults.ShouldNotBeEmpty();

        }

        [Fact(Skip = RequiresDynamics)]
        public async void GetUnsentMedicalUpdates()
        {
            var unsentItems = await caseService.GetUnsentMedicalPass(new EmptyRequest(), null);

            var size = unsentItems.Items.Count;
        }


        [Fact(Skip = RequiresDynamics)]
        public async void GetUnsentMedicalUpdates1()
        {
            var unsentItems = await caseService.GetUnsentMedicalAdjudication(new EmptyRequest(), null);

            var size = unsentItems.Items.Count;
        }

        [Fact(Skip = RequiresDynamics)]
        public async void CheckAddGetDocument()
        {
            var driverLicenseNumber = configuration["ICBC_TEST_DL"];
            var newDocument = new Service.LegacyDocument
            {
                DpsDocumentId = 88,
                Driver = new Service.Driver{ DriverLicenseNumber = driverLicenseNumber },
                BatchId = "1234",
                CaseId = string.Empty,
                DocumentId = string.Empty,
                DocumentPages = 4,
                DocumentTypeCode = "001", // DMER
                DocumentUrl = "http://localhost",
                ImportId = "4321",
                OriginatingNumber = "123-123-1234",
                ValidationMethod = "TEST",
                ValidationPrevious = "TEST1",
                SequenceNumber = -1,
                UserId = "SYSTEM",
                Priority = "Critical Review",
                Queue = "Nurse Case Managers"
            };

            var newDocumentResponse = await caseService.CreateDocumentOnDriver(newDocument, null);

            Assert.True(newDocumentResponse.ResultStatus == ResultStatus.Success);
            
            var response = await caseService.GetLegacyDocument(
                new LegacyDocumentRequest() { DocumentId = newDocumentResponse.Id }, null);

            Assert.True(response.ResultStatus == ResultStatus.Success);

            Assert.Equal(newDocument.DpsDocumentId, response.Document.DpsDocumentId);
            Assert.Equal(newDocument.BatchId, response.Document.BatchId);
            Assert.Equal(newDocument.DocumentPages, response.Document.DocumentPages);
            Assert.Equal(newDocument.DocumentUrl, response.Document.DocumentUrl);
            Assert.Equal(newDocument.ImportId, response.Document.ImportId);
            Assert.Equal(newDocument.ValidationMethod, response.Document.ValidationMethod);
            Assert.Equal(newDocument.ValidationPrevious, response.Document.ValidationPrevious);
            Assert.Equal(newDocument.Priority, response.Document.Priority);
            Assert.Equal(newDocument.Queue, response.Document.Queue);

        }
    }
}
