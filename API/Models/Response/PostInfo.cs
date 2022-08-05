using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class PostInfo
    {
        public PostInfo()
        {
            Comments = new List<CommentInfo>();
            Votes = new List<VoteInfo>();
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid? UserId { get; set; }
        public List<CommentInfo> Comments { get; set; }
        public int NrOfVotes { get; set; }
        public List<VoteInfo> Votes { get; set; }
        override
        public string ToString()
        {
            string post = "User:" + Author + "\n" + Title + "\n" + Description + "\n";
            return post;
        }
        public string PostWithComments()
        {
            StringBuilder post = new StringBuilder();
            post.Append(ToString());
            if (Comments != null)
            {
                foreach (var comm in Comments)
                {
                    post.Append(comm.ToString());
                }
            }
            return post.ToString();
        }
    }
}
