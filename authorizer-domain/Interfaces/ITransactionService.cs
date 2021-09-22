using authorizer_domain.Entities;

namespace authorizer_domain.Interfaces
{
    public interface ITransactionService
    {
        string Insert(Transaction transaction);
    }
}