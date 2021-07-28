using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory; // To interact with Amazon S3.

// reference - https://docs.aws.amazon.com/sdkfornet/v3/apidocs/Index.html
// reference - https://docs.ceph.com/en/latest/radosgw/s3/csharp/

namespace Rsbc.Dmf.Interfaces
{
    public class S3
    {
        public const string METADATA_KEY_ENTITY = "Entity";
        public const string METADATA_KEY_ENTITY_ID = "EntityId";
        public const string METADATA_KEY_TAG1 = "Tag1";
        public const string METADATA_KEY_TAG2 = "Tag2";
        public const string METADATA_KEY_TAG3 = "Tag3";

        public const string DefaultDocumentListTitle = "Account";
        public const string DefaultDocumentUrlTitle = "account";
        public const string ApplicationDocumentListTitle = "Application";
        public const string ApplicationDocumentUrlTitle = "adoxio_application";
        public const string ContactDocumentListTitle = "contact";
        public const string WorkerDocumentListTitle = "Worker Qualification";
        public const string WorkerDocumentUrlTitle = "adoxio_worker";
        public const string EventDocumentListTitle = "adoxio_event";
        public const string FederalReportListTitle = "adoxio_federalreportexport";
        public const string LicenceDocumentUrlTitle = "adoxio_licences";
        public const string LicenceDocumentListTitle = "Licence";


        private const int MaxUrlLength = 260; // default maximum URL length.
        private readonly string Bucket;
        private readonly IConfiguration Configuration;
        private HttpClient _Client;
        private CookieContainer _CookieContainer;
        private HttpClientHandler _HttpClientHandler;

        private AuthenticationResult authenticationResult;
        private string Digest;
        private readonly AmazonS3Client S3Client;

        public S3(IConfiguration configuration)
        {
            // create the HttpClient that is used for our direct REST calls.
            /*
            _CookieContainer = new CookieContainer();
            _HttpClientHandler = new HttpClientHandler() { UseCookies = true, AllowAutoRedirect = false, CookieContainer = _CookieContainer };
            _Client = new HttpClient(_HttpClientHandler);

            _Client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
            */
            Configuration = configuration;

            // check that we have the right settings.
            if (Configuration["ACCESS_KEY"] != null && Configuration["SECRET_KEY"] != null &&
                Configuration["SERVICE_URL"] != null && Configuration["BUCKET"] != null)
            {
                // S3 configuration settings.
                var accessKey = Configuration["ACCESS_KEY"];
                var secretKey = Configuration["SECRET_KEY"];
                Bucket = Configuration["BUCKET"];
                AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

                var config = new AmazonS3Config
                {
                    ServiceURL = Configuration["SERVICE_URL"],
                    ForcePathStyle = true,
                    SignatureVersion = "2",
                    SignatureMethod = SigningAlgorithm.HmacSHA1,
                    UseHttp = false,
                };

                S3Client = new AmazonS3Client(credentials, config);
            }
            else
            {
                S3Client = null;
            }
        }

        public string OdataUri { get; set; }
        public string ServerAppIdUri { get; set; }
        public string WebName { get; set; }
        public string ApiEndpoint { get; set; }
        public string NativeBaseUri { get; set; }
        private string Authorization { get; set; }

        private string GetPrefix(string listTitle, string folderName)
        {
            var prefix = $"/{listTitle}/${folderName}/";
            return prefix;
        }

        public bool IsValid()
        {
            var result = false;
            if (S3Client != null) result = true;
            return result;
        }

        /// <summary>
        ///     Escape the apostrophe character.  Since we use it to enclose the filename it must be escaped.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Filename, with apropstophes escaped.</returns>
        private string EscapeApostrophe(string filename)
        {
            string result = null;
            if (!string.IsNullOrEmpty(filename)) result = filename.Replace("'", "''");
            return result;
        }

