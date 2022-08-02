using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Vote
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("Post")]
        public Guid? PostId { get; set; }
        [ForeignKey("Comment")]
        public Guid? CommentId { get; set; }
        [Required]
        public bool IsUpVote { get; set; }
    }
}
