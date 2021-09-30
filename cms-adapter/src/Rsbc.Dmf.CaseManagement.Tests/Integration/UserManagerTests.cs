using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class UserManagerTests : WebAppTestBase
    {
        private readonly IUserManager userManager;

        public UserManagerTests(ITestOutputHelper output) : base(output)
        {
            userManager = services.GetRequiredService<IUserManager>();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSearchUsersByBcscExternalId()
        {
            var userId = "d5068e96-3a04-ec11-b82d-00505683fbf4";
            var system = "bcsc";
            var result = await userManager.SearchUsers(new SearchUsersRequest { ByType = UserType.MedicalPractitioner, ByExternalUserId = (userId, system) });

            result.Items.ShouldNotBeEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSearchUsersByBceidExternalId()
        {
            var userId = "b59ea4a2-3a04-ec11-b82d-00505683fbf4";
            var system = "bceid";
            var result = await userManager.SearchUsers(new SearchUsersRequest { ByType = UserType.Driver, ByExternalUserId = (userId, system) });

            result.Items.ShouldNotBeEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanLoginNewDriver()
        {
            var userId = Guid.NewGuid().ToString();
            var system = "bcsc";

            var newDriver = new DriverUser
            {
                ExternalSystem = system,
                ExternalSystemUserId = userId,
                FirstName = "test driver",
                LastName = "integration test",
            };

            var driverId = (await userManager.LoginUser(new LoginUserRequest { User = newDriver })).Userid;

            var result = await userManager.SearchUsers(new SearchUsersRequest { ByType = UserType.Driver, ByExternalUserId = (userId, system) });

            var actualDriver = result.Items.ShouldHaveSingleItem().ShouldBeAssignableTo<DriverUser>();

            actualDriver.Id.ShouldBe(driverId);
            actualDriver.ExternalSystem.ShouldBe(system, StringCompareShould.IgnoreCase);
            actualDriver.ExternalSystemUserId.ShouldBe(userId);
            actualDriver.FirstName.ShouldBe(newDriver.FirstName);
            actualDriver.FirstName.ShouldBe(newDriver.FirstName);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanLoginNewMedicalPractitioner()
        {
            var userId = Guid.NewGuid().ToString();
            var system = "bcsc";

            var newMedicalPractitioner = new MedicalPractitionerUser
            {
                ExternalSystem = system,
                ExternalSystemUserId = userId,
                FirstName = "test med pro",
                LastName = "integration test",
                ClinicAssignments = new[]
                {
                    new ClinicAssignment{Clinic = new Clinic { Id = "3bec7901-541d-ec11-b82d-00505683fbf4" }, Roles = new [] { "Physician" } }
                }
            };

            var medicalPractitionerId = (await userManager.LoginUser(new LoginUserRequest { User = newMedicalPractitioner })).Userid;

            var result = await userManager.SearchUsers(new SearchUsersRequest { ByType = UserType.MedicalPractitioner, ByExternalUserId = (userId, system) });

            var actualMedicalPractitioner = result.Items.ShouldHaveSingleItem().ShouldBeAssignableTo<MedicalPractitionerUser>();

            actualMedicalPractitioner.Id.ShouldBe(medicalPractitionerId);
            actualMedicalPractitioner.ExternalSystemUserId.ShouldBe(userId);
            actualMedicalPractitioner.ExternalSystem.ShouldBe(system, StringCompareShould.IgnoreCase);
            actualMedicalPractitioner.FirstName.ShouldBe(newMedicalPractitioner.FirstName);
            actualMedicalPractitioner.FirstName.ShouldBe(newMedicalPractitioner.FirstName);
            var actualClinicAssignment = actualMedicalPractitioner.ClinicAssignments.ShouldHaveSingleItem();
            actualClinicAssignment.Roles.ShouldHaveSingleItem().ShouldBe(newMedicalPractitioner.ClinicAssignments.First().Roles.First());
            actualClinicAssignment.Clinic.ShouldNotBeNull();
            actualClinicAssignment.Clinic.Id.ShouldBe(newMedicalPractitioner.ClinicAssignments.First().Clinic.Id);
            actualClinicAssignment.Clinic.Name.ShouldNotBeNull();
        }
    }
}