using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Service;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CaseManagerTests : WebAppTestBase
    {
        private readonly ICaseManager caseManager;

        public CaseManagerTests(ITestOutputHelper output) : base(output)
        {
            caseManager = services.GetRequiredService<ICaseManager>();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetFlags()
        {
            var caseId = "222";
            List<Flag> flags = new List<Flag>()
            {
                new Flag(){Description  = "testFlag - 1", Id = "flagTestItem1"},
                new Flag(){Description  = "testFlag - 2", Id = "flagTestItem2"},
            };
            var result = await caseManager.SetCaseFlags(caseId, false, flags, testLogger);
            result.ShouldNotBeNull().Success.ShouldBe(true);

            var actualCase = (await caseManager.CaseSearch(new CaseSearchRequest { CaseId = caseId })).Items.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();

            actualCase.Flags.Count().ShouldBe(flags.Count);
            foreach (var actualFlag in actualCase.Flags)
            {
                var expectedFlag = flags.Where(f => f.Id == actualFlag.Id && f.Description == actualFlag.Description).ShouldHaveSingleItem();
            }
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByCaseId()
        {
            var caseId = "222";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { CaseId = caseId })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            dmerCase.Id.ShouldBe(caseId);

            dmerCase.CreatedBy.ShouldNotBeNullOrEmpty();
            dmerCase.DriverLicenseNumber.ShouldNotBeNullOrEmpty();
            dmerCase.DriverName.ShouldNotBeNullOrEmpty();
            dmerCase.Flags.ShouldNotBeEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByDriverLicense()
        {
            var driverLicenseNumber = "1234567";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;

            queryResults.ShouldNotBeEmpty();
            foreach (var dmerCase in queryResults)
            {
                dmerCase.ShouldBeAssignableTo<DmerCase>().DriverLicenseNumber.ShouldBe(driverLicenseNumber);
            }
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByClinicId()
        {
            var expectedClinicId = "a5a45383-8ff4-eb11-b82b-00505683fbf4";
            var expectedClinicName = "Adam Hancock";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { ClinicId = expectedClinicId })).Items;

            queryResults.ShouldNotBeEmpty();
            foreach (var dmerCase in queryResults)
            {
                dmerCase.ShouldBeAssignableTo<DmerCase>().ClinicId.ShouldBe(expectedClinicId);
                dmerCase.ShouldBeAssignableTo<DmerCase>().ClinicName.ShouldBe(expectedClinicName);
            }
        }


        [Fact(Skip = RequiresDynamics)]
        public async Task CanGetFlags()
        {

            var queryResults = await caseManager.GetAllFlags();

            queryResults.ShouldNotBeEmpty();
            
        }


    }
}