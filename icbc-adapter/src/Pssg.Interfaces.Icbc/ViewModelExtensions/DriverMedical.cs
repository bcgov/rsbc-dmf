using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.Interfaces.ViewModelExtensions
{
   
    public static class DriverMedicalExtensions
    {
        public static DriverMedical ToViewModel (this DR1MEDNITEM item)
        {
            DriverMedical result = null;

            if (item != null)
            {
                result = new DriverMedical()
                {
                    IssuingOffice = item.ISOF,
                    IssuingOfficeDescription = item.ISOFDESC,
                    MedicalIssueDate = item.MIDT,
                    MedicalExamDate = item.MEDT,
                    MedicalDisposition = item.MDSP,
                    MedicalDispositionDescription = item.MDSPDESC,
                    PGN1 = item.PGN1,
                    PGN2 = item.PGN2
                };
            }
            return result;

                
        }
    }
    
}
