using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter.Models
{
    public class Driver
    {
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public double Weight { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public double Height { get; set; }
        public Address Address { get; set; }
        public string DriverLicenceNumber { get; set; }
    }
}
