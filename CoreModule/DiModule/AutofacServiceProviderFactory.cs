using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ModularInjection
{
    public class AutofacServiceProviderFactory<TStartupModule> : IServiceProviderFactory<ContainerBuilder>
        where TStartupModule : DiModule

    {
        private readonly Action<ContainerBuilder> _configurationAction;

        public AutofacServiceProviderFactory(Action<ContainerBuilder> configurationAction = null)
        {
            _configurationAction = configurationAction ?? (builder => { });
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var abpBootstrapper = DiBootstrapper.Create<TStartupModule>(services);
            abpBootstrapper.Initialize();
            abpBootstrapper.ContainerBuilder.Populate(services);
            _configurationAction(abpBootstrapper.ContainerBuilder);
            return abpBootstrapper.ContainerBuilder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}
