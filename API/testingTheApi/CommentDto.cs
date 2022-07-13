using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingTheApi
{
    internal class CommentDto
    {
        public int Id { get; set; }
        public string UserPost { get; set; }
        public string PostTitle { get; set; }
        public string UserComment { get; set; }
        public string TextComment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string ToString()
        {
            string comm = "UserPost: " + UserPost + "   " + PostTitle + "\nUser:" + UserComment + ":   " + TextComment;
            return comm; 
        }
    }
}
