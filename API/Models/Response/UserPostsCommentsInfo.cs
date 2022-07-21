using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class UserPostsCommentsInfo
    {
        public UserPostsCommentsInfo()
        {
            Comments = new List<CommentInfo>();
            Posts = new List<PostInfo>();
        }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<PostInfo> Posts { get; set; }
        public List<CommentInfo> Comments { get; set; }
    }
}
