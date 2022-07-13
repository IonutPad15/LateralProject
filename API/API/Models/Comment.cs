using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    //TODO: you don't need the Model suffix. you're already in the models namespace
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } //TODO: use Guid if possible
        [Required]
        //UserPost refers to the user that posted
        public string UserPost { get; set; }

        //TODO: can a user have multiple posts with the same title?
        //I was thinking the comments table needs to know for each comment wich post it belongs to
        [Required]
        public string PostTitle { get; set; } 
        //userComment refers to the user that posted the comment, while the textComment refers to 
        //the comment itself
        //TODO: what's the difference between an UserComment and a TextComment?
        [Required]
        public string UserComment { get; set; }
        [Required]
        public string TextComment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}
