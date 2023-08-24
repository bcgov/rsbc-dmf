using System;
using System.Text;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Service;
using System.IO;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;
using Serilog;
using Pssg.DocumentStorageAdapter;
using Renci.SshNet.Messages;

namespace Rsbc.Interfaces
{
    /// <summary>
    /// SFEG Utils
    /// </summary>
    public class SftpUtils
    {

        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private IConfiguration _configuration { get; }

        /// <summary>
        /// SFEG Utils
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="caseManagerClient"></param>
        /// <param name="documentStorageAdapterClient"></param>
        public SftpUtils(IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
        }



        private bool CheckScpSettings(string host, string username, string key)
        {
            return string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(key);
        }

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

        private void SpiderFolder(SftpClient client, string folder)
        {
            var files = client.ListDirectory(folder);

            foreach (var file in files)
            {
                Log.Information(folder + "/" + file.Name);

                if (file.Attributes.IsDirectory && !file.Name.StartsWith("."))
                {
                    SpiderFolder(client, folder + "/" + file.Name);
                }
            }
        }


        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>

        public void CheckConnection()
        {
            Log.Information("Starting CheckConnection.");

            // Attempt to connect to a SCP server.

            string username = _configuration["SCP_USER"];
            string host = _configuration["SCP_HOST"];
            string key = _configuration["SCP_KEY"];

            if (CheckScpSettings(host, username, key))
            {
                Log.Information("No SCP configuration, skipping operation.");
            }
            else
            {
                var connectionInfo = GetConnectionInfo(host, username, key);

                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    Log.Information("Connected.");

                    string folder = _configuration["SCP_FOLDER"];

                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = client.WorkingDirectory;
                    }

                    SpiderFolder(client, folder);

                }

            }
            Log.Information("End of CheckConnection.");

        }

        /// <summary>
        /// Send Documents To BcMail
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Dmf.BcMailAdapter.ResultStatusReply SendDocumentsToBcMail()
        {
            var result = new Dmf.BcMailAdapter.ResultStatusReply();
            //Step 1: 
            // call cms adpter to get the list of documents in "Send to BC Mail " Status
            var emptyRequest = new Rsbc.Dmf.CaseManagement.Service.EmptyRequest();
            PdfDocumentReply documentsResponse = _caseManagerClient.GetPdfDocuments(emptyRequest);

            // Step 2:
            //Is the fileurl is same as the document url?
            //Get the actual pdf documents for the above list from document storage adapter

            foreach (var doc in documentsResponse.PdfDocuments)
            {
                // get document from s3
                var fileResult = _documentStorageAdapterClient.DownloadFile(new DownloadFileRequest()
                {
                    // ServerRelativeUrl = doc.PdfDocumentId
                    // Are we storing the document url in pdfDocument 
                    ServerRelativeUrl = doc.ServerUrl,

                });

                // Step 3
                // After getting the documents put the files on the SFEG Directory

                string username = _configuration["SCP_USER"];
                string password = _configuration["SCP_PASS"];
                string host = _configuration["SCP_HOST"];
                string key = _configuration["SCP_KEY"];

                // Check the folder and file name and confirm
                string folder = _configuration["SCP_FOLDER_DOCUMENTS"];

                // verify file name
                var filename = doc.Filename;

                if (CheckScpSettings(host, username, key))
                {
                    Log.Logger.Information("No SCP configuration, skipping check for work.");

                }
                else
                {
                    var connectionInfo = GetConnectionInfo(host, username, key);

                    using (var client = new SftpClient(connectionInfo))
                    {
                        client.Connect();
                        Log.Logger.Information("Connected");

                        if (string.IsNullOrEmpty(folder))
                        {
                            folder = client.WorkingDirectory;
                        }

                        var stream = new MemoryStream(fileResult.Data.ToByteArray());

                        var filePath = Path.Combine(folder, filename);

                        try
                        {
                            client.UploadFile(stream, filePath);
                            // Update the status to SEND and attach the document

                            Log.Information($"SFTP Upload complete for {filePath}");

                            var pdfDocument = new PdfDocument()
                            {
                                PdfDocumentId = doc.PdfDocumentId,
                                StatusCode = PdfDocument.Types.StatusCodeOptions.Sent
                            };


                            _caseManagerClient.UpdateDocumentStatus(pdfDocument);
                        }

                        catch (Exception ex)

                        {
                            // set the status to Fail To 
                            result.ResultStatus = Dmf.BcMailAdapter.ResultStatus.Fail;
                            Log.Error(ex, "Send Documents to BC mail : Set the status to Failed to send ");

                            _caseManagerClient.UpdateDocumentStatus(new PdfDocument()
                            {
                                PdfDocumentId = doc.PdfDocumentId,
                                StatusCode = PdfDocument.Types.StatusCodeOptions.FailedToSend
                            });

                        }
                    }

                }


            }
            return result;
        }
    }
}
