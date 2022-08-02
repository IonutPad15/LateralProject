using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    //TODO: you don't need the Model suffix. you're already in the models namespace
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } 
        public string? Author { get; set; }
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        [ForeignKey("Post")]
        public Guid? PostId { get; set; }
        [Required]
        public string? CommentBody { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<Vote> Votes { get; set; } = null!;
        public bool IsDeleted { get; set; }

    }
}
