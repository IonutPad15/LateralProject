using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    //TODO: you don't need the Model suffix. you're already in the models namespace
    public class CommentModel
    {
        [Key]
        public int Id { get; set; } //TODO: use Guid if possible
        [Required]
        public string UserPost { get; set; }

        //TODO: can a user have multiple posts with the same title?
        [Required]
        public string PostTitle { get; set; }
        //TODO: what's the difference between an UserComment and a TextComment?
        [Required]
        public string UserComment { get; set; }
        [Required]
        public string TextComment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}
