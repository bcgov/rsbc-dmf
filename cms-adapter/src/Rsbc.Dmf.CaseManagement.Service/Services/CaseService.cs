using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using static Rsbc.Dmf.CaseManagement.Service.DecisionItem.Types;
using static Rsbc.Dmf.CaseManagement.Service.FlagItem.Types;
using static Rsbc.Dmf.CaseManagement.Service.PdfDocument.Types;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class CaseService : CaseManager.CaseManagerBase
    {
        private readonly ILogger<CaseService> _logger;
        private readonly ICaseManager _caseManager;
        private readonly IDocumentManager _documentManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public CaseService(ILogger<CaseService> logger, ICaseManager caseManager, IDocumentManager documentManager,
            IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _logger = logger;
            _caseManager = caseManager;
            _documentManager = documentManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Create Legacy Case Comment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber;
                driver.Surname = request.Driver.Surname;
            }

            var commentDate = request.CommentDate.ToDateTimeOffset();

            if (commentDate.Year < 1753)
            {
                commentDate = DateTimeOffset.Now;
            }

            string caseIdString = null;
            Guid caseId;

            if (Guid.TryParse(request.CaseId, out caseId))
            {
                caseIdString = caseId.ToString();
            }

            var newComment = new CaseManagement.LegacyComment()
            {
                CaseId = caseIdString,
                CommentText = request.CommentText,
                CommentTypeCode = request.CommentTypeCode,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId,
                Driver = driver,
                CommentDate = commentDate,
                CommentId = request.CommentId
            };

            var result = await _caseManager.CreateLegacyCaseComment(newComment);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = result.ErrorDetail ?? string.Empty;
            }


            return reply;
        }

        /// <summary>
        /// Create ICBC Medical Candidate Comment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateICBCMedicalCandidateComment(LegacyComment request,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber;
                driver.Surname = request.Driver.Surname;
            }

            var commentDate = request.CommentDate.ToDateTimeOffset();

            if (commentDate.Year < 1753)
            {
                commentDate = DateTimeOffset.Now;
            }

            string caseIdString = null;
            Guid caseId;

            if (Guid.TryParse(request.CaseId, out caseId))
            {
                caseIdString = caseId.ToString();
            }

            var newComment = new CaseManagement.LegacyComment()
            {
                CaseId = caseIdString,
                CommentText = request.CommentText,
                CommentTypeCode = request.CommentTypeCode,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId,
                Driver = driver,
                CommentDate = commentDate,
                CommentId = request.CommentId,
                Assignee = request.Assignee ?? string.Empty
            };

            var result = await _caseManager.CreateICBCMedicalCandidateComment(newComment);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = result.ErrorDetail ?? string.Empty;
            }

            return reply;
        }

        public async override Task<CreateStatusReply> CreateUnsolicitedDocumentOnDriver(LegacyDocument request,
            ServerCallContext context)
        {
            return await CreateDocument(request, false, context);
        }

        public async override Task<CreateStatusReply> CreateDocumentOnDriver(LegacyDocument request,
            ServerCallContext context)
        {
            return await CreateDocument(request, true, context);
        }

        private async Task<CreateStatusReply> CreateDocument(LegacyDocument request, bool solicited,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber ?? string.Empty;
                driver.Surname = request.Driver.Surname ?? string.Empty;
                driver.Id = request.Driver.Id ?? string.Empty;
            }

            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId ?? string.Empty,
                CaseId = request.CaseId ?? string.Empty,
                DocumentId = request.DocumentId ?? string.Empty,
                DocumentPages = (int)request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode ?? string.Empty,
                DocumentType = request.DocumentType ?? string.Empty,
                BusinessArea = request.BusinessArea ?? string.Empty,
                DocumentUrl = request.DocumentUrl ?? string.Empty,
                FilenameOverride = request.FilenameOverride ?? string.Empty,
                ImportId = request.ImportId ?? string.Empty,
                OriginatingNumber = request.OriginatingNumber ?? string.Empty,
                ValidationMethod = request.ValidationMethod ?? string.Empty,
                ValidationPrevious = request.ValidationPrevious ?? string.Empty,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId ?? string.Empty,
                Driver = driver,
                Priority = request.Priority ?? string.Empty,
                Owner = request.Owner ?? string.Empty,
                SubmittalStatus = request.SubmittalStatus ?? string.Empty,
                Queue = request.Queue ?? string.Empty,
                DpsDocumentId = request.DpsDocumentId,
                Origin = request.Origin,
                DocumentSubTypeId = request.DocumentSubTypeId ?? string.Empty,
                Solicited = solicited
            };

            if (request.FaxReceivedDate != null)
            {
                newDocument.FaxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset();
            }

            if (request.ImportDate != null)
            {
                newDocument.ImportDate = request.ImportDate.ToDateTimeOffset();
            }

            var result = await _caseManager.CreateDocumentOnDriver(newDocument);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        [Obsolete("CreateLegacyCaseDocument is deprecated, please use CreateDocumentOnDriver instead.", false)]
        /// <summary>
        /// Create Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber ?? string.Empty;
                driver.Surname = request.Driver.Surname ?? string.Empty;
            }


            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId ?? string.Empty,
                CaseId = request.CaseId ?? string.Empty,
                DocumentId = request.DocumentId ?? string.Empty,
                DocumentPages = (int)request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode ?? string.Empty,
                DocumentType = request.DocumentType ?? string.Empty,
                BusinessArea = request.BusinessArea ?? string.Empty,
                DocumentUrl = request.DocumentUrl ?? string.Empty,
                ImportId = request.ImportId ?? string.Empty,
                OriginatingNumber = request.OriginatingNumber ?? string.Empty,
                ValidationMethod = request.ValidationMethod ?? string.Empty,
                ValidationPrevious = request.ValidationPrevious ?? string.Empty,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId ?? string.Empty,
                Driver = driver,
                Priority = request.Priority ?? string.Empty,
                Owner = request.Owner ?? string.Empty,
                SubmittalStatus = request.SubmittalStatus ?? string.Empty,
                Queue = request.Queue ?? string.Empty,
            };

            if (request.FaxReceivedDate != null)
            {
                newDocument.FaxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset();
            }

            if (request.ImportDate != null)
            {
                newDocument.ImportDate = request.ImportDate.ToDateTimeOffset();
            }

            var result = await _caseManager.CreateLegacyCaseDocument(newDocument);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Create Unsolicitated Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateUnsolicitedCaseDocument(LegacyDocument request,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber ?? string.Empty;
                driver.Surname = request.Driver.Surname ?? string.Empty;
            }

            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId ?? string.Empty,
                CaseId = request.CaseId ?? string.Empty,
                DocumentId = request.DocumentId ?? string.Empty,
                DocumentPages = (int)request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode ?? string.Empty,
                DocumentType = request.DocumentType ?? string.Empty,
                BusinessArea = request.BusinessArea ?? string.Empty,
                DocumentUrl = request.DocumentUrl ?? string.Empty,
                ImportId = request.ImportId ?? string.Empty,
                OriginatingNumber = request.OriginatingNumber ?? string.Empty,
                ValidationMethod = request.ValidationMethod ?? string.Empty,
                ValidationPrevious = request.ValidationPrevious ?? string.Empty,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId ?? string.Empty,
                Driver = driver,
                Priority = request.Priority ?? string.Empty,
                Owner = request.Owner ?? string.Empty,
                SubmittalStatus = request.SubmittalStatus ?? string.Empty,
                Queue = request.Queue ?? string.Empty,
            };

            if (request.FaxReceivedDate != null)
            {
                newDocument.FaxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset();
            }

            if (request.ImportDate != null)
            {
                newDocument.ImportDate = request.ImportDate.ToDateTimeOffset();
            }

            var result = await _caseManager.CreateUnsolicitedCaseDocument(newDocument);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Create Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateCaseDocument(LegacyDocument request,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber ?? string.Empty;
                driver.Surname = request.Driver.Surname ?? string.Empty;
            }

            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId ?? string.Empty,
                CaseId = request.CaseId ?? string.Empty,
                DocumentId = request.DocumentId ?? string.Empty,
                DocumentPages = (int)request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode ?? string.Empty,
                DocumentType = request.DocumentType ?? string.Empty,
                BusinessArea = request.BusinessArea ?? string.Empty,
                DocumentUrl = request.DocumentUrl ?? string.Empty,
                ImportId = request.ImportId ?? string.Empty,
                OriginatingNumber = request.OriginatingNumber ?? string.Empty,
                ValidationMethod = request.ValidationMethod ?? string.Empty,
                ValidationPrevious = request.ValidationPrevious ?? string.Empty,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId ?? string.Empty,
                Driver = driver,
                Priority = request.Priority ?? string.Empty,
                Owner = request.Owner ?? string.Empty,
                SubmittalStatus = request.SubmittalStatus ?? string.Empty,
                //Queue = request.Queue ?? string.Empty
            };

            if (request.FaxReceivedDate != null)
            {
                newDocument.FaxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset();
            }

            if (request.ImportDate != null)
            {
                newDocument.ImportDate = request.ImportDate.ToDateTimeOffset();
            }

            var result = await _caseManager.CreateCaseDocument(newDocument);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Create Unsolisitaed Document enevelope
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateICBCDocumentEnvelope(LegacyDocument request,
            ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber ?? string.Empty;
                driver.Surname = request.Driver.Surname ?? string.Empty;
            }


            var faxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset();

            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId ?? string.Empty,
                CaseId = request.CaseId ?? string.Empty,
                DocumentId = request.DocumentId ?? string.Empty,
                DocumentPages = (int)request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode ?? string.Empty,
                DocumentType = request.DocumentType ?? string.Empty,
                BusinessArea = request.BusinessArea ?? string.Empty,
                DocumentUrl = request.DocumentUrl ?? string.Empty,
                ImportId = request.ImportId ?? string.Empty,
                OriginatingNumber = request.OriginatingNumber ?? string.Empty,
                ValidationMethod = request.ValidationMethod ?? string.Empty,
                ValidationPrevious = request.ValidationPrevious ?? string.Empty,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId ?? string.Empty,
                Driver = driver,
                Priority = request.Priority ?? string.Empty,
                Owner = request.Owner ?? string.Empty,
                SubmittalStatus = request.SubmittalStatus ?? string.Empty,
            };

            if (faxReceivedDate != DateTimeOffset.MinValue)
            {
                newDocument.FaxReceivedDate = faxReceivedDate;
            }

            if (request.ImportDate != null)
            {
                newDocument.ImportDate = request.ImportDate.ToDateTimeOffset();
            }

            var result = await _caseManager.CreateICBCDocumentEnvelope(newDocument);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Delete Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> DeleteComment(CommentIdRequest request, ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply() { ResultStatus = ResultStatus.Fail };

            // fetch the document.
            try
            {
                var d = await _caseManager.GetComment(request.CommentId);
                if (d != null)
                {
                    if (await _caseManager.DeleteComment(request.CommentId))
                    {
                        reply.ResultStatus = ResultStatus.Success;
                    }
                }
                else
                {
                    reply.ErrorDetail = "Comment ID not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }


            return reply;
        }

        /// <summary>
        /// Delete Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> DeleteLegacyCaseDocument(LegacyDocumentRequest request,
            ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply() { ResultStatus = ResultStatus.Fail };

            // fetch the document.
            try
            {
                var d = await _documentManager.GetLegacyDocument(request.DocumentId);
                if (d != null)
                {
                    if (await _caseManager.DeactivateLegacyDocument(request.DocumentId))
                    {
                        reply.ResultStatus = ResultStatus.Success;
                    }
                }
                else
                {
                    reply.ErrorDetail = "Document ID not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }


            return reply;
        }

        public async override Task<GetCasesReply> GetCases(CaseStatusRequest caseStatusRequest,
            ServerCallContext context)
        {
            var reply = new GetCasesReply();

            try
            {
                var driverId = Guid.Parse(caseStatusRequest.DriverId);
                var activeStatus = caseStatusRequest.Status.Convert<EntityState, Dynamics.EntityState>();
                var cases = await _caseManager.GetCases(driverId, activeStatus);
                if (cases == null)
                {
                    reply.ErrorDetail = "No cases match driver ID";
                    return reply;
                }

                var mappedCases = _mapper.Map<IEnumerable<CaseDetail>>(cases);
                reply.Items.AddRange(mappedCases);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        public async override Task<GetCaseDetailReply> GetCaseDetail(CaseIdRequest request, ServerCallContext context)
        {
            var reply = new GetCaseDetailReply() { ResultStatus = ResultStatus.Fail };

            try
            {
                var c = await _caseManager.GetCaseDetail(request.CaseId);
                if (c != null)
                {
                    // TODO use automapper
                    // add null DateTimeOffset converter in AutoMapper (null to DateTimeOffset.MinValue)
                    reply.Item = new CaseDetail();
                    reply.Item.CaseSequence = c.CaseSequence;
                    reply.Item.CaseId = c.CaseId;
                    reply.Item.Title = c.Title ?? string.Empty;
                    reply.Item.IdCode = c.IdCode ?? string.Empty;
                    reply.Item.OpenedDate = Timestamp.FromDateTimeOffset(c.OpenedDate);
                    reply.Item.CaseType = c.CaseType ?? string.Empty;
                    reply.Item.DmerType = c.DmerType ?? string.Empty;
                    reply.Item.Status = c.Status ?? string.Empty;
                    reply.Item.AssigneeTitle = c.AssigneeTitle ?? string.Empty;
                    reply.Item.LastActivityDate = Timestamp.FromDateTimeOffset(c.LastActivityDate);
                    if (c.DecisionDate == null)
                    {
                        reply.Item.DecisionDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue);
                    }
                    else
                    {
                        reply.Item.DecisionDate = Timestamp.FromDateTimeOffset(c.DecisionDate.Value);
                    }

                    reply.Item.LatestDecision = c.LatestDecision ?? string.Empty;
                    reply.Item.DecisionForClass = c.DecisionForClass ?? string.Empty;
                    reply.Item.DpsProcessingDate = Timestamp.FromDateTimeOffset(c.DpsProcessingDate);
                    reply.Item.LatestComplianceDate = Timestamp.FromDateTimeOffset(c.LatestComplianceDate);

                    // Medical Conditions
                    // reply.Item.MedicalConditions = c.MedicalConditions;
                    
                    // Driver
                    reply.Item.DriverId = c.DriverId;
                    reply.Item.Name = c.Name ?? string.Empty;
                    reply.Item.DriverLicenseNumber = c.DriverLicenseNumber ?? string.Empty;
                    reply.Item.BirthDate = c.BirthDate?.ToTimestamp() ?? DateTimeOffset.MinValue.ToTimestamp();
                    reply.Item.LastName = c.LastName ?? string.Empty;
                    reply.Item.FirstName = c.FirstName ?? string.Empty;
                    reply.Item.Middlename = c.LastName ?? string.Empty;
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ErrorDetail = "Case ID not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        public async override Task<GetCaseDetailReply> GetCaseByIdCode(GetCaseByIdCodeRequest request,
            ServerCallContext context)
        {
            var reply = new GetCaseDetailReply() { ResultStatus = ResultStatus.Fail };

            try
            {
                var c = await _caseManager.GetCaseByIdCode(request.IdCode);
                if (c != null)
                {
                    reply.Item = _mapper.Map<CaseDetail>(c);
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ErrorDetail = "Case Number not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetCaseDetailReply> GetMostRecentCaseDetail(DriverIdRequest request,
            ServerCallContext context)
        {
            var reply = new GetCaseDetailReply() { ResultStatus = ResultStatus.Fail };

            try
            {
                var driverId = Guid.Parse(request.Id);
                var c = await _caseManager.GetMostRecentCaseDetail(driverId);

                if (c != null)
                {
                    reply.Item = _mapper.Map<CaseDetail>(c);
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ErrorDetail = "Case ID not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Get Case Comments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetCommentsReply> GetComments(CommentsRequest request, ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                CaseManagement.OriginRestrictions originRestrictions = CaseManagement.OriginRestrictions.None;
                switch (request.OriginRestrictions)
                {
                    case OriginRestrictions.None:
                        originRestrictions = CaseManagement.OriginRestrictions.None;
                        break;
                    case OriginRestrictions.UserOnly:
                        originRestrictions = CaseManagement.OriginRestrictions.UserOnly;
                        break;
                    case OriginRestrictions.SystemOnly:
                        originRestrictions = CaseManagement.OriginRestrictions.SystemOnly;
                        break;
                }

                IEnumerable<CaseManagement.LegacyComment> result;

                if (originRestrictions == CaseManagement.OriginRestrictions.SystemOnly)
                {
                    result = await _caseManager.GetDriverLegacyComments(request.DriverId, false, originRestrictions);
                }
                else
                {
                    result = await _caseManager.GetCaseLegacyComments(request.CaseId, false, originRestrictions);
                }


                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }

                    reply.Items.Add(new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)(item.SequenceNumber ?? -1),
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty,
                        SignatureName = item.SignatureName ?? string.Empty
                    });
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Get Case Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDocumentsReply> GetCaseDocuments(CaseIdRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentsReply();
            try
            {
                var result = await _documentManager.GetCaseLegacyDocuments(request.CaseId);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber ?? string.Empty;
                        driver.Surname = item.Driver.Surname ?? string.Empty;
                    }

                    var newDocument = new LegacyDocument
                    {
                        BatchId = item.BatchId ?? string.Empty,
                        DocumentPages = item.DocumentPages,
                        DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,
                        CaseId = item.CaseId ?? string.Empty,
                        ImportId = item.ImportId ?? string.Empty,
                        OriginatingNumber = item.OriginatingNumber ?? string.Empty,
                        DocumentId = item.DocumentId ?? string.Empty,
                        SequenceNumber = (long)(item.SequenceNumber ?? -1),
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        DocumentUrl = item.DocumentUrl ?? string.Empty,
                        ValidationMethod = item.ValidationMethod ?? string.Empty,
                        ValidationPrevious = item.ValidationPrevious ?? string.Empty
                    };

                    if (newDocument.FaxReceivedDate != null)
                    {
                        newDocument.FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate.Value);
                    }

                    if (newDocument.ImportDate != null)
                    {
                        newDocument.ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate.Value);
                    }

                    reply.Items.Add(newDocument);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, $"GetCaseDocuments {request.CaseId} Error");
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }


        public async override Task<GetCommentReply> GetComment(CommentIdRequest request, ServerCallContext context)
        {
            var reply = new GetCommentReply();
            try
            {
                var item = await _caseManager.GetComment(request.CommentId);

                if (item != null)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }

                    reply.Item = new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)(item.SequenceNumber ?? 0),
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty,
                        SignatureName = item.SignatureName ?? string.Empty
                    };
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Get Driver Comments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 
        public async override Task<GetCommentsReply> GetDriverComments(DriverLicenseRequest request,
            ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                var result = await _caseManager.GetDriverLegacyComments(request.DriverLicenseNumber, false);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }

                    reply.Items.Add(new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)item.SequenceNumber,
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty,
                        SignatureName = item.SignatureName ?? string.Empty
                    });
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        public async override Task<GetCasesReply> GetActiveCases(DriverLicenseRequest request,
            ServerCallContext context)
        {
            var reply = new GetCasesReply();
            try
            {
                var driver =
                    (await _caseManager.GetDriverByLicenseNumber(request.DriverLicenseNumber)).FirstOrDefault();
                if (driver == null)
                {
                    reply.ErrorDetail = "Driver not found.";
                    reply.ResultStatus = ResultStatus.Fail;
                }
                else
                {
                    var activeStatus = Dynamics.EntityState.Active;
                    var cases = await _caseManager.GetCases(Guid.Parse(driver.Id), activeStatus);
                    if (cases == null)
                    {
                        reply.ResultStatus = ResultStatus.Fail;
                        reply.ErrorDetail = "No cases";
                        return reply;
                    }

                    var mappedCases = _mapper.Map<IEnumerable<CaseDetail>>(cases);
                    reply.Items.AddRange(mappedCases);
                    reply.ResultStatus = ResultStatus.Success;
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Get All Driver Comments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetCommentsReply> GetAllDriverComments(DriverLicenseRequest request,
            ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                var result = await _caseManager.GetDriverLegacyComments(request.DriverLicenseNumber, true);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }

                    reply.Items.Add(new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)item.SequenceNumber,
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty,
                        SignatureName = item.SignatureName ?? string.Empty
                    });
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Get Driver Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDocumentsReply> GetDriverDocumentsById(DriverIdRequest request,
            ServerCallContext context)
        {
            var reply = new GetDocumentsReply();

            try
            {
                var result = await _caseManager.GetDriverLegacyDocuments(Guid.Parse(request.Id));
                var documents = _mapper.Map<IEnumerable<LegacyDocument>>(result);
                reply.Items.AddRange(documents);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        public async override Task<GetDocumentsReply> GetIcbcDmerEnvelopes(DriverLicenseRequest request,
            ServerCallContext context)
        {
            var reply = new GetDocumentsReply();
            try
            {
                var result = await _documentManager.GetDriverLegacyDocuments(request.DriverLicenseNumber, true);
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.DocumentUrl) && item.DocumentType == "DMER" &&
                        item.SubmittalStatus == "Open-Required")
                    {
                        var driver = new Driver();
                        if (item.Driver != null)
                        {
                            driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                            driver.Surname = item.Driver.Surname ?? string.Empty;
                        }

                        // TODO use automapper, see CaseService.GetDriverDocumentsById
                        var newDocument = new LegacyDocument
                        {
                            BatchId = item.BatchId,
                            BusinessArea = item.BusinessArea,
                            CaseId = item.CaseId ?? string.Empty,
                            DocumentPages = item.DocumentPages,
                            DocumentId = item.DocumentId,
                            DocumentType = item.DocumentType ?? string.Empty,
                            DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,
                            DocumentUrl = item.DocumentUrl ?? string.Empty,
                            ImportId = item.ImportId ?? string.Empty,
                            OriginatingNumber = item.OriginatingNumber ?? string.Empty,
                            ValidationMethod = item.ValidationMethod ?? string.Empty,
                            ValidationPrevious = item.ValidationPrevious ?? string.Empty,
                            SequenceNumber = item.SequenceNumber ?? -1,
                            Driver = driver,
                            SubmittalStatus = item.SubmittalStatus ?? string.Empty,
                        };

                        if (item.FaxReceivedDate != null)
                        {
                            newDocument.FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate.Value);
                        }

                        if (item.ImportDate != null)
                        {
                            newDocument.ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate.Value);
                        }

                        reply.Items.Add(newDocument);
                    }
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Get Driver Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDocumentsReply> GetDriverDocuments(DriverLicenseRequest request,
            ServerCallContext context)
        {
            var reply = new GetDocumentsReply();
            try
            {
                var result = await _documentManager.GetDriverLegacyDocuments(request.DriverLicenseNumber, false);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname ?? string.Empty;
                    }

                    // TODO use automapper, see CaseService.GetDriverDocumentsById
                    var newDocument = new LegacyDocument
                    {
                        BatchId = item.BatchId,
                        BusinessArea = item.BusinessArea,
                        CaseId = item.CaseId ?? string.Empty,
                        DocumentPages = item.DocumentPages,
                        DocumentId = item.DocumentId,
                        DocumentType = item.DocumentType ?? string.Empty,
                        DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,
                        DocumentUrl = item.DocumentUrl ?? string.Empty,
                        ImportId = item.ImportId ?? string.Empty,
                        OriginatingNumber = item.OriginatingNumber ?? string.Empty,
                        ValidationMethod = item.ValidationMethod ?? string.Empty,
                        ValidationPrevious = item.ValidationPrevious ?? string.Empty,
                        SequenceNumber = item.SequenceNumber ?? -1,
                        Driver = driver,
                        SubmittalStatus = item.SubmittalStatus ?? string.Empty,
                    };

                    if (item.FaxReceivedDate != null)
                    {
                        newDocument.FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate.Value);
                    }

                    if (item.ImportDate != null)
                    {
                        newDocument.ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate.Value);
                    }

                    reply.Items.Add(newDocument);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Get Drivers
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDriversReply> GetDrivers(EmptyRequest request, ServerCallContext context)
        {
            var reply = new GetDriversReply();
            try
            {
                var result = await _caseManager.GetDrivers();

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item != null && item.DriverLicenseNumber != null)
                    {
                        driver.DriverLicenseNumber = item.DriverLicenseNumber;
                        driver.Surname = item.Surname ?? string.Empty;
                        driver.BirthDate = Timestamp.FromDateTime(item.BirthDate.ToUniversalTime());
                    }

                    reply.Items.Add(driver);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        // Get Driver With BirthDate by linking driver to person
        public async override Task<GetDriversReply> GetDriverPerson(DriverLicenseRequest request,
            ServerCallContext context)
        {
            return await GetDriver(request, true);
        }

        [Obsolete("Use GetDriverPerson method instead.")]
        public async override Task<GetDriversReply> GetDriver(DriverLicenseRequest request, ServerCallContext context)
        {
            return await GetDriver(request, false);
        }

        private async Task<GetDriversReply> GetDriver(DriverLicenseRequest request, bool includeBirthdate)
        {
            var reply = new GetDriversReply();
            try
            {
                var result = await _caseManager.GetDriverByLicenseNumber(request.DriverLicenseNumber);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item != null && item.DriverLicenseNumber != null)
                    {
                        driver.DriverLicenseNumber = item.DriverLicenseNumber;
                        driver.Surname = item.Surname ?? string.Empty;
                        driver.GivenName = item.GivenName ?? string.Empty;
                        driver.Id = item.Id;
                        if (includeBirthdate)
                        {
                            driver.BirthDate = Timestamp.FromDateTime(item.BirthDate.ToUniversalTime());
                        }
                    }

                    reply.Items.Add(driver);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Get Driver
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDriversReply> GetDriverById(DriverIdRequest request, ServerCallContext context)
        {
            var reply = new GetDriversReply();
            try
            {
                var result = await _caseManager.GetDriverById(request.Id);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item != null && item.DriverLicenseNumber != null)
                    {
                        driver.DriverLicenseNumber = item.DriverLicenseNumber;
                        driver.Surname = item.Surname ?? string.Empty;
                        driver.GivenName = item.GivenName ?? string.Empty;
                        driver.Id = item.Id;
                    }

                    reply.Items.Add(driver);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }


        /// <summary>
        /// Process Legacy Candidate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<LegacyCandidateReply> ProcessLegacyCandidate(LegacyCandidateRequest request,
            ServerCallContext context)
        {
            var reply = new LegacyCandidateReply();

            // start by checking to see if there is an existing case.

            try
            {
                var searchRequest = new LegacyCandidateSearchRequest()
                {
                    DriverLicenseNumber = request.LicenseNumber,
                    Surname = request.Surname ?? string.Empty
                };
                var searchResult = await _caseManager.LegacyCandidateSearch(searchRequest);

                bool found = false;
                if (searchResult != null && searchResult.Items.Count() > 0)
                {
                    var closedStatus = new HashSet<string>
                    {
                        // check for state code wether it is resolved or cancelled
                        "Decision Rendered",
                        "Canceled"
                    };

                    foreach (var item in searchResult.Items)
                    {
                        if (!closedStatus.Contains(item.Status))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    // case exists.
                    reply.ResultStatus = ResultStatus.Success;
                    reply.IsNewCase = false;
                }

                else
                {
                    DateTimeOffset? dto = null;
                    if (request.EffectiveDate != null)
                    {
                        dto = request.EffectiveDate.ToDateTimeOffset();
                    }

                    DateTimeOffset? birthdate = null;

                    if (request.BirthDate != null)
                    {
                        birthdate = request.BirthDate.ToDateTimeOffset();
                    }

                    // create the case.
                    await _caseManager.LegacyCandidateCreate(searchRequest, birthdate, dto, "ProcessLegacyCandidate",
                        request.MedicalType);

                    reply.ResultStatus = ResultStatus.Success;
                    reply.IsNewCase = true;
                }
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }

        /// <summary>
        /// Resolve Case Status Updates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> ResolveCaseStatusUpdates(EmptyRequest request,
            ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                // call case manager
                await _caseManager.ResolveCaseStatusUpdates();
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Update Non Comply status 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateNonComplyDocuments(EmptyRequest request,
            ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                // call case manager
                await _caseManager.UpdateNonComplyDocuments();
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Get List OfLettersSentToBcMail
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<PdfDocumentReply> GetPdfDocuments(EmptyRequest request, ServerCallContext context)
        {
            var reply = new PdfDocumentReply();

            try
            {
                var result = await _caseManager.GetPdfDocuments();

                foreach (var item in result)
                {
                    var pdfDocument = new PdfDocument()
                    {
                        PdfDocumentId = item.PdfDocumentId,
                        Filename = item.Filename,
                        ServerUrl = item.ServerUrl,
                        StatusCode = ConvertStatusCode(item.StatusCode)
                    };
                    reply.PdfDocuments.Add(pdfDocument);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<PdfDocumentReply> UpdateDocumentStatus(PdfDocument request,
            ServerCallContext context)
        {
            var reply = new PdfDocumentReply();

            var pdfDocument = new CaseManagement.PdfDocument()
            {
                PdfDocumentId = request.PdfDocumentId,
                StatusCode = ConvertPdfDocumentStatusCodes(request.StatusCode)
            };

            //PdfDocumentId = request.PdfDoumentId,
            //StatusCode = ConvertPdfDocumentStatusCodes(request.StatusCode)
            //StatusCode = ConvertPdfDocumentStatusCodes()


            try
            {
                if (pdfDocument != null)
                {
                    await _caseManager.UpdatePdfDocumentStatus(pdfDocument);
                    reply.ResultStatus = ResultStatus.Success;
                }
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        StatusCodeOptionSet ConvertPdfDocumentStatusCodes(StatusCodeOptions value)
        {
            StatusCodeOptionSet result = StatusCodeOptionSet.SendToBCMail;
            switch (value)
            {
                case StatusCodeOptions.Sent:
                    result = StatusCodeOptionSet.Sent;
                    break;
                case StatusCodeOptions.FailedToSend:
                    result = StatusCodeOptionSet.FailedToSend;
                    break;
            }

            return result;
        }


        /// <summary>
        /// Resolve Birthdate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateBirthDate(UpdateDriverRequest request,
            ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            var driverRequest = new CaseManagement.UpdateDriverRequest
            {
                BirthDate = request.BirthDate.ToDateTime(),
                DriverLicenseNumber = request.DriverLicenseNumber,
            };

            try
            {
                if (request.BirthDate != null)
                {
                    // call case manager
                    await _caseManager.UpdateBirthDate(driverRequest);

                    reply.ResultStatus = ResultStatus.Success;
                }
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Case Search
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            var reply = new SearchReply();

            try
            {
                var cases = (await _caseManager.CaseSearch(new CaseSearchRequest
                {
                    CaseId = request.CaseId,
                    Title = request.Title,
                    ClinicId = request.ClinicId,
                    DriverLicenseNumber = request.DriverLicenseNumber
                })).Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

                reply.Items.Add(cases.Select(c =>
                {
                    Provider provider = null;
                    if (c.Provider != null)
                    {
                        provider = new Provider()
                        {
                            Id = c.Provider.Id,
                            Address = new Address()
                            {
                                City = c.Provider.Address.City ?? string.Empty,
                                Postal = c.Provider.Address.Postal ?? string.Empty,
                                Line1 = c.Provider.Address.Line1 ?? string.Empty,
                                Line2 = c.Provider.Address.Line2 ?? string.Empty,
                            },
                            FaxNumber = c.Provider.FaxNumber ?? string.Empty,
                            FaxUseType = c.Provider.FaxUseType ?? string.Empty,
                            GivenName = c.Provider.GivenName ?? string.Empty,
                            Surname = c.Provider.Surname ?? string.Empty,
                            Name = c.Provider.Name ?? string.Empty,
                            PhoneExtension = c.Provider.PhoneExtension ?? string.Empty,
                            PhoneNumber = c.Provider.PhoneNumber ?? string.Empty,
                            PhoneUseType = c.Provider.PhoneUseType ?? string.Empty,
                            ProviderDisplayId = c.Provider.ProviderDisplayId ?? string.Empty,
                            ProviderDisplayIdType = c.Provider.ProviderDisplayIdType ?? string.Empty,
                            ProviderRole = c.Provider.ProviderRole ?? string.Empty,
                            ProviderSpecialty = c.Provider.ProviderSpecialty ?? string.Empty
                        };
                    }

                    Driver driver = null;
                    if (c.Driver != null && c.Driver.Id != null) // only create a driver if it is a valid record
                    {
                        driver = new Driver
                        {
                            Id = c.Driver.Id,
                            Surname = c.Driver.Surname ?? string.Empty,
                            Middlename = c.Driver.Middlename ?? string.Empty,
                            GivenName = c.Driver.GivenName ?? string.Empty,
                            BirthDate = Timestamp.FromDateTime(c.Driver.BirthDate.ToUniversalTime()),
                            DriverLicenseNumber = c.Driver.DriverLicenseNumber ?? string.Empty,
                            Address = new Address()
                            {
                                City = c.Driver.Address.City ?? string.Empty,
                                Postal = c.Driver.Address.Postal ?? string.Empty,
                                Line1 = c.Driver.Address.Line1 ?? string.Empty,
                                Line2 = c.Driver.Address.Line2 ?? string.Empty,
                            },
                            Sex = c.Driver.Sex ?? string.Empty,
                            Name = c.Driver.Name ?? string.Empty
                        };
                    }

                    var newCase = new DmerCase
                    {
                        CaseId = c.Id,
                        Title = c.Title ?? string.Empty,
                        CreatedBy = c.CreatedBy ?? string.Empty,
                        CreatedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                        ModifiedBy = c.CreatedBy ?? string.Empty,
                        ModifiedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                        Driver = driver,
                        Provider = provider,
                        IsCommercial = c.IsCommercial,
                        ClinicName = c.ClinicName ?? string.Empty,
                        Status = c.Status,
                        DmerType = c.DmerType ?? string.Empty,
                        CaseSequence = c.CaseSequence
                    };
                    newCase.Flags.Add(c.Flags.Select(f => new FlagItem
                    {
                        Identifier = f.Id,
                        Question = f.Description ?? "Unknown",
                        FlagType = ConvertFlagType(f.FlagType),
                        FormId = f.FormId ?? ""
                    }));
                    newCase.MedicalConditions.Add(
                        c.MedicalConditions.Select(m => new MedicalConditionItem
                        {
                            Identifier = m.Id,
                            Question = m.Description ?? "Unknown",
                            FormId = m.FormId ?? ""
                        }));
                    return newCase;
                }));
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting case {request.CaseId}.");
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> CreateCase(CreateCaseRequest request, ServerCallContext context)
        {
            var reply = new ResultStatusReply();


            try
            {
                var caseCreateRequest = new CaseManagement.CreateCaseRequest()
                {
                    CaseId = request.CaseId,
                };


                var createDriver = await _caseManager.CreateCase(caseCreateRequest);


                reply.ResultStatus = ResultStatus.Success;
            }

            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while updating case.");
                reply.ErrorDetail = e.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }


        /// <summary>
        /// Update Case
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<UpdateCaseReply> UpdateCase(UpdateCaseRequest request, ServerCallContext context)
        {
            var reply = new UpdateCaseReply();
            try
            {
                _logger.LogInformation(
                    $"UPDATE CASE - {request.CaseId}, clean pass is {request.IsCleanPass}, files - {request.DataFileKey} {request.PdfFileKey}");

                // convert the flags to a list of strings.

                List<Flag> flags = new List<Flag>();

                foreach (var item in request.Flags)
                {
                    Flag newFlag = new Flag()
                    {
                        Description = item.Question,
                        Id = item.Identifier,
                        FormId = item.FormId
                    };
                    flags.Add(newFlag);
                    _logger.LogInformation($"Added flag {item.Question} to flags for set case flags.");
                }

                // set the flags.

                var x = await _caseManager.SetCaseFlags(request.CaseId, request.IsCleanPass, flags);
                _logger.LogInformation($"Set Flags result is {x.Success}.");

                // update files.

                _logger.LogInformation(
                    $"Add file - {request.CaseId}, files - {request.DataFileKey} {request.PdfFileKey}");

                if (!string.IsNullOrEmpty(request.PdfFileKey))
                {
                    await _caseManager.AddDocumentUrlToCaseIfNotExist(request.CaseId, request.PdfFileKey,
                        request.PdfFileSize);
                }

                if (!string.IsNullOrEmpty(request.DataFileKey))
                {
                    await _caseManager.AddDocumentUrlToCaseIfNotExist(request.CaseId, request.DataFileKey,
                        request.DataFileSize);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while updating case.");
                reply.ErrorDetail = e.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        /// <summary>
        /// Create Decision
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> CreateDecision(LegacyDecision request, ServerCallContext context)
        {
            var reply = new ResultStatusReply() { ResultStatus = ResultStatus.Fail };
            try
            {
                var createDecisionRequest = new CaseManagement.CreateDecisionRequest
                {
                    CaseId = request.CaseId,
                    DriverId = request.DriverId,
                    OutcomeText = request.OutcomeText,
                    StatusDate = request.StatusDate.ToDateTimeOffset(),
                    SubOutcomeText = request.SubOutcomeText
                };

                var createDecisionResult = await _caseManager.CreateDecision(createDecisionRequest);
                if (createDecisionResult.Success)
                {
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ErrorDetail = createDecisionResult.ErrorDetail ?? "unknown error";
                }
            }
            catch (Exception e)
            {
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        public async override Task<CreateDriverReply> CreateDriverPerson(CreateDriverPersonRequest request,
            ServerCallContext context)
        {
            var createDriverRequest = new CreateDriverRequest();
            createDriverRequest.BirthDate = request.BirthDate;
            createDriverRequest.DriverLicenseNumber = request.DriverLicenseNumber;
            createDriverRequest.Surname = request.Surname;
            createDriverRequest.GivenName = request.GivenName;

            var reply = await CreateDriver(createDriverRequest, false);
            if (reply.ResultStatus == ResultStatus.Success && !string.IsNullOrEmpty(request.LoginId))
            {
                var loginId = Guid.Parse(request.LoginId);
                var driverId = Guid.Parse(reply.DriverId);
                var linkReply = await _caseManager.LinkLoginToDriver(loginId, driverId);
                reply.ResultStatus = linkReply.Success ? ResultStatus.Success : ResultStatus.Fail;
            }

            return reply;
        }

        [Obsolete("Use CreateDriverPerson method instead.")]
        public async override Task<ResultStatusReply> CreateDriver(CreateDriverRequest request,
            ServerCallContext context)
        {
            var reply = await CreateDriver(request, true);
            var resultStatusReply = new ResultStatusReply();
            resultStatusReply.ResultStatus = reply.ResultStatus;
            resultStatusReply.ErrorDetail = reply.ErrorDetail;
            return resultStatusReply;
        }

        private async Task<CreateDriverReply> CreateDriver(CreateDriverRequest request, bool tryGetDriver)
        {
            var reply = new CreateDriverReply() { ResultStatus = ResultStatus.Fail };

            try
            {
                // start by getting the driver.
                bool isChange = false;
                if (tryGetDriver)
                {
                    var drivers = await _caseManager.GetDriverByLicenseNumber(request.DriverLicenseNumber);

                    // check the drivers.
                    foreach (var driver in drivers)
                    {
                        if (driver.Surname != request.Surname || driver.BirthDate != request.BirthDate.ToDateTime())
                        {
                            isChange = true;
                        }
                    }
                }

                if (!isChange) // create
                {
                    var createDriverRequest = new CaseManagement.CreateDriverRequest
                    {
                        DriverLicenseNumber = request.DriverLicenseNumber,
                        Surname = request.Surname,
                        GivenName = request.GivenName
                    };
                    if (request.BirthDate != null)
                    {
                        try
                        {
                            createDriverRequest.BirthDate = request.BirthDate.ToDateTime();
                        }
                        catch (Exception e)
                        {
                            createDriverRequest.BirthDate = null;
                        }
                    }

                    var createDriverResult = await _caseManager.CreateDriver(createDriverRequest);
                    if (createDriverResult.Success)
                    {
                        reply.DriverId = createDriverResult.Id;
                        reply.ResultStatus = ResultStatus.Success;
                    }
                    else
                    {
                        reply.ErrorDetail = createDriverResult.ErrorDetail ?? "unknown error";
                    }
                }
                else // update
                {
                }
            }
            catch (Exception e)
            {
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Update Driver
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateDriver(Driver request, ServerCallContext context)
        {
            var reply = new ResultStatusReply() { ResultStatus = ResultStatus.Fail };

            try
            {
                // start by getting the driver.

                var drivers = await _caseManager.GetDriverByLicenseNumber(request.DriverLicenseNumber);

                // check the drivers.

                bool isChange = false;

                foreach (var driver in drivers)
                {
                    if (driver.Surname != request.Surname || driver.BirthDate != request.BirthDate.ToDateTime())
                    {
                        isChange = true;
                    }
                }

                if (isChange)
                {
                    var updateStatus = await _caseManager.UpdateDriver(new CaseManagement.Driver
                    {
                        DriverLicenseNumber = request.DriverLicenseNumber,
                        BirthDate = request.BirthDate.ToDateTime(),
                        GivenName = request.GivenName,
                        Surname = request.Surname
                    });
                    if (updateStatus.Success)
                    {
                        reply.ResultStatus = ResultStatus.Success;
                    }
                    else
                    {
                        reply.ErrorDetail = updateStatus.ErrorDetail ?? "unknown error";
                    }
                }
            }
            catch (Exception e)
            {
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }

        /// <summary>
        /// Convert Flag Type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        FlagTypeOptions ConvertFlagType(FlagTypeOptionSet? value)
        {
            FlagTypeOptions result = FlagTypeOptions.Unknown;
            switch (value)
            {
                case FlagTypeOptionSet.FollowUp:
                    result = FlagTypeOptions.FollowUp;
                    break;
                case FlagTypeOptionSet.Message:
                    result = FlagTypeOptions.Message;
                    break;
                case FlagTypeOptionSet.Review:
                    result = FlagTypeOptions.Review;
                    break;
                case FlagTypeOptionSet.Submittal:
                    result = FlagTypeOptions.Submittal;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Convert Status Code
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        StatusCodeOptions ConvertStatusCode(StatusCodeOptionSet? value)
        {
            StatusCodeOptions result = StatusCodeOptions.SendToBcmail;
            switch (value)
            {
                case StatusCodeOptionSet.Sent:
                    result = StatusCodeOptions.Sent;
                    break;
                case StatusCodeOptionSet.FailedToSend:
                    result = StatusCodeOptions.FailedToSend;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Set Case Practitioner Clinic
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<SetCasePractitionerClinicReply> SetCasePractitionerClinic(
            SetCasePractitionerClinicRequest request, ServerCallContext context)
        {
            var reply = new SetCasePractitionerClinicReply();

            // call the case manager to update data
            try
            {
                await _caseManager.SetCasePractitionerClinic(request.ClinicId, request.PractitionerId,
                    request.ClinicId);
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }


        DecisionOutcomeOptions ConvertDecisionOutcome(DecisionOutcome? value)
        {
            DecisionOutcomeOptions result = DecisionOutcomeOptions.Unknown;
            switch (value)
            {
                case DecisionOutcome.NonComply:
                    result = DecisionOutcomeOptions.NonComply;
                    break;
                case DecisionOutcome.FitToDrive:
                    result = DecisionOutcomeOptions.FitToDrive;
                    break;
                case DecisionOutcome.UnfitToDrive:
                    result = DecisionOutcomeOptions.UnfitToDrive;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get All Flags
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetAllFlagsReply> GetAllFlags(EmptyRequest request, ServerCallContext context)
        {
            var reply = new GetAllFlagsReply();
            var flags = await _caseManager.GetAllFlags();
            foreach (var flag in flags)
            {
                FlagItem newFlag = new FlagItem()
                {
                    Identifier = flag.Id,
                    Question = flag.Description ?? "",
                    FlagType = ConvertFlagType(flag.FlagType),
                    FormId = flag.FormId ?? ""
                };
                reply.Flags.Add(newFlag);
            }

            return reply;
        }

        /// <summary>
        /// Get All Medical Conditions
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetAllMedicalConditionsReply> GetAllMedicalConditions(EmptyRequest request,
            ServerCallContext context)
        {
            var reply = new GetAllMedicalConditionsReply();
            var flags = await _caseManager.GetAllMedicalConditions();
            foreach (var flag in flags)
            {
                MedicalConditionItem newFlag = new MedicalConditionItem()
                {
                    Identifier = flag.Id,
                    Question = flag.Description ?? "",
                    FormId = flag.FormId ?? "",
                };
                reply.MedicalConditions.Add(newFlag);
            }

            return reply;
        }

        /// <summary>
        /// Get Unsent Medical Updates for clean pass and manual pass
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<SearchReply> GetUnsentMedicalPass(EmptyRequest request, ServerCallContext context)
        {
            var data = await _caseManager.GetUnsentMedicalPass();

            var cases = data.Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

            SearchReply reply = new();

            reply.Items.Add(cases.Select(c =>
            {
                Driver driver = null;
                if (c.Driver != null && c.Driver.Id != null)
                {
                    driver = new Driver()
                    {
                        Id = c.Driver.Id ?? string.Empty,
                        Surname = c.Driver.Surname ?? string.Empty,
                        GivenName = c.Driver.GivenName ?? string.Empty,
                        BirthDate = Timestamp.FromDateTime(c.Driver.BirthDate.ToUniversalTime()),
                        DriverLicenseNumber = c.Driver.DriverLicenseNumber ?? string.Empty,
                        Address = new Address()
                        {
                            City = c.Driver.Address.City ?? string.Empty,
                            Postal = c.Driver.Address.Postal ?? string.Empty,
                            Line1 = c.Driver.Address.Line1 ?? string.Empty,
                            Line2 = c.Driver.Address.Line2 ?? string.Empty,
                        },
                        Sex = c.Driver.Sex ?? string.Empty,
                        Name = c.Driver.Name ?? string.Empty
                    };
                }

                var newCase = new DmerCase
                {
                    CaseId = c.Id,
                    Title = c.Title ?? string.Empty,
                    CreatedBy = c.CreatedBy ?? string.Empty,
                    CreatedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    ModifiedBy = c.CreatedBy ?? string.Empty,
                    ModifiedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    Driver = driver,
                    IsCommercial = c.IsCommercial,
                    ClinicName = c.ClinicName ?? string.Empty,
                    Status = c.Status
                };

                newCase.Decisions.Add(c.Decisions.Select(d => new DecisionItem
                {
                    Identifier = d.Id,
                    Outcome = ConvertDecisionOutcome(d.Outcome),
                    CreatedOn = Timestamp.FromDateTime(d.CreatedOn.DateTime.ToUniversalTime())
                }));

                return newCase;
            }));
            reply.ResultStatus = ResultStatus.Success;
            return reply;
        }

        /// <summary>
        /// Get Unsent Medical Updates for adjudication
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<SearchReply> GetUnsentMedicalAdjudication(EmptyRequest request,
            ServerCallContext context)
        {
            var data = await _caseManager.GetUnsentMedicalAdjudication();

            var cases = data.Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

            SearchReply reply = new();

            reply.Items.Add(cases.Select(c =>
            {
                Driver driver = null;
                if (c.Driver != null && c.Driver.Id != null)
                {
                    driver = new Driver()
                    {
                        Id = c.Driver.Id ?? string.Empty,
                        Surname = c.Driver.Surname ?? string.Empty,
                        GivenName = c.Driver.GivenName ?? string.Empty,
                        BirthDate = Timestamp.FromDateTime(c.Driver.BirthDate.ToUniversalTime()),
                        DriverLicenseNumber = c.Driver.DriverLicenseNumber ?? string.Empty,
                        Address = new Address()
                        {
                            City = c.Driver.Address.City ?? string.Empty,
                            Postal = c.Driver.Address.Postal ?? string.Empty,
                            Line1 = c.Driver.Address.Line1 ?? string.Empty,
                            Line2 = c.Driver.Address.Line2 ?? string.Empty,
                        },
                        Sex = c.Driver.Sex ?? string.Empty,
                        Name = c.Driver.Name ?? string.Empty
                    };
                }

                var newCase = new DmerCase
                {
                    CaseId = c.Id,
                    Title = c.Title ?? string.Empty,
                    CreatedBy = c.CreatedBy ?? string.Empty,
                    CreatedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    ModifiedBy = c.CreatedBy ?? string.Empty,
                    ModifiedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    Driver = driver,
                    IsCommercial = c.IsCommercial,
                    ClinicName = c.ClinicName ?? string.Empty,
                    Status = c.Status
                };

                /*newCase.Decisions.Add(c.Decisions.Select(d => new DecisionItem
                {
                    Identifier = d.Id,
                    Outcome = ConvertDecisionOutcome(d.Outcome),
                    CreatedOn = Timestamp.FromDateTime(d.CreatedOn.DateTime.ToUniversalTime())
                }));
                */

                return newCase;
            }));
            reply.ResultStatus = ResultStatus.Success;
            return reply;
        }

        /// <summary>
        /// Mark Medical Updates Sent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> MarkMedicalUpdatesSent(IdListRequest request,
            ServerCallContext context)
        {
            ResultStatusReply result = new ResultStatusReply();
            try
            {
                await _caseManager.MarkMedicalUpdatesSent(request.IdList.ToList());

                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Mark Medical Update Error when ICBC fails to update
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> MarkMedicalUpdateError(IcbcErrorRequest request,
            ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();
            try
            {
                var icbcErrorRequest = new CaseManagement.IcbcErrorRequest()
                {
                    CaseId = request.CaseId,
                    ErrorMessage = request.ErrorMessage
                };

                await _caseManager.MarkMedicalUpdateError(icbcErrorRequest);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }


        /// <summary>
        /// Update Clean Pass Flag
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateCleanPassFlag(CaseIdRequest request,
            ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();
            try
            {
                await _caseManager.UpdateCleanPassFlag(request.CaseId);

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }

        /// <summary>
        /// Update Manual Pass Flag on the case
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateManualPassFlag(CaseIdRequest request,
            ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();
            try
            {
                await _caseManager.UpdateManualPassFlag(request.CaseId);

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            var result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            var configuredSecret = _configuration["JWT_TOKEN_KEY"];


            if (configuredSecret != null && !string.IsNullOrEmpty(request?.Secret) &&
                configuredSecret.Equals(request.Secret))
            {
                byte[] key = Encoding.UTF8.GetBytes(_configuration["JWT_TOKEN_KEY"]);
                Array.Resize(ref key, 32);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

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

        /// <summary>
        /// Get Legacy Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetLegacyDocumentReply> GetLegacyDocument(LegacyDocumentRequest request,
            ServerCallContext context)
        {
            GetLegacyDocumentReply reply = new GetLegacyDocumentReply();

            // fetch the document.
            try
            {
                var d = await _documentManager.GetLegacyDocument(request.DocumentId);
                reply.Document = new LegacyDocument
                {
                    BatchId = d.BatchId ?? string.Empty,
                    BusinessArea = d.BusinessArea ?? string.Empty,
                    CaseId = d.CaseId ?? string.Empty,
                    CreateDate = Timestamp.FromDateTimeOffset(d.CreateDate),
                    DocumentId = d.DocumentId ?? string.Empty,
                    DocumentPages = (int)d.DocumentPages,
                    DocumentTypeCode = d.DocumentTypeCode ?? string.Empty,
                    DocumentUrl = d.DocumentUrl ?? string.Empty,
                    DpsDocumentId = d.DpsDocumentId,
                    ImportId = d.ImportId ?? string.Empty,
                    Origin = d.Origin ?? string.Empty,
                    OriginatingNumber = d.OriginatingNumber ?? string.Empty,
                    Priority = d.Priority ?? string.Empty,
                    Queue = d.Queue ?? string.Empty,
                    SequenceNumber = (int)(d.SequenceNumber ?? -1),
                    SubmittalStatus = d.SubmittalStatus ?? string.Empty,
                    UserId = d.UserId ?? string.Empty,
                    ValidationMethod = d.ValidationMethod ?? string.Empty,
                    ValidationPrevious = d.ValidationPrevious ?? string.Empty,
                };

                if (d.Driver != null)
                {
                    reply.Document.Driver = new Driver
                        { Id = d.Driver.Id, DriverLicenseNumber = d.Driver?.DriverLicenseNumber };
                }

                if (d.FaxReceivedDate != null)
                {
                    reply.Document.FaxReceivedDate = Timestamp.FromDateTimeOffset(d.FaxReceivedDate.Value);
                }

                if (d.ImportDate != null)
                {
                    reply.Document.ImportDate = Timestamp.FromDateTimeOffset(d.ImportDate.Value);
                }

                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }


            return reply;
        }

        [Obsolete("Use CallbackService.Create instead.")]
        public async override Task<ResultStatusReply> CreateBringForward(BringForwardRequest request,
            ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();

            try
            {
                var bringForwardRequest = new CaseManagement.BringForwardRequest()
                {
                    CaseId = request.CaseId ?? string.Empty,
                    Assignee = request.Assignee ?? string.Empty,
                    Description = request.Description ?? string.Empty,
                    Subject = request.Subject ?? string.Empty,
                    Priority = (CaseManagement.CallbackPriority?)request.Priority
                };

                var result = await _caseManager.CreateBringForward(bringForwardRequest);
                if (result != null && result.Success)
                {
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }
    }
}