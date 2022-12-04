
using Properties;
using Sociolite.Models;
using System.Net;

namespace WebAPI.Model.DisccusionFolder
{
    public interface IDiscussionContext
    {
       
        Task<HttpResponseMessage> PostDiscussions(List<CustomDiscussionProperty> discussions);
        Task<HttpResponseMessage> DeleteDiscussion(List<CustomDiscussionProperty> discussions);

        Task<List<CustomDiscussionProperty>> GetAllDiscussions(int teamId);

    }
}
