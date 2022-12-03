using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Properties;
using Properties.Team;
using Sociolite.Models;
using System.Net;
using System.Text.Json.Nodes;

namespace WebAPI.Model.MembershipFolder
{
    public interface IMembershipContext
    {
        List<Object> GetMembership(List<int> data);
        Task<List<SocioliteTeamMembershipProperty>> GetMemberships();
        Task<HttpResponseMessage> UpdateMembership(int teamId, string userId, string newRole);
        Task<HttpResponseMessage> TieUserToTeams(List<SocioliteTeam> teamsWithChannels, string userId);
        Task<int> DeleteMembership(int id);
    }
}
