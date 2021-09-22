using authorizer_domain.Entities;
using authorizer_domain.Interfaces;
using authorizer_infra.Context;
using System;

namespace authorizer_infra.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private static Guid UserGUID;
        protected readonly PersistContext _persistContext;

        public AccountRepository(PersistContext persistContext)
        {
            _persistContext = persistContext;
        }

        public Guid UserID()
        {
            if (UserGUID == Guid.Empty)
                UserGUID = Guid.NewGuid();

            return UserGUID;
        }

        public Account Insert(Account account)
        {
            return _persistContext.Add<Account>(UserGUID, account);
        }

        public Account Select(Guid key)
        {
            return _persistContext.Get<Account>(key);
        }

        public Account UpdateAvailableLimit(Guid userId, int newLimit)
        {
            var account = _persistContext.Get<Account>(userId);

            account.AvailableLimit = newLimit;

            _persistContext.Update<Account>(userId, account);

            return account;
        }
    }
}