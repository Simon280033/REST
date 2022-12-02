using Azure;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using Properties;
using Properties.Team;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WebAPI.Model.MembershipFolder
{
    public class MembershipContext : IMembershipContext
    {

        private DatabaseContext ctx;

        public MembershipContext(DatabaseContext db)
        {
            ctx = db;
        }
        public Task<int> DeleteMembership(int id)
        {
            throw new NotImplementedException();
        }

        public List<Object> GetMembership(List<int> data)
        {
            List<Object> list = new List<Object>();
            var teams = ctx.TeamMemberships.Where(t=> t.UserId.Equals(data[0])).ToList();


            foreach (var theTeam in teams) {
                List<UserProperty> AllUsers = new List<UserProperty>();
                List<string> Roles = new List<string>();

                List<SocioliteTeamMembershipProperty> memberships = ctx.TeamMemberships.Where(t => t.TeamId == theTeam.TeamId).ToList();
                foreach (var membership in memberships)
                {
                    List<UserProperty> users = ctx.Users.Where(t => t.MSTeamsId.Equals(membership.TeamId)).ToList();
                    AllUsers = users;
                    Roles.Add(membership.TeamSpecificRole);
                }
                list.Add(AllUsers);
                list.Add(Roles);

                List<CustomPollProperty> polls = ctx.CustomPolls.Where(t => t.TeamId == theTeam.TeamId).ToList();
                List<CustomDiscussionProperty> discussions = ctx.CustomDiscussions.Where(t => t.TeamId == theTeam.TeamId).ToList();
                
                if (polls.Any())
                {
                    list.Add(polls);
                }
                if (discussions.Any())
                {
                    list.Add(discussions);
                }

            }



            return list;

        }

        public async Task<List<SocioliteTeamMembershipProperty>> GetMemberships()
        {
            List<SocioliteTeamMembershipProperty> memberships = (from a in ctx.TeamMemberships select a).ToList();
            return memberships;
        }

        public async Task<HttpResponseMessage> TieUserToTeams(List<Tuple<Tuple<string, string>, List<string>>> teamsWithChannels, string id)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            int tiedTo = 0;

            try
            {
            // We get a list of the total channel IDs
            List<string> totalChannelIds = new List<string>();  

            foreach (var team in teamsWithChannels)
            {
                totalChannelIds.AddRange(team.Item2);
            }

            // First, we check if there are any teams with any of the supplied channel IDs
            var teams = ctx.Teams.Where(t => totalChannelIds.Any(id => id.Equals(t.MSTeamsChannelId))).ToList();

            // If there isn't, we return a bad statuscode
            if (teams.Count == 0)
            {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Content = new StringContent("ERROR: No teams with channel IDs supplied!");
                    return response;
            }

            // We create the user if they don't already exist
            bool userExists = ctx.Users.Where(user => user.MSTeamsId.Equals(id)).Any();

            if (!userExists)
            {
                UserProperty user = new UserProperty();
                user.MSTeamsId = id;
                user.FirstName = "";
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
            }
            else // If the user already existed, we filter out the channel IDs for teams where the user was already a member
            {
                List<string> channelsToLink = new List<string>();
                List<int> teamsAlreadyPartOf = new List<int>();

                var memberships = ctx.TeamMemberships.Where(membership => membership.UserId.Equals(id)).ToList();

                foreach (var membership in memberships)
                {
                    teamsAlreadyPartOf.Add(membership.TeamId);
                }

                foreach (var team in teams)
                {
                    if (!teamsAlreadyPartOf.Contains(team.TeamId))
                    {
                        channelsToLink.Add(team.MSTeamsChannelId);
                    }
                }

                totalChannelIds = channelsToLink;
            }

            // Then we create memberships for each of them
            foreach(var channel in totalChannelIds)
            {

                // We get the appropriate team
                foreach(var team in teamsWithChannels)
                {
                    if (team.Item2.Contains(channel))
                    {
                        // We add MS Teams ID to team
                        var result = ctx.Teams.SingleOrDefault(b => b.MSTeamsChannelId.Equals(channel));
                        if (result != null)
                        {
                            result.MSTeamsTeamId = team.Item1.Item1;
                            result.Name = team.Item1.Item2;
                            ctx.SaveChanges();

                            // We check if the team already has members, if this is the first, we add as manager
                            bool firstMember = !ctx.TeamMemberships.Where(m => m.TeamId.Equals(result.TeamId)).Any();

                            string role = "Scheduler";

                            if (firstMember)
                            {
                                role = "Manager";
                            }

                            // We create the membership
                            SocioliteTeamMembershipProperty membership = new SocioliteTeamMembershipProperty{
                                UserId = id,
                                TeamId = result.TeamId,
                                TeamSpecificRole = role
                            };

                            ctx.TeamMemberships.Add(membership);
                            await ctx.SaveChangesAsync();

                            tiedTo++;
                        }
                        break;
                    }
                }
            }
            } 
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("ERROR: Something went wrong!");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("Succesfully tied user to " + tiedTo + " team(s)!");
            return response;
        }

        public async Task<HttpResponseMessage> PutMembership(int teamId, string userId)
        {
            var result = ctx.TeamMemberships.SingleOrDefault(b => b.UserId.Equals(userId));

            result.TeamSpecificRole = "Manager";

            await ctx.SaveChangesAsync();

            return null;
        }
    }
}
