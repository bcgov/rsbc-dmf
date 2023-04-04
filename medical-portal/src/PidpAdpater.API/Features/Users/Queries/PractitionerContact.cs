using pdipadapter.Data.ef;
using pdipadapter.Features.Users.Queries;
using pdipadapter.Features.Users.Services;
using MediatR;
using Rsbc.Dmf.CaseManagement;

namespace MedicalPortal.API.Features.Users.Queries;
public class PractitionerContact
{
    public sealed record Query(string Hpdid) : IRequest<Practitioner>;
    public class QueryHandler : IRequestHandler<Query, Practitioner>
    {
        private readonly IUserManager userManager;
        public QueryHandler(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Practitioner> Handle(Query request, CancellationToken cancellationToken)
        {
            return await userManager.GetPractitionerContact(request.Hpdid);
        }
    }

}

