using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Moq;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Threading;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    public static class CmsHelper
    {
        /// <summary>
        /// Mock for the CMS adapter
        /// See https://docs.microsoft.com/en-us/aspnet/core/grpc/test-client?view=aspnetcore-6.0
        /// </summary>
        /// <returns></returns>
        /// 


        public static CaseManager.CaseManagerClient CreateMock(IConfiguration configuration)
        {

            var mockDriverResult = new GetDriversReply
            {
                ResultStatus = ResultStatus.Success
            };
            for (int i = 0; i < 55; i++)
            {
                mockDriverResult.Items.Add(new Driver() { DriverLicenseNumber = "2222222" });
            }

            var mockClient = new Mock<CaseManager.CaseManagerClient>();

            mockClient
                .Setup(m => m.Search(It.IsAny<SearchRequest>(), null, null, CancellationToken.None))
                .Returns<SearchRequest, Metadata, DateTime?, CancellationToken> ((a,b,c,d) => {
                    SearchReply reply = new() { ResultStatus = ResultStatus.Success };

                    if (!string.IsNullOrEmpty(a.CaseId))
                    {
                        Driver driver = new Driver { DriverLicenseNumber = configuration["ICBC_TEST_DL"], Surname = configuration["ICBC_TEST_SURCODE"] ?? string.Empty };
                        reply.Items.Add(new DmerCase() { CaseId = a.CaseId , Driver = driver});
                    }
                    else if (a.DriverLicenseNumber != null)
                    {
                        Driver driver = new Driver {  DriverLicenseNumber = a.DriverLicenseNumber, Surname = configuration["ICBC_TEST_SURCODE"] ?? string.Empty };
                        reply.Items.Add(new DmerCase() { CaseId = Guid.NewGuid().ToString(), Driver = driver });
                    }
                    return reply;
                });
            mockClient
                .Setup(m => m.CreateLegacyCaseDocument(It.IsAny<LegacyDocument>(), null, null, CancellationToken.None))
                .Returns<LegacyDocument, Metadata, DateTime?, CancellationToken>((a, b, c, d) => {
                    CreateStatusReply reply = new() { ResultStatus = ResultStatus.Success };
                    return reply;
                });  

            mockClient
                .Setup(m => m.GetCaseDocuments(It.IsAny<CaseIdRequest>(), null, null, CancellationToken.None))
                .Returns<CaseIdRequest, Metadata, DateTime?, CancellationToken>((a, b, c, d) => {
                    GetDocumentsReply reply = new() { ResultStatus = ResultStatus.Success };
                    Driver driver = new Driver { DriverLicenseNumber = configuration["ICBC_TEST_DL"], Surname = configuration["ICBC_TEST_SURCODE"] ?? string.Empty };
                    reply.Items.Add(new LegacyDocument() {  CaseId = Guid.NewGuid().ToString(), 
                                                            Driver = driver,
                                                            FaxReceivedDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                                                            ImportDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                                                            DocumentId = Guid.NewGuid().ToString(),
                                                            SequenceNumber = 1,
                                                            UserId = "TESTUSER"
                    });

                    return reply;
                });

            mockClient
                .Setup(m => m.GetDriverDocuments(It.IsAny<DriverLicenseRequest>(), null, null, CancellationToken.None))
                .Returns<DriverLicenseRequest, Metadata, DateTime?, CancellationToken>((a, b, c, d) => {
                    GetDocumentsReply reply = new() { ResultStatus = ResultStatus.Success };
                    Driver driver = new Driver { DriverLicenseNumber = configuration["ICBC_TEST_DL"], Surname = configuration["ICBC_TEST_SURCODE"] ?? string.Empty };
                    reply.Items.Add(new LegacyDocument()
                    {
                        CaseId = Guid.NewGuid().ToString(),
                        Driver = driver,
                        FaxReceivedDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                        ImportDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                        DocumentId = Guid.NewGuid().ToString(),
                        SequenceNumber = 1,
                        UserId = "TESTUSER"
                    });

                    return reply;
                });

            mockClient
                .Setup(m => m.GetDriverComments(It.IsAny<DriverLicenseRequest>(), null, null, CancellationToken.None))
                .Returns<DriverLicenseRequest, Metadata, DateTime?, CancellationToken>((a, b, c, d) => {
                    GetCommentsReply reply = new() { ResultStatus = ResultStatus.Success };
                    Driver driver = new Driver { DriverLicenseNumber = configuration["ICBC_TEST_DL"], Surname = configuration["ICBC_TEST_SURCODE"] ?? string.Empty };
                    reply.Items.Add(new LegacyComment()
                    { Driver = driver, 
                      CaseId = Guid.NewGuid().ToString(),
                      CommentDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now) });

                    return reply;
                });

            



            mockClient
                .Setup(m => m.GetCaseComments(It.IsAny<CaseIdRequest>(), null, null, CancellationToken.None))
                .Returns<CaseIdRequest, Metadata, DateTime?, CancellationToken>((a, b, c, d) => {
                    GetCommentsReply reply = new() { ResultStatus = ResultStatus.Success };
                    Driver driver = new Driver { DriverLicenseNumber = configuration["ICBC_TEST_DL"], Surname = configuration["ICBC_TEST_SURCODE"] ?? string.Empty };

                    if (a.CaseId != null)
                    {
                        reply.Items.Add(new LegacyComment() { Driver = driver, CaseId = a.CaseId, CommentDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now) });
                    }
                    return reply;
                });

            mockClient
                .Setup(m => m.CreateLegacyCaseComment(It.IsAny<LegacyComment>(), null, null, CancellationToken.None))
                .Returns<LegacyComment, Metadata, DateTime?, CancellationToken>((a, b, c, d) => {
                    CreateStatusReply reply = new() { ResultStatus = ResultStatus.Success };
                    
                    return reply;
                });

            

            mockClient
                .Setup(m => m.GetDrivers(
                    It.IsAny<EmptyRequest>(), null, null, CancellationToken.None))
                .Returns(mockDriverResult);

            mockClient
                .Setup(m => m.GetUnsentMedicalUpdates(
                    It.IsAny<EmptyRequest>(), null, null, CancellationToken.None))
                .Returns(new SearchReply { ResultStatus = ResultStatus.Success });

            mockClient
                .Setup(m => m.ProcessLegacyCandidate(
                    It.IsAny<LegacyCandidateRequest>(), null, null, CancellationToken.None))
                .Returns(new LegacyCandidateReply { ResultStatus = ResultStatus.Success });

            
            return mockClient.Object;
        }
    }
}
