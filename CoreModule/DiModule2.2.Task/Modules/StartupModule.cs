using Autofac;
using ModularInjection;

namespace DiModule2._2.Task.Modules
{
    public class StartupModule:DiModule
    {
        public override void PreInitialize()
        {
            var s = Configuration["AllowedHosts"];
            ContainerBuilder.RegisterType<StartupModule>();

        }

        public override void Initialize()
        {

        }

        public override void PostInitialize()
        {

        }
    }
}
