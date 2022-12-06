using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Properties;
using REST.Model.ActivityFolder;
using REST.Model.ExchangeClasses;
using WebAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private IActivityContext ac;

        public ActivityController(IActivityContext ac)
        {
            this.ac = ac;
        }

        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetNextActivityForTeam([FromHeader] string teamId)
        {
            Tuple<HttpResponseMessage, string> responseAndType = await ac.GetNextActivityForTeam(teamId);
            HttpResponseMessage response = responseAndType.Item1;
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

        [HttpGet("TeamAndActivityByChannelId")]
        public async Task<IActionResult> TeamAndActivityByChannelId([FromHeader] string channelId)
        {
            ActivityRequestObject data = await ac.TeamAndActivityByChannelId(channelId);
            string message = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return Ok(message);
        }

        [HttpGet("LatestActivityType")]
        public async Task<IActionResult> LatestActivityType([FromHeader] string channelId)
        {
            HttpResponseMessage response = await ac.LastActivityType(channelId);
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


        [HttpPost("Vote")]
        public async Task<IActionResult> Vote([FromHeader] string channelId, [FromHeader] string userId, [FromHeader] int optionNumber)
        {
            HttpResponseMessage response = await ac.Vote(channelId, userId, optionNumber);
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

        [HttpGet("ActivePoll")]
        public async Task<IActionResult> GetActivePoll([FromHeader] string channelId)
        {
            HttpResponseMessage response = await ac.GetActivePollOptionAmount(channelId);
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
