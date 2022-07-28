using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class PostRequest
    {
        [MaxLength(100, ErrorMessage = "Title too long")]
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
