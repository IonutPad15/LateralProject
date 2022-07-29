using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        override
        public string ToString()
        {
            string user = "User: " + UserName + "\nemail: " + Email + "\n";
            return user;
        }
    }
}
