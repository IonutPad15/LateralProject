using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30,ErrorMessage ="Username too long")]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(256,ErrorMessage ="Password too long")]
        [MinLength(8,ErrorMessage ="Password too short")]
        public string Password { get; set; }    
    }
}
