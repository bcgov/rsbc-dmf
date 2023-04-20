using System;
using System.Text;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Service;
using Pssg.DocumentStorageAdapter;
using System.IO;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;
using Serilog;
using Hangfire.Server;
using Hangfire;
using Hangfire.Console;



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

        /// <summary>
        /// Check SCP Settings
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool CheckScpSettings(string host, string username, string key)
        {
            return string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private ConnectionInfo GetConnectionInfo(string host, string username, string key)
        {
            // note - key must be in RSA format.  If your key is in OpenSSH format, use this to convert it:
            // ssh-keygen -p -P "" -N "" -m pem -f \path\to\key\file
            // (above command will overwrite your key file)

            byte[] keyData = Encoding.UTF8.GetBytes(key);

            PrivateKeyFile pkf = null;

            using (var privateKeyStream = new MemoryStream(keyData))
            {
                pkf = new PrivateKeyFile(privateKeyStream);
            }

            var connectionInfo = new ConnectionInfo(host,
                                    username,
                                    new PrivateKeyAuthenticationMethod(username, pkf));

            return connectionInfo;
        }

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckConnection(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting CheckConnection.");

            // Attempt to connect to a SCP server.

            string username = _configuration["SCP_USER"];
            string host = _configuration["SCP_HOST"];
            string key = _configuration["SCP_KEY"];

            if (CheckScpSettings(host, username, key))
            {
                LogStatement(hangfireContext, "No SCP configuration, skipping operation.");
            }
            else
            {
                var connectionInfo = GetConnectionInfo(host, username, key);

                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    string folder = _configuration["SCP_FOLDER"];

                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = client.WorkingDirectory;
                    }

                    SpiderFolder(client, hangfireContext, folder);

                }

            }
            LogStatement(hangfireContext, "End of CheckConnection.");

        }

        private void SpiderFolder(SftpClient client, PerformContext hangfireContext, string folder)
        {
            var files = client.ListDirectory(folder);

            foreach (var file in files)
            {
                LogStatement(hangfireContext, folder + "/" + file.Name);

                if (file.Attributes.IsDirectory && !file.Name.StartsWith("."))
                {
                    SpiderFolder(client, hangfireContext, folder + "/" + file.Name);
                }
            }
        }

        /// <summary>
        /// Log Statement
        /// </summary>
        /// <param name="hangfireContext"></param>
        /// <param name="message"></param>
        private void LogStatement(PerformContext hangfireContext, string message)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine(message);
            }
            // emit to Serilog.
            Log.Logger.Information(message);
        }
    }
}
