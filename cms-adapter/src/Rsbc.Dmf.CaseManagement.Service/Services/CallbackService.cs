using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class CallbackService : CallbackManager.CallbackManagerBase
    {
        private readonly ICallbackManager _callbackManager;
        private readonly ILogger<CallbackService> _logger;
        private readonly IMapper _mapper;

        public CallbackService(ICallbackManager callbackManager, ILogger<CallbackService> logger, IMapper mapper)
        {
            _callbackManager = callbackManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async override Task<GetDriverCallbacksReply> GetDriverCallbacks(DriverIdRequest request, ServerCallContext context)
        {
            var reply = new GetDriverCallbacksReply();

            try
            {
                var result = await _callbackManager.GetDriverCallbacks(Guid.Parse(request.Id));
                var callbacks = _mapper.Map<IEnumerable<Callback>>(result);
                reply.Items.AddRange(callbacks);
                reply.DriverId = request.Id;
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        public async override Task<ResultStatusReply> Cancel(CallbackIdRequest request, ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                var callbackId = Guid.Parse(request.Id);
                var result = await _callbackManager.Cancel(callbackId);
                if (result.Success)
                {
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ResultStatus = ResultStatus.Fail;
                    reply.ErrorDetail = result.ErrorDetail;
                }
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
