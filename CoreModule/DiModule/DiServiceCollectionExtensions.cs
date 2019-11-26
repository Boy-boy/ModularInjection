using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ModularInjection
{
    public static class DiServiceCollectionExtensions
    {
        public static IServiceProvider AddModule<TStartupModule>(this IServiceCollection services)
            where TStartupModule : DiModule
        {
            var diBootstrapper = AddDiBootstrapper<TStartupModule>(services);
            return AutofacServiceProvider(services, diBootstrapper);
        }

        private static DiBootstrapper AddDiBootstrapper<TStartupModule>(IServiceCollection services)
            where TStartupModule : DiModule
        {
            var abpBootstrapper = DiBootstrapper.Create<TStartupModule>(services);
            abpBootstrapper.Initialize();
            return abpBootstrapper;
        }

        private static IServiceProvider AutofacServiceProvider(IServiceCollection services, DiBootstrapper diBootstrapper)
        {
            diBootstrapper.ContainerBuilder.Populate(services);
            var applicationContainer = diBootstrapper.ContainerBuilder.Build();
            return new AutofacServiceProvider(applicationContainer);
        }
    }
}
