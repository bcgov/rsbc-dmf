

using FileHelpers;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.FlatFileModels;
using Pssg.Interfaces.Icbc.FlatFileModels;
using Pssg.Interfaces.Icbc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using System.Net.Http;
using System.Net;
using Grpc.Net.Client;
using Rsbc.Dmf.CaseManagement.Service;
using Pssg.Interfaces.Icbc.Helpers;
using Rsbc.Dmf.CaseManagement.Helpers;
using static Rsbc.Dmf.IcbcAdapter.EnhancedIcbcApiUtils;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public class EnhancedApiTest
    {

        IConfiguration Configuration;
        FlatFileUtils flatFileUtils;
        private IIcbcClient IcbcClient { get; set; }
        CaseManager.CaseManagerClient CaseManagerClient { get; set; }
        EnhancedIcbcApiUtils enhancedIcbcApiUtils;
        private const string FileBase64 = "MDEyMzQ1NjcwMTIzNDU2NzJTTUlUSEVORSAgICAgICAgICAgICAgICAgICAgICAgICAgIE1DQ0MgIDE5OTYtMDItMjYgICAgICAgICAgMjAxNy0wNS0wMzIwMTctMDUtMDMxNTAwMjAyNS0wNi0xMQ0KOTg3NjU0MzI5ODc2NTQzMjNHUkFZICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEZDQ0MgIDE5NjYtMDEtMDQgICAgICAgICAgMjAxMy0wNy0xOTIwMjUtMDMtMjAxNTAwMjAyNS0wNi0xMQ0KMTkyODM3NDYxOTI4Mzc0NjFBUkNISUJBTERFTlkgICAgICAgICAgICAgICAgICAgICAgIE1BRE1JTjE5OTktMDQtMjAyMDI5LTA0LTIwMjAyNC0wOC0yODIwMTktMDUtMTkxMzYwMjAyNS0wNi0xMQ0KNTY0NzM4Mjk1NjQ3MzgyOTJST0JCICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE1BRE1JTjE5OTktMDctMDEyMDI2LTA3LTAxMjAyMS0wOS0wOTIwMjQtMDEtMTYxNTAwMjAyNS0wNi0xMQ0KMTEyMjMzNDQxMTIyMzM0NDJNT09SRVNZICAgICAgICAgICAgICAgICAgICAgICAgICAgIE1BRE1JTjIwMDQtMDctMTUgICAgICAgICAgMjAyMi0wNS0yNTIwMjAtMTItMDUxMDAwMjAyNS0wNi0xMQ0K";

        /// <summary>
        /// Setup the test
        /// </summary>        
        public EnhancedApiTest()
        {
            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for the icbc adapter in this test project
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();
            // create a new case manager client.
            if (Configuration["ICBC_SERVICE_URI"] != null)
            {
                IcbcClient = new EnhancedIcbcClient(Configuration);
            }
            else
            {
                IcbcClient = IcbcHelper.CreateMock();
            }

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"];

            if (string.IsNullOrEmpty(cmsAdapterURI))
            {
                // setup from Mock
                CaseManagerClient = CmsHelper.CreateMock(Configuration);
            }
            else
            {
                var httpClientHandler = new HttpClientHandler();
                // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                
                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                if (!string.IsNullOrEmpty(Configuration["CMS_ADAPTER_JWT_SECRET"]))
                {
                    var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                    var initialClient = new CaseManager.CaseManagerClient(initialChannel);
                    // call the token service to get a token.
                    var tokenRequest = new CaseManagement.Service.TokenRequest
                    {
                        Secret = Configuration["CMS_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest);

                    if (tokenReply != null && tokenReply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                    }
                }

                var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                CaseManagerClient = new CaseManager.CaseManagerClient(channel);
            }

            flatFileUtils = new FlatFileUtils(Configuration, CaseManagerClient);

            enhancedIcbcApiUtils = new EnhancedIcbcApiUtils(Configuration, CaseManagerClient,null);

            
        }

        public static IFormFile CreateTestFile()
        {
            // Decode the base64 string into bytes
            var fileBytes = Convert.FromBase64String(FileBase64);
            var stream = new MemoryStream(fileBytes);

            // Create the IFormFile
            return new FormFile(stream, 0, stream.Length, "file", "drv-ilsnew-202506110013.dat")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }

        [Fact]
        public void GetUnsentMedicalUpdates()
        {
            var unsentItems = CaseManagerClient.GetUnsentMedicalPass(new CaseManagement.Service.EmptyRequest());
        }

        [Fact]
        public void GetUnsentAdjudications()
        {
            var unsentItems = CaseManagerClient.GetUnsentMedicalAdjudication(new CaseManagement.Service.EmptyRequest());
        }


        [Fact]
        public async void BasicConnectionTest()
        {
            await flatFileUtils.CheckConnection(null);
        }

        [Fact]
        public void ProcessCandidatesTextFile()
        {
            string filename = Configuration["CANDIDATES_TEST_FILE"];

            if (filename != null)
            {
                string data = File.ReadAllText(filename);

                flatFileUtils.ProcessCandidates(null, data);
            }
        }

        [Fact]
        public async void TestCheckForCandidates()
        {
            await flatFileUtils.CheckForCandidates(null);
        }

        [Fact]
        public void TestAddCandidate()
        {
            CLNT client = IcbcClient.GetDriverHistory(Configuration["ICBC_TEST_DL"]);

            LegacyCandidateRequest lcr = new LegacyCandidateRequest()
            {
                LicenseNumber = Configuration["ICBC_TEST_DL"] ?? string.Empty,
                Surname = client?.INAM?.SURN ?? string.Empty,
            };
            CaseManagerClient.ProcessLegacyCandidate(lcr);
        }

        [Fact]
        public void TestGetDriverHistory()
        {
            if(Configuration["ICBC_TEST_DL"] != null)
            {
                CLNT client = IcbcClient.GetDriverHistory(Configuration["ICBC_TEST_DL"]);
                Assert.NotNull(client);
            }           
        }

        [Fact]
        public void TestSendPass()
        {

            if (Configuration["ICBC_TEST_DL"] != null)
            {
                //CLNT client = IcbcClient.GetDriverHistory(Configuration["ICBC_TEST_DL"]);
                var update = new Pssg.Interfaces.IcbcModels.IcbcMedicalUpdate()
                {
                    DlNumber = Configuration["ICBC_TEST_DL"],
                    LastName = "EXP", //client.INAM.SURN ?? string.Empty,
                    MedicalDisposition = "P",
                    MedicalIssueDate = new DateTimeOffset (2023, 1, 13, 0,0,0, TimeSpan.Zero ) //enhancedIcbcApiUtils.GetMedicalIssueDate(client)
                };
                string result = IcbcClient.SendMedicalUpdate(update);
                
                Assert.NotNull(result);
            }

        }

        [Fact]
        public void TestSendFail()
        {

            if (Configuration["ICBC_TEST_DL"] != null)
            {
               CLNT client = IcbcClient.GetDriverHistory(Configuration["ICBC_TEST_DL"]);

                var update = new Pssg.Interfaces.IcbcModels.IcbcMedicalUpdate()
                {
                    DlNumber = Configuration["ICBC_TEST_DL"],
                    LastName = client.INAM.SURN ?? string.Empty,
                    MedicalDisposition = "J",
                    MedicalIssueDate = enhancedIcbcApiUtils.GetMedicalIssueDate(client)
                };

                update = new Pssg.Interfaces.IcbcModels.IcbcMedicalUpdate()
                {
                    DlNumber = Configuration["ICBC_TEST_DL"],
                    LastName = "EXP", //client.INAM.SURN ?? string.Empty,
                    MedicalDisposition = "J",
                    MedicalIssueDate = new DateTimeOffset(2023, 1, 13, 0, 0, 0, TimeSpan.Zero) //enhancedIcbcApiUtils.GetMedicalIssueDate(client)
                };

                string result = IcbcClient.SendMedicalUpdate(update);

                Assert.NotNull(result);
            }

        }

        [Fact]
        public async void TestSendUpdates()
        {
            await flatFileUtils.SendMedicalUpdates(null);
        }

        [Fact]
        public void CheckDriverNewFileFormat()
        {
            var engine = new FileHelperEngine<NewDriver>();
            string sampleData = "2222222022222224EXPERIMENTAL_______________________2012-01-011M2002-02-012004-01-011998-01-012002-04-042002-04-040100";
            var records = engine.ReadString(sampleData);
            Assert.Equal(records[0].LicenseNumber, sampleData.Substring(0,7));
        }

        [Fact]
        public void CheckMedicalUpdateFormat()
        {
            var engine = new FileHelperEngine<MedicalUpdate>();
            string sampleData = "2222222EXPERIMENTAL_______________________P2012-01-01";
            var records = engine.ReadString(sampleData);
            Assert.Equal(records[0].LicenseNumber, sampleData.Substring(0, 7));
        }
        
        [Fact]
        public void FlatCandidateListTest()
        {
            LegacyCandidateRequest lcr = new LegacyCandidateRequest()
            {
                LicenseNumber = Configuration["ICBC_TEST_DL"] ?? string.Empty,
                Surname = Configuration["ICBC_TEST_SURNAME"] ?? string.Empty,
                ClientNumber = String.Empty,
            };
            var result = CaseManagerClient.ProcessLegacyCandidate(lcr);
            Assert.NotNull(result);            
        }

        [Fact]
        public void MedicalStatusPass()
        {
            // create a FlatFilesUtil class.
            var f = new FlatFileUtils(Configuration, CaseManagerClient);

            SearchReply searchReply = new SearchReply();
            var testCase = new DmerCase() { Driver = new Driver() { Surname = "TEST" } };
            testCase.Decisions.Add(
                new DecisionItem () { CreatedOn = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow), Identifier = Guid.NewGuid().ToString(), Outcome = DecisionItem.Types.DecisionOutcomeOptions.FitToDrive  });
            searchReply.Items.Add(testCase);
            var medicalUpdateData = f.GetMedicalUpdateData(searchReply);
            // should be P for Pass
            Assert.Equal("P", medicalUpdateData[0].MedicalDisposition);
        }

        [Fact]
        public void MedicalStatusFail()
        {
            // create a FlatFilesUtil class.
            var f = new FlatFileUtils(Configuration, CaseManagerClient);

            SearchReply searchReply = new SearchReply();
            var testCase = new DmerCase() { Driver = new Driver() { Surname = "TEST" } };
            testCase.Decisions.Add(
                new DecisionItem() { CreatedOn = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow), Identifier = Guid.NewGuid().ToString(), Outcome = DecisionItem.Types.DecisionOutcomeOptions.UnfitToDrive });
            searchReply.Items.Add(testCase);
            var medicalUpdateData = f.GetMedicalUpdateData(searchReply);
            // should be J for Adjudication
            Assert.Equal("J", medicalUpdateData[0].MedicalDisposition);
        }

        [Fact]
        public void MedicalStatusFailPass()
        {
            // create a FlatFilesUtil class.
            var f = new FlatFileUtils(Configuration, CaseManagerClient);

            SearchReply searchReply = new SearchReply();

            var testCase = new DmerCase() { Driver = new Driver() { Surname = "TEST" } };

            testCase.Decisions.Add(
                new DecisionItem() { CreatedOn = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.AddDays(-1)), Identifier = Guid.NewGuid().ToString(), Outcome = DecisionItem.Types.DecisionOutcomeOptions.UnfitToDrive });

            testCase.Decisions.Add(
                new DecisionItem() { CreatedOn = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow), Identifier = Guid.NewGuid().ToString(), Outcome = DecisionItem.Types.DecisionOutcomeOptions.FitToDrive });
            searchReply.Items.Add(testCase);
            var medicalUpdateData = f.GetMedicalUpdateData(searchReply);
            // should be P for Pass, as the Pass Decision is after the Fail.
            Assert.Equal("P", medicalUpdateData[0].MedicalDisposition);
        }

        [Fact]
        public async Task ParseNotifacationFailPassAsync()
        {
           var testRecords= await enhancedIcbcApiUtils.ParseIcbcNotication(CreateTestFile());

            Assert.Equal("01234567", testRecords[0].LNUM);
            Assert.Equal("012345672", testRecords[0].CLNO);
            Assert.Equal("SMITHENE", testRecords[0].SURNAME);
            Assert.Equal("M", testRecords[0].GENDER);
            Assert.Equal("CCC", testRecords[0].CAND_CAUSE_CD);
            Assert.Equal("1996-02-26", testRecords[0].BIRTH_DT);
            Assert.Equal("          ", testRecords[0].LIC_EXPIRY_DT);
            Assert.Equal("2017-05-03", testRecords[0].LAST_EXAM_DT);
            Assert.Equal("2017-05-03", testRecords[0].ADDR_DOCMNT_DT);
            Assert.Equal("1", testRecords[0].MASTER_STATUS_CD);
            Assert.Equal("500", testRecords[0].LIC_CLASS);
            Assert.Equal("2025-06-11", testRecords[0].CAND_SENT_DT);
        }

    }
}
