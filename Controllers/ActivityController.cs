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
    }
}