        /// <summary>
        ///     Get file details list from S3 filtered by folder name and document type
        /// </summary>
        /// <param name="listTitle"></param>
        /// <param name="folderName"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public async Task<List<FileDetailsList>> GetFileDetailsListInFolder(string listTitle, string folderName,
            string documentType)
        {
            // return early if S3 is disabled.
            if (!IsValid()) return null;

            folderName = FixFoldername(folderName);

            var prefix = GetPrefix(listTitle, folderName);

            var request = new ListObjectsV2Request
            {
                BucketName = Bucket,
                Prefix = prefix
            };

            var response = await S3Client.ListObjectsV2Async(request);
            var fileDetailsList = new List<FileDetailsList>();

            foreach (var o in response.S3Objects)
            {
                var fdl = new FileDetailsList();
                fdl.Length = o.Size.ToString();
                fdl.ServerRelativeUrl = o.Key;
                fdl.Name = o.Key.Substring(o.Key.LastIndexOf("/") + 1);
                var fileDoctypeEnd = fdl.Name.IndexOf("__");
                if (fileDoctypeEnd > -1)
                {
                    var fileDoctype = fdl.Name.Substring(0, fileDoctypeEnd);
                    fdl.DocumentType = documentType;
                }

                if (documentType == null || fdl.DocumentType == documentType)
                {
                    fileDetailsList.Add(fdl);
                }
            }

            return fileDetailsList;
        }

        public string RemoveInvalidCharacters(string filename)
        {
            var osInvalidChars = new string(Path.GetInvalidFileNameChars());
            osInvalidChars += "~#%&*()[]{}"; // add additional characters that do not work with S3
            var invalidChars = Regex.Escape(osInvalidChars);
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            // Get the validated file name string
            var result = Regex.Replace(filename, invalidRegStr, "_");

            return result;
        }

        public string FixFoldername(string foldername)
        {
            var result = RemoveInvalidCharacters(foldername);

            return result;
        }

        public string FixFilename(string filename, int maxLength = 128)
        {
            var result = RemoveInvalidCharacters(filename);

            // S3 requires that the filename is less than 128 characters.

            if (result.Length >= maxLength)
            {
                var extension = Path.GetExtension(result);
                result = Path.GetFileNameWithoutExtension(result).Substring(0, maxLength - extension.Length);
                result += extension;
            }

            return result;
        }

        /// <summary>
        ///     Create Folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task CreateFolder(string listTitle, string folderName)
        {
            // no need to create a folder with S3.
        }

        /// <summary>
        ///     Create Folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<object> CreateDocumentLibrary(string listTitle, string documentTemplateUrlTitle = null)
        {
            // return early if S3 is disabled.
            if (!IsValid()) return null;

            // S3 does not need a document library.

            var library = CreateNewDocumentLibraryRequest(documentTemplateUrlTitle);


            return library;
        }

        public async Task<object> UpdateDocumentLibrary(string listTitle)
        {
            // S3 does not use document libraries.

            var library = CreateNewDocumentLibraryRequest(listTitle);

            return library;
        }

        private object CreateNewDocumentLibraryRequest(string listName)
        {
            var type = new {type = "SP.List"};
            var request = new
            {
                __metadata = type,
                BaseTemplate = 101,
                Title = listName
            };
            return request;
        }


        public async Task<bool> DeleteFolder(string listTitle, string folderName)
        {
            // return early if S3 is disabled.
            if (!IsValid()) return false;

            var result = true;


            folderName = FixFoldername(folderName);

            var prefix = GetPrefix(listTitle, folderName);

            var request = new ListObjectsV2Request
            {
                BucketName = Bucket,
                Prefix = prefix
            };

            var response = await S3Client.ListObjectsV2Async(request);

            foreach (var o in response.S3Objects)
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = Bucket,
                    Key = o.Key
                };

                try
                {
                    var deleteResult = await S3Client.DeleteObjectAsync(deleteRequest);
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;
        }

        public async Task<bool> FolderExists(string listTitle, string folderName)
        {
            bool result;

            folderName = FixFoldername(folderName);

            var prefix = GetPrefix(listTitle, folderName);

            var request = new ListObjectsV2Request
            {
                BucketName = Bucket,
                Prefix = prefix
            };

            var response = await S3Client.ListObjectsV2Async(request);
            result = response.KeyCount > 0;
            return result;
        }

        public async Task<bool> DocumentLibraryExists(string listTitle)
        {
            return true; // folders always exist in S3.
        }

