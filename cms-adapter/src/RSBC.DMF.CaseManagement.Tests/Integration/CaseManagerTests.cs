using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Service;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CaseManagerTests : WebAppTestBase
    {
        private readonly ICaseManager caseManager;

        public CaseManagerTests(ITestOutputHelper output, WebApplicationFactory<Startup> webApplicationFactory) : base(output, webApplicationFactory)
        {
            caseManager = services.GetRequiredService<ICaseManager>();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetFlags()
        {
            List<Flag> flags = new List<Flag>()
            {
                new Flag(){Description  = "testFlag - 1", Id = "flagTestItem1"},
                new Flag(){Description  = "testFlag - 2", Id = "flagTestItem2"},
            };
            var result = await caseManager.SetCaseFlags("222", false, flags, testLogger);
            result.ShouldNotBeNull().Success.ShouldBe(true);
        }

        [Fact]
        public async Task CanQueryCasesByCaseId()
        {
            var caseId = "222";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { CaseId = caseId })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            dmerCase.Id.ShouldBe(caseId);
            //TODO: dmerCase.CreatedBy.ShouldNotBeNullOrEmpty();
            dmerCase.DriverLicenseNumber.ShouldNotBeNullOrEmpty();
            dmerCase.DriverName.ShouldNotBeNullOrEmpty();
            dmerCase.Flags.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task CanQueryCasesByDriverLicense()
        {
            var driverLicenseNumber = "1234567";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            dmerCase.Id.ShouldBe(driverLicenseNumber);
            //TODO: dmerCase.CreatedBy.ShouldNotBeNullOrEmpty();
            dmerCase.DriverLicenseNumber.ShouldNotBeNullOrEmpty();
            dmerCase.DriverName.ShouldNotBeNullOrEmpty();
            dmerCase.Flags.ShouldNotBeEmpty();
        }
    }
}