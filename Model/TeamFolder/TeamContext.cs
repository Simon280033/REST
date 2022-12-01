using Azure;
using Properties;
using Properties.Team;
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
            //List<TeamProperty> teams = (from a in ctx.Teams select a).ToList();
            //return new List<TeamProperty>();
            return null;
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
    }
}
