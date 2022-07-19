namespace API.Models
{
    public class UserPostsInfo
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Post>? Posts { get; set; }
    }
}
