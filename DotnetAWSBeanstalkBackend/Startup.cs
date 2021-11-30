using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AspNet.Security.OAuth.KakaoTalk;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAWSBeanstalkBackend
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
            // services.AddCors(opt =>
            // {
            //     opt.AddDefaultPolicy(builder =>
            //     {
            //         builder.AllowAnyOrigin()
            //             .AllowAnyHeader()
            //             .AllowAnyMethod();
            //     });
            // });

            // services.AddHttpsRedirection(opt => opt.HttpsPort = 443);
            //
            // services.Configure<ForwardedHeadersOptions>(options =>
            // {
            //     options.ForwardedHeaders =
            //         ForwardedHeaders.XForwardedFor |
            //         ForwardedHeaders.XForwardedProto;
            //
            //     options.KnownNetworks.Clear();
            //     options.KnownProxies.Clear();
            // });

            services.AddControllers().AddNewtonsoftJson(
                x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotnetAWSBeanstalkBackend", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotnetAWSBeanstalkBackend v1"));
            }

            app.UseCors(x =>
                x.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());

            // app.UseHsts();
            // app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
