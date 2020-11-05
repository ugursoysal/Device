namespace Server.Models
{
    public class Account
    {
        public string Username { get; set; }

        public string Password { get; set; }
        public Account(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
