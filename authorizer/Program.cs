using authorizer.Helpers;
using authorizer_domain.Interfaces;
using authorizer_infra_crosscutting.InversionOfControl;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace authorizer
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public Program()
        {

        }

        static void Main(string[] args)
        {
            RegisterServices();

            IServiceScope scope = _serviceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<Runner>().Run();
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<Runner>();

            services.AddRepositoryDependency();
            services.AddServiceDependency();
            services.AddContextDependency();
            services.AddLogging();

            services.AddSingleton<ICommandValidator, CommandValidator>();

            _serviceProvider = services.BuildServiceProvider(true);
        }
    }
}