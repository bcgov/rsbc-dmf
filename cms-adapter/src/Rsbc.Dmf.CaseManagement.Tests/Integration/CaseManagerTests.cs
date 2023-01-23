using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Rsbc.Dmf.CaseManagement.Service;
using Xunit;
using Xunit.Abstractions;
using System;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CaseManagerTests : WebAppTestBase
    {
        private readonly ICaseManager caseManager;

          public CaseManagerTests(ITestOutputHelper output) : base(output)
        {
           
            caseManager = services.GetRequiredService<ICaseManager>();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetFlagsAndSearchById()
        {
            var title = "222";
            // first do a search to get this case by title.
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            List<Flag> flags = new List<Flag>()
            {
                new Flag(){Description  = "testFlag - 1", Id = "flagTestItem1"},
                new Flag(){Description  = "testFlag - 2", Id = "flagTestItem2"},
            };
            var result = await caseManager.SetCaseFlags(caseId, false, flags, testLogger);
            result.ShouldNotBeNull().Success.ShouldBe(true);

            var actualCase = (await caseManager.CaseSearch(new CaseSearchRequest { CaseId = caseId })).Items.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();

            actualCase.Flags.Count().ShouldBe(flags.Count);
            foreach (var actualFlag in actualCase.Flags)
            {
                var expectedFlag = flags.Where(f => f.Id == actualFlag.Id && f.Description == actualFlag.Description).ShouldHaveSingleItem();
            }
        }


        /// <summary>
        /// Verify that the Practioner and Clinic set function can be called with the empty string.
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetCasePractitionerClinicEmpty()
        {
            var title = "222";
            // first do a search to get this case by title.
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            await caseManager.SetCasePractitionerClinic (caseId, "", "");

        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByTitle()
        {
            var title = "222";
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            dmerCase.Title.ShouldBe(title);

            dmerCase.CreatedBy.ShouldNotBeNullOrEmpty();
            dmerCase.Driver.DriverLicenseNumber.ShouldNotBeNullOrEmpty();
            dmerCase.Driver.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByDriverLicense()
        {
            var driverLicenseNumber = "1000098";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;

            queryResults.ShouldNotBeEmpty();
            foreach (var dmerCase in queryResults)
            {
                dmerCase.ShouldBeAssignableTo<DmerCase>().Driver.DriverLicenseNumber.ShouldBe(driverLicenseNumber);
            }
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByClinicId()
        {
            var title = "222";
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var testItem = queryResults.First().ShouldBeAssignableTo<DmerCase>();

            var expectedClinicId = testItem.ClinicId;

            queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { ClinicId = expectedClinicId })).Items;

            queryResults.ShouldNotBeEmpty();
            foreach (var dmerCase in queryResults)
            {
                dmerCase.ShouldBeAssignableTo<DmerCase>().ClinicId.ShouldBe(expectedClinicId);
            }
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetFlags()
        {

            var queryResults = await caseManager.GetAllFlags();

            queryResults.ShouldNotBeEmpty();
            
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanDoDpsProcessingDate()
        {
            var queryResults = caseManager.GetDpsProcessingDate();

            Assert.NotEqual (queryResults, DateTimeOffset.MinValue );
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateNonComplyDocuments()
        {
            await caseManager.UpdateNonComplyDocuments();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateResolveCaseStatus()
        {
            var driverLicenseNumber = configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults1 = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;
                
            var queryResults = queryResults1.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            // set the Case Resolve Date to get past date
            DateTimeOffset caseResolveDate = DateTimeOffset.UtcNow.AddDays(-500);     

            // Get the case and Set the dfp_caseresolvedate to date in past

            await caseManager.SetCaseResolveDate(caseId, caseResolveDate);

            // Set the case status to false
            
            await caseManager.SetCaseStatus(caseId, false);

            // Act
            await caseManager.ResolveCaseStatusUpdates();

            // Assert

           // Manually verify the case status is set
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanResolveCaseStatusUpdates()
        {
            await caseManager.ResolveCaseStatusUpdates();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanDeleteLegacyDocument()
        {
            var driverLicenseNumber = configuration["ICBC_TEST_DL"];
            // get the case
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            string documentUrl = $"TEST-DOCUMENT-{DateTime.Now.ToFileTimeUtc()}";

            // add a document

            LegacyDocument legacyDocumentRequest = new LegacyDocument { BatchId = "1", 
                CaseId = caseId,
                DocumentType = "Legacy Review",
                DocumentTypeCode = "LegacyReview",
                Driver = new Driver { DriverLicenseNumber = driverLicenseNumber },
                FaxReceivedDate = DateTimeOffset.UtcNow,
                ImportDate = DateTimeOffset.UtcNow,
                FileSize = 10,
                DocumentPages = 1,
                OriginatingNumber = "1",
                SequenceNumber = 1,
                DocumentUrl = documentUrl                
            };

            await caseManager.CreateLegacyCaseDocument(legacyDocumentRequest);

            // confirm it is present

            var docs = await caseManager.GetCaseLegacyDocuments(caseId);

            bool found = false;

            string documentId = null;

            foreach (var doc in docs)
            {
                if (doc.DocumentUrl == documentUrl)
                {
                    found = true;
                    documentId = doc.DocumentId;
                    break;
                }
            }

            Assert.True(found);

            // delete it

            await caseManager.DeleteLegacyDocument(documentId);

            // confirm that it is deleted

            found = false;

            docs = await caseManager.GetCaseLegacyDocuments(caseId);

            foreach (var doc in docs)
            {
                if (doc.DocumentUrl == documentUrl)
                {
                    found = true;                    
                    break;
                }
            }

            Assert.False(found);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateBringForward()
        {
            var driverLicenseNumber = configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            // We need to get a valid case Id to test

            var bringForwardRequest = new CaseManagement.BringForwardRequest()
            {
                CaseId = caseId,
                Assignee = string.Empty,
                Description = "Test Description1",
                Subject = "ICBC Error",
                Priority = (CaseManagement.BringForwardPriority?)BringForwardPriority.Normal
            };
            var result = await caseManager.CreateBringForward(bringForwardRequest);
            result.ShouldNotBeNull();
            Assert.True(result.Success);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateIcbcError()
        {
            var driverLicenseNumber = configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            // We need to get a valid case Id to test
            var icbcErrorRequest = new IcbcErrorRequest()
            {
                CaseId = caseId,
                ErrorMessage = "Icbc Error Testing"

            };
            var result = await caseManager.MarkMedicalUpdateError(icbcErrorRequest);
            result.ShouldNotBeNull();
            Assert.True(result.Success);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetUnsentMedicalUpdates()
        {
            var queryResults = await caseManager.GetUnsentMedicalUpdates();            
            queryResults.Items.ShouldNotBeEmpty();
        }


    }
}