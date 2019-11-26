using Autofac;
using ModularInjection.Attributes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModularInjection
{
    public abstract class DiModule
    {
        protected internal ContainerBuilder ContainerBuilder { get; internal set; }

        protected internal IConfiguration Configuration { get; internal set; }

        public virtual void PreInitialize()
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void PostInitialize()
        {
        }
        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesRecursively(list, moduleType);
            return list;
        }

        private static void AddModuleAndDependenciesRecursively(List<Type> modules, Type module)
        {
            if (!IsDiModule(module))
            {
                throw new Exception("This type is not an Di module: " + module.AssemblyQualifiedName);
            }

            if (modules.Contains(module))
            {
                return;
            }

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(modules, dependedModule);
            }
        }

        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsDiModule(moduleType))
            {
                throw new Exception("This type is not an Di module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }
            return list;
        }

        public static bool IsDiModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(DiModule).IsAssignableFrom(type);
        }
    }
}
