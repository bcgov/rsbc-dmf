using EnumsNET;
using Pssg.SharedUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.SharedUtils
{
    public  class GroupSubmittalStatusUtil
    {
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="submittalStatus"></param>
        /// <returns></returns>
        public static string GroupSubmittalStatus(string submittalStatus)
        {
            var canParseEnum = Enums.TryParse<SubmittalStatus>(submittalStatus, true, out var submittalStatusEnum, EnumFormat.Description);
            if (!canParseEnum)
                return submittalStatus;

            switch (submittalStatusEnum)
            {
                case SubmittalStatus.OpenRequired:
                    return SubmittalStatus.OpenRequired.AsString(EnumFormat.Description);
                case SubmittalStatus.Noncomply:
                case SubmittalStatus.ActionedNoncomply:
                    return SubmittalStatus.Noncomply.AsString(EnumFormat.Description);
                case SubmittalStatus.Received:
                case SubmittalStatus.CleanPass:
                case SubmittalStatus.ManualPass:
                case SubmittalStatus.Reviewed:
                case SubmittalStatus.UnderReview:
                    return SubmittalStatus.Received.AsString(EnumFormat.Description);
                case SubmittalStatus.Rejected:
                    return SubmittalStatus.Rejected.AsString(EnumFormat.Description);
                case SubmittalStatus.Uploaded:
                    return SubmittalStatus.Uploaded.AsString(EnumFormat.Description);
                default:
                    //_logger.LogError($"Error parsing SubmittalStatus: {submittalStatus}");
                    return submittalStatus;
            }
        }
    }
}
