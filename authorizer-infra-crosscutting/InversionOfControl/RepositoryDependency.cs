using authorizer_domain.Interfaces;
using authorizer_infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace authorizer_infra_crosscutting.InversionOfControl
{
    public static class RepositoryDependency
    {
        public static void AddRepositoryDependency(this IServiceCollection services)
        {
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<ITransactionRepository, TransactionRepository>();
        }
    }
}