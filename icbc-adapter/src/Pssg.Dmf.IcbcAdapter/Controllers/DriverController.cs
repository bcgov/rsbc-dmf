using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.HttpSys;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Grpc.Core;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;
using DocumentStorageClient = Pssg.DocumentStorageAdapter.DocumentStorageAdapter.DocumentStorageAdapterClient;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using System.Linq;
using Org.BouncyCastle.Crypto;
using System.Globalization;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{

    public class DriverAndDocumentsResponse //for Agentic DMER processing
    {
        public string driver_id {get;set;}
        public string first_name {get;set;}
        public string middle_name {get;set;}
        public string last_name {get;set;}
        public string licence_number {get;set;}
        public List<DriverDocument> documents {get;set;}
        public DmerDocumentCaseRecord activeCase {get;set;}
    }


    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DriverController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DmersController> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public DriverController(IConfiguration configuration, ILogger<DmersController> logger, IServiceScopeFactory serviceScopeFactory)
        {
           _configuration = configuration;
           _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

      
    /// <summary>
    /// GET: /dmers?pageSize=50&amp;cursor=0
    /// </summary>
    /// <returns>DMER items, the requested page_size, and a nextLink for full pages.</returns>
    /// <param name="pageSize">Maximum number of records to return</param>
    /// <param name="cursor">Zero-based offset of the first record in the page</param>
    [HttpGet("{driver_licence_number}")]
    [Produces("application/json")]
    public ActionResult GetDriverMetaandDocuments([FromRoute] string driver_licence_number)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var caseManagerClient = scope.ServiceProvider.GetRequiredService<CaseManager.CaseManagerClient>();
            var driverRequest = new DriverLicenseRequest {DriverLicenseNumber = driver_licence_number};
            var driverDocumentsReply = caseManagerClient.GetDriverDocuments(driverRequest);
            var driverPersonReply = caseManagerClient.GetDriverPerson(driverRequest);
            var driverActiveCaseReply = caseManagerClient.GetActiveCases(driverRequest);



            var driverPerson = caseManagerClient.GetDriverPerson(driverRequest).Items[0];
            var driverActiveCase = driverActiveCaseReply.Items[0];

            var documents = driverDocumentsReply.Items.Select(document => new DriverDocument
            {
                FileName = document.DocumentUrl,
                DocumentType = document.DocumentType,
                SubmittalStatus = document.SubmittalStatus,
                UploadedDate = document.FaxReceivedDate?.ToDateTime()
                    .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture) ?? string.Empty,
                DocumentRecordId = document.DocumentId
            }).ToList();

            var driver_case = new DmerDocumentCaseRecord {
                CaseId =  driverActiveCase.CaseId,
                CaseTitle = driverActiveCase.Title,
                CaseType = driverActiveCase.CaseType,
                CasePriority = driverActiveCase.Priority,
                CaseOwner = driverActiveCase.AssigneeTitle,
                CaseStatus = driverActiveCase.Status
            };

            var response = new DriverAndDocumentsResponse {
                driver_id = driverPerson.Id,
                first_name = driverPerson.GivenName,
                middle_name = driverPerson.Middlename,
                last_name = driverPerson.Surname,
                licence_number = driverPerson.DriverLicenseNumber,
                documents = documents,
                activeCase = driver_case
            };

            return Json(response);
        }
    }
}
