using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserPost { get; set; }
        [Required]
        public string PostTitle { get; set; }
        [Required]
        public string UserComment { get; set; }
        [Required]
        public string TextComment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}
