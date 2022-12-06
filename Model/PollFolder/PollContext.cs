using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using NuGet.Protocol;
using Properties;
using System.Net;
using System.Web.Http;

namespace WebAPI.Model.PollFolder
{
    public class PollContext : IPollContext
    {
        private DatabaseContext ctx;

        public PollContext(DatabaseContext db)
        {
            ctx = db;
        }

        public async Task<HttpResponseMessage> DeletePolls(List<int> pollIds)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            var poll = await ctx.CustomPolls.Where(c => c.Id == pollIds[0]).FirstOrDefaultAsync();
            if (poll != null)
            {
                ctx.Remove(poll);
                var res = await ctx.SaveChangesAsync();
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Succesfully deleted poll!");
                return response;
            }
            response.StatusCode = HttpStatusCode.BadRequest;
            response.Content = new StringContent("Failed to delete poll!");
            return response;
        }

        public async Task<List<CustomPollProperty>> GetAllPolls(int teamId)
        {
            // We get a list of polls that have already been used for the team
            List<ActivityOccurenceProperty> activities = ctx.Activities.Where(c => c.TeamId == teamId && c.Type.ToLower().Equals("poll")).ToList();
            List<int> usedPollIds = new List<int>();

            foreach (var poll in activities)
            {
                usedPollIds.Add(poll.DiscussionOrPollId);
            }

            List<CustomPollProperty> polls = ctx.CustomPolls.Where(c => c.TeamId == teamId).ToList();
            // Then we filter out those that were already used
            List<CustomPollProperty> unusedPolls = new List<CustomPollProperty>();

            foreach (var poll in polls)
            {
                if (!usedPollIds.Contains(poll.Id))
                {
                    unusedPolls.Add(poll);
                }
            }

            return unusedPolls;
        }

        public async Task<HttpResponseMessage> PostPolls(List<CustomPollProperty> polls)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                foreach (CustomPollProperty poll in polls)
                {
                    if (poll != null)
                    {
                        await ctx.CustomPolls.AddAsync(poll);
                        await ctx.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent("Succesfully added polls!");
                    return response;
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to add polls!");
                return response;
            }
            response.StatusCode = HttpStatusCode.NotAcceptable;
            response.Content = new StringContent("Failed to add polls!");
            return response;
        }
    }
}
