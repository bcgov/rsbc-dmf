using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rsbc.Dmf.CaseManagement.Service;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    /// <summary>
    /// CaseManagerTests using XUnit dependency injection
    /// </summary>
    public class CaseManagerTests : TestBase
    {
        private readonly ICaseManager _caseManager;
        private readonly IDocumentManager _documentManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger = new NullLogger<CaseManagerTests>();

        public CaseManagerTests(ICaseManager caseManager, IDocumentManager documentManager, IConfiguration configuration)
        {
            _caseManager = caseManager;
            _documentManager = documentManager;
            _configuration = configuration;
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetFlagsAndSearchById()
        {
            var title = "222";
            // first do a search to get this case by title.
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;
            if (queryResults.Count() > 0)
            {
                var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
                var caseId = dmerCase.Id;

                List<Flag> flags = new List<Flag>()
                {
                    new Flag(){Description  = "testFlag - 1", Id = "flagTestItem1"},
                    new Flag(){Description  = "testFlag - 2", Id = "flagTestItem2"},
                };
                var result = await _caseManager.SetCaseFlags(caseId, false, flags, _logger);
                result.ShouldNotBeNull().Success.ShouldBe(true);

                var actualCase = (await _caseManager.CaseSearch(new CaseSearchRequest { CaseId = caseId })).Items.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();

                actualCase.Flags.Count().ShouldBe(flags.Count);
                foreach (var actualFlag in actualCase.Flags)
                {
                    var expectedFlag = flags.Where(f => f.Id == actualFlag.Id && f.Description == actualFlag.Description).ShouldHaveSingleItem();
                }
            }
        }

        /// <summary>
        /// CanSetCleanPassValue
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetCleanPassValue()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];

            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;
            if (queryResults.Count() > 0)
            {
                queryResults.ShouldNotBeEmpty();
                foreach (var dmerCase in queryResults)
                {
                    dmerCase.ShouldBeAssignableTo<DmerCase>().Driver.DriverLicenseNumber.ShouldBe(driverLicenseNumber);
                    List<Flag> flags = new List<Flag>()
                    {
                        new Flag(){Description  = "testFlag - 1", Id = "flagTestItem1"},
                        new Flag(){Description  = "testFlag - 2", Id = "flagTestItem2"},
                    };
                    await _caseManager.SetCaseFlags(dmerCase.Id, true, flags, _logger);
                }
            }
        }

        /// <summary>
        /// CanSetCleanPassValue
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateCleanPassValue()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];

            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;
            if (queryResults.Count() > 0)
            {
                queryResults.ShouldNotBeEmpty();
                foreach (var dmerCase in queryResults)
                {
                    var caseId = dmerCase.Id;
                    // set the value to true
                    await _caseManager.SetCleanPassFlag(caseId, false);

                    // Update Clean Pass Flag

                    await _caseManager.UpdateCleanPassFlag(caseId);
                }

                // verify in dynamics whether this is updated

            }


        }

        /// <summary>
        /// CanSetManualPassValue
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetManualPassValue()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];

            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;
            if (queryResults.Count() > 0)
            {
                queryResults.ShouldNotBeEmpty();
                foreach (var dmerCase in queryResults)
                {
                    var caseId = dmerCase.Id;
                    // set the value to true
                    await _caseManager.SetManualPassFlag(caseId, false);

                    // Update Manaul Pass Flag
                    await _caseManager.UpdateManualPassFlag(caseId);
                }

                // verify in dynamics wether this is updated

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
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;
            if (queryResults.Count() > 0)
            {
                var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
                var caseId = dmerCase.Id;

                await _caseManager.SetCasePractitionerClinic(caseId, "", "");
            }

        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByTitle()
        {
            var title = "222";
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            if (queryResults.Count() > 0)
            {

                var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
                dmerCase.Title.ShouldBe(title);

                dmerCase.CreatedBy.ShouldNotBeNullOrEmpty();
                dmerCase.Driver.DriverLicenseNumber.ShouldNotBeNullOrEmpty();
                dmerCase.Driver.Name.ShouldNotBeNullOrEmpty();
            }
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByDriverLicense()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];

            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;

            if (queryResults.Count() > 0)
            {

                queryResults.ShouldNotBeEmpty();
                foreach (var dmerCase in queryResults)
                {
                    dmerCase.ShouldBeAssignableTo<DmerCase>().Driver.DriverLicenseNumber.ShouldBe(driverLicenseNumber);
                }
            }
        }


        /// <summary>
        /// Verify that the Practioner and Clinic set function can be called with the empty string.
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = RequiresDynamics)]
        public async Task CanLegacyCandidateCreate()
        {
            var newDriver = new LegacyCandidateSearchRequest()

            {
                DriverLicenseNumber = "999" + (DateTime.Now.Year % 10).ToString()
                    + (DateTime.Now.Hour % 10).ToString() + (DateTime.Now.Minute % 10).ToString()
                    + (DateTime.Now.Second % 10).ToString() + (DateTime.Now.Millisecond % 10).ToString(),
                Surname = "TEST",
                SequenceNumber = 1
            };
            DateTime testDate = DateTime.Now;

            await _caseManager.LegacyCandidateCreate(newDriver, testDate, testDate, null) ;
            await _caseManager.LegacyCandidateCreate(newDriver, testDate, DateTime.Now, null );

            var newCaseId = await _caseManager.GetNewestCaseIdForDriver(newDriver);

            Assert.True(newCaseId.HasValue);


        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByClinicId()
        {
            var title = "222";
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;
            if (queryResults.Count() > 0)
            {
                var testItem = queryResults.First().ShouldBeAssignableTo<DmerCase>();

                var expectedClinicId = testItem.ClinicId;

                queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { ClinicId = expectedClinicId })).Items;

                queryResults.ShouldNotBeEmpty();
                foreach (var dmerCase in queryResults)
                {
                    dmerCase.ShouldBeAssignableTo<DmerCase>().ClinicId.ShouldBe(expectedClinicId);
                }
            }

        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetFlags()
        {

            var queryResults = await _caseManager.GetAllFlags();

            queryResults.ShouldNotBeEmpty();

        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanDoDpsProcessingDate()
        {
            var queryResults = _caseManager.GetDpsProcessingDate();

            Assert.NotEqual(queryResults, DateTimeOffset.MinValue);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateNonComplyDocuments()
        {
            await _caseManager.UpdateNonComplyDocuments();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateResolveCaseStatus()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults1 = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;

            var queryResults = queryResults1.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            // set the Case Resolve Date to get past date
            //DateTimeOffset caseResolveDate = DateTimeOffset.UtcNow.AddDays(-500);     
            DateTimeOffset caseResolveDate = DateTimeOffset.UtcNow;

            // Get the case and Set the dfp_caseresolvedate to date in past

            await _caseManager.SetCaseResolveDate(caseId, caseResolveDate);

            // Set the case status to false

            await _caseManager.SetCaseStatus(caseId, false);

            // Act
            await _caseManager.ResolveCaseStatusUpdates();

            // Assert

            // Manually verify the case status is set
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanResolveCaseStatusUpdates()
        {
            await _caseManager.ResolveCaseStatusUpdates();
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanDeleteComment()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];
            // get the case
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            string documentUrl = $"TEST-DOCUMENT-{DateTime.Now.ToFileTimeUtc()}";

            // add a document

            LegacyComment legacyCommentRequest = new LegacyComment
            {
                CaseId = caseId,
                Driver = new Driver { DriverLicenseNumber = driverLicenseNumber },
                SequenceNumber = 1,
                CommentDate = DateTimeOffset.UtcNow.AddDays(-1),
                CommentText = "AUTOMATED TEST COMMENT",
                CommentTypeCode = "W",
                UserId = "TEST"
            };

            await _caseManager.CreateLegacyCaseComment(legacyCommentRequest);

            // confirm it is present

            var comments = await _caseManager.GetCaseLegacyComments(caseId, true, OriginRestrictions.None);

            bool found = false;

            string commentId = null;

            foreach (var comment in comments)
            {
                if (comment.CommentText == legacyCommentRequest.CommentText)
                {
                    found = true;
                    commentId = comment.CommentId;
                    break;
                }
            }

            Assert.True(found);

            // test the get
            var c = await _caseManager.GetComment(commentId);
            // delete it            

            await _caseManager.DeleteComment(commentId);

            // confirm that it is deleted

            found = false;

            comments = await _caseManager.GetCaseLegacyComments(caseId, true, OriginRestrictions.None);

            foreach (var comment in comments)
            {
                if (comment.CommentText == legacyCommentRequest.CommentText)
                {
                    found = true;
                    break;
                }
            }

            Assert.False(found);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanDeleteLegacyDocument()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];


            string documentUrl = $"TEST-DOCUMENT-{DateTime.Now.ToFileTimeUtc()}";

            // add a document

            LegacyDocument legacyDocumentRequest = new LegacyDocument
            {
                BatchId = "1",
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

            await _caseManager.CreateDocumentOnDriver(legacyDocumentRequest);

            // confirm it is present

            var docs = await _documentManager.GetDriverLegacyDocuments(driverLicenseNumber, false);

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

            var getDocument = await _documentManager.GetLegacyDocument(documentId);

            Assert.NotNull(getDocument.Driver.Id);

            Assert.NotNull(getDocument.Driver.DriverLicenseNumber);

            // delete it

            await _caseManager.DeactivateLegacyDocument(documentId);

            // confirm that it is deleted

            found = false;

            docs = await _documentManager.GetDriverLegacyDocuments(driverLicenseNumber, false);

            foreach (var doc in docs)
            {
                if (doc.DocumentId == documentId)
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
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            // We need to get a valid case Id to test

            var bringForwardRequest = new CaseManagement.BringForwardRequest()
            {
                CaseId = caseId,
                Assignee = string.Empty,
                Description = "Test Description1",
                Subject = "ICBC Error Test",
                Priority = (CaseManagement.CallbackPriority?)CallbackPriority.High
            };
            var result = await _caseManager.CreateBringForward(bringForwardRequest);
            result.ShouldNotBeNull();
            Assert.True(result.Success);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task SwitchTo8Dl()
        {
            //await _caseManager.SwitchTo8Dl();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task MakeFakeDls()
        {
            //await _caseManager.MakeFakeDls();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateIcbcError()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            // We need to get a valid case Id to test
            var icbcErrorRequest = new IcbcErrorRequest()
            {
                CaseId = caseId,
                ErrorMessage = "Icbc Error Testing"

            };
            var result = await _caseManager.MarkMedicalUpdateError(icbcErrorRequest);
            result.ShouldNotBeNull();
            Assert.True(result.Success);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetUnsentMedicalUpdates()
        {
          /*  var driverLicenseNumber = _configuration["ICBC_TEST_DL"];
            // first do a search to get this case by title.
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;*/

            // Load the documents for that case

            var result = await _caseManager.GetUnsentMedicalPass();

            result.Items.ShouldNotBeEmpty();

        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetUnsentMedicalUpdatesAdjudication()
        {
            var queryResults = await _caseManager.GetUnsentMedicalAdjudication();
            queryResults.Items.ShouldNotBeEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateDriverBirthDate()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];

            var request = new UpdateDriverRequest()
            {
                BirthDate = new DateTime(1994, 02, 16),
                DriverLicenseNumber = driverLicenseNumber
            };
            // Get the driver

            var result = await _caseManager.UpdateBirthDate(request);
            result.ShouldNotBeNull();
            Assert.True(result.Success);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetListOfPdfDocuments()
        {

            await _caseManager.GetPdfDocuments();

        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdatePdfDocumentStatus()
        {

            var pdfDocumentId = await _caseManager.CreatePdfDocument(new PdfDocument { StatusCode = StatusCodeOptionSet.SendToBCMail });
            var request = new PdfDocument()
            {

                PdfDocumentId = pdfDocumentId.ToString(),
                StatusCode = StatusCodeOptionSet.Sent
            };
            var result = await _caseManager.UpdatePdfDocumentStatus(request);
            result.ShouldNotBeNull();
            Assert.True(result.Success);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateDriver()
        {
            var dynamicsContext = ((CaseManager)_caseManager).dynamicsContext;
            // Act : Create the driver
            var request = new CreateDriverRequest()
            {
                DriverLicenseNumber = "01234119",
                BirthDate = DateTimeOffset.UtcNow,
                Surname = "TestDriver"

            };

            // check to driver exsists and delete 
            var driverExists = dynamicsContext.dfp_drivers.Expand(c => c.dfp_PersonId).Where(x => x.dfp_licensenumber == request.DriverLicenseNumber).FirstOrDefault();


            if (driverExists != null)
            {
                // Delete if driver exsists
                bool result = false;
                dynamicsContext.DeleteObject(driverExists);
                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.DetachAll();
                result = true;
            }


            // Create Driver
            var createDriver = await _caseManager.CreateDriver(request);

            // Query dynamics to check if the driver is created

            var createdDriverExists = dynamicsContext.dfp_drivers.Expand(c => c.dfp_PersonId).Where(x => x.dfp_licensenumber == request.DriverLicenseNumber).FirstOrDefault();

            Assert.Equal(createdDriverExists.dfp_licensenumber, request.DriverLicenseNumber);


        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateCase()
        {

            var dynamicsContext = ((CaseManager)_caseManager).dynamicsContext;

            // Arrange
            var request = new CreateCaseRequest()
            {
                DriverLicenseNumber = "01234571",
                SequenceNumber = 2,
                CaseTypeCode = "EVF"
            };

            // check to driver 
            var driverExists = dynamicsContext.dfp_drivers.Expand(c => c.dfp_PersonId).Where(x => x.dfp_licensenumber == request.DriverLicenseNumber).FirstOrDefault();

            if (driverExists != null)
            {

                // check for case 
                var caseQuery = dynamicsContext.incidents.Where(i => i._dfp_driverid_value == driverExists.dfp_driverid).FirstOrDefault();

                if (caseQuery != null)
                {

                    // delete the case
                    bool result = false;
                    dynamicsContext.DeleteObject(caseQuery);
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();
                    result = true;

                }

                // Create a case  

                var createCase = await _caseManager.CreateCase(request);

                // Search for the case id 

                var searchaseQuery = dynamicsContext.incidents.Where(i => i._dfp_driverid_value == driverExists.dfp_driverid).FirstOrDefault();

                Assert.Equal(searchaseQuery._dfp_driverid_value, driverExists.dfp_driverid);

            }
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateDocumentEnvelope()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];
            // get the case
            var queryResults = (await _caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items.FirstOrDefault();

            var dmerCase = queryResults.ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

            string documentUrl = $"DMER-TEST-DOCUMENT-{DateTime.Now.ToFileTimeUtc()}";

            // add a document

            LegacyDocument legacyDocumentRequest = new LegacyDocument
            {
                BatchId = "1",
                CaseId = caseId,
                DocumentType = "DMER",
                DocumentTypeCode = "001",
                Driver = new Driver { DriverLicenseNumber = driverLicenseNumber },
                // FaxReceivedDate = DateTime.MinValue,
                ImportDate = DateTimeOffset.UtcNow,
                FileSize = 10,
                DocumentPages = 1,
                OriginatingNumber = "1",
                SequenceNumber = 1,
                DocumentUrl = documentUrl,
                SubmittalStatus = "Open-Required",
                Owner = "Team - Intake"
            };

            await _caseManager.CreateICBCDocumentEnvelope(legacyDocumentRequest);

            // confirm it is present

            var docs = await _documentManager.GetCaseLegacyDocuments(caseId);

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

            Assert.False(found);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateDocumentOnDriver()
        {
            var driverLicenseNumber = _configuration["ICBC_TEST_DL"];

            string documentUrl = $"DMER-TEST-DOCUMENT-{DateTime.Now.ToFileTimeUtc()}";

            if (driverLicenseNumber != null)
            {
                LegacyDocument legacyDocumentRequest = new LegacyDocument
                {
                    BatchId = "1",
                    //CaseId = caseId,
                    DocumentType = "DMER",
                    DocumentTypeCode = "001",
                    Driver = new Driver { DriverLicenseNumber = driverLicenseNumber, Surname = "WAVE1", BirthDate = new DateTime(1994, 02, 16), GivenName = "Test" },
                    // FaxReceivedDate = DateTime.MinValue,
                    ImportDate = DateTimeOffset.UtcNow,
                    FileSize = 10,
                    DocumentPages = 1,
                    OriginatingNumber = "1",
                    SequenceNumber = 1,
                    DocumentUrl = documentUrl,
                    SubmittalStatus = "Uploaded",
                    
                };

                await _caseManager.CreateDocumentOnDriver(legacyDocumentRequest);

            }

        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetCaseDetails()
        {
            var caseId = _configuration["ICBC_TEST_CASEID"];
            var c = await _caseManager.GetCaseDetail(caseId);
            Assert.NotNull(c);
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetCaseByIdCode()
        {
            var IdCode = _configuration["ICBC_TEST_IDCODE"];
            var c = await _caseManager.GetCaseByIdCode(IdCode);
            Assert.NotNull(c);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task GetMostRecentCase()
        {
            string driverId = _configuration["ICBC_TEST_DRIVERID"];
            var response = _caseManager.GetMostRecentCaseDetail(Guid.Parse(driverId));

            Assert.NotNull(response);

        }
    }
}