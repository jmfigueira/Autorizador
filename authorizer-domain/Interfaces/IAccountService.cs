using authorizer_domain.Entities;

namespace authorizer_domain.Interfaces
{
    public interface IAccountService
    {
        string Insert(Account account);
    }
}