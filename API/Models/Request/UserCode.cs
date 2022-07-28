using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class UserCode
    {
        public Guid Id { get; set; }
        [MaxLength(30, ErrorMessage = "Username too long")]
        [MinLength(3, ErrorMessage = "Username too short")]
        public string UserName { get; set; }

        public string Email { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "Password too long")]
        [MinLength(8, ErrorMessage = "Password too short")]
        public string Password { get; set; }
        public ValidationCode Code { get; set; }
    }
}