        public async Task<object> GetFolder(string listTitle, string folderName)
        {
            // return early if S3 is disabled.
            if (!IsValid()) return null;
            // this is just used to confirm that a folder exists - so in S3 that is always true.
            return true;
        }

        public async Task<object> GetDocumentLibrary(string listTitle)
        {
            // return early if S3 is disabled.
            if (!IsValid()) return null;
            // this is just used to confirm that a folder exists - so in S3 that is always true.
            return true;
        }

        public async Task<string> AddFile(string folderName, string fileName, Stream fileData, string contentType)
        {
            return await AddFile(DefaultDocumentListTitle, folderName, fileName, fileData, contentType);
        }

        public async Task<string> AddFile(string documentLibrary, string folderName, string fileName, Stream fileData,
            string contentType)
        {
            folderName = FixFoldername(folderName);


            // now add the file to the folder.

            fileName = await UploadFile(fileName, documentLibrary, folderName, fileData, contentType);

            return fileName;
        }

        public async Task<string> AddFile(string folderName, string fileName, byte[] fileData, string contentType)
        {
            return await AddFile(DefaultDocumentListTitle, folderName, fileName, fileData, contentType);
        }

        public async Task<string> AddFile(string documentLibrary, string folderName, string fileName, byte[] fileData,
            string contentType)
        {
            folderName = FixFoldername(folderName);

            // now add the file to the folder.

            fileName = await UploadFile(fileName, documentLibrary, folderName, fileData, contentType);

            return fileName;
        }

        public string GetServerRelativeURL(string listTitle, string folderName)
        {
            folderName = FixFoldername(folderName);
            var serverRelativeUrl = GetPrefix(listTitle, folderName);

            return serverRelativeUrl;
        }


        private string GenerateUploadRequestUriString(string folderServerRelativeUrl, string fileName)
        {
            var requestUriString = ApiEndpoint + "web/getFolderByServerRelativeUrl('" +
                                   EscapeApostrophe(folderServerRelativeUrl) + "')/Files/add(url='"
                                   + EscapeApostrophe(fileName) + "',overwrite=true)";
            return requestUriString;
        }

        /// <summary>
        ///     Upload a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="listTitle"></param>
        /// <param name="folderName"></param>
        /// <param name="fileData"></param>
        /// <param name="contentType"></param>
        /// <returns>Uploaded Filename, or Null if not successful.</returns>
        public async Task<string> UploadFile(string fileName, string listTitle, string folderName, Stream fileData,
            string contentType)
        {
            string result = null;
            if (IsValid())
            {
                folderName = FixFoldername(folderName);
                fileName = GetTruncatedFileName(fileName, listTitle, folderName);

                // convert the stream into a byte array.
                var prefix = GetPrefix(listTitle, folderName);
                var fileKey = prefix + fileName;

                var request = new PutObjectRequest();
                request.BucketName = Bucket;
                request.Key = fileKey;
                request.ContentType = contentType;
                request.InputStream = fileData;
                await S3Client.PutObjectAsync(request);
                result = fileName;
            }

            return result;
        }


        /// <summary>
        ///     SharePoint is very particular about the file name length and the total characters in the URL to access a file.
        ///     This method returns the input file name or a truncated version of the file name if it is over the max number of
        ///     characters.
        /// </summary>
        /// <param name="fileName">The file name to check; e.g. "abcdefg1111222233334444.pdf"</param>
        /// <param name="listTitle">The list title</param>
        /// <param name="folderName">The folder name where the file would be uploaded</param>
        /// <returns>The (potentially truncated) file name; e.g. "abcd.pdf"</returns>
        public string GetTruncatedFileName(string fileName, string listTitle, string folderName)
        {
            // return early if S3 is disabled.
            if (!IsValid()) return fileName;

            // S3 requires that filenames are less than 128 characters.
            var maxLength = 128;
            fileName = FixFilename(fileName, maxLength);

            // S3 also imposes a limit on the whole URL
            var serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
            var requestUriString = GenerateUploadRequestUriString(serverRelativeUrl, fileName);
            if (requestUriString.Length > MaxUrlLength)
            {
                var delta = requestUriString.Length - MaxUrlLength;
                maxLength -= delta;
                fileName = FixFilename(fileName, maxLength);
            }

            return fileName;
        }

