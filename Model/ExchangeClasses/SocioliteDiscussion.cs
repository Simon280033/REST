using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociolite.Models
{
    public class SocioliteDiscussion
    {
        public string Id { get; set; }
        public string CreatedById { get; set; }
        public string CreationTime { get; set; }
        public string Topic { get; set; }
    }
}
