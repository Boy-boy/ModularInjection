# ModularInjection
使用autofac实现模块化注入

## 使用方式
1.创建.net core2.2或 3.0项目，  
   
2.添加初始化模块类，此类需继承DiModule，如``` public class StartupDiModule: ModularInjection.DiModule```，若想实现跨类库注入,
请在初始化模块类添加此特性 ``` [DependsOn(typeof(模块化类))]```
## core2.2项目代码演示
1.在Startup类中添加如下代码片段
  ```
    public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            return  services.AddModule<StartupDiModule>();
        }
  ```
  
## core3.0项目代码演示
1.  在Program类中添加``` .UseServiceProviderFactory(new AutofacServiceProviderFactory<初始化模块的类>())```
```
   public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory<StartupDiModule>())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
   
   ```
  2.在Startup类中添加如下代码片段
  ```
    public void ConfigureContainer(ContainerBuilder builder)
        {
           //自定义注入
        }
  ```
  ## core2.2或3.0项目添加初始化模块类
   ###添加初始化模块类
 
    [DependsOn(typeof(模块化类))]
    public class StartupDiModule: ModularInjection.DiModule
    {
        public override void PreInitialize()
        {
            //预初始化
        }

        public override void Initialize()
        {
            //初始化
        }

        public override void PostInitialize()
        {
            //后初始化
        }
    }
    
