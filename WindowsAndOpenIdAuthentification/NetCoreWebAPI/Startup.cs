using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NetCoreWebAPI.Configuration;

namespace NetCoreWebAPI
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
            var apiConfiguration = Configuration.GetSection("ApiConfiguration")
                                             .Get<ApiConfiguration>();
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        var openIdConnectMetadataAddress = $"{apiConfiguration.Authority}/.well-known/openid-configuration";

                        var configurationManager =
                            new ConfigurationManager<OpenIdConnectConfiguration>(
                                openIdConnectMetadataAddress,
                                new OpenIdConnectConfigurationRetriever());

                        var openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None)
                                                               .GetAwaiter()
                                                               .GetResult();

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = openIdConfig.Issuer,
                            IssuerSigningKeys = openIdConfig.SigningKeys,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,

                            ValidateAudience = true,

                            // We are considering cloud to on-premises connect scenarios. Due to this fact, ValidAudience does not make sense.
                            // The ValidAudience and ClientId in the token will be the same and equal to ClientId.
                            ValidAudience = apiConfiguration.ClientId,
                        };
                    })
                    .AddNegotiate();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("apim", policy => policy.RequireClaim("apiid", apiConfiguration.ClientId));
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                         .RequireAuthorization();
            });
        }
    }
}
