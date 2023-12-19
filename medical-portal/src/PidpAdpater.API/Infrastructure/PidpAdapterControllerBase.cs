using DomainResults.Common;
using IdentityModel.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using pdipadapter.Extensions;
using pdipadapter.Infrastructure.Services;
using System.Net;

namespace PidpAdapter.API.Infrastructure
{
    public class PidpAdapterControllerBase : ControllerBase
    {
        protected IPidpAdapterAuthorizationService AuthorizationService { get; }
        private readonly IMediator mediator;
        private readonly ILogger<PidpAdapterControllerBase> logger;

        protected PidpAdapterControllerBase(IPidpAdapterAuthorizationService authService, IMediator mediator, ILogger<PidpAdapterControllerBase> logger)
        {
            this.AuthorizationService = authService;
            this.mediator = mediator;
            this.logger = logger;
        }
        /// <summary>
        /// Checks that the given Contact both exists and is owned by the current User before executing the handler.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="contactId"></param>
        /// <param name="handler"></param>
        /// <param name="request"></param>
        protected async Task<TResponse> AuthorizeContactBeforeHandleAsync<TRequest, TResponse>(string contactId, TRequest request)
        where TRequest : IRequest<TResponse>
        {
            var access = await this.AuthorizationService.CheckContactAccessibility(contactId, this.User);
            if (!access.IsSuccess)
            {
                // Return a 403 Forbidden result if the user is not authorized
                this.logger.LogWarning("User {UserId} is not authorized to perform this operation", this.User.GetUserId());
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await HttpContext.Response.WriteAsync("Access denied.");
                await HttpContext.Response.CompleteAsync();
                return (TResponse)(object)Unauthorized();
            }
            // If authorized, send the request to the MediatR mediator
            return await this.mediator.Send(request);
        }
    }
}
