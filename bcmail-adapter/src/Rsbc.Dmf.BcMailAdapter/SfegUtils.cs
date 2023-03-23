using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Service;
using Pssg.DocumentStorageAdapter;
using DocumentFormat.OpenXml.InkML;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Rsbc.Interfaces
{
    /// <summary>
    /// SFEG Utils
    /// </summary>
    public class SfegUtils
    {
       
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;

        /// <summary>
        /// SFEG Utils
        /// </summary>
        /// <param name="caseManagerClient"></param>
        /// <param name="documentStorageAdapterClient"></param>
        public SfegUtils(CaseManager.CaseManagerClient caseManagerClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {
           _caseManagerClient = caseManagerClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
        }

        /// <summary>
        /// Send Documents To BcMail
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultStatusReply> SendDocumentsToBcMail()
        {
            var result = new ResultStatusReply();
            //Step 1: 
            // call cms adpter to get the list of documents in "Send to BC Mail " Status
            var documentsResponse = _caseManagerClient.GetListOfLettersSentToBcMail(new EmptyRequest());


            // Step 2:
            //Is the fileurl is same as the document url?
            //Get the actual pdf documents for the above list from document storage adapter

            foreach (var doc in documentsResponse.Items )
            {
                // get list of documnets
                var fileResult = _documentStorageAdapterClient.DownloadFile(new DownloadFileRequest()
                {
                   ServerRelativeUrl = doc.DocumentUrl
        
                });

                // Step 3
                // After getting the documents put the files on the SFEG Directory

                var sfegResult = true;

                // step 4
                // sucess : call cms adpter and update the status to SEND
                // Failure : Create a BF

                if (sfegResult)
                {
                    _caseManagerClient.UpdateDocumentStatus(new LegacyDocumentStatusRequest());
                }
                else
                {
                    var bringForwardRequest = new BringForwardRequest
                    {
                       // CaseId = unsentItem.CaseId,
                        Subject = "Failed to Send the document to SFEG",
                        //Description = responseContent,
                        Assignee = string.Empty,
                        Priority = BringForwardPriority.Normal

                    };
                    _caseManagerClient.CreateBringForward(bringForwardRequest);
                }

            }
            return result;
        }
    }
}
