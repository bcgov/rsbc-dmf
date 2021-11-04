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
        public async Task CanSetFlagsAndSearchById()
        {
            var title = "222";
            // first do a search to get this case by title.
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            var caseId = dmerCase.Id;

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
        public async Task CanQueryCasesByTitle()
        {
            var title = "222";
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var dmerCase = queryResults.ShouldHaveSingleItem().ShouldBeAssignableTo<DmerCase>();
            dmerCase.Title.ShouldBe(title);

            dmerCase.CreatedBy.ShouldNotBeNullOrEmpty();
            dmerCase.Driver.DriverLicenceNumber.ShouldNotBeNullOrEmpty();
            dmerCase.Driver.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByDriverLicense()
        {
            var driverLicenseNumber = "1234567";

            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { DriverLicenseNumber = driverLicenseNumber })).Items;

            queryResults.ShouldNotBeEmpty();
            foreach (var dmerCase in queryResults)
            {
                dmerCase.ShouldBeAssignableTo<DmerCase>().Driver.DriverLicenceNumber.ShouldBe(driverLicenseNumber);
            }
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanQueryCasesByClinicId()
        {
            var title = "222";
            var queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { Title = title })).Items;

            var testItem = queryResults.First().ShouldBeAssignableTo<DmerCase>();

            var expectedClinicId = testItem.ClinicId;

            queryResults = (await caseManager.CaseSearch(new CaseSearchRequest { ClinicId = expectedClinicId })).Items;

            queryResults.ShouldNotBeEmpty();
            foreach (var dmerCase in queryResults)
            {
                dmerCase.ShouldBeAssignableTo<DmerCase>().ClinicId.ShouldBe(expectedClinicId);
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