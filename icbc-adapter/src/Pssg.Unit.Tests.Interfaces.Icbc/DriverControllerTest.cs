using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.Controllers;
using Xunit;

namespace Pssg.Unit.Tests.Interfaces.Icbc
{
    public class DriverControllerTest
    {
        [Theory]
        [InlineData("documents", "Unable to get the driver's documents.")]
        [InlineData("driver", "Unable to get the driver.")]
        [InlineData("cases", "Unable to get the driver's active cases.")]
        public void FailedReply_ReturnsProblemWithoutExposingBackendError(string failedLookup, string title)
        {
            var documents = new GetDocumentsReply { ResultStatus = ResultStatus.Success };
            var driver = new GetDriversReply { ResultStatus = ResultStatus.Success };
            driver.Items.Add(new Driver { Id = "driver-id" });
            var cases = new GetCasesReply { ResultStatus = ResultStatus.Success };
            if (failedLookup == "documents")
            {
                documents.ResultStatus = ResultStatus.Fail;
                documents.ErrorDetail = "private backend error";
            }
            else if (failedLookup == "driver")
            {
                driver.ResultStatus = ResultStatus.Fail;
                driver.ErrorDetail = "private backend error";
            }
            else
            {
                cases.ResultStatus = ResultStatus.Fail;
                cases.ErrorDetail = "private backend error";
            }
            var client = CreateClient(documents, driver, cases);
            using var provider = new ServiceCollection().AddSingleton(client.Object).BuildServiceProvider();
            var controller = CreateController(provider);

            var result = Assert.IsType<ObjectResult>(controller.GetDriverMetaandDocuments(new DriverLicenseNumber { driver_licence_number = "12345" }));

            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var problem = Assert.IsType<ProblemDetails>(result.Value);
            Assert.Equal(title, problem.Title);
            Assert.Null(problem.Detail);
        }

        [Fact]
        public void MissingDriver_ReturnsNotFound()
        {
            var client = CreateClient(
                new GetDocumentsReply { ResultStatus = ResultStatus.Success },
                new GetDriversReply { ResultStatus = ResultStatus.Success },
                new GetCasesReply { ResultStatus = ResultStatus.Success });
            using var provider = new ServiceCollection().AddSingleton(client.Object).BuildServiceProvider();

            var result = Assert.IsType<ObjectResult>(CreateController(provider).GetDriverMetaandDocuments(new DriverLicenseNumber { driver_licence_number = "12345" }));

            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            client.Verify(x => x.GetActiveCases(It.IsAny<DriverLicenseRequest>(), null, null, default), Times.Never);
        }

        [Fact]
        public void NoActiveCases_ReturnsDriverAndEmptyDocumentsWithoutAnActiveCase()
        {
            var driver = new GetDriversReply { ResultStatus = ResultStatus.Success };
            driver.Items.Add(new Driver { Id = "driver-id", DriverLicenseNumber = "12345" });
            var client = CreateClient(
                new GetDocumentsReply { ResultStatus = ResultStatus.Success },
                driver,
                new GetCasesReply { ResultStatus = ResultStatus.Success });
            using var provider = new ServiceCollection().AddSingleton(client.Object).BuildServiceProvider();

            var result = Assert.IsType<JsonResult>(CreateController(provider).GetDriverMetaandDocuments(new DriverLicenseNumber { driver_licence_number = "12345" }));

            var response = Assert.IsType<DriverAndDocumentsResponse>(result.Value);
            Assert.Equal("driver-id", response.driver_id);
            Assert.Empty(response.documents);
            Assert.Null(response.activeCase);
            client.Verify(x => x.GetDriverPerson(It.IsAny<DriverLicenseRequest>(), null, null, default), Times.Once);
        }

        [Theory]
        [InlineData("DMER", "Uploaded", true)]
        [InlineData("DMER", "Reviewed", false)]
        [InlineData("Other", "Uploaded", false)]
        [InlineData("Other", "Reviewed", false)]
        [InlineData("", "", false)]
        public void Documents_IncludeOnlyUploadedDmers(string documentType, string status, bool included)
        {
            var documents = new GetDocumentsReply { ResultStatus = ResultStatus.Success };
            documents.Items.Add(new LegacyDocument
            {
                DocumentId = "document-id",
                DocumentType = documentType,
                SubmittalStatus = status
            });
            var driver = new GetDriversReply { ResultStatus = ResultStatus.Success };
            driver.Items.Add(new Driver { Id = "driver-id" });
            var client = CreateClient(documents, driver,
                new GetCasesReply { ResultStatus = ResultStatus.Success });
            using var provider = new ServiceCollection().AddSingleton(client.Object).BuildServiceProvider();

            var result = Assert.IsType<JsonResult>(CreateController(provider).GetDriverMetaandDocuments(new DriverLicenseNumber { driver_licence_number = "12345" }));

            var response = Assert.IsType<DriverAndDocumentsResponse>(result.Value);
            if (included)
            {
                var document = Assert.Single(response.documents);
                Assert.Equal("document-id", document.DocumentRecordId);
                Assert.Equal("DMER", document.DocumentType);
                Assert.Equal("Uploaded", document.SubmittalStatus);
            }
            else
            {
                Assert.Empty(response.documents);
            }
        }

        private static Mock<CaseManager.CaseManagerClient> CreateClient(
            GetDocumentsReply documents, GetDriversReply driver, GetCasesReply cases)
        {
            var client = new Mock<CaseManager.CaseManagerClient>();
            client.Setup(x => x.GetDriverDocuments(It.IsAny<DriverLicenseRequest>(), null, null, default))
                .Returns(documents);
            client.Setup(x => x.GetDriverPerson(It.IsAny<DriverLicenseRequest>(), null, null, default))
                .Returns(driver);
            client.Setup(x => x.GetActiveCases(It.IsAny<DriverLicenseRequest>(), null, null, default))
                .Returns(cases);
            return client;
        }

        private static DriverController CreateController(System.IServiceProvider provider)
        {
            return new DriverController(new ConfigurationBuilder().Build(),
                NullLogger<DmersController>.Instance,
                provider.GetRequiredService<IServiceScopeFactory>());
        }
    }
}
