using Properties;

namespace REST.Model.ActivityFolder
{
    public interface IActivityContext
    {
        Task<HttpResponseMessage> GetNextActivityForTeam(string teamId);
    }
}
