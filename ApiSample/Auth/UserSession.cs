namespace ApiSample.Auth
{
    public class UserSession
    {
        public int Id { get; set; }
        public List<string> Profiles { get; set; } = new();
    }
}
