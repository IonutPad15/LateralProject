using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    public class PostModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Title too long")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string ToString()
        {
            string Post = "User: " + User + "\n" + Title + "\n" + Description + "\n";
            return Post;
        }
    }
}
