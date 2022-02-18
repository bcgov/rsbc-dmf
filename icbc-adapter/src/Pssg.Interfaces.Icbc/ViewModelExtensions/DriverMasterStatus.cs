using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.Interfaces.ViewModelExtensions
{

    public static class DriverMasterStatusExtensions
    {
        public static DriverMasterStatus ToViewModel(this DR1MST item)
        {
            DriverMasterStatus result = null;

            if (item != null)
            {
                result = new DriverMasterStatus()
                {
                    MasterStatusCode = item.MSCD,                    
                    LicenceExpiryDate = item.RRDT,
                    LicenceNumber = item.LNUM,
                    LicenceClass = item.LCLS
                };

                if (item.RSCD != null)
                {
                    result.RestrictionCodes = item.RSCD;
                }

                if (item.DR1STAT != null)
                {
                    result.DriverStatus = new List<DriverStatus>();
                    foreach (var stat in item.DR1STAT)
                    {
                        result.DriverStatus.Add( new DriverStatus()
                        {
                            EffectiveDate = stat.EFDT,
                            NewExpandedStatus = stat.NECD,
                            NewMasterStatusCode = stat.NMCD,
                            Section = stat.SECT,
                            StatusReviewDate = stat.SRDT
                        });
                    }                    
                }

                if (item.DR1MEDN != null)
                {
                    result.DriverMedicals = new List<DriverMedical>();
                    foreach (var medItem in item.DR1MEDN)
                    {
                        result.DriverMedicals.Add(medItem.ToViewModel());
                    }
                }
            }
            return result;
        }
    }

}
