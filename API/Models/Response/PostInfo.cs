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
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<CommentInfo> Comments { get; set; }
        public string ToString()
        {
            string post = "User:" + Author + "\n" + Title + "\n" + Body + "\n";
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
