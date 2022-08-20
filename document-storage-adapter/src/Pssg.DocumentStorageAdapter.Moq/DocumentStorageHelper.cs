using Castle.Core.Configuration;
using Moq;
using Pssg.DocumentStorageAdapter;
using System.Threading;


namespace Pssg.DocumentStorageAdapter.Helpers
{
    public static class DocumentStorageHelper
    {
        /// <summary>
        /// Mock for the CMS adapter
        /// See https://docs.microsoft.com/en-us/aspnet/core/grpc/test-client?view=aspnetcore-6.0
        /// </summary>
        /// <returns></returns>
        /// 


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