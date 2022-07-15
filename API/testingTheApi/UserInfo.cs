namespace testingTheApi
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ToString()
        {
            string user = "User: " + UserName + "\nemail: " + Email + "\n";
            return user;
        }
    }
}
