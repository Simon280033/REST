using Azure;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using Properties;
using Properties.Team;
using REST.Model.ExchangeClasses;
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

        public async Task<HttpResponseMessage> TieUserToTeams(List<SocioliteTeam> teamsWithChannels, string userId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            int tiedTo = 0;

            try
            {
            // We get a list of the total channel IDs
            List<string> totalChannelIds = new List<string>();  

            foreach (var team in teamsWithChannels)
            {
                totalChannelIds.AddRange(team.TeamsChannelIds);
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
            bool userExists = ctx.Users.Where(user => user.MSTeamsId.Equals(userId)).Any();

            if (!userExists)
            {
                    string name = "Unnamed user";
                    bool found = false;

                    foreach(var team in teamsWithChannels)
                    {
                        foreach(var member in team.Members)
                        {
                            if(member.Id.Equals(userId))
                            {
                                name = member.Name;
                                found = true;
                                break;
                            }
                            if(found)
                            {
                                break;
                            }
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                UserProperty user = new UserProperty();
                user.MSTeamsId = userId;
                user.FirstName = name;
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
            }
            else // If the user already existed, we filter out the channel IDs for teams where the user was already a member
            {
                List<string> channelsToLink = new List<string>();
                List<int> teamsAlreadyPartOf = new List<int>();

                var memberships = ctx.TeamMemberships.Where(membership => membership.UserId.Equals(userId)).ToList();

                foreach (var membership in memberships)
                {
                    teamsAlreadyPartOf.Add(membership.TeamId);
                }

                foreach (var team in teams)
                {
                    channelsToLink.Add(team.MSTeamsChannelId);
                }

                totalChannelIds = channelsToLink;
            }

            // Then we create memberships for each of them
            foreach(var channel in totalChannelIds)
            {
                // We get the appropriate team
                foreach(var team in teamsWithChannels)
                {
                    if (team.TeamsChannelIds.Contains(channel))
                    {
                        // We add MS Teams ID to team
                        var result = ctx.Teams.SingleOrDefault(b => b.MSTeamsChannelId.Equals(channel));
                        if (result != null)
                        {
                            result.MSTeamsTeamId = team.TeamsTeamId;
                            result.Name = team.Name;
                            ctx.SaveChanges();

                            // We create memberships for all the users
                            foreach (var member in team.Members)
                                {
                                    // We create the user if they don't already exist
                                    bool thisUserExists = ctx.Users.Where(user => user.MSTeamsId.Equals(member.Id)).Any();

                                    if (!thisUserExists)
                                    {
                                        UserProperty user = new UserProperty();
                                        user.MSTeamsId = member.Id;
                                        user.FirstName = member.Name;
                                        ctx.Users.Add(user);
                                        await ctx.SaveChangesAsync();
                                    }

                                    // Then we create the memberships if they don't exist
                                    bool isMember = ctx.TeamMemberships.Where(m => m.UserId.Equals(member.Id) && m.TeamId.Equals(result.TeamId)).Any();

                                    if (!isMember)
                                    {
                                        string role = "Default";

                                        if(member.Id.Equals(userId))
                                        {
                                            role = "Manager";
                                        }

                                        SocioliteTeamMembershipProperty membership = new SocioliteTeamMembershipProperty
                                        {
                                            UserId = member.Id,
                                            TeamId = result.TeamId,
                                            TeamSpecificRole = role
                                        };

                                        ctx.TeamMemberships.Add(membership);
                                        await ctx.SaveChangesAsync();
                                    }
                                }

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

        public async Task<HttpResponseMessage> UpdateMembership(int teamId, string userId, string newRole)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                // First we check if user exists
                bool userExists = ctx.Users.Where(user => user.MSTeamsId.Equals(userId)).Any();

                // If they don't, we create them
                if (!userExists)
                {
                    UserProperty user = new UserProperty();
                    user.MSTeamsId = userId;
                    user.FirstName = "";
                    ctx.Users.Add(user);
                    await ctx.SaveChangesAsync();
                }

                // Then we check if the membership exists
                bool membershipExists = ctx.TeamMemberships.Where(membership => membership.UserId.Equals(userId) && membership.TeamId.Equals(teamId)).Any();

                // If it doesn't, we create it
                if (!membershipExists)
                {
                    SocioliteTeamMembershipProperty membership = new SocioliteTeamMembershipProperty();
                    membership.UserId = userId;
                    membership.TeamId = teamId;
                    membership.TeamSpecificRole = newRole;
                    ctx.TeamMemberships.Add(membership);
                    await ctx.SaveChangesAsync();
                } 
                else // Otherwise, we update it
                {
                    var result = ctx.TeamMemberships.SingleOrDefault(membership => membership.UserId.Equals(userId) && membership.TeamId.Equals(teamId));

                    result.TeamSpecificRole = newRole;

                    await ctx.SaveChangesAsync();
                }
            } catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("ERROR: Something went wrong!");
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("Succesfully updated user role!");
            return response;
        }
    }
}
