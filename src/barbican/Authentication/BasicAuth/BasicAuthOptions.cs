namespace barbican.Authentication.BasicAuth
{
    public class BasicAuthOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Realm { get; set; } = "barbican";
    }
}
