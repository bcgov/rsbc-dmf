using Moq;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using Rsbc.Interfaces;
using Rsbc.Interfaces.CdgsModels;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using Grpc.Core;

namespace Rsbc.Dmf.BcMailAdapter.Tests.Helpers
{
    public static class CdgsClientHelper
    {
        public static ICdgsClient CreateMock(IConfiguration configuration)
        {

            var mockClient = new Mock<ICdgsClient>();

            var theStream = new MemoryStream(Encoding.UTF8.GetBytes(""));

            mockClient
                .Setup(m => m.TemplateRender(It.IsAny<CdgsRequest>()))
                .Returns(Task.FromResult<Stream>((Stream)theStream));                           

            return mockClient.Object;
        }

    }
}

