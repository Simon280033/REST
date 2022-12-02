using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Properties;
using Properties.Team;
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
        public async Task<IActionResult> TieUserToTeams([FromBody] List<Tuple<Tuple<string, string>, List<string>>> joinedChannels, [FromHeader] string userId)
        {
            HttpResponseMessage response = await membership.TieUserToTeams(joinedChannels, userId);
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
        [HttpPut()]
        public async Task<IActionResult> Put([FromHeader] int teamId, [FromHeader] string userId)
        {
            await membership.PutMembership(teamId, userId);
            return Ok();
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<int> Delete(int id)
        {
            return await membership.DeleteMembership(id);
        }
    }
}
