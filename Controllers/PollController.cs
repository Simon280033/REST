using Microsoft.AspNetCore.Mvc;
using Properties;
using REST.Model.ExchangeClasses;
using WebAPI.Model.PollFolder;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollController : ControllerBase
    {
        private IPollContext PollContext;

        public PollController(IPollContext poll)
        {
            this.PollContext = poll;
        }

        [HttpPost("{teamId}")]
        public async Task<IActionResult> Post([FromBody] List<SociolitePoll> polls, [FromHeader] string teamId)
        {
            List<CustomPollProperty> customPollProperties = new List<CustomPollProperty>();

            foreach (var poll in polls)
            {
                CustomPollProperty pollToAdd = new CustomPollProperty
                {
                    Id = 0,
                    TeamId = Int32.Parse(teamId),
                    CreatedBy = poll.CreatedById,
                    Question = poll.Question,
                    PollOptions = Newtonsoft.Json.JsonConvert.SerializeObject(poll.Answers),
                    CreatedAt = DateTime.ParseExact(poll.CreationTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                customPollProperties.Add(pollToAdd);
            }

            HttpResponseMessage response = await PollContext.PostPolls(customPollProperties);
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


        [HttpPut("{teamId}")]
        public async Task<IActionResult> Delete([FromBody] List<SociolitePoll> polls, [FromHeader] string teamId)
        {
            List<int> pollIds = new List<int>();

            foreach (var poll in polls)
            {
                pollIds.Add(Int32.Parse(poll.Id));
            }

            HttpResponseMessage response = await PollContext.DeletePolls(pollIds);
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

        [HttpGet("{teamId}")]
        public async Task<List<CustomPollProperty>> Get([FromHeader] string teamId)
        {
            return await PollContext.GetAllPolls(Int32.Parse(teamId));
        }

    }
}
