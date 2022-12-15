using Microsoft.EntityFrameworkCore;
using Properties;
using System.Net;

namespace WebAPI.Model.DisccusionFolder
{
    public class DiscussionContext : IDiscussionContext
    {
        private DatabaseContext ctx;

        public DiscussionContext(DatabaseContext db)
        {
            ctx = db;
        }

        public async Task<HttpResponseMessage> DeleteDiscussion(List<CustomDiscussionProperty> discussions)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                foreach (var item in discussions)
                {
                    var discussion = await ctx.CustomDiscussions.Where(c => c.Id == item.Id).FirstOrDefaultAsync();
                    if (discussion != null)
                    {
                        ctx.Remove(discussion);
                        var res = await ctx.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Succesfully deleted discussion!");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to delete discussion!");
                return response;
            }
        }

        public async Task<List<CustomDiscussionProperty>> GetAllDiscussions(int teamId)
        {
            // We get a list of discussions that have already been used for the team
            List<ActivityOccurenceProperty> activities = ctx.Activities.Where(c => c.TeamId == teamId && c.Type.ToLower().Equals("discussion")).ToList();
            List<int> usedDiscussionIds = new List<int>();

            foreach (var discussion in activities)
            {
                usedDiscussionIds.Add(discussion.DiscussionOrPollId);
            }

            List<CustomDiscussionProperty> discussions = ctx.CustomDiscussions.Where(c => c.TeamId == teamId).ToList();

            List<CustomDiscussionProperty> unusedDiscussions = new List<CustomDiscussionProperty>();

            foreach (var discussion in discussions)
            {
                if (!usedDiscussionIds.Contains(discussion.Id))
                {
                    unusedDiscussions.Add(discussion);
                }
            }
            return unusedDiscussions;
        }

        public async Task<HttpResponseMessage> PostDiscussions(List<CustomDiscussionProperty> discussions)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                if (discussions != null)
                {
                    foreach (var discussion in discussions)
                    {
                        await ctx.CustomDiscussions.AddAsync(discussion);
                        await ctx.SaveChangesAsync();
                    }
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent("Succesfully added discussions!");
                    return response;
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to add discussions!");
                return response;
            }
            response.StatusCode = HttpStatusCode.NotAcceptable;
            response.Content = new StringContent("Failed to add discussions!");
            return response;
        }
    }
}
