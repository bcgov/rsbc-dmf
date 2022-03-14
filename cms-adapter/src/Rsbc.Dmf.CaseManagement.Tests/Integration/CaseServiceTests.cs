using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using Rsbc.Dmf.CaseManagement.Service;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CaseServiceTests : WebAppTestBase
    {
        private readonly CaseService caseService;


        public CaseServiceTests(ITestOutputHelper output) : base(output)
        {
            var logger = services.GetRequiredService<ILogger<CaseService>>();
            var caseManager = services.GetRequiredService<ICaseManager>();
            caseService =
                new CaseService(logger, caseManager, configuration); 
        }
        

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByStatusPending()
        {
            var expectedClinicId = "a5a45383-8ff4-eb11-b82b-00505683fbf4";
            var request = new SearchRequest()
            {
                ClinicId = expectedClinicId
            };
            request.Statuses.Add("Pending");

            var queryResults = (await caseService.Search(request, null)).Items;

            queryResults.ShouldNotBeEmpty();
            
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetFlags()
        {
            var request = new EmptyRequest();

            var queryResults = (await caseService.GetAllFlags(request, null)).Flags;

            queryResults.ShouldNotBeEmpty();

        }
    }
}
