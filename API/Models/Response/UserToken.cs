using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Response
{
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public Guid UserId { get; set; }
    }
}
