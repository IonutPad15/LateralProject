using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingTheApi
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<Post> Posts { get; set; }
        public bool IsDeleted { get; set; }
        override
        public string ToString()
        {
            string user = "User: " + UserName + "\nemail: " + Email + "\n";
            return user;
        }
    }
}
