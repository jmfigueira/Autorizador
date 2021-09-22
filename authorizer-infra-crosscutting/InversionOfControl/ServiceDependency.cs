using authorizer_domain.Interfaces;
using authorizer_service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace authorizer_infra_crosscutting.InversionOfControl
{
    public static class ServiceDependency
    {
        public static void AddServiceDependency(this IServiceCollection services)
        {
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<ITransactionService, TransactionService>();
        }
    }
}