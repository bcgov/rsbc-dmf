using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.IcbcAdapter.Controllers;
using Xunit;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public class DmerDownloadTest
    {
        [Theory]
        [InlineData("folder/a file+#.pdf", "application/pdf", "a file+#.pdf")]
        [InlineData("folder/document.unknown", "application/octet-stream", "document.unknown")]
        public async Task DownloadReturnsFileUsingExistingClient(string fileUrl, string contentType, string fileName)
        {
            var data = new byte[] { 0, 1, 2, 128, 255 };
            var storage = CreateStorageClient(new DownloadFileReply
            {
                ResultStatus = Pssg.DocumentStorageAdapter.ResultStatus.Success,
                Data = ByteString.CopyFrom(data)
            });
            using var server = CreateServer(storage.Object);
            using var client = server.CreateClient();
            using var request = CreateRequest(new JObject { ["fileUrl"] = fileUrl }.ToString());

            using var response = await client.PostAsync("/dmers/download", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(data, await response.Content.ReadAsByteArrayAsync());
            Assert.Equal(contentType, response.Content.Headers.ContentType.MediaType);
            Assert.Equal("attachment", response.Content.Headers.ContentDisposition.DispositionType);
            Assert.Equal(fileName, response.Content.Headers.ContentDisposition.FileNameStar);
            storage.Verify(client => client.DownloadFileAsync(
                It.Is<DownloadFileRequest>(request => request.ServerRelativeUrl == fileUrl),
                null, null, It.Is<CancellationToken>(token => token.CanBeCanceled)), Times.Once);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"fileUrl\":null}")]
        [InlineData("{\"fileUrl\":\"\"}")]
        [InlineData("{\"fileUrl\":\"   \"}")]
        public async Task DownloadRejectsMissingOrBlankPaths(string body)
        {
            var storage = new Mock<DocumentStorageAdapter.DocumentStorageAdapterClient>(MockBehavior.Strict);
            using var server = CreateServer(storage.Object);
            using var client = server.CreateClient();
            using var request = CreateRequest(body);

            using var response = await client.PostAsync("/dmers/download", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            storage.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DownloadReportsStorageFailure()
        {
            var storage = CreateStorageClient(new DownloadFileReply { ResultStatus = Pssg.DocumentStorageAdapter.ResultStatus.Fail });
            using var server = CreateServer(storage.Object);
            using var client = server.CreateClient();
            using var request = CreateRequest("{\"fileUrl\":\"folder/dmer.pdf\"}");

            using var response = await client.PostAsync("/dmers/download", request);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [InlineData(StatusCode.Unavailable, HttpStatusCode.ServiceUnavailable)]
        [InlineData(StatusCode.DeadlineExceeded, HttpStatusCode.GatewayTimeout)]
        [InlineData(StatusCode.Internal, HttpStatusCode.BadGateway)]
        public async Task DownloadReportsGrpcFailure(StatusCode grpcStatus, HttpStatusCode expectedStatus)
        {
            var storage = new Mock<DocumentStorageAdapter.DocumentStorageAdapterClient>();
            storage.Setup(client => client.DownloadFileAsync(
                It.IsAny<DownloadFileRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(CreateCall(Task.FromException<DownloadFileReply>(
                    new RpcException(new Status(grpcStatus, "Internal storage details")))));
            using var server = CreateServer(storage.Object);
            using var client = server.CreateClient();
            using var request = CreateRequest("{\"fileUrl\":\"folder/dmer.pdf\"}");

            using var response = await client.PostAsync("/dmers/download", request);

            Assert.Equal(expectedStatus, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.DoesNotContain("Internal storage details", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SwaggerIncludesDownloadEndpoint()
        {
            var storage = new Mock<DocumentStorageAdapter.DocumentStorageAdapterClient>();
            using var server = CreateServer(storage.Object);
            using var client = server.CreateClient();

            using var response = await client.GetAsync("/swagger/v1/swagger.json");

            response.EnsureSuccessStatusCode();
            var document = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.NotNull(document["paths"]?["/Dmers/download"]?["post"]);
        }

        private static StringContent CreateRequest(string body) => new StringContent(body, Encoding.UTF8, "application/json");

        private static Mock<DocumentStorageAdapter.DocumentStorageAdapterClient> CreateStorageClient(DownloadFileReply reply)
        {
            var storage = new Mock<DocumentStorageAdapter.DocumentStorageAdapterClient>();
            storage.Setup(client => client.DownloadFileAsync(
                It.IsAny<DownloadFileRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(CreateCall(Task.FromResult(reply)));
            return storage;
        }

        private static AsyncUnaryCall<DownloadFileReply> CreateCall(Task<DownloadFileReply> response)
        {
            return new AsyncUnaryCall<DownloadFileReply>(response, Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
        }

        private static TestServer CreateServer(DocumentStorageAdapter.DocumentStorageAdapterClient storage)
        {
            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(storage);
                    services.AddControllers(options =>
                        {
                            options.EnableEndpointRouting = false;
                            options.Filters.Add(new AllowAnonymousFilter());
                        })
                        .AddApplicationPart(typeof(DmersController).Assembly);
                    services.AddSwaggerGen(options => options.EnableAnnotations());
                })
                .Configure(app =>
                {
                    app.UseSwagger();
                    app.UseMvc();
                }));
        }
    }
}
