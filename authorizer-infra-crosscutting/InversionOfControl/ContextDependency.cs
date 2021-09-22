using authorizer_infra.Context;
using Microsoft.Extensions.DependencyInjection;

namespace authorizer_infra_crosscutting.InversionOfControl
{
    public static class ContextDependency
    {
        public static void AddContextDependency(this IServiceCollection services)
        {
            services.AddSingleton<PersistContext>();
        }
    }
}