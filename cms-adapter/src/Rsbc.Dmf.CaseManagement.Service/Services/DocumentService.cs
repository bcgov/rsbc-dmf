using Grpc.Core;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Rsbc.Dmf.CaseManagement.Service
{
    // DOMAIN documents, document types, document sub types, submittal types

    public class DocumentService : DocumentManager.DocumentManagerBase
    {
        private readonly IDocumentManager _documentManager;
        private readonly IDocumentTypeManager _documentTypeManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentService> _logger;
        private readonly string _driverDocumentTypeCode;

        public DocumentService(IDocumentManager documentManager, IDocumentTypeManager documentTypeManager, IMapper mapper, IConfiguration configuraiton, ILoggerFactory loggerFactory)
        {
            _documentManager = documentManager;
            _documentTypeManager = documentTypeManager;
            _mapper = mapper;
            _configuration = configuraiton;
            _logger = loggerFactory.CreateLogger<DocumentService>();
            _driverDocumentTypeCode = _configuration["DRIVER_DOCUMENT_TYPE_CODE"];
        }

        // get Guid from document sub type that is in the set of documents belonging to 666
        // use this in conjunction with GetDocumentsSubTypes method
        public async override Task<GetDocumentSubTypeIdReply> GetDocumentSubTypeGuid(DocumentIdRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentSubTypeIdReply();

            try
            {
                reply.Id = _documentTypeManager
                    .GetDocumentSubTypeGuid(request.Id, _driverDocumentTypeCode)
                    .ToString();
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        public async override Task<GetDocumentSubTypesReply> GetDocumentSubTypes(DocumentTypeRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentSubTypesReply();

            try
            {
                // validation
                // NOTE for now only 666 code is allowed
                if (request.DocumentTypeCode != _driverDocumentTypeCode)
                {
                    reply.ResultStatus = ResultStatus.Fail;
                    reply.ErrorDetail = "Invalid document type code";
                    return reply;
                }

                var result = _documentTypeManager.GetDocumentSubTypes(request.DocumentTypeCode);
                var mappedResults = _mapper.Map<IEnumerable<DocumentSubType>>(result);
                reply.Items.AddRange(mappedResults);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }
        public async override Task<GetDocumentsByTypeForUsersReply> GetDocumentsByTypeForUsers(GetDocumentsByTypeForUsersRequest request, ServerCallContext context)
        {
            var result = new GetDocumentsByTypeForUsersReply();

            try
            {
                var loginIds = request.LoginIds.Select(Guid.Parse);
                var documents = _documentManager.GetDocumentsByTypeForUsers(loginIds, request.DocumentTypeCode);
                var mappedDocuments = _mapper.Map<IEnumerable<Document>>(documents);
                result.Items.AddRange(mappedDocuments);
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }

            return result;
        }

        public async override Task<GetDmerReply> GetDmer(CaseIdRequest request, ServerCallContext context) 
        {
            var result = new GetDmerReply();
            
            try {
                var caseId = Guid.Parse(request.CaseId);
                var dmerCase = _documentManager.GetDmer(caseId);
                if (dmerCase == null)
                {
                    result.ResultStatus = ResultStatus.Fail;
                    result.ErrorDetail = "No DMER found for the case";
                    return result;
                }
                result.Item = new DmerCase();
                result.Item.DmerType = dmerCase.DmerType ?? string.Empty;
                result.Item.Status = dmerCase.DmerStatus ?? string.Empty;
                result.Item.Provider = new Provider();
                result.Item.Provider.Name = dmerCase.Login?.FullName ?? string.Empty;
                result.Item.DocumentId = dmerCase.DocumentId.ToString();
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
        /// Update ClaimDmer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<UpdateClaimReply> UpdateClaimDmer(UpdateClaimRequest request, ServerCallContext context)
        {
            var result = new UpdateClaimReply();

            try
            {
                var loginId = Guid.Parse(request.LoginId);
                var documentId = Guid.Parse(request.DocumentId);
                var document = _documentManager.UpdateClaimDmer(loginId, documentId);
                var mappedDocuments = _mapper.Map<Document>(document);
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
        /// Update UnClaimDmer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<UpdateClaimReply> UpdateUnClaimDmer(UpdateClaimRequest request, ServerCallContext context)
        {
            var result = new UpdateClaimReply();

            try
            {
                var loginId = Guid.Parse(request.LoginId);
                var documentId = Guid.Parse(request.DocumentId);
                var document = _documentManager.UpdateUnClaimDmer(loginId, documentId);
                var mappedDocuments = _mapper.Map<IEnumerable<Document>>(document);
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }

            return result;
        }
    }
}
