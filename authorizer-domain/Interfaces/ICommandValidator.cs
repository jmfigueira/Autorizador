namespace authorizer_domain.Interfaces
{
    public interface ICommandValidator
    {
        bool IsValid(string command);
    }
}