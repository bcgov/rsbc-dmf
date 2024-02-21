using Grpc.Core;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class DocumentService : DocumentManager.DocumentManagerBase
    {
        private readonly IDocumentTypeManager _documentTypeManager;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IDocumentTypeManager documentTypeManager, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _documentTypeManager = documentTypeManager;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<DocumentService>();
        }

        public override Task<GetDocumentSubTypesReply> GetDriverDocumentSubTypes(EmptyRequest emptyRequest, ServerCallContext context)
        {
            var reply = new GetDocumentSubTypesReply();

            try
            {
                var result = _documentTypeManager.GetDriverDocumentSubTypes();
                var mappedResults = _mapper.Map<IEnumerable<DocumentSubType>>(result);
                reply.Items.AddRange(mappedResults);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return Task.FromResult(reply);
        }
    }
}
