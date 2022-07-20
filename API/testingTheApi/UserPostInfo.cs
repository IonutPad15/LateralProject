using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingTheApi
{
    public class UserPostInfo
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Post>? Posts { get; set; }
        public List<Comment>? Comments { get; set; }
        public string ToString()
        {
            StringBuilder user = new StringBuilder();
            user.Append("User: " + UserName + "\nemail: " + Email + "\n");
            user.Append("Posts:\n");
            foreach(var post in Posts)
            {
                user.AppendLine("User:" + UserName + "\n" + post.Title + "\n" + post.Description);
            }
            user.Append("Comments:\n");
            if(Comments!= null)
            foreach (var comment in Comments)
            {
                user.Append(comment.ToString());
            }
            return user.ToString();
        }

    }
}
