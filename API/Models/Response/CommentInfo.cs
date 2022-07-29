using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class CommentInfo
    {
        public Guid Id { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PostId { get; set; }
        override
        public string ToString()
        {
            string comm = "      Author:" + Author + "\n      Comment:" + Body + "\n";
            return comm;
        }
    }
}
