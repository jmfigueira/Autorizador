using authorizer_domain.Entities;
using authorizer_domain.Enums;
using authorizer_domain.Interfaces;
using authorizer_infra_shared;
using authorizer_service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace authorizer_service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public string Insert(Transaction transaction)
        {
            AccountResponse response = new()
            {
                Violations = new List<string>(),
                Account = new Model.Account()
            };

            var user = _accountRepository.Select(_accountRepository.UserID());

            if (user == null)
            {
                response.Violations.Add(Enumerations.GetEnumDescription(Violations.AccountNotInitialized));

                return JsonConvert.SerializeObject(response);
            }

            if (!user.ActiveCard)
                response.Violations.Add(Enumerations.GetEnumDescription(Violations.CardNotActive));

            if (user.AvailableLimit < transaction.Amount)
            {
                response.Violations.Add(Enumerations.GetEnumDescription(Violations.InsufficientLimit));
            }

            var allTransactions = _transactionRepository.GetAllTransactions();

            if (allTransactions.Count() >= 3)
            {
                var hsfi = true;

                foreach (var tr in allTransactions)
                {
                    var diferencaDatas = transaction.Time - tr.Time;

                    if (diferencaDatas >= new TimeSpan(0, 2, 0))
                        hsfi = false;
                }

                if (hsfi)
                    response.Violations.Add(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval));
            }

            foreach (var tr in allTransactions)
            {
                if (tr.Amount == transaction.Amount && tr.Merchant == transaction.Merchant)
                {
                    var diferencaDatas = transaction.Time - tr.Time;

                    if (diferencaDatas <= new TimeSpan(0, 2, 0))
                        response.Violations.Add(Enumerations.GetEnumDescription(Violations.DoubledTransaction));
                }
            }

            if (user.AvailableLimit > 0 && user.AvailableLimit >= transaction.Amount)
            {
                var newLimit = user.AvailableLimit - transaction.Amount;

                _transactionRepository.Insert(transaction);

                user = _accountRepository.UpdateAvailableLimit(_accountRepository.UserID(), newLimit);
            }

            response.Account = new Model.Account()
            {
                ActiveCard = user.ActiveCard,
                AvailableLimit = user.AvailableLimit
            };

            return JsonConvert.SerializeObject(response);
        }
    }
}
