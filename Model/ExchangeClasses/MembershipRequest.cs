using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociolite.Models
{
    public class MembershipRequest
    {
        public string userId { get; set; }
        public int teamId { get; set; }
        public string newRole { get; set; }
    }
}
