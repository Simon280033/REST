using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Properties;
using REST.Model.ExchangeClasses;
using System;
using System.Net;
using System.Reflection.Metadata;
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

        public async Task<HttpResponseMessage> GetActivePoll(string channelId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
            // We get the latest activity
            var team = ctx.Teams.Where(t => t.MSTeamsChannelId.Equals(channelId)).FirstOrDefault();
            var latest = ctx.Activities.Where(c => c.TeamId == team.TeamId).OrderByDescending(c => c.ActivityOccuranceId).FirstOrDefault();
            if (latest != null)
            {
                if (latest.Type.ToLower().Equals("poll"))
                    {
                        var poll = ctx.CustomPolls.Where(p => p.Id == latest.DiscussionOrPollId).FirstOrDefault();
                        response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(poll));
                        response.StatusCode = HttpStatusCode.OK;
                        return response;
                    }
                    else
                    {
                        throw new Exception("Current activity is not poll!");
                    }
            }
            } catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(e.Message);
                return response;
            }
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Content = new StringContent("Failed to get active poll!");
            return response;
        }

        public async Task<HttpResponseMessage> GetActivePollOptionAmount(string channelId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                // We get the latest activity
                var team = ctx.Teams.Where(t => t.MSTeamsChannelId.Equals(channelId)).FirstOrDefault();
                var latest = ctx.Activities.Where(c => c.TeamId == team.TeamId).OrderByDescending(c => c.ActivityOccuranceId).FirstOrDefault();
                if (latest != null)
                {
                    if (latest.Type.ToLower().Equals("poll"))
                    {
                        var poll = ctx.CustomPolls.Where(p => p.Id == latest.DiscussionOrPollId).FirstOrDefault();
                        response.Content = new StringContent("" + JsonConvert.DeserializeObject<List<string>>(poll.PollOptions).Count());
                        response.StatusCode = HttpStatusCode.OK;
                        return response;
                    }
                    else
                    {
                        throw new Exception("Current activity is not poll!");
                    }
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(e.Message);
                return response;
            }
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Content = new StringContent("Failed to get active poll!");
            return response;
        }

        public async Task<Tuple<HttpResponseMessage, string>> GetNextActivityForTeam(string teamId)
        {
            string type = "poll";
            HttpResponseMessage response = new HttpResponseMessage();
            StringContent content = new StringContent("ERROR");

            try { 
            // We get a list of polls that have already been used for the team
            List<ActivityOccurenceProperty> pollActivities = ctx.Activities.Where(c => c.TeamId == Int32.Parse(teamId) && c.Type.ToLower().Equals("poll")).ToList();
            List<int> usedPollIds = new List<int>();

            foreach (var poll in pollActivities)
            {
                usedPollIds.Add(poll.DiscussionOrPollId);
            }

            // We get a list of discussions that have already been used for the team
            List<ActivityOccurenceProperty> discussionActivities = ctx.Activities.Where(c => c.TeamId == Int32.Parse(teamId) && c.Type.ToLower().Equals("discussion")).ToList();
            List<int> usedDiscussionIds = new List<int>();

            foreach (var discussion in discussionActivities)
            {
                usedDiscussionIds.Add(discussion.DiscussionOrPollId);
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
                type = "discussion";
                }
                else
            {
                Random rand = new Random();

                    //bool returnPoll = (rand.Next(0, 2) == 0);
                    bool returnPoll = true;


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
                            type = "discussion";
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
                        type = "discussion";
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
            Tuple<HttpResponseMessage, string> responseAndType = new Tuple<HttpResponseMessage, string>(response, type);
            return responseAndType;
        }

        public async Task<HttpResponseMessage> LastActivityType(string channelId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent("No previous activity");

            // We get the latest activity
            var team = ctx.Teams.Where(t => t.MSTeamsChannelId.Equals(channelId)).FirstOrDefault();
            var latest = ctx.Activities.Where(c => c.TeamId == team.TeamId).OrderByDescending(c => c.ActivityOccuranceId).FirstOrDefault();
            if (latest != null)
            {
                response.Content = new StringContent(latest.Type);
            }
            return response;
        }

        public async Task<ActivityRequestObject> TeamAndActivityByChannelId(string channelId)
        {
            // We get the team
            var team = ctx.Teams.Where(t => t.MSTeamsChannelId.Equals(channelId)).FirstOrDefault();

            if (team != null)
            {
                if (team.isActive)
                {
                    Tuple<HttpResponseMessage, string> contentAndType = await GetNextActivityForTeam("" + team.TeamId);

                    string content = await contentAndType.Item1.Content.ReadAsStringAsync();

                    ActivityRequestObject data = new ActivityRequestObject
                    {
                        IsActive = team.isActive,
                        Type = contentAndType.Item2,
                        RecurranceString = team.Recurring,
                        Content = content
                    };

                    // We log this occurance
                    int discussionOrPollId = 0;
                    if (contentAndType.Item2.Equals("poll"))
                    {
                        discussionOrPollId = JsonConvert.DeserializeObject<CustomPollProperty>(content).Id;
                    } else
                    {
                        discussionOrPollId = JsonConvert.DeserializeObject<CustomDiscussionProperty>(content).Id;
                    }

                    ActivityOccurenceProperty aop = new ActivityOccurenceProperty
                    {
                        ActivityOccuranceId = ctx.Activities.Count(me => me != null) + 1,
                        TeamId = team.TeamId,
                        OcurredAt = DateTime.Now,
                        Type = contentAndType.Item2,
                        DiscussionOrPollId = discussionOrPollId
                    };

                    ctx.Activities.Add(aop);
                    ctx.SaveChanges();

                    return data;
                }
                
            }

            ActivityRequestObject emptyData = new ActivityRequestObject
            {
                IsActive = team.isActive,
                Type = "none",
                RecurranceString = team.Recurring,
                Content = "none"
            };

            return emptyData; // Do something with this...
        }

        public async Task<HttpResponseMessage> Vote(string channelId, string userId, int optionNumber)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                var team = ctx.Teams.Where(t => t.MSTeamsChannelId.Equals(channelId)).FirstOrDefault();
                if (team == null)
                {
                    throw new Exception();
                }

                int teamId = team.TeamId;

            var activity = ctx.Activities.Where(c => c.TeamId == teamId).OrderByDescending(c => c.ActivityOccuranceId).FirstOrDefault();

            if (activity != null)
            {
                PollVoteProperty pvp = new PollVoteProperty
                {
                    UserId = userId,
                    ActivityOccuranceId = activity.ActivityOccuranceId,
                    VoteOptionNumber = optionNumber
                };

                bool alreadyVoted = ctx.PollVotes.Where(v => v.UserId.Equals(userId) && v.ActivityOccuranceId == activity.ActivityOccuranceId).Any();

                if (alreadyVoted)
                {
                    PollVoteProperty vote = ctx.PollVotes.Where(v => v.UserId.Equals(userId) && v.ActivityOccuranceId == activity.ActivityOccuranceId).FirstOrDefault();
                    vote.VoteOptionNumber = optionNumber;
                    ctx.SaveChanges();
                    response.Content = new StringContent("Succesfully updated your vote!");
                    }
                    else
                {
                    ctx.PollVotes.Add(pvp);
                    ctx.SaveChanges();
                    response.Content = new StringContent("Succesfully added your vote!");
                    }
                } else
                {
                    throw new Exception();
                }
            } catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to add/update vote!");
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        public async Task<HttpResponseMessage> GetLastPollResults(string channelId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                // We get the latest activity
                var team = ctx.Teams.Where(t => t.MSTeamsChannelId.Equals(channelId)).FirstOrDefault();
                var latest = ctx.Activities.Where(c => c.TeamId == team.TeamId).OrderByDescending(c => c.ActivityOccuranceId).FirstOrDefault();
                if (latest != null)
                {
                    if (!latest.Type.ToLower().Equals("poll"))
                    {
                        throw new Exception("Not poll!");
                    }

                    var poll = ctx.CustomPolls.Where(p => p.Id == latest.DiscussionOrPollId).FirstOrDefault();

                    var answers = ctx.PollVotes.Where(v => v.ActivityOccuranceId == (latest.ActivityOccuranceId - 1)).ToList(); // TODO: I don't like having to retract 1, fix the logic...

                    if (poll == null)
                    {
                        throw new Exception("Poll not found!");
                    }

                    List<Tuple<int, string>> answersAndRespondants = new List<Tuple<int, string>>();

                    foreach (var answer in answers)
                    {
                        Tuple<int, string> answerNumberAndRespondant = new Tuple<int, string>(answer.VoteOptionNumber, answer.UserId); // Deduct user name from the id
                        answersAndRespondants.Add(answerNumberAndRespondant);
                    }

                    if (answersAndRespondants.Count == 0)
                    {
                        Tuple<int, string> answerNumberAndRespondant = new Tuple<int, string>(1, "testhest"); // Deduct user name from the id
                        answersAndRespondants.Add(answerNumberAndRespondant);
                    }

                    PollResultDisplayObject prdo = new PollResultDisplayObject
                    {
                        PollQuestion = poll.Question,
                        PossibleAnswers = JsonConvert.DeserializeObject<List<string>>(poll.PollOptions),
                        AnswersAndRespondants = answersAndRespondants
                    };

                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(prdo));
                    return response;
                }
                throw new Exception("Latest activity is not a poll!");
            } 
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(e.Message);
                return response;
            }
        }
    }
}
