namespace SharedTemplate
{
    public interface IRegistrationForm
    {
        string UserName { get; set; }
        string Password { get; set; }
        string RolesCommaSeparated { get; set; }
        string Email { get; set; }
    }
}