using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Properties;
using Properties.Team;
using System.Collections.Generic;
using System.Net;

namespace WebAPI.Model.TeamFolder
{
    public class TeamContext : ITeamContext
    {
        private DatabaseContext ctx;

        public TeamContext(DatabaseContext db)
        {
            ctx = db;
        }

        public async Task<HttpResponseMessage> PostTeam(string channelID)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var team = new SocioliteTeamProperty();
                team.MSTeamsChannelId = channelID;
                team.Recurring = "00000000000";

                if (ctx.Teams.Where(t=>t.MSTeamsChannelId.Equals(channelID)).Any())
                {
                    throw new Exception();
                }
                var res = ctx.Teams.Add(team);
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent("ERROR: This bot is already tied to a team!");
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("This team has succesfully been connected to Sociolite, with you as manager!");
            return response;
        }




        public List<SocioliteTeamProperty> GetTeam(int id)
        {
            try
            {
                var teams =  ctx.Teams.Where(t => t.TeamId == id).ToList();
                return teams;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return null;
        }

        public List<SocioliteTeamProperty> GetTeams()
        {
            List<SocioliteTeamProperty> teams = (from a in ctx.Teams select a).ToList();
            return teams;
        }

        public int PutTeam(string recurrence, int teamId)
        {
           SocioliteTeamProperty user = ctx.Teams.Where(t=>t.TeamId == teamId).ToList().First();

            user.Recurring = recurrence;

            ctx.SaveChanges();
            return 2;

        }

        public async Task<HttpStatusCode> ChangeActiveStatus( int teamId)
        {
            try
            {
                SocioliteTeamProperty user = ctx.Teams.Where(t => t.TeamId == teamId).First();
                
                    if (user.isActive)
                    {
                        user.isActive = false;
                    }
                    else
                    {
                        user.isActive = true;
                    }
                    await ctx.SaveChangesAsync();
                    return HttpStatusCode.OK;
                
            }
            catch(Exception e)
            {
                return HttpStatusCode.NotFound;
            }
        }

        public async Task<HttpStatusCode> DeleteTeam(int id)
        {
            //try
            //{
            //    var team = ctx.Teams.Where((t) => t.Id == id).ToList();
            //    if (team.Any())
            //    {
            //        ctx.Teams.Remove(team.First());
            //        await ctx.SaveChangesAsync();
            //        return StatusCodes.Status200OK;
            //    }

            //    return StatusCodes.Status404NotFound;
                
            //}
            //catch (Exception e)
            //{
            //    return StatusCodes.Status404NotFound;
            //}
            return HttpStatusCode.NotFound;
        }

        public List<SocioliteTeamProperty> GetUnconnectedTeams()
        {
            return ctx.Teams.Where(t => t.MSTeamsTeamId == null).ToList();
        }

        public async Task<HttpResponseMessage> GetJoinedTeams(string userId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            List<SocioliteTeamProperty> teams;
            try
            {
                // We get the memberships for the user
                var memberships = ctx.TeamMemberships.Where(m => m.UserId.Equals(userId)).ToList();

                // We make a list with the team IDs
                List<int> teamIds = new List<int>();

                foreach(var membership in memberships)
                {
                    teamIds.Add(membership.TeamId);
                }

                // Then we get the joined teams
                teams = ctx.Teams.Where(t => teamIds.Any(id => id == t.TeamId)).ToList();
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("ERROR: Something went wrong!");
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;

                // We get the roles for the teams
                List<Tuple<SocioliteTeamProperty, List<Tuple<UserProperty, string>>>> teamsWithRoles = new List<Tuple<SocioliteTeamProperty, List<Tuple<UserProperty, string>>>>();
                
                foreach (var team in teams)
                {
                    if (team.MSTeamsTeamId != null)
                    {
                        var membershipsForTeam = ctx.TeamMemberships.Where(m => m.TeamId == team.TeamId).ToList();

                        List<Tuple<UserProperty, string>> usersWithRoles = new List<Tuple<UserProperty, string>>();

                        foreach (var membership in membershipsForTeam)
                        {
                            UserProperty user = ctx.Users.Where(u => u.MSTeamsId.Equals(membership.UserId)).FirstOrDefault();
                            Tuple<UserProperty, string> userWithRole = new Tuple<UserProperty, string>(user, membership.TeamSpecificRole);
                            usersWithRoles.Add(userWithRole);
                        }

                        teamsWithRoles.Add(new Tuple<SocioliteTeamProperty, List<Tuple<UserProperty, string>>>(team, usersWithRoles));
                    }
                }
                if (teams.Count == 0)
                {
                    response.Content = new StringContent("No joined teams!");
                }
                response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(teamsWithRoles));
            
            return response;
        }

        public async Task<HttpResponseMessage> WipeAll()
        {
            foreach (var item in ctx.TeamMemberships)
            {
                ctx.TeamMemberships.Remove(item);
            }
            ctx.SaveChanges();

            foreach (var item in ctx.Users)
            {
                ctx.Users.Remove(item);
            }
            ctx.SaveChanges();

            foreach (var item in ctx.Teams)
            {
                ctx.Teams.Remove(item);
            }

            foreach (var item in ctx.Activities)
            {
                ctx.Activities.Remove(item);

            }
            foreach (var item in ctx.PollVotes)
            {
                ctx.PollVotes.Remove(item);
            }
            // Don't remove default activities
            foreach (var item in ctx.CustomPolls.Where(x => x.TeamId != 0).ToList())
            {
                ctx.CustomPolls.Remove(item);
            }
            foreach (var item in ctx.CustomDiscussions.Where(x => x.TeamId != 0).ToList())
            {
                ctx.CustomDiscussions.Remove(item);
            }
            ctx.SaveChanges();

            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("ALL WIPED!");
            return response;
        }

        public async Task<HttpResponseMessage> UpdateRecurranceString(int teamId, string recurranceString)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                SocioliteTeamProperty team = ctx.Teams.Where(t => t.TeamId == teamId).First();

                team.Recurring = recurranceString;
                await ctx.SaveChangesAsync();
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Succesfully updated recurrance string!");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to update recurrance string!");
                return response;
            }
        }


    }
}
