using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Properties;
using Properties.Team;
using Sociolite.Models;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using WebAPI.Model;
using WebAPI.Model.MembershipFolder;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        // GET: MembershipController

        private IMembershipContext membership;

        public MembershipController(IMembershipContext membership)
        {
            this.membership = membership;
        }

        // GET: api/<UserController>
        //[HttpGet]
        //public async Task<List<SocioliteTeamMembershipProperty>> Get()
        //{
        //    //return await membership.GetMemberships();
        //    return new List<SocioliteTeamMembershipProperty>();
        //}


        

        // GET api/<UserController>/5
        [HttpGet]
        public async Task<List<SocioliteTeamMembershipProperty>> Get()
        {
            return await membership.GetMemberships();
        }

        // POST api/<UserController>
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

        // PUT api/<UserController>/5
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateMembership([FromHeader] int teamId, [FromHeader] string userId, [FromHeader] string newRole)
        {
            HttpResponseMessage response = await membership.UpdateMembership(teamId, userId, newRole);
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

        [HttpPost("Update2")]
        public async Task<IActionResult> UpdateMembership2([FromBody] MembershipRequest test)
        {
            HttpResponseMessage response = await membership.UpdateMembership(test.teamId, test.userId, test.newRole);
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

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<int> Delete(int id)
        {
            return await membership.DeleteMembership(id);
        }
    }
}
