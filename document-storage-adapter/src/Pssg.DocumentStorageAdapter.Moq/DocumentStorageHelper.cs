
using Grpc.Core;
using Moq;
using Pssg.DocumentStorageAdapter;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;


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

            mockClient
                .Setup (m => m.UploadFile(It.IsAny<UploadFileRequest>(),null,null, CancellationToken.None))
                .Returns<UploadFileRequest, Metadata, DateTime ?, CancellationToken > ((x, b, c, d) => 
                {
                    UploadFileReply reply = new UploadFileReply()
                    {
                         ResultStatus = ResultStatus.Success,
                         FileName = x.FileName
                    };
                    return reply;
                });

            
            mockClient
                .Setup(m => m.DownloadFile(It.IsAny<DownloadFileRequest>(), null, null, CancellationToken.None))
                .Returns<DownloadFileRequest, Metadata, DateTime?, CancellationToken>((x, b, c, d) =>
                {
                    DownloadFileReply reply = new DownloadFileReply()
                    {
                        ResultStatus = ResultStatus.Success,
                        
                    };
                    return reply;
                });
            



            return mockClient.Object;
        }
    }
}