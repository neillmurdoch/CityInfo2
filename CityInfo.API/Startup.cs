using System.Security.Cryptography.X509Certificates;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CityInfo.API
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)     //  IHostingEnvironment env)
        {
            // This is all done automatically in ASP.NET Core 2 as part of CreateDefaultBuilder()
            //var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            //    .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);        // For the appSettings.Production.json file
            //    .AddEnvironmentVariables();       // This is already there in ASP.NET Core 2. Gives access to environment variables.
            // If both json files are specified, the LAST one specified wins.

            //Configuration = builder.Build();

            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                // Add the XML output formatter that can be specified on the Accept header (application/xml)
                // Can also .Clear to remove all or Remove to remove a specific one
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()));
            // By default, json properties get their names set to camel casing (pointsOfInterest) in the json result. We can override this if necessary. Maybe
            // updating an old api where we don't want to change the casing.
            //.AddJsonOptions(o =>
            //{
            //    if (o.SerializerSettings.ContractResolver is DefaultContractResolver castedResolver)
            //    {
            //        castedResolver.NamingStrategy = null;
            //    }
            //});

            //services.AddTransient<>()           // Created each time they are requested - best for lightweight, stateless services
            //services.AddScoped<>()              // Created once per request
            //services.AddSingleton<>()           // Created the first time they are requested. All subsequent requests use same instance

            //services.AddTransient<LocalMailService>();      // This is wrong as we've registered a concrete type

            // However, we are still telling it to use the concrete LocalMailService
            // Could use compiler directives
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>(); // Told the container that whenever with inject IMailService, provide an instance of LocalMailService. 
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif

            // If the requested configuration setting is found in the environment variables, it will override the settings found elsewhere, like the appSettings.json file.
            // However, adding this setting to the project properties puts the connection string in
            // the Properties\launchSettings.json file which isn't secure. Make sure this isn't submitted to source control
            // Instead, add to system environment variables.
            // It is recommended to use appSettings while developing and only add the environment variable on the production server
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));       // Defaulted to Scope lifetime.

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CityInfoContext cityInfoContext)
        {
            // Asp.Net Core 2 does this automatically in CreateDefaultBuilder()
            //loggerFactory.AddConsole();
            //loggerFactory.AddDebug();           // Default is information level
            
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());        // This is the long way of doing it. Most logging providers give a shortcut extension method
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            // Call the extension method to add sample data to the database.
            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();       // Show a text formatted status code page

            AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
                    cfg.CreateMap<Entities.City, Models.CityDto>();
                    cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
                    // Mapping in the other direction for creation, update
                    cfg.CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();
                    cfg.CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
                    cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
                });

            app.UseMvc();

            //app.Run(context =>
            //{
            //    throw new Exception("Example exception");
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
