using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociolite.Models
{
    public class SocioliteTeam
    {
        public string Id { get; set; }

        public string TeamsTeamId { get; set; }

        public List<string> TeamsChannelIds { get; set; }

        public string SocioliteChannelId { get; set; }

        public string Name { get; set; }

        public List<SocioliteUser> Members { get; set; }

        public string RecurranceString { get; set; }
        public List<SocioliteDiscussion> CustomDiscussions { get; set; }
        public List<SociolitePoll> CustomPolls { get; set; }

        public List<string> ManagerIds { get; set; }

        public List<string> SchedulerIds { get; set; }

        public bool ActivityIsActive { get; set; }

    }
}
