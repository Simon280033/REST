using Microsoft.AspNetCore.Mvc;
using Properties;
using REST.Model.ExchangeClasses;
using WebAPI.Model.DisccusionFolder;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private IDiscussionContext DiscussionContext;

        public DiscussionController(IDiscussionContext disucssion)
        {
            this.DiscussionContext = disucssion;
        }

        [HttpGet("{teamId}")]
        public async Task<List<CustomDiscussionProperty>> Get([FromHeader] string teamId)
        {
            return await DiscussionContext.GetAllDiscussions(Int32.Parse(teamId));
        }

        [HttpPost("{teamId}")]
        public async Task<IActionResult> Post([FromBody] List<SocioliteDiscussion> discussions, [FromHeader] string teamId)
        {
            List<CustomDiscussionProperty> customDiscussionProperties = new List<CustomDiscussionProperty>();

            foreach (var discussion in discussions)
            {
                CustomDiscussionProperty customDiscussionProperty = new CustomDiscussionProperty
                {
                    Id = 0,
                    TeamId = Int32.Parse(teamId),
                    CreatedBy = discussion.CreatedById,
                    TopicText = discussion.Topic,
                    CreatedAt = DateTime.ParseExact(discussion.CreationTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                customDiscussionProperties.Add(customDiscussionProperty);
            }

            HttpResponseMessage response = await DiscussionContext.PostDiscussions(customDiscussionProperties);
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
        public async Task<IActionResult> Delete([FromBody] List<SocioliteDiscussion> discussions, [FromHeader] string teamId)
        {
            // WE MAKE THIS A PUT SO WE CAN SEND A BODY
            List<CustomDiscussionProperty> customDiscussionProperties = new List<CustomDiscussionProperty>();

            foreach (var discussion in discussions)
            {
                CustomDiscussionProperty customDiscussionProperty = new CustomDiscussionProperty
                {
                    Id = Int32.Parse(discussion.Id),
                    TeamId = Int32.Parse(teamId),
                    CreatedBy = discussion.CreatedById,
                    TopicText = discussion.Topic,
                    CreatedAt = DateTime.ParseExact(discussion.CreationTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                customDiscussionProperties.Add(customDiscussionProperty);
            }

            HttpResponseMessage response = await DiscussionContext.DeleteDiscussion(customDiscussionProperties);
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
