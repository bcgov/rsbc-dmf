using pdipadapter.Data.ef;
using pdipadapter.Features.Users.Queries;
using pdipadapter.Features.Users.Services;
using MediatR;
using Rsbc.Dmf.CaseManagement;
using PidpAdapter.API.Features.Users.Models;
using Rsbc.Dmf.CaseManagement.Service;
using Google.Protobuf.WellKnownTypes;

namespace MedicalPortal.API.Features.Users.Queries;
public class PractitionerContactQuery
{
    public sealed record Query(string contactId) : IRequest<List<Model>>;
    
    public class Model
    {
        public string Id { get; set;} = string.Empty;
    }
    public class QueryHandler : IRequestHandler<Query, List<Model>>
    {
        private readonly UserManager.UserManagerClient userManager;
        public QueryHandler(UserManager.UserManagerClient userManager)
        {
            this.userManager = userManager;
        }

        public async Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
        {
            var pReponse =  await userManager.GetPractitionerContactAsync(new PractitionerRequest
            {
                Hpdid = request.contactId,
            });

            if (pReponse.ContactId.ToString() == string.Empty)
            {
                return new List<Model>();

            }

            var g = new Model
            {
                Id = pReponse.ContactId
            };

            return new List<Model> { g };
        }
    }
    

}


public class ContactQuery
{
    public sealed record Query(string contactId) : IRequest<PractitionerContactResponse>;
    public class QueryHandler : IRequestHandler<Query, PractitionerContactResponse>
    {
        private readonly UserManager.UserManagerClient userManager;
        public QueryHandler(UserManager.UserManagerClient userManager)
        {
            this.userManager = userManager;
        }

        public async Task<PractitionerContactResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var pReponse = await userManager.GetPractitionerContactAsync(new PractitionerRequest
            {
                Hpdid = request.contactId,
            });

            if (pReponse.ContactId.ToString() == string.Empty)
            {
                return new PractitionerContactResponse();
            }
            return new PractitionerContactResponse
            {
                FirstName = pReponse.FirstName,
                LastName = pReponse.LastName,
                Email = pReponse.Email,
                Id = pReponse.ContactId,
                Birthdate = pReponse.Birthdate.ToDateTime().ToUniversalTime(),
                Role = pReponse.Role,


            };
        }
    }

}