using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.LegacyAdapter
{
    public class CommentUtils
    {
        private readonly IConfiguration _configuration;

        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        
        CommentUtils(IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient)
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;         
        }

        /// <summary>
        /// Add Comment to a given driver.
        /// </summary>
        /// <param name="dlNumber"></param>
        /// <param name="comment"></param>
        public void AddComment(string dlNumber, ViewModels.Comment comment)
        {
            // start by checking to see if we have a driver for the given comment.
            var searchResult = _cmsAdapterClient.Search (new SearchRequest() { DriverLicenseNumber = dlNumber });

            if (searchResult.ResultStatus == ResultStatus.Success)
            {
                foreach ( var item in searchResult.Items)
                {
                    if (item.CaseId == comment.CaseId)
                    {
                        // match is found, add the comment.
                        
                        //_cmsAdapterClient.CreateCaseComment(new );
                    }
                }
            }


        }

    }
}
