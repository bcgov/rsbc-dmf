using AutoMapper;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public DocumentServiceTests(ITestOutputHelper output) : base(output)
        {
            var documentTypeManager = services.GetRequiredService<IDocumentTypeManager>();
            var mapper = services.GetRequiredService<IMapper>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            _configuration = services.GetRequiredService<IConfiguration>();
            _documentService = new DocumentService(documentTypeManager, mapper, _configuration, loggerFactory);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task Get_Driver_DocumentSubTypes()
        {
            if (_configuration["DRIVER_DOCUMENT_TYPE_CODE"] != null)
            {
                return;
            }

            var request = new DocumentTypeRequest();
            request.DocumentTypeCode = _configuration["DRIVER_DOCUMENT_TYPE_CODE"];
            var response = await _documentService.GetDocumentSubTypes(request, null);

            Assert.NotNull(response);
            response.Items.ShouldNotBeEmpty();
        }
    }
}
