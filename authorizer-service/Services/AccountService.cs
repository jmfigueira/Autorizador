using authorizer_domain.Enums;
using authorizer_domain.Interfaces;
using authorizer_infra_shared;
using authorizer_service.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace authorizer_service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repositoryAccount;

        public AccountService(IAccountRepository repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        public string Insert(authorizer_domain.Entities.Account account)
        {
            AccountResponse response = new()
            {
                Violations = new List<string>(),
                Account = new Model.Account()
            };

            var user = _repositoryAccount.Select(_repositoryAccount.UserID());

            if (user == null)
            {
                user = _repositoryAccount.Insert(account);

                response.Account = new Model.Account()
                {
                    ActiveCard = user.ActiveCard,
                    AvailableLimit = user.AvailableLimit
                };
            }
            else
                response.Violations.Add(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized));

            return JsonConvert.SerializeObject(response);
        }
    }
}