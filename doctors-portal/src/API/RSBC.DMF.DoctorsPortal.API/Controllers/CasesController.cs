using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSBC.DMF.DoctorsPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private Case[] cases = new[]
        {
            new Case{ Id = "123123-DFC0100", DriverLicense = "0200700", Status="Pending", PatientName = "patient 1", CreatedOn = DateTime.Parse("01/10/2021"), ModifiedOn = DateTime.Parse("01/10/2021"), ModifiedBy = "user1" },
            new Case{ Id = "222", DriverLicense = "222", Status="Completed", PatientName = "patient 2", CreatedOn = DateTime.Parse("01/20/2021"), ModifiedOn = DateTime.Parse("01/20/2021"), ModifiedBy = "user1" },
            new Case{ Id = "333", DriverLicense = "333", Status="Expired", PatientName = "patient 3", CreatedOn = DateTime.Parse("02/03/2021"), ModifiedOn = DateTime.Parse("02/03/2021"), ModifiedBy = "user2" },
            new Case{ Id = "444", DriverLicense = "444", Status="Pending", PatientName = "patient 4", CreatedOn = DateTime.Parse("02/28/2021"), ModifiedOn = DateTime.Parse("02/28/2021"), ModifiedBy = "user3" },
            new Case{ Id = "555", DriverLicense = "111", Status="Completed", PatientName = "patient 1", CreatedOn = DateTime.Parse("03/05/2021"), ModifiedOn = DateTime.Parse("03/05/2021"), ModifiedBy = "user4" },
            new Case{ Id = "666", DriverLicense = "222", Status="Expired", PatientName = "patient 2", CreatedOn = DateTime.Parse("04/18/2021"), ModifiedOn = DateTime.Parse("04/18/2021"), ModifiedBy = "user1" },
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Case>>> GetCases([FromQuery] SearchCases searchParameters)
        {
            await Task.CompletedTask;
            var searchQuery = cases.AsEnumerable();
            if (searchParameters.ByCaseId != null) searchQuery = searchQuery.Where(c => c.Id == searchParameters.ByCaseId);
            if (searchParameters.ByDriverLicense != null) searchQuery = searchQuery.Where(c => c.DriverLicense == searchParameters.ByDriverLicense);
            if (searchParameters.ByPatientName != null) searchQuery = searchQuery.Where(c => c.PatientName.Equals(searchParameters.ByPatientName, StringComparison.InvariantCultureIgnoreCase));
            if (searchParameters.ByStatus != null && searchParameters.ByStatus.Any()) searchQuery = searchQuery.Where(c => searchParameters.ByStatus.Contains(c.Status));
            return Ok(searchQuery.ToArray());
        }

        public record SearchCases
        {
            public string ByCaseId { get; set; }
            public string ByPatientName { get; set; }
            public string ByDriverLicense { get; set; }
            public IEnumerable<string> ByStatus { get; set; }
        }

        public record Case
        {
            public string Id { get; set; }
            public string DriverLicense { get; set; }
            public string PatientName { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CreatedBy { get; set; }
            public DateTime ModifiedOn { get; set; }
            public string ModifiedBy { get; set; }
            public string Status { get; set; }
        }
    }
}