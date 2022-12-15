using Microsoft.AspNetCore.Mvc;
using WebAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUser user;

        public UserController(IUser user)
        {
            this.user = user;
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = user.GetUsers();
            if (users.Count > 0)
            {
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(users)); // List as JSON
            }
            else
            {
                return BadRequest("No users found");
            }
        }
    }
}
