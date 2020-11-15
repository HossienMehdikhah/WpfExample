using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfExample.DataLayer;
using WpfExample.WpfClient.DialogForm;
using AutoMapper;
using CommonLibrary.BuilderExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WpfExample.Model;
using WpfExample.Model.Entity;


namespace WpfExample.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             ;

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            
        }

        

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<WindowsNavigatore>();
            services.AddTransient<MainWindow>();
            services.AddTransient<OrderCreate_DialogForm>();
            services.AddTransient<OrderDetailCreate_DialogForm>();
            services.AddAutoMapper(typeof(ConverterConf));
            services.AddLogging();
            services.AddDbContext<Context>(x => x.UseSqlServer("Data Source=(Local);Initial Catalog=RangiExample;Integrated Security=True"),ServiceLifetime.Transient);


            services.AddCommonEntity<Personal>()
                .StoreBuilder
                .AddEntityFrameworkStores<Context>()
                ;

            services.AddCommonEntity<Produce>()
               .StoreBuilder
               .AddEntityFrameworkStores<Context>()
               ;

            services.AddCommonEntity<Order>()
              .StoreBuilder
              .AddEntityFrameworkStores<Context>()
              ;

            services.AddCommonEntity<OrderDetail>()
             .StoreBuilder
             .AddEntityFrameworkStores<Context>()
             ;
        }
    }
}
