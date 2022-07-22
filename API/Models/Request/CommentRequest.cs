using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class CommentRequest
    {
        public Guid PostId { get; set; }
        public string Body { get; set; }
    }
}
