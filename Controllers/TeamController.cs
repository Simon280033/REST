using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;
using Properties;
using Properties.Team;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using WebAPI.Model;
using WebAPI.Model.TeamFolder;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private ITeamContext team;

        public TeamController(ITeamContext team)
        {
            this.team = team;
        }
        // GET: api/<UserController>
        //[HttpGet]
        //public List<string> Get()
        //{

        //    //return team.GetTeams();
        //    return null;
        //}

        // GET api/<UserController>/5
        [HttpGet()]
        public List<SocioliteTeamProperty> Get([FromHeader] int teamId)
        {
            var TeamId = team.GetTeam(teamId);
            if (TeamId != null)
            {
                return TeamId;
            }
            return null;

        }


        [HttpGet("AllTeams")]
        public List<SocioliteTeamProperty> GetAllTeams()
        {
            return team.GetTeams();
        }

        [HttpGet("JoinedTeams/{userId}")]
        public async Task<IActionResult> GetJoinedTeams([FromHeader] string userId)
        {
            var response = await team.GetJoinedTeams(userId);
            string JsonContentString = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return Ok(JsonContentString); // List as JSON
            }
            else
            {
                return NotFound(JsonContentString);
            }
        }

        [HttpGet("UnconnectedTeams")]
        public List<SocioliteTeamProperty> GetUnconnectedTeams()
        {
            return team.GetUnconnectedTeams();
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string channelId)
        {
            HttpResponseMessage response = await team.PostTeam(channelId);
            string message = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Ok(message);
            } else
            {
                return BadRequest(message);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut()]
        public int Put([FromHeader] int teamId, [FromBody] JsonObject Recurring)
        {
            string re = Recurring.First().Value.ToString();
            return team.PutTeam(re, teamId);
            //return 0;
        }


        [HttpPut("{teamId}")]
        public async Task<HttpStatusCode> Put(int teamId)
        {
            
            return await team.ChangeActiveStatus(teamId);
            //return 0;
        }

        // DELETE api/<UserController>/5
        [HttpDelete("WipeAll")]
        public async Task<IActionResult> WipeAll()
        {
            HttpResponseMessage response = await team.WipeAll();
            string message = await response.Content.ReadAsStringAsync();
            return Ok(message);
        }

        [HttpPut("RecurranceString")]
        public async Task<IActionResult> UpdateRecurranceString([FromBody] Tuple<string, string> teamIdAndNewRecurranceString)
        {
            HttpResponseMessage response = await team.UpdateRecurranceString(Int32.Parse(teamIdAndNewRecurranceString.Item1), teamIdAndNewRecurranceString.Item2);
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
