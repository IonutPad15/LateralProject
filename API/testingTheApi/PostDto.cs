using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: projects should start with an uppercase letter
namespace testingTheApi
{
    //TODO: remove the Dto suffix
    internal class PostDto
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        override 
        public string ToString()
        {
            string post = "User:" +User+"\n"+Title+"\n"+Description;
            return post;
        }
    }
}
