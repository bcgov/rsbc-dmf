using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using Pssg.Interfaces.ViewModelExtensions;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Rsbc.Dmf.CaseManagement.Service;
using Org.BouncyCastle.Asn1.Ocsp;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Bcpg;
using Microsoft.Extensions.Caching.Memory;
using Pssg.Interfaces.IcbcModels;
using Serilog;
using Rsbc.Dmf.IcbcAdapter.BackgroundWorkItem;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{

    public class NewCandidate
    {
        /// <summary>
        /// Driver's License Number
        /// </summary>
        [SwaggerSchema("Driver's License Number")]
        public string DlNumber { get; set; }
        [SwaggerSchema("Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Date that the requirement for a Medical Exam was issued
        /// </summary>
        [SwaggerSchema("Date that the requirement for a Medical Exam was issued")]
        public DateTimeOffset? EffectiveDate { get; set; }

        /// <summary>
        /// Birthdate for the Driver
        /// </summary>
        [SwaggerSchema("Birthdate for the Driver")]
        public DateTimeOffset? BirthDate { get; set; }

    }

    public class CaseStatus
    {
        public string CaseId { get; set; }
        public string DlNumber { get; set; }
        public string Status { get; set; }
    }



    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class IcbcController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverHistoryController> _logger;
        private readonly IIcbcClient _icbcClient;
        private readonly IBackgroundTaskQueue _backgroundWorkerQueue;

        private readonly IMemoryCache _cache;

        private readonly CaseManager.CaseManagerClient _caseManagerClient;

        public IcbcController(ILogger<DriverHistoryController> logger, IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, IMemoryCache cache, IIcbcClient icbcClient, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _configuration = configuration;
            _logger = logger;
            _icbcClient = icbcClient;
            _backgroundWorkerQueue = backgroundTaskQueue;
            _caseManagerClient = caseManagerClient;
            _cache = cache; 
        }

        /// <summary>
        /// POST: /Icbc/Candidates
        /// </summary>
        /// <param name="newCandidates">List of Candidates to be added to the case management system</param>
        /// <returns></returns>
        [HttpPost("Candidates")]
        [SwaggerResponse(200, "The candidates were processed correctly")]
        [SwaggerResponse(400, "The format of the provided data was invalid.  Please refer to the model.")]
        [SwaggerResponse(500, "An unexpected server error occurred while processing. Please retry.")]
        public ActionResult CreateCandidates([FromBody] List<NewCandidate> newCandidates)
        {
            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {                

            //check for duplicates; if there is an existing case then do not create a new one

            foreach (var item in newCandidates)
            {
                string medicalType = "";
                // first check that the item is not in the cache.
                CLNT data = null;
                if (!_cache.TryGetValue(item.DlNumber, out data))
                {
                    // get the history from ICBC
                    data = _icbcClient.GetDriverHistory(item.DlNumber);

                    // ensure the presentation of the DL matches the calling system.
                    data.DR1MST.LNUM = item.DlNumber;
                    // Key not in cache, so get data.
                    //cacheEntry = DateTime.Now;
                    if (data != null)
                    {
                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                        // Save data in cache.
                        _cache.Set(item.DlNumber, data, cacheEntryOptions);
                    }
                }
                if (data != null && data.DR1MST != null && data.DR1MST.DR1MEDN != null)
                {
                    try
                    {
                        medicalType = data.DR1MST.DR1MEDN.OrderByDescending(x => x.MIDT).FirstOrDefault().MedicalType;
                    }
                    catch (Exception e)
                    {
                        Serilog.Log.Error(e, "ICBC Create Candidate Lookup Error");
                    }                    
                }

                // determine if there is active cases for driver.
                var driverLicenseRequest = new DriverLicenseRequest()
                    { DriverLicenseNumber = item.DlNumber };
                var activeCaseReply = _caseManagerClient.GetActiveCases(driverLicenseRequest);
                
                var commentDate = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow);
                var commentTypeCode = "C";
                var userId = "System";
                var assignee = string.Empty;
                var dmerIssuranceDate = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt");

                if (activeCaseReply.ResultStatus != CaseManagement.Service.ResultStatus.Success ||
                    activeCaseReply.Items.Count == 0)
                {
                    // no cases, create new one with document.
                    LegacyCandidateRequest lcr = new LegacyCandidateRequest()
                    {
                        LicenseNumber = item.DlNumber,
                        Surname = item.LastName ?? string.Empty,
                        ClientNumber = string.Empty,
                        BirthDate = Timestamp.FromDateTimeOffset(item.BirthDate ?? DateTimeOffset.MinValue),
                        EffectiveDate = Timestamp.FromDateTimeOffset(item.EffectiveDate ?? DateTimeOffset.MinValue),
                        MedicalType = medicalType
                    };

                    var candidateCreation = _caseManagerClient.ProcessLegacyCandidate(lcr);
                    
                    if (candidateCreation != null)
                    {
                        var caseId = _caseManagerClient.GetCaseId(lcr.LicenseNumber, lcr.Surname);

                        // Create DMER envelope for the case

                        _caseManagerClient.CreateICBCDocumentEnvelope(new LegacyDocument()
                        {
                            CaseId = caseId ?? string.Empty,
                            Driver = new CaseManagement.Service.Driver()
                            {
                                DriverLicenseNumber = lcr.LicenseNumber,
                            },
                            SubmittalStatus = "Open-Required",
                            DocumentType = "DMER",
                            DocumentTypeCode = "001",
                            FaxReceivedDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                            ImportDate = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
                            DocumentId = Guid.NewGuid().ToString(),
                            SequenceNumber = 1,
                            Owner = "Team - Intake"
                        });

                        // Create Comment
                        _caseManagerClient.CreateICBCMedicalCandidateComment(new LegacyComment()
                        {
                            CaseId = caseId,
                            Driver = new CaseManagement.Service.Driver()
                            {
                                DriverLicenseNumber = lcr.LicenseNumber,
                            },
                            SequenceNumber = 1,
                            CommentDate = commentDate,
                            CommentText =
                                $"This case was opened because a DMER was issued to this driver by ICBC on {dmerIssuranceDate}",
                            CommentTypeCode = commentTypeCode,
                            UserId = userId,
                            Assignee = assignee
                        });

                    }

                }
                else // existing active case
                {
                    // add a bring forward and comment.
                    var firstCase = activeCaseReply.Items.OrderByDescending(x => x.OpenedDate).First();
                    var caseId = firstCase.CaseId;
                    if (caseId != null)
                    {

                        // Create a bring forward
                        _caseManagerClient.CreateBringForward(new BringForwardRequest()
                        {
                            CaseId = caseId,
                            Description = "ICBC",
                            Assignee = string.Empty,
                            Priority = CallbackPriority.High,
                            Subject = "A DMER Candidate was introduced to a Case In Progress",

                        });

                        // Create Comment
                        _caseManagerClient.CreateICBCMedicalCandidateComment(new LegacyComment()
                        {
                            CaseId = caseId,
                            Driver = new CaseManagement.Service.Driver()
                            {
                                DriverLicenseNumber = item.DlNumber,
                            },
                            SequenceNumber = 1,
                            CommentDate = commentDate,
                            CommentText = $"A DMER was issued to this driver by ICBC on {dmerIssuranceDate} while this case was already in progress.",
                            CommentTypeCode = commentTypeCode,
                            UserId = userId,
                            Assignee = assignee
                        });
                    }
                    else
                    {
                        Log.Error("Unexpected error during ICBC Create Candidate - CaseId is null.");
                    }

                    // Check for an open/required document on the driver.
                    var documents = _caseManagerClient.GetIcbcDmerEnvelopes(driverLicenseRequest);
                    if (documents.ResultStatus != CaseManagement.Service.ResultStatus.Success ||
                        documents.Items.Count == 0)
                    {
                        // add a document requirement.
                        _caseManagerClient.CreateICBCDocumentEnvelope(new LegacyDocument()
                        {
                            CaseId = caseId ?? string.Empty,
                            Driver = new CaseManagement.Service.Driver()
                            {
                                DriverLicenseNumber = item.DlNumber,
                            },
                            SubmittalStatus = "Open-Required",
                            DocumentType = "DMER",
                            DocumentTypeCode = "001",
                            FaxReceivedDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                            ImportDate = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
                            DocumentId = Guid.NewGuid().ToString(),
                            SequenceNumber = 1,
                            Owner = "Team - Intake"
                        });
                    }
                }

                _logger.LogInformation($"Received Candidate {item.DlNumber}");

            }

            });

            return Ok();
        }

        /// <summary>
        /// POST: /Icbc/Candidates
        /// </summary>
        /// <param name="newCandidates">List of Candidates to be added to the case management system</param>
        /// <returns></returns>
        [HttpPost("CandidatesError")]
        [SwaggerResponse(200, "The candidates were processed correctly")]
        [SwaggerResponse(400, "The format of the provided data was invalid.  Please refer to the model.")]
        [SwaggerResponse(500, "An unexpected server error occurred while processing. Please retry.")]
        public ActionResult CreateCandidatesError([FromBody] List<NewCandidate> newCandidates)
        {
            throw new Exception("Sample Error.");
        }

        [AllowAnonymous]
        [HttpGet("Cases")]
        public CaseStatus GetCaseStatus(string caseId)
        {
            return new CaseStatus();
        }

    }

}
