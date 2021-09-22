using authorizer_domain.Entities;
using authorizer_domain.Interfaces;
using authorizer_infra.Context;
using System;
using System.Collections.Generic;

namespace authorizer_infra.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        protected readonly PersistContext _persistContext;

        public TransactionRepository(PersistContext persistContext)
        {
            _persistContext = persistContext;
        }

        public void Insert(Transaction transaction)
        {
            _persistContext.Add<Transaction>(Guid.NewGuid(), transaction);
        }

        public Transaction Select(Guid transactionId)
        {
            return _persistContext.Get<Transaction>(transactionId);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _persistContext.GetAll<Transaction>();
        }
    }
}