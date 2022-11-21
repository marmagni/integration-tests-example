namespace ApiSample.Auth
{
    public interface IAuthService
    {
        string CreateJwtToken(UserSession userSession);
    }
}
