using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingTheApi
{
    public class UserCodeDto
    {
       
            public Guid Id { get; set; }

            public string UserName { get; set; }

            public string Email { get; set; }
            public string Password { get; set; }
            public RegisterCode Code { get; set; }
        
    }
}
