using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Moq;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Threading;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    public static class DocumentStorageHelper
    {
        /// <summary>
        /// Mock for the CMS adapter
        /// See https://docs.microsoft.com/en-us/aspnet/core/grpc/test-client?view=aspnetcore-6.0
        /// </summary>
        /// <returns></returns>
        /// 
        public static string DEFAULT_DL = "2222222";
        public static string DEFAULT_SURCODE = "TST";

        public static DocumentStorageAdapter.DocumentStorageAdapterClient CreateMock(IConfiguration configuration)
        {

            var mockClient = new Mock<DocumentStorageAdapter.DocumentStorageAdapterClient>();

            /*
            mockClient
                .Setup(m => m.GetDriverComments(It.IsAny<DriverLicenseRequest>(), null, null, CancellationToken.None))
                .Returns<DriverLicenseRequest, Metadata, DateTime?, CancellationToken>((a, b, c, d) =>
                {
                    GetCommentsReply reply = new() { ResultStatus = ResultStatus.Success };
                    Driver driver = new Driver
                    {
                        DriverLicenseNumber = configuration["ICBC_TEST_DL"] ?? DEFAULT_DL,
                        Surname = configuration["ICBC_TEST_SURCODE"] ?? DEFAULT_SURCODE
                    }; 
                    reply.Items.Add(new LegacyComment()
                    {
                        Driver = driver,
                        CaseId = Guid.NewGuid().ToString(),
                        CommentDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now)
                    });

                    return reply;
                });

            */



            return mockClient.Object;
        }
    }
}
