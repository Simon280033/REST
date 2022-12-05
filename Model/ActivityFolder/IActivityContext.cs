using Properties;
using REST.Model.ExchangeClasses;

namespace REST.Model.ActivityFolder
{
    public interface IActivityContext
    {
        Task<Tuple<HttpResponseMessage, string>> GetNextActivityForTeam(string teamId);

        Task<ActivityRequestObject> TeamAndActivityByChannelId(string channelId);
    }
}
