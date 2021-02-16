// <copyright file="ServicesExtension.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Identity.Client;
    using Microsoft.Teams.Apps.LearnNow.Bot;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Common.Interfaces;
    using Microsoft.Teams.Apps.LearnNow.Helpers;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;

    /// <summary>
    /// Class which helps to extend ServiceCollection.
    /// </summary>
    public static class ServicesExtension
    {
        /// <summary>
        /// Adds application configuration settings to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BotSettings>(options =>
            {
                options.AppBaseUri = configuration.GetValue<string>("Bot:AppBaseUri");
                options.MicrosoftAppId = configuration.GetValue<string>("MicrosoftAppId");
                options.MicrosoftAppPassword = configuration.GetValue<string>("MicrosoftAppPassword");
                options.CacheDurationInMinutes = configuration.GetValue<double>("Bot:CacheDurationInMinutes");
                options.CardCacheDurationInHour = configuration.GetValue<double>("Bot:CardCacheDurationInHour");
                options.TenantId = configuration.GetValue<string>("Bot:TenantId");
            });

            services.Configure<AzureActiveDirectorySettings>(options =>
            {
                options.ApplicationIdURI = configuration.GetValue<string>("AzureAd:ApplicationIdURI");
                options.ValidIssuers = configuration.GetValue<string>("AzureAd:ValidIssuers");
                options.Instance = configuration.GetValue<string>("AzureAd:Instance");
                options.GraphScope = configuration.GetValue<string>("AzureAd:GraphScope");
            });

            services.Configure<TelemetrySettings>(options =>
            {
                options.InstrumentationKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
            });

            services.Configure<BingSearchServiceSettings>(bingCognitiveServiceSetting =>
            {
                bingCognitiveServiceSetting.Key = configuration.GetValue<string>("BingSearch:Key");
                bingCognitiveServiceSetting.Endpoint = configuration.GetValue<string>("BingSearch:Endpoint");
                bingCognitiveServiceSetting.SafeSearch = configuration.GetValue<string>("BingSearch:SafeSearch");
            });

            services.Configure<StorageSettings>(options =>
            {
                options.BlobConnectionString = configuration.GetValue<string>("Storage:BlobConnectionString");
            });

            services.Configure<SecurityGroupSettings>(options =>
            {
                options.TeacherSecurityGroupId = configuration.GetValue<string>("SecurityGroup:TeacherSecurityGroupId");
                options.AdminGroupId = configuration.GetValue<string>("SecurityGroup:AdminSecurityGroupId");
                options.ModeratorGroupId = configuration.GetValue<string>("SecurityGroup:ModeratorsGroupId");
            });
        }

        /// <summary>
        /// Adds credential providers for authentication.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddCredentialProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            services
                .AddSingleton(new MicrosoftAppCredentials(configuration.GetValue<string>("MicrosoftAppId"), configuration.GetValue<string>("MicrosoftAppPassword")));

#pragma warning disable CA2000 // This is singleton which has lifetime same as the app
            services.AddSingleton(new OAuthClient(new MicrosoftAppCredentials(configuration.GetValue<string>("MicrosoftAppId"), configuration.GetValue<string>("MicrosoftAppPassword"))));
#pragma warning restore CA2000 // This is singleton which has lifetime same as the app
        }

        /// <summary>
        /// Add confidential credential provider to access api.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddConfidentialCredentialProvider(this IServiceCollection services, IConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            IConfidentialClientApplication confidentialClientApp = ConfidentialClientApplicationBuilder.Create(configuration["MicrosoftAppId"])
                .WithClientSecret(configuration["MicrosoftAppPassword"])
                .Build();

            services.AddSingleton<IConfidentialClientApplication>(confidentialClientApp);
        }

        /// <summary>
        /// Adds providers to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public static void AddProviders(this IServiceCollection services)
        {
            services
                .AddTransient<IFileDownloadProvider, FileDownloadProvider>();
            services
                .AddTransient<IFileUploadProvider, FileUploadProvider>();
        }

        /// <summary>
        /// Adds helpers to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddApplicationInsightsTelemetry(configuration.GetValue<string>("ApplicationInsights:InstrumentationKey"));
            services.
                AddHttpClient<IImageProviderService, BingImageService>();
            services
               .AddSingleton<TokenAcquisitionHelper>();
            services
                .AddTransient<IMessagingExtensionHelper, MessagingExtensionHelper>();
            services
                .AddScoped<IResourceMapper, ResourceMapper>();
            services
                .AddScoped<ISubjectMapper, SubjectMapper>();
            services
                .AddScoped<ITagMapper, TagMapper>();
            services
                .AddScoped<IGradeMapper, GradeMapper>();
            services
                .AddScoped<ILearningModuleMapper, LearningModuleMapper>();
            services
                .AddScoped<IResourceModuleMapper, ResourceModuleMapper>();
            services
                .AddScoped<IUserSettingMapper, UserSettingMapper>();
        }

        /// <summary>
        /// Adds services to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public static void AddServices(this IServiceCollection services)
        {
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, LearnNowAdapterWithErrorHandler>();

            services.AddTransient<IBot, LearnNowActivityHandler>();
            services.AddTransient(serviceProvider => (BotFrameworkAdapter)serviceProvider.GetRequiredService<IBotFrameworkHttpAdapter>());
            services.AddSingleton<ITokenHelper, AccessTokenHelper>();
            services.AddSingleton<MemberValidationService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IGroupMembersService, GroupMembersService>();
            services.AddTransient<IMemberValidationService, MemberValidationService>();
        }

        /// <summary>
        /// Adds localization settings to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddLocalizationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var defaultCulture = CultureInfo.GetCultureInfo(configuration.GetValue<string>("i18n:DefaultCulture"));
                var supportedCultures = configuration.GetValue<string>("i18n:SupportedCultures").Split(',')
                    .Select(culture => CultureInfo.GetCultureInfo(culture))
                    .ToList();

                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new LearnNowLocalizationCultureProvider(),
                };
            });
        }

        /// <summary>
        /// Adds storage repositories to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LearnNowContext>(options =>
            options.UseSqlServer(
                configuration.GetValue<string>("SQLStorage:ConnectionString")));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}