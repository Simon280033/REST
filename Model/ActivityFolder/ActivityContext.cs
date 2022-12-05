using Properties;
using System.Net;
using System.Threading.Channels;
using WebAPI.Model;

namespace REST.Model.ActivityFolder
{
    public class ActivityContext : IActivityContext
    {

        private DatabaseContext ctx;

        public ActivityContext(DatabaseContext db)
        {
            ctx = db;
        }
        public async Task<HttpResponseMessage> GetNextActivityForTeam(string teamId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            StringContent content = new StringContent("ERROR");

            try { 
            // We get a list of polls that have already been used for the team
            List<ActivityOccurenceProperty> pollActivities = ctx.Activities.Where(c => c.TeamId == Int32.Parse(teamId) && c.DiscussionId == 0).ToList();
            List<int> usedPollIds = new List<int>();

            foreach (var poll in pollActivities)
            {
                usedPollIds.Add(poll.DiscussionId);
            }

            // We get a list of discussions that have already been used for the team
            List<ActivityOccurenceProperty> discussionActivities = ctx.Activities.Where(c => c.TeamId == Int32.Parse(teamId) && c.PollId == 0).ToList();
            List<int> usedDiscussionIds = new List<int>();

            foreach (var discussion in discussionActivities)
            {
                usedDiscussionIds.Add(discussion.DiscussionId);
            }

            bool customPollsLeft = ctx.CustomPolls.Where(p => p.TeamId == Int32.Parse(teamId) && !usedPollIds.Contains(p.Id)).Any();
            bool customDiscussionsLeft = ctx.CustomDiscussions.Where(d => d.TeamId == Int32.Parse(teamId) && !usedDiscussionIds.Contains(d.Id)).Any();

            // Scenarios:

            // No custom discs, but custom polls
            if (customPollsLeft && !customDiscussionsLeft)
            {
                var polls = ctx.CustomPolls.Where(p => p.TeamId == Int32.Parse(teamId) && !usedPollIds.Contains(p.Id)).ToList();
                content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(polls[0]));
                response.StatusCode = HttpStatusCode.OK;
            }
            else if (!customPollsLeft && customDiscussionsLeft) // No custom polls, but custom discs
            {
                var discussions = ctx.CustomDiscussions.Where(d => d.TeamId == Int32.Parse(teamId) && !usedDiscussionIds.Contains(d.Id)).ToList();
                content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(discussions[0])); 
                response.StatusCode = HttpStatusCode.OK;

                }
                else
            {
                Random rand = new Random();

                bool returnPoll = (rand.Next(0, 2) == 0);

                    // No custom activities at all
                    if (!customPollsLeft && !customDiscussionsLeft)
                {
                    // 50/50 to return either a default poll or discussion that has not been used 
                    if (returnPoll)
                    {
                        var polls = ctx.CustomPolls.Where(p => p.TeamId == 0 && !usedPollIds.Contains(p.Id)).ToList();

                            // If they have all been used, we return one anyways
                            if (!polls.Any())
                            {
                                polls = ctx.CustomPolls.Where(p => p.TeamId == 0).ToList();
                            }
                        content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(polls[0]));
                    } else
                    {
                        var discussions = ctx.CustomDiscussions.Where(d => d.TeamId == 0 && !usedDiscussionIds.Contains(d.Id)).ToList();
                            // If they have all been used, we return one anyways
                            if (!discussions.Any())
                            {
                                discussions = ctx.CustomDiscussions.Where(p => p.TeamId == 0).ToList();
                            }
                            content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(discussions[0]));
                    }
                        response.StatusCode = HttpStatusCode.OK;
                    }
                    else if (customPollsLeft && customDiscussionsLeft) // Both are present
                {
                    // 50/50 to return either a custom poll or discussion that has not been used 
                    if (returnPoll)
                    {
                        var polls = ctx.CustomPolls.Where(p => p.TeamId == Int32.Parse(teamId) && !usedPollIds.Contains(p.Id)).ToList();
                        content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(polls[0]));
                    }
                    else
                    {
                        var discussions = ctx.CustomDiscussions.Where(d => d.TeamId == Int32.Parse(teamId) && !usedDiscussionIds.Contains(d.Id)).ToList();
                        content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(discussions[0]));
                    }
                        response.StatusCode = HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            response.Content = content;
            return response;
        }
    }
}
