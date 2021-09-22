using authorizer_domain.Interfaces;

namespace authorizer.Helpers
{
    public class CommandValidator : ICommandValidator
    {
        public bool IsValid(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return true;
        }
    }
}