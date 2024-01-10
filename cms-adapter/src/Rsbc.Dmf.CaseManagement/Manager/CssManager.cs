using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface ICssManager
    {
        Task<string> GetCss(Guid id);

    }

    internal class CssManager : ICssManager
    {
        private readonly DynamicsContext dynamicsContext;

        public CssManager(DynamicsContext dynamicsContext)
        {
            this.dynamicsContext = dynamicsContext;
        }

        public async Task<string> GetCss(Guid id)
        {
            string result = null;

            try
            {
                var cssObject = dynamicsContext.dfp_cssfiles.ByKey(id).GetValue();
                result = cssObject.dfp_css;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "GetCss - Unexpected error retrieving CSS");
                result = null;
            }
            return result;
            
        }
    }
}