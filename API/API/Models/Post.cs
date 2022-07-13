using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    public class Post
    {
        [Key]
        public Guid Id{ get; set; }
        //TODO: is this user id, name, email, ...?
        //the post id,
        [Required]
        public string User { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Title too long")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
       
    }
}
