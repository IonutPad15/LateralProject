using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: projects should start with an uppercase letter
namespace testingTheApi
{
    //TODO: remove the Dto suffix
    public class Post
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public Guid UserId { get; set; }
        public string Author { get; set; }
        public List<Comment> Comments { get; set; }
        override 
        public string ToString()
        {
            string post = "User:" +Author+"\n"+Title+"\n"+Description;
            return post;
        }
    }
}
