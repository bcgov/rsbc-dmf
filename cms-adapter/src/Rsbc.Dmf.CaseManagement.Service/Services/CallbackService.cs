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

        public async override Task<ResultStatusReply> Create(Callback request, ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                // TODO automapper
                var callbackRequest = new CaseManagement.Callback()
                {
                    CaseId = request.CaseId ?? string.Empty,
                    Assignee = request.Assignee ?? string.Empty,
                    Description = request.Description ?? string.Empty,
                    Subject = request.Subject ?? string.Empty,
                    Priority = (CaseManagement.CallbackPriority)request.Priority,
                    CallStatus = (CallbackCallStatus)request.CallStatus,
                    Phone = request.Phone ?? string.Empty,
                    PreferredTime = (PreferredTime)request.PreferredTime,
                    NotifyByMail = request.NotifyByMail,
                    NotifyByEmail = request.NotifyByEmail,
                };

                var result = await _callbackManager.Create(callbackRequest);
                if (result != null && result.Success)
                {
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CallbackService)}.{nameof(Create)} failed: {ex}");
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
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

        public async override Task<ResultStatusReply> Cancel(CallbackCancelRequest request, ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                var callbackId = Guid.Parse(request.CallbackId);
                var caseId = Guid.Parse(request.CaseId);
                var result = await _callbackManager.Cancel(caseId, callbackId);
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
