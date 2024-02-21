using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class DocumentServiceTests : WebAppTestBase
    {
        private readonly DocumentService _documentService;

        public DocumentServiceTests(ITestOutputHelper output) : base(output)
        {
            var documentTypeManager = services.GetRequiredService<IDocumentTypeManager>();
            var mapper = services.GetRequiredService<IMapper>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            _documentService = new DocumentService(documentTypeManager, mapper, loggerFactory);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task GetDriverDocumentSubTypes()
        {
            var request = new EmptyRequest();

            var queryResults = await _documentService.GetDriverDocumentSubTypes(request, null);

            Assert.NotNull(queryResults);
            queryResults.Items.ShouldNotBeEmpty();
        }
    }
}
