using Grpc.Core;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Rsbc.Dmf.CaseManagement.Service
{
    // DOMAIN documents, document types, document sub types, submittal types
    public class DocumentService : DocumentManager.DocumentManagerBase
    {
        private readonly IDocumentTypeManager _documentTypeManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IDocumentTypeManager documentTypeManager, IMapper mapper, IConfiguration configuraiton, ILoggerFactory loggerFactory)
        {
            _documentTypeManager = documentTypeManager;
            _mapper = mapper;
            _configuration = configuraiton;
            _logger = loggerFactory.CreateLogger<DocumentService>();
        }

        public async override Task<GetDocumentSubTypesReply> GetDocumentSubTypes(DocumentTypeRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentSubTypesReply();

            try
            {
                // validation
                // NOTE for now only 666 code is allowed
                if (request.DocumentTypeCode != _configuration["DRIVER_DOCUMENT_TYPE_CODE"])
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
    }
}
