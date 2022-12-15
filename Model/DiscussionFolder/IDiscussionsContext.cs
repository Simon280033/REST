
using Properties;

namespace WebAPI.Model.DisccusionFolder
{
    public interface IDiscussionContext
    {

        Task<HttpResponseMessage> PostDiscussions(List<CustomDiscussionProperty> discussions);
        Task<HttpResponseMessage> DeleteDiscussion(List<CustomDiscussionProperty> discussions);
        Task<List<CustomDiscussionProperty>> GetAllDiscussions(int teamId);
    }
}
