using Properties.Team;
using REST.Model.ExchangeClasses;

namespace WebAPI.Model.MembershipFolder
{
    public interface IMembershipContext
    {
        List<Object> GetMembership(List<int> data);
        Task<List<SocioliteTeamMembershipProperty>> GetMemberships();
        Task<HttpResponseMessage> UpdateMembership(int teamId, string userId, string newRole);
        Task<HttpResponseMessage> TieUserToTeams(List<SocioliteTeam> teamsWithChannels, string userId);
    }
}
