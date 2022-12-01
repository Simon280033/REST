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

        public Task<List<SocioliteTeamMembershipProperty>> GetMemberships()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> PostMembership(SocioliteTeamProperty team, string id)
        {
            try
            {
                UserProperty user = ctx.Users.Where(t => t.MSTeamsId.Equals(id)).FirstOrDefault();
                SocioliteTeamProperty teamFromdB = ctx.Teams.Where(t => t.TeamId == team.TeamId).FirstOrDefault();

                if (user != null && teamFromdB != null)
                {
                    SocioliteTeamMembershipProperty membership = new SocioliteTeamMembershipProperty();
                    membership.UserId = user.MSTeamsId;
                    membership.TeamId = team.TeamId;
                    membership.TeamSpecificRole = "Manager";
                    

                    teamFromdB.MSTeamsTeamId = team.MSTeamsTeamId;
                    teamFromdB.Name = team.Name;
                    teamFromdB.Recurring = team.Recurring;
                    teamFromdB.isActive = team.isActive;
                    ctx.TeamMemberships.Add(membership);
                    ctx.SaveChanges();
                } else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                return HttpStatusCode.NotFound;
            }
            return HttpStatusCode.OK;
        }

        public Task<int> PutMembership(int teamId, JsonObject jsonObject)
        {
            
            var listOfUSers = jsonObject.ToArray();

           // ICollection<UserProperty> users = jsonObject.Where(t => t.Key.Contains("UserProperty"));

            //listOfUSers.Where(t=> t.Value.=teamId).ToList();
            throw new NotImplementedException();
        }
    }
}
