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
            HttpResponseMessage response = await ac.GetNextActivityForTeam(teamId);
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
