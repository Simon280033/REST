using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Properties;
using Properties.Team;
using System.Net;
using System.Text.Json.Nodes;

namespace WebAPI.Model.MembershipFolder
{
    public interface IMembershipContext
    {
        List<Object> GetMembership(List<int> data);
        Task<List<SocioliteTeamMembershipProperty>> GetMemberships();
        Task<HttpResponseMessage> PutMembership(int teamId, string userId);
        Task<HttpResponseMessage> TieUserToTeams(List<Tuple<Tuple<string, string>, List<string>>> data, string userId);
        Task<int> DeleteMembership(int id);
    }
}
