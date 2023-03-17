using System;

namespace Pssg.Interfaces.IcbcModels
{
    public class Address
    {
        public string BuildingUnitNumber { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string StreetDirection { get; set; }
        public string Site { get; set; }
        public string Comp { get; set; }
        public string RuralRoute { get; set; }
        public string City { get; set; }
        public string ProvinceOrState { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PostOfficeBox { get; set;}

        public string AddressPrefix1 { get; set; }
        public string AddressPrefix2 { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

}