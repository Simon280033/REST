using Microsoft.AspNetCore.Mvc;
using Properties.Team;
using REST.Model.ExchangeClasses;
using WebAPI.Model.MembershipFolder;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private IMembershipContext membership;

        public MembershipController(IMembershipContext membership)
        {
            this.membership = membership;
        }

        [HttpGet]
        public async Task<List<SocioliteTeamMembershipProperty>> Get()
        {
            return await membership.GetMemberships();
        }

        [HttpPost("TieUserToTeams/{userId}")]
        public async Task<IActionResult> TieUserToTeams([FromBody] List<SocioliteTeam> teamsWithChannels, [FromHeader] string userId)
        {
            HttpResponseMessage response = await membership.TieUserToTeams(teamsWithChannels, userId);
            string message = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Ok(message);
            }
            else
            {
                return BadRequest(message);
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateMembership([FromBody] MembershipRequest request)
        {
            HttpResponseMessage response = await membership.UpdateMembership(request.teamId, request.userId, request.newRole);
            string message = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Ok(message);
            }
            else
            {
                return BadRequest(message);
            }
        }
    }
}