        /// <summary>
        ///     Upload a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="listTitle"></param>
        /// <param name="folderName"></param>
        /// <param name="fileData"></param>
        /// <param name="contentType"></param>
        /// <returns>Uploaded Filename, or Null if not successful.</returns>
        public async Task<string> UploadFile(string fileName, string listTitle, string folderName, byte[] data,
            string contentType)
        {
            string result = null;
            if (IsValid())
            {
                folderName = FixFoldername(folderName);
                fileName = GetTruncatedFileName(fileName, listTitle, folderName);
                var prefix = GetPrefix(listTitle, folderName);
                var fileKey = prefix + fileName;

                result = await UploadFile(fileKey, data, contentType);
            }

            return result;
        }

        /// <summary>
        ///     Upload a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="listTitle"></param>
        /// <param name="folderName"></param>
        /// <param name="fileData"></param>
        /// <param name="contentType"></param>
        /// <returns>Uploaded Filename, or Null if not successful.</returns>
        public async Task<string> UploadFile(string fileName, byte[] data, string contentType, Dictionary< string, string> metadata = null )
        {
            string result = null;
            if (IsValid())
            {

                using (var inputStream = new MemoryStream(data))
                {
                    var request = new PutObjectRequest()
                    {
                        BucketName = Bucket,
                        Key = fileName,
                        ContentType = contentType,
                        InputStream = inputStream
                    };

                    if (metadata != null)
                    {
                        foreach (var item in metadata)
                        {
                            request.Metadata.Add(item.Key, item.Value);
                        }
                    }

                    await S3Client.PutObjectAsync(request);
                    result = fileName;
                }
            }

            return result;
        }


        /// <summary>
        ///     Download a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFile(string serverRelativeUrl)
        {
            byte[] result = null;

            var strings = serverRelativeUrl.Split("/");
            if (strings.Length != 4) return result;

            var prefix = GetPrefix(strings[1], strings[2]);

            var fileKey = prefix + strings[3];

            var request = new GetObjectRequest();
            request.BucketName = Bucket;
            request.Key = fileKey;
            var response = await S3Client.GetObjectAsync(request);
            // convert the response stream into a byte array.
            using (var memoryStream = new MemoryStream())
            {
                using (var x = response.ResponseStream)
                {
                    x.CopyTo(memoryStream);
                }

                result = memoryStream.ToArray();
            }

            return result;
        }

        public async Task<string> GetDigest(HttpClient client)
        {
            // not required for S3

            return null;
        }

        /// <summary>
        ///     Delete a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(string listTitle, string folderName, string fileName)
        {
            var result = false;
            // Delete is very similar to a GET.
            var prefix = GetPrefix(listTitle, folderName);

            var fileKey = prefix + fileName;

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = Bucket,
                Key = fileKey
            };

            try
            {
                var deleteResult = await S3Client.DeleteObjectAsync(deleteRequest);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> DeleteFile(string serverRelativeUrl)
        {
            var strings = serverRelativeUrl.Split("/");
            if (strings.Length == 4) return await DeleteFile(strings[1], strings[2], strings[3]);

            return false;
        }

        /// <summary>
        ///     Rename a file.  Note that this only works for files with relatively short names due to the max URL length.  It may
        ///     be possible to allow that to work by using @variables in the URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> RenameFile(string oldServerRelativeUrl, string newServerRelativeUrl)
        {
            var result = false;

            // not currently implemented for S3.

            return result;
        }

        public class FileSystemItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Documenttype { get; set; }
            public int Size { get; set; }
            public string Serverrelativeurl { get; set; }
            public DateTime Timecreated { get; set; }
            public DateTime Timelastmodified { get; set; }
        }


        public class FileDetailsList
        {
            public string Name { get; set; }
            public string TimeLastModified { get; set; }
            public string TimeCreated { get; set; }
            public string Length { get; set; }
            public string DocumentType { get; set; }
            public string ServerRelativeUrl { get; set; }
        }
    }

    internal class DocumentLibraryResponse
    {
        public DocumentLibraryResponseContent d { get; set; }
    }

    internal class DocumentLibraryResponseContent
    {
        public string Id { get; set; }
    }
}