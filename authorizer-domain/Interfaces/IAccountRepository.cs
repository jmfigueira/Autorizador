using authorizer_domain.Entities;
using System;

namespace authorizer_domain.Interfaces
{
    public interface IAccountRepository
    {
        Account UpdateAvailableLimit(Guid userId, int newLimit);
        Account Insert(Account account);
        Account Select(Guid userId);
        Guid UserID();
    }
}