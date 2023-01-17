using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.IcbcModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.Interfaces
{
    public interface IIcbcClient
    {
        public CLNT GetDriverHistory(string dlNumber);

        public string SendMedicalUpdate(IcbcMedicalUpdate item);
    }
}
