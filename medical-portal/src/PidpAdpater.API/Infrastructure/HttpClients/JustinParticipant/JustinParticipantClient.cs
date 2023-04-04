using pdipadapter.Features.Participants.Models;

namespace pdipadapter.Infrastructure.HttpClients.JustinParticipant
{
    public class JustinParticipantClient : BaseClient, IJustinParticipantClient
    {
        public JustinParticipantClient(HttpClient httpClient, ILogger<JustinParticipantClient> logger) : base(httpClient, logger) { }

        public async Task<Participant> GetParticipantByUserName(string username, string accessToken)
        {
            var result = await this.GetAsync<Party>($"?user_id={username}", accessToken);

            if (!result.IsSuccess)
            {
                return null;
            }
            var participants = result.Value;
            if (participants.participant.participantDetails.Count == 0)
            {
                this.Logger.LogNoUserFound(username);
                return null;
            }
            if (participants.participant.participantDetails[0].assignedAgencies.Count == 0)
            {
                this.Logger.LogDisabledUserFound(username);
                return null;
            }
            return participants.participant;
        }

        public async Task<Participant> GetParticipantPartId(decimal partId, string accessToken)
        {
            var result = await this.GetAsync<Party>($"?part_id={partId}", accessToken);

            if (!result.IsSuccess)
            {
                return null;
            }
            var participants = result.Value;
            if (participants.participant.participantDetails.Count == 0)
            {
                this.Logger.LogNoUserWithPartIdFound(partId);
                return null;
            }
            if (participants.participant.participantDetails[0].assignedAgencies.Count == 0)
            {
                this.Logger.LogDisabledPartIdFound(partId);
                return null;
            }
            return participants.participant;
        }
    }
}
public static partial class JustinParticipantClientLoggingExtensions
{
    [LoggerMessage(1, LogLevel.Warning, "No User found in JUM with Username = {username}.")]
    public static partial void LogNoUserFound(this ILogger logger, string username);
    [LoggerMessage(2, LogLevel.Warning, "No User found in JUM with PartId = {partId}.")]
    public static partial void LogNoUserWithPartIdFound(this ILogger logger, decimal partId);
    [LoggerMessage(3, LogLevel.Warning, "User found but disabled in JUM with Username = {username}.")]
    public static partial void LogDisabledUserFound(this ILogger logger, string username);
    [LoggerMessage(4, LogLevel.Warning, "User found but disabled in JUM with PartId = {partId}.")]
    public static partial void LogDisabledPartIdFound(this ILogger logger, decimal partId);
    [LoggerMessage(5, LogLevel.Error, "Justin user not found.")]
    public static partial void LogJustinUserNotFound(this ILogger logger);
}
