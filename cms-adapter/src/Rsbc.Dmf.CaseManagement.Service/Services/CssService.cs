using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class CssService : CssManager.CssManagerBase
    {
        private readonly ICssManager cssManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cssManager"></param>
        public CssService(ICssManager cssManager)
        {
            this.cssManager = cssManager;
        }

        /// <summary>
        /// Fetch a CSS from Dynamics
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CssReply> GetCss(CssRequest request, ServerCallContext context)
        {
            CssReply result = new CssReply();
            result.ResultStatus = ResultStatus.Fail;
            try
            {
                Guid id = Guid.Parse(request.Id);

                result.Css = await cssManager.GetCss(id);
                
                result.ResultStatus = ResultStatus.Success;                
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }
            
            return result;
        }
    }
}