using pdipadapter.Infrastructure.HttpClients;
using MedicalPortal.API.Features.Endorsement.Model;
using MedicalPortal.API.Features.Endorsement.Services.Interfaces;
using Rsbc.Dmf.CaseManagement;
using Rsbc.Dmf.CaseManagement.Service;
using pdipadapter.Core.Extension;

namespace MedicalPortal.API.Features.Endorsement.Services;
public class Endorsement : BaseClient, IEndorsement
{
    private readonly UserManager.UserManagerClient userManager;
    public Endorsement(HttpClient client, ILogger<Endorsement> logger, UserManager.UserManagerClient userManager) : base(client, logger)
    {
        this.userManager = userManager;
    }

    public async Task<IEnumerable<Model.Endorsement>> GetEndorsement(string hpDid)
    {
        var result = await this.GetAsync<IEnumerable<EndorsementData.Model>>($"/api/v1/ext/parties/{hpDid}/endorsements");

        if (!result.IsSuccess)
        {
            return null;
        }
        var endorsements = result.Value;
        if (!endorsements.Any())
        {
            this.Logger.LogNoEndorsementFound(hpDid);
            return null;
        }
        var endorsementRelations = endorsements
            .Select(async endorsement => new
            {
                contact = await userManager.GetPractitionerContactAsync(new PractitionerRequest { Hpdid = $"{endorsement.Hpdid}"}),
                License = endorsement.Licences,
                HpDid = endorsement.Hpdid
            }).ToList();

       return endorsementRelations.Select(e => new Model.Endorsement
        {
            ContactId = e.Result.contact.ContactId,
            Email = e.Result.contact.Email,
            Hpdid = e.Result.HpDid!,
            BirthDate = e.Result.contact.Birthdate == null ? "" : e.Result.contact.Birthdate.ToDateTime().ToString(),
            Role = e.Result.contact.Role,
            FirstName = e.Result.contact.FirstName,
            LastName = e.Result.contact.LastName,
            Licences = e.Result.License
        });
    }
}

public static partial class JustinParticipantClientLoggingExtensions
{
    [LoggerMessage(1, LogLevel.Warning, "No Endorsement found in PiDP with Hpdid = {hpdid}.")]
    public static partial void LogNoEndorsementFound(this ILogger logger, string hpdid);

}