// <copyright file="Startup.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow
{
    using System;
    using global::Azure.Identity;
    using global::Azure.Security.KeyVault.Secrets;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Teams.Apps.LearnNow.Authentication;

    /// <summary>
    /// The Startup class is responsible for configuring the DI container and acts as the composition root.
    /// </summary>
    public sealed class Startup
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The environment provided configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var useKeyVault = this.configuration.GetValue<bool>("UseKeyVault");

            if (useKeyVault)
            {
                this.GetKeyVaultByManagedServiceIdentity();
            }
        }

        /// <summary>
        /// Configure the composition root for the application.
        /// </summary>
        /// <param name="services">The stub composition root.</param>
        /// <remarks>
        /// For more information see: https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
#pragma warning disable CA1506 // Composition root expected to have coupling with many components.
        public void ConfigureServices(IServiceCollection services)
        {
            string appId = this.configuration.GetValue<string>("App:MicrosoftAppId");
            string appPassword = this.configuration.GetValue<string>("App:MicrosoftAppPassword");

            services.Configure<MvcOptions>(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddHttpClient();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddConfidentialCredentialProvider(this.configuration);
            services.AddConfigurationSettings(this.configuration);
            services.AddCredentialProviders(this.configuration);
            services.AddLearnNowAuthentication(this.configuration);
            services.AddProviders();
            services.AddServices();
            services.AddHelpers(this.configuration);
            services.AddRepositories(this.configuration);
            services.AddSingleton<IChannelProvider, SimpleChannelProvider>();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
           .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddMemoryCache();

            // Add i18n.
            services.AddLocalizationSettings(this.configuration);
        }
#pragma warning restore CA1506

        /// <summary>
        /// Configure the application request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">Hosting Environment.</param>
#pragma warning disable CA1822 // This method is provided by the framework
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#pragma warning restore CA1822
        {
            app.UseAuthentication();
            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        /// <summary>
        /// Get KeyVault secrets and set application settings values.
        /// </summary>
        private void GetKeyVaultByManagedServiceIdentity()
        {
            // Create a new secret client using the default credential from Azure.Identity using environment variables.
            var client = new SecretClient(
                vaultUri: new Uri($"{this.configuration["KeyVaultUrl:BaseURL"]}/"),
                credential: new DefaultAzureCredential());

            this.configuration["MicrosoftAppId"] = client.GetSecret("MicrosoftAppId").Value.Value;
            this.configuration["MicrosoftAppPassword"] = client.GetSecret("MicrosoftAppPassword").Value.Value;
            this.configuration["SQLStorage:ConnectionString"] = client.GetSecret("SQLStorageConnectionString").Value.Value;
            this.configuration["Storage:BlobConnectionString"] = client.GetSecret("StorageBlobConnectionString").Value.Value;
            this.configuration["SecurityGroup:TeacherSecurityGroupId"] = client.GetSecret("TeacherSecurityGroupId").Value.Value;
            this.configuration["SecurityGroup:AdminSecurityGroupId"] = client.GetSecret("AdminSecurityGroupId").Value.Value;
            this.configuration["SecurityGroup:ModeratorsGroupId"] = client.GetSecret("ModeratorsGroupId").Value.Value;
            this.configuration["BingSearch:Key"] = client.GetSecret("BingSearchKey").Value.Value;
        }
    }
}