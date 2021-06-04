using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter.ViewModels
{
    public class SmartConfiguration
    {
        public string Authorization_endpoint { get; set; }
        public string Token_endpoint { get; set; }
        public string Introspection_endpoint { get; set; }
        public List<string> Capabilities { get; set; }
    }
}
