namespace ProjectAPI.Controllers
{
    public enum StatementState
    {
        Undefined = 0,
        Valid = 1,
        PendingInvalid = 2,
        Invalid = 3,
        PendingValid = 4,
    }
}