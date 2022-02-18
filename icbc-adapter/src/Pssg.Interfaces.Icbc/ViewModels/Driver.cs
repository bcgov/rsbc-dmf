namespace Pssg.Interfaces.Icbc.ViewModels
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Driver
    {
        /// <summary>
        /// Initializes a new instance of the Driver class.
        /// </summary>
        public Driver()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CLNT class.
        /// </summary>
        public Driver(string surname = default(string),
            string givenName = default(string),
            string middleName = default(string),
            double? wGHT = default(double?), string sEX = default(string), System.DateTime? bIDT = default(System.DateTime?), double? hGHT = default(double?), string sECK = default(string), 
            string addressLine1 = default(string),
            string city = default(string),
            string postalCode = default(string),
            
            DriverMasterStatus dR1MST = default(DriverMasterStatus))
        {
            Surname = surname;
            GivenName = givenName;
            MiddleName = middleName;
            Weight = wGHT;
            Sex = sEX;
            BirthDate = bIDT;
            Height = hGHT;
            SecurityKeyword = sECK;
            AddressLine1 = addressLine1;
            City = city;
            PostalCode = postalCode;
            
            DriverMasterStatus = dR1MST;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        public string Surname { get; set; }
        public string GivenName { get; set; }

        public string MiddleName { get; set;}

        /// <summary>
        /// </summary>
        
        public double? Weight { get; set; }

        /// <summary>
        /// </summary>
        
        public string Sex { get; set; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(DateJsonConverter))]
        public System.DateTime? BirthDate { get; set; }

        /// <summary>
        /// </summary>        
        public double? Height { get; set; }

        /// <summary>
        /// </summary>        
        public string SecurityKeyword { get; set; }

        public string AddressLine1 { get; set;}
        public string City { get; set;}
        public string PostalCode { get; set;}

        public string Province { get; set; }

        public string Country { get; set; }


        /// <summary>
        /// </summary>        
        public DriverMasterStatus DriverMasterStatus { get; set; }

    }
}
