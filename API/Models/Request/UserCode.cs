using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class UserCode
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public ValidationCode Code { get; set; }
    }
}
