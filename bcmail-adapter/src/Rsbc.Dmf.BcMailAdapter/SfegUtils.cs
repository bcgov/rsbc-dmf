using System;
using System.Text;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Service;
using System.IO;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;
using Serilog;




namespace Rsbc.Interfaces
{
    /// <summary>
    /// SFEG Utils
    /// </summary>
    public class SfegUtils
    {
       
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        //private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private IConfiguration _configuration { get; }

        /// <summary>
        /// SFEG Utils
        /// </summary>
        /// <param name="caseManagerClient"></param>
        /// <param name="documentStorageAdapterClient"></param>
        public SfegUtils(IConfiguration configuration , CaseManager.CaseManagerClient caseManagerClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
           // _documentStorageAdapterClient = documentStorageAdapterClient;
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
           
            var documentsResponse = _caseManagerClient.GetPdfDocuments(new EmptyRequest());

            // Step 2:
            //Is the fileurl is same as the document url?
            //Get the actual pdf documents for the above list from document storage adapter

            foreach (var doc in documentsResponse.PdfDocuments )
            {
               /* // get document from s3
                var fileResult = _documentStorageAdapterClient.DownloadFile(new DownloadFileRequest()
                {
                  // ServerRelativeUrl = doc.PdfDocumentId
                  // Are we storing the document url in pdfDocument 
                    ServerRelativeUrl = doc.PdfDocumentId,

                });

                // Step 3
                // After getting the documents put the files on the SFEG Directory

                string username = _configuration["SCP_USER"];
                string password = _configuration["SCP_PASS"];
                string host = _configuration["SCP_HOST"];
                string keyUser = _configuration["SCP_KEY_USER"];
                string key = _configuration["SCP_KEY"];

                 // Check the folder and file name and confirm
                string folder = _configuration["SCP_FOLDER_DOCUMENTS"];

                // verify file name
                var filename = doc.PdfDocumentId;

                if (CheckScpSettings(host, username, key))
                {
                    LogStatement(hangfireContext, "No SCP configuration, skipping check for work.");
                }
                else
                {
                    var connectionInfo = GetConnectionInfo(host, username, key);

                    using (var client = new SftpClient(connectionInfo))
                    {
                        client.Connect();
                        LogStatement(hangfireContext, "Connected.");

                        if (string.IsNullOrEmpty(folder))
                        {
                            folder = client.WorkingDirectory;
                        }

                        var stream = new MemoryStream(fileResult.Data.ToByteArray());

                        var filePath = Path.Combine(folder, filename);*/

                        try
                        {
                           // client.UploadFile(stream, filePath);
                            // Update the status to SEND and attach the document

                            PdfDocumentRequest pdfDocument1 = new PdfDocumentRequest()
                            {
                                PdfDoumentId = doc.PdfDocumentId,
                                StatusCode = 100000003

                            };
                            
                            _caseManagerClient.UpdateDocumentStatus(pdfDocument1);
                        }

                        catch(Exception ex)

                        {
                            // set the status to Fail To 

                            PdfDocumentRequest pdfDocument2 = new PdfDocumentRequest()
                            {
                                PdfDoumentId = doc.PdfDocumentId,
                                StatusCode = 100000004

                            };
                            _caseManagerClient.UpdateDocumentStatus(pdfDocument2);
                        }

            }
            return result;
        }

     
    }
}
