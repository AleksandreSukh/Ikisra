namespace SharedTemplate
{
    public interface IAuthToken
    {
        string access_token { get; set; }
        int expires_in { get; set; }
        string token_type { get; set; }
    }
}