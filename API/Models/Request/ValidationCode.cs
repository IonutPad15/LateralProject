using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class ValidationCode
    {
        public string Code { get; set; }
        public DateTime Created { get; set; }
    }
}
