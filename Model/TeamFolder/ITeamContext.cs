using Properties;

namespace WebAPI.Model.TeamFolder
{
    public interface ITeamContext
    {
        Task<HttpResponseMessage> GetJoinedTeams(string userId);

        List<SocioliteTeamProperty> GetTeam(int user);
        List<SocioliteTeamProperty> GetTeams();

        Task<HttpResponseMessage> ChangeActiveStatus(int teamId);
        Task<HttpResponseMessage> PostTeam(string channelID);
        List<SocioliteTeamProperty> GetUnconnectedTeams();

        Task<HttpResponseMessage> WipeAll();
        Task<HttpResponseMessage> UpdateRecurranceString(int teamId, string recurranceString);


    }
}
