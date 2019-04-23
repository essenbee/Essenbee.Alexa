using System;
using Essenbee.Alexa.Clients;
using Essenbee.Alexa.Interfaces;
using Essenbee.Alexa.Lib.HttpClients;
using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Essenbee.Alexa
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
            services.AddHttpClient<IAlexaClient, AlexaClient>();
            services.AddScoped<IChannelClient, ChannelGraphClient>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Essenbee Alexa",
                    Version = "v1"
                });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Configuration["SkillId"] = GetSkillAppId();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Essenbee Alexa v1");
            });

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/alexa"), (appBuilder) =>
            {
                appBuilder.UseAlexaRequestValidation();
            });

            app.UseStaticFiles();

            app.UseMvc();
        }

        private string GetSkillAppId()
        {
            var retries = 0;
            var retry = false;

            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            do
            {
                var waitTime = Math.Min(GetWaitTime(retries), 2000000);
                System.Threading.Thread.Sleep(waitTime);

                try
                {
                    var secret = keyVaultClient
                        .GetSecretAsync("https://codebasealphakeys.vault.azure.net/secrets/DevStreamsAppId/de4526409e184b439ab110198a4021d4")
                        .Result;
                    return secret.Value;
                }
                catch (KeyVaultErrorException keyVaultException)
                {
                    if ((int)keyVaultException.Response.StatusCode == 429)
                    {
                        retry = true;
                        retries++;
                    }
                }
            }
            while (retry && (retries++ < 10));

            return string.Empty;
        }

        // This method implements exponential backoff if there are 429 errors from Azure Key Vault
        private static int GetWaitTime(int retryCount) => retryCount > 0 ? ((int)Math.Pow(2, retryCount) * 100) : 0;
    }
}
