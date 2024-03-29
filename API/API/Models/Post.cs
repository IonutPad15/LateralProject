﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Post
    {
        [Key]
        public Guid Id{ get; set; }
        //TODO: is this user id, name, email, ...?
        //the post id,
        //[Required]
        //public string User { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Title too long")]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        //lista de comentarii cu foreing key

        //navigation properties
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public string? Author { get; set; }
        public List<Comment> Comments { get; set; } = null!;
        public List<Vote> Votes { get; set; } = null!;
        public bool IsDeleted { get; set; }
        
        //public UserInfo User { get; set; }
    }
}
