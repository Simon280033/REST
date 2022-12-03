using Microsoft.AspNetCore.Mvc;
using Properties;
using Properties.Team;
using System.Net;

namespace WebAPI.Model.TeamFolder
{
    public interface ITeamContext
    {
        Task<HttpResponseMessage> GetJoinedTeams(string userId);

        List<SocioliteTeamProperty> GetTeam(int user);
        List<SocioliteTeamProperty> GetTeams();
        int PutTeam(string recurrence, int teamId);

        Task<HttpStatusCode> ChangeActiveStatus(int teamId);
        Task<HttpResponseMessage> PostTeam(string channelID);
        Task<HttpStatusCode> DeleteTeam(int id);
        List<SocioliteTeamProperty> GetUnconnectedTeams();

        Task<HttpResponseMessage> WipeAll();
        Task<HttpResponseMessage> UpdateRecurranceString(int teamId, string recurranceString);
    }
}
