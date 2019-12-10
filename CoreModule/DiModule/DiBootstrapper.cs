using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ModularInjection
{
    public class DiBootstrapper
    {
        public Type StartupModule { get; }

        public ContainerBuilder ContainerBuilder { get; }

        private IServiceCollection ServiceDescriptors { get; }

        public List<DiModule> Instances { get; }

        public DiBootstrapper(Type startupModule, IServiceCollection services)
        {
            StartupModule = startupModule;
            ContainerBuilder = new ContainerBuilder();
            ServiceDescriptors = services;
            Instances = new List<DiModule>();
        }

        public static DiBootstrapper Create<TStartupModule>(IServiceCollection services)
            where TStartupModule : DiModule
        {
            return new DiBootstrapper(typeof(TStartupModule), services);
        }

        public virtual void Initialize()
        {
            var moduleTypes = FindAllModuleTypes();
            RegisterModules(moduleTypes);
            CreateModules(moduleTypes);
            StartModules();
        }


        private List<Type> FindAllModuleTypes()
        {
            var modules = DiModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(StartupModule);
            return modules;
        }
        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                ServiceDescriptors.AddTransient(moduleType);
            }
        }

        private void CreateModules(ICollection<Type> moduleTypes)
        {
            var serviceProvider = ServiceDescriptors.BuildServiceProvider();
            foreach (var moduleType in moduleTypes)
            {
                if (!(serviceProvider.GetService(moduleType) is DiModule instance)) continue;
                instance.ContainerBuilder = ContainerBuilder;
                instance.Configuration = serviceProvider.GetService<IConfiguration>();
                Instances.Add(instance);
            }
        }

        private void StartModules()
        {
            Instances.ForEach(module => module.PreInitialize());
            Instances.ForEach(module => module.Initialize());
            Instances.ForEach(module => module.PostInitialize());
        }
    }
}
