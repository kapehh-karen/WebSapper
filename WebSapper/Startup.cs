using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using WebSapper.Repository;
using WebSapper.Service;
using WebSapper.Service.GameStates;

namespace WebSapper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            RegisterNHibernate(services);
            RegisterCommon(services);
        }

        private void RegisterNHibernate(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var cfg = new Configuration();

                cfg.DataBaseIntegration(db =>
                {
                    db.ConnectionString = @"Server=.\LOCALSQLEXPRESS;Database=SapperDB;User Id=sa;Password=123;";
                    db.Dialect<MsSql2012Dialect>();
                });

                var mapper = new ModelMapper();
                mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
                var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
                cfg.AddMapping(mapping);

                return cfg;
            });

            services.AddSingleton(provider =>
            {
                var cfg = provider.GetService<Configuration>();
                return cfg.BuildSessionFactory();
            });

            services.AddScoped(provider =>
            {
                var factory = provider.GetService<ISessionFactory>();
                return factory.OpenSession();
            });
        }

        private void RegisterCommon(IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IGameState, RunningState>();
            services.AddScoped<IGameState, WonState>();
            services.AddScoped<IGameState, LoseState>();
            services.AddScoped<IGameService, GameService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseFileServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}