using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class VoteInfo
    {
        public Guid? UserId { get; set; }
        public bool IsUpVote { get; set; }
    }
}
