

using System;

namespace Rsbc.Dmf.IcbcAdapter.IcbcModels
{
    public class ClientDetails
    {
       public string ClientNumber { get; set; }
       public string Gender { get; set; }
       public string SecurityKeyword { get; set; }
       public DateTime Birthdate { get; set; }
       public double Weight { get; set; }
       public double Height { get; set; }
       public Name Name { get; set; }
       public Address Address { get; set; }
    }
}

