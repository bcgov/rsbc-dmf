using Moq;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Rsbc.Dmf.CaseManagement.Service;
using System.Threading;

namespace Pssg.Unit.Tests.Interfaces.Icbc.Helpers
{
    public static class CmsHelper
    {
        /// <summary>
        /// Mock for the CMS adapter
        /// See https://docs.microsoft.com/en-us/aspnet/core/grpc/test-client?view=aspnetcore-6.0
        /// </summary>
        /// <returns></returns>
        /// 


        public static CaseManager.CaseManagerClient CreateMock()
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
