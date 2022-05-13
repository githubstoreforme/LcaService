using System.Text.Json;
using Certes;
using Certes.Acme;
using GraphQL;
using GraphQL.NewtonsoftJson;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using LcaService.GraphQL;
using LcaService.GraphQL.CSR;
using LcaService.LetsEncrypt;
using LcaService.OrderStorage;
using LcaService.Worker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LcaService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            services.AddTransient<ISchema, CASchema>();
            services.AddTransient<CAQueries>();
            services.AddTransient<CertificateAuthorityMutation>();
            services.AddTransient<OrderType>();
            services.AddTransient<CSRInputType>();


            services.AddHostedService<IncomingOrderWorkflowManager>();
            services.AddHostedService<PlaceOrderWorkflowManager>();
            services.AddHostedService<FinalizeOrderWorkflowManager>();
            services.AddSingleton<IOrderStorageService, OrderStorageService>();
            services.AddSingleton<IAcmeProvider, AcmeProvider>();
            services.AddSingleton<IAcmeContext>(provider => new AcmeContext(WellKnownServers.LetsEncryptStagingV2));


            services.AddGraphQL(options =>
            {
                options.EndPoint = "/graphql";
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();             
            }            

            app.UseRouting();

            app.UseGraphQL();

            app.UseGraphQLPlayground();
        }
    }
}
