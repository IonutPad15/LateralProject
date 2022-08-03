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
        public string CommentBody { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PostId { get; set; }
        public int Votes { get; set; }  
        override
        public string ToString()
        {
            string comm = "      Author:" + Author + "\n      Comment:" + CommentBody + "\n";
            return comm;
        }
    }
}
