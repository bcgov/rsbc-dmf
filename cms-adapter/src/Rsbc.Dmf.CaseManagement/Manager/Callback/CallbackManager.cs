using AutoMapper;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    internal partial class CallbackManager : ICallbackManager
    {
        internal readonly DynamicsContext _dynamicsContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CallbackManager> _logger;

        public CallbackManager(DynamicsContext dynamicsContext, IMapper mapper, ILogger<CallbackManager> logger)
        {
            _dynamicsContext = dynamicsContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultStatusReply> Create(Callback request)
        {
            var result = new ResultStatusReply();

            string caseId = request.CaseId;
            if (!string.IsNullOrEmpty(caseId))
            {
                var newTask = _mapper.Map<task>(request);
                // let Dynamics create the Id Guid
                newTask.activityid = null;

                // Get the case
                var @case = _dynamicsContext.incidents
                    .Where(d => d.incidentid == Guid.Parse(caseId))
                    .FirstOrDefault();

                _dynamicsContext.AddTotasks(newTask);

                // load owner
                if (string.IsNullOrEmpty(request.Assignee))
                {
                    if (@case._owningteam_value != null)
                    {
                        // create a reference to team
                        var caseTeam = _dynamicsContext.teams
                            .ByKey(@case._owningteam_value.Value)
                            .GetValue();
                        _dynamicsContext.SetLink(newTask, nameof(task.ownerid), caseTeam);
                    }
                    else
                    {
                        // create a reference to system user
                        var caseUser = _dynamicsContext.systemusers
                            .ByKey(@case._owninguser_value)
                            .GetValue();
                        _dynamicsContext.SetLink(newTask, nameof(task.ownerid), caseUser);
                    }
                }
                else
                {
                    // set the callback owner to case owner
                    if (@case._ownerid_value != null)
                    {
                        _dynamicsContext.SetLink(newTask, nameof(task.ownerid), @case._ownerid_value);
                    };
                }

                // Create a bring Forward
                try
                {
                    // set Case Id
                    _dynamicsContext.SetLink(newTask, nameof(task.regardingobjectid_incident), @case);

                    await _dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                    _dynamicsContext.DetachAll();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    _logger.LogError(ex.Message);
                    result.ErrorDetail = ex.Message;
                }
            }
            return result;
        }

        public async Task<IEnumerable<Callback>> GetDriverCallbacks(Guid driverId)
        {
            // get cases and include callbacks
            var cases = _dynamicsContext.incidents
                .Expand(c => c.Incident_Tasks)
                .Where(c => c._dfp_driverid_value == driverId && c.statecode == (int)EntityState.Active);

            // compile a list of callbacks from each case
            var results = new List<Callback>();
            foreach (var @case in cases)
            {
                // skip if tasks is null or has no active task
                if (!(@case.Incident_Tasks?.Any() ?? false))
                    break;

                var callbacks = _mapper.Map<IEnumerable<Callback>>(@case.Incident_Tasks);
                results.AddRange(callbacks);
            }

            return results;
        }

        public async Task<ResultStatusReply> Cancel(Guid caseId, Guid callbackId)
        {
            var reply = new ResultStatusReply();

            try
            {
                var callback = _dynamicsContext.incidents
                    .Expand(c => c.Incident_Tasks)
                    .Where(c => c.incidentid == caseId && c.statecode == (int)EntityState.Active)
                    .ToList()
                    .First()
                    .Incident_Tasks
                    .Where(cb => cb.activityid == callbackId)
                    .ToList()
                    .First();

                if (callback == null)
                {
                    reply.Success = false;
                    reply.ErrorDetail = "Callback not found.";
                    return reply;
                }

                callback.statecode = (int)EntityState.Cancelled;
                _dynamicsContext.UpdateObject(callback);

                await _dynamicsContext.SaveChangesAsync();
                reply.Success = true;
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"{nameof(Cancel)} failed.");
                reply.Success = false;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }
    }
}
