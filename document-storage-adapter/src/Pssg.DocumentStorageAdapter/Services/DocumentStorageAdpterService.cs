using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pssg.Interfaces;
using Serilog;

/** makes use of the AWS S3 SDK
 
https://aws.amazon.com/sdk-for-net/

*/

namespace Pssg.DocumentStorageAdapter.Services
{
    // Default to require authorization
    [Authorize]
    public class DocumentStorageAdapterService : DocumentStorageAdapter.DocumentStorageAdapterBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentStorageAdapterService> _logger;

        public DocumentStorageAdapterService(ILogger<DocumentStorageAdapterService> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override Task<CreateFolderReply> CreateFolder(CreateFolderRequest request, ServerCallContext context)
        {
            var result = new CreateFolderReply();

            var logFolder = WordSanitizer.Sanitize(request.FolderName);

            

            var _S3 = new S3(_configuration);
            
            var listTitle = _S3.GetDocumentListTitle(request.EntityName);

            CreateDocumentLibraryIfMissing(listTitle, GetDocumentTemplateUrlPart(request.EntityName));

            var folderExists = false;
            try
            {
                var folder = _S3.GetFolder(listTitle, request.FolderName).GetAwaiter().GetResult();
                if (folder != null) folderExists = true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Generic Exception creating folder");
                folderExists = false;
            }

            if (folderExists)
                result.ResultStatus = ResultStatus.Success;
            else
                try
                {
                    _S3.CreateFolder(_S3.GetDocumentListTitle(request.EntityName), request.FolderName)
                        .GetAwaiter().GetResult();
                    var folder = _S3.GetFolder(listTitle, request.FolderName).GetAwaiter()
                        .GetResult();
                    if (folder != null) result.ResultStatus = ResultStatus.Success;
                }
                catch (Exception e)
                {
                    result.ResultStatus = ResultStatus.Fail;
                    result.ErrorDetail = $"ERROR in creating folder {logFolder}";
                    Log.Error(e, result.ErrorDetail);
                }

            return Task.FromResult(result);
        }

        public override Task<FileExistsReply> FileExists(FileExistsRequest request, ServerCallContext context)
        {
            var result = new FileExistsReply();

            var _S3 = new S3(_configuration);

            List<S3.FileDetailsList> fileDetailsList = null;
            try
            {
                fileDetailsList = _S3
                    .GetFileDetailsListInFolder(GetDocumentTemplateUrlPart(request.EntityName), request.FolderName,
                        request.DocumentType).GetAwaiter().GetResult();
                if (fileDetailsList != null)

                {
                    var hasFile = fileDetailsList.Any(f => f.ServerRelativeUrl == request.ServerRelativeUrl);

                    if (hasFile)
                        result.ResultStatus = FileExistStatus.Exist;
                    else
                        result.ResultStatus = FileExistStatus.NotExist;
                }
            }

            catch (Exception e)
            {
                result.ResultStatus = FileExistStatus.Error;
                result.ErrorDetail = "Error determining if file exists";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        

        private string GetDocumentTemplateUrlPart(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = S3.DefaultDocumentUrlTitle;
                    break;
                case "application":
                    listTitle = "adoxio_application";
                    break;
                case "contact":
                    listTitle = S3.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = "adoxio_worker";
                    break;
                case "event":
                    listTitle = S3.EventDocumentListTitle;
                    break;
                case "federal_report":
                    listTitle = S3.FederalReportListTitle;
                    break;
                case "licence":
                    listTitle = S3.LicenceDocumentUrlTitle;
                    break;
            }

            return listTitle;
        }

        private void CreateDocumentLibraryIfMissing(string listTitle, string documentTemplateUrl = null)
        {
            var _S3 = new S3(_configuration);
            var exists = _S3.DocumentLibraryExists(listTitle).GetAwaiter().GetResult();
            if (!exists)
                _S3.CreateDocumentLibrary(listTitle, documentTemplateUrl).GetAwaiter().GetResult();
        }

        public override Task<DeleteFileReply> DeleteFile(DeleteFileRequest request, ServerCallContext context)
        {
            var result = new DeleteFileReply();

            var logUrl = WordSanitizer.Sanitize(request.ServerRelativeUrl);

            var _S3 = new S3(_configuration);

            try
            {
                var success = _S3.DeleteFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

                if (success)
                    result.ResultStatus = ResultStatus.Success;
                else
                    result.ResultStatus = ResultStatus.Fail;
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in deleting file {logUrl}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<DownloadFileReply> DownloadFile(DownloadFileRequest request, ServerCallContext context)
        {
            var result = new DownloadFileReply();
            var logUrl = WordSanitizer.Sanitize(request.ServerRelativeUrl);
            var _S3 = new S3(_configuration);

            try
            {
                var data = _S3.DownloadFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

                if (data != null)
                {
                    result.ResultStatus = ResultStatus.Success;
                    result.Data = ByteString.CopyFrom(data);
                }
                else
                {
                    result.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in downloading file {logUrl}";
                Log.Error(e, result.ErrorDetail);
            }


            return Task.FromResult(result);
        }

        public override Task<UploadFileReply> UploadFile(UploadFileRequest request, ServerCallContext context)
        {
            var result = new UploadFileReply();
            var logFileName = WordSanitizer.Sanitize(request.FileName);
            var logFolderName = WordSanitizer.Sanitize(request.FolderName);

            var _S3 = new S3(_configuration);

            CreateDocumentLibraryIfMissing(_S3.GetDocumentListTitle(request.EntityName),
                GetDocumentTemplateUrlPart(request.EntityName));

            try
            {
                var fileName = _S3.AddFile(GetDocumentTemplateUrlPart(request.EntityName),
                    request.FolderName,
                    request.FileName,
                    request.Data.ToByteArray(), request.ContentType).GetAwaiter().GetResult();

                result.FileName = fileName;
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in uploading file {logFileName} to folder {logFolderName}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<FolderFilesReply> FolderFiles(FolderFilesRequest request, ServerCallContext context)
        {
            var result = new FolderFilesReply();

            // Get the file details list in folder
            List<S3.FileDetailsList> fileDetailsList = null;
            var _S3 = new S3(_configuration);
            try
            {
                fileDetailsList = _S3
                    .GetFileDetailsListInFolder(GetDocumentTemplateUrlPart(request.EntityName), request.FolderName,
                        request.DocumentType).GetAwaiter().GetResult();
                if (fileDetailsList != null)

                {
                    // gRPC ensures that the collection has space to accept new data; no need to call a constructor
                    foreach (var item in fileDetailsList)
                    {
                        // Sharepoint API responds with dates in UTC format
                        var utcFormat = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
                        DateTime parsedCreateDate, parsedLastModified;
                        DateTime.TryParse(item.TimeCreated, CultureInfo.InvariantCulture, utcFormat,
                            out parsedCreateDate);
                        DateTime.TryParse(item.TimeLastModified, CultureInfo.InvariantCulture, utcFormat,
                            out parsedLastModified);

                        var newItem = new FileSystemItem
                        {
                            DocumentType = item.DocumentType,
                            Name = item.Name,
                            ServerRelativeUrl = item.ServerRelativeUrl,
                            Size = int.Parse(item.Length),
                            TimeCreated = Timestamp.FromDateTime(parsedCreateDate),
                            TimeLastModified = Timestamp.FromDateTime(parsedLastModified)
                        };

                        result.Files.Add(newItem);
                    }

                    result.ResultStatus = ResultStatus.Success;
                }
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "Error getting SharePoint File List";
                Log.Error(e, result.ErrorDetail);
            }
            

            return Task.FromResult(result);
        }

        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            var result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            var configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(request.Secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                );
                result.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                result.ResultStatus = ResultStatus.Success;
            }
            else
            {
                result.ErrorDetail = "Bad Request";
            }

            return Task.FromResult(result);
        }

        public override Task<TruncatedFilenameReply> GetTruncatedFilename(TruncatedFilenameRequest request,
            ServerCallContext context)
        {
            var result = new TruncatedFilenameReply();
            var logFileName = WordSanitizer.Sanitize(request.FileName);
            var logFolderName = WordSanitizer.Sanitize(request.FolderName);
            try
            {
                var _S3 = new S3(_configuration);

                // Ask SharePoint whether this filename would be truncated upon upload
                var listTitle = _S3.GetDocumentListTitle(request.EntityName);
                var maybeTruncated =
                    _S3.GetTruncatedFileName(request.FileName, listTitle, request.FolderName);
                result.FileName = maybeTruncated;
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in getting truncated filename {logFileName} for folder {logFolderName}";
                Log.Error(e, result.ErrorDetail);
            }
            

            return Task.FromResult(result);
        }
    }
}