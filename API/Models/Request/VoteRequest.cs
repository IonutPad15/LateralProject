using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class VoteRequest
    {
        public Guid? Id { get; set; }    
        public Guid? UserId { get; set; }
        
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public bool IsUpVote { get; set; }
    }
}
