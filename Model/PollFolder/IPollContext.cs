
using Properties;

namespace WebAPI.Model.PollFolder
{
    public interface IPollContext
    {

        Task<HttpResponseMessage> PostPolls(List<CustomPollProperty> polls);
        Task<HttpResponseMessage> DeletePolls(List<int> pollIds);

        Task<List<CustomPollProperty>> GetAllPolls(int teamId);
    }
}
