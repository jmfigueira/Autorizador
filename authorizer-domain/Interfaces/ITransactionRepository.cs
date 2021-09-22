using authorizer_domain.Entities;
using System;
using System.Collections.Generic;

namespace authorizer_domain.Interfaces
{
    public interface ITransactionRepository
    {
        void Insert(Transaction transaction);
        Transaction Select(Guid transactionId);
        IEnumerable<Transaction> GetAllTransactions();
    }
}