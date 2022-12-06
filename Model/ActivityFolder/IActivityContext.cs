using Microsoft.AspNetCore.Mvc;
using Properties;
using REST.Model.ExchangeClasses;

namespace REST.Model.ActivityFolder
{
    public interface IActivityContext
    {
        Task<Tuple<HttpResponseMessage, string>> GetNextActivityForTeam(string teamId);

        Task<ActivityRequestObject> TeamAndActivityByChannelId(string channelId);

        Task<HttpResponseMessage> LastActivityType(string channelId);

        Task<HttpResponseMessage> Vote(string channelId, string userId, int optionNumber);

        Task<HttpResponseMessage> GetActivePoll(string channelId);

        Task<HttpResponseMessage> GetActivePollOptionAmount(string channelId);

        Task<HttpResponseMessage> GetLastPollResults(string channelId);

    }
}
