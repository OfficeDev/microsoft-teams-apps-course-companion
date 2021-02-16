// <copyright file="MessagingExtensionHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using AdaptiveCards;
    using AdaptiveCards.Templating;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Common.Interfaces;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;

    /// <summary>
    /// A class that handles the search activities for Messaging Extension.
    /// </summary>
    public class MessagingExtensionHelper : IMessagingExtensionHelper
    {
        /// <summary>
        /// Search text parameter name in the manifest file.
        /// </summary>
        private const string SearchTextParameterName = "searchQuery";

        /// <summary>
        /// The current cultures' string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Information about the web hosting environment an application is running in.
        /// </summary>
        private readonly IWebHostEnvironment env;

        /// <summary>
        /// Instance for handling commom operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// A set of key/value application configuration properties for Activity settings.
        /// </summary>
        private readonly IOptions<BotSettings> botOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingExtensionHelper"/> class.
        /// </summary>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="unitOfWork">The repository for working with entity collection.</param>
        /// <param name="env">Information about the web hosting environment an application is running in.</param>
        /// <param name="memoryCache">MemoryCache instance for caching authorization result.</param>
        /// <param name="botOptions">A set of key/value application configuration properties for activity handler.</param>
        public MessagingExtensionHelper(
            IStringLocalizer<Strings> localizer,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IMemoryCache memoryCache,
            IOptions<BotSettings> botOptions)
        {
            this.env = env;
            this.localizer = localizer;
            this.unitOfWork = unitOfWork;
            this.memoryCache = memoryCache;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
        }

        /// <summary>
        /// Get the results from Azure Sql server and populate the result (card + preview).
        /// </summary>
        /// <param name="query">Query which the user had typed in Messaging Extension search field.</param>
        /// <param name="commandId">Command id to determine which tab in Messaging Extension has been invoked.</param>
        /// <param name="userObjectId">Azure Active Directory id of the user.</param>
        /// <param name="count">Number of search results to return.</param>
        /// <param name="skip">Number of search results to skip.</param>
        /// <returns><see cref="Task"/>Returns Messaging Extension result object, which will be used for providing the card.</returns>
        public async Task<MessagingExtensionResult> GetTeamPostSearchResultAsync(
            string query,
            string commandId,
            string userObjectId,
            int? count,
            int? skip)
        {
            int skipPost = (int)skip;
            int countPost = (int)count;
            MessagingExtensionResult composeExtensionResult = new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = AttachmentLayoutTypes.List,
                Attachments = new List<MessagingExtensionAttachment>(),
            };
            switch (commandId)
            {
                case BotCommandConstants.ResourceCommandId:

#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                    var resourceList = query == null ? await this.unitOfWork.ResourceRepository.GetResourcesAsync(skipPost, countPost) : await this.unitOfWork.ResourceRepository.FindAsync(resource => resource.Title.Contains(query));
#pragma warning restore CA1307 // Specify StringComparison
                    composeExtensionResult = this.GetResourceResult(resourceList);
                    break;
                case BotCommandConstants.LearningModuleCommandId:
#pragma warning disable CA1307 //  Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                    var moduleList = query == null ? await this.unitOfWork.LearningModuleRepository.GetLearningModulesAsync(skipPost, countPost) : await this.unitOfWork.LearningModuleRepository.FindAsync(resource => resource.Title.Contains(query));
#pragma warning restore CA1307 // Specify StringComparison
                    composeExtensionResult = await this.GetLearningModuleResultAsync(moduleList);
                    break;
            }

            return composeExtensionResult;
        }

        /// <summary>
        /// Get the value of the searchText parameter in the Messaging Extension query.
        /// </summary>
        /// <param name="query">Contains Messaging Extension query keywords.</param>
        /// <returns>A value of the searchText parameter.</returns>
        public string GetSearchResult(MessagingExtensionQuery query)
        {
            return query?.Parameters.FirstOrDefault(parameter => parameter.Name.Equals(SearchTextParameterName, StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        }

        /// <summary>
        /// Get resource list result for Messaging Extension.
        /// </summary>
        /// <param name="resourceList">List of resource search result.</param>
        /// <returns><see cref="Task"/>Returns Messaging Extension result object, which will be used for providing the card.</returns>
        private MessagingExtensionResult GetResourceResult(IEnumerable<Resource> resourceList)
        {
            MessagingExtensionResult composeExtensionResult = new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = AttachmentLayoutTypes.List,
                Attachments = new List<MessagingExtensionAttachment>(),
            };

            if (resourceList == null)
            {
                return composeExtensionResult;
            }

            foreach (var resource in resourceList)
            {
                var learningModuleDetails = new LearningModuleCardViewModel
                {
                    ImageUrl = resource.ImageUrl,
                    Title = resource.Title,
                    Description = resource.Description.Trim().Length > Constants.ResourceCardDescriptionLength ? $"{resource.Description.Trim().Substring(0, Constants.ResourceCardDescriptionLength)}..." : resource.Description.Trim(),
                    GradeLabel = this.localizer.GetString("grade"),
                    GradeName = resource.Grade.GradeName,
                    SubjectLabel = this.localizer.GetString("subject"),
                    SubjectName = resource.Subject.SubjectName,
                    TagLabel = this.localizer.GetString("tags"),
                    Tags = this.FormateTagString(resource.ResourceTag),
                    IsTagVisible = resource.ResourceTag.Any(),
                    Id = resource.Id,
                    ViewDetailLabel = this.localizer.GetString("viewDetail"),
                    TaskModuleData = new AdaptiveSubmitActionData { AdaptiveActionType = BotCommandConstants.ViewResource, Id = resource.Id.ToString() },
                };

                var cardPayload = this.GetCardPayload(CacheKeysConstants.ResourceCardJSONTemplate, "ResourceCard.json");
                var template = new AdaptiveCardTemplate(cardPayload);
                var card = template.Expand(learningModuleDetails);
                AdaptiveCard adaptiveCard = AdaptiveCard.FromJson(card).Card;

                ThumbnailCard previewCard = new ThumbnailCard
                {
                    Images = new List<CardImage>
                    {
                        new CardImage(resource.ImageUrl),
                    },
                    Title = $"<p style='font-weight: 600;font-size: 14px;'>{HttpUtility.HtmlEncode(resource.Title)}</p>",
                    Text = $"<p style='font-size: 14px;'>{HttpUtility.HtmlEncode(resource.Subject.SubjectName)} &nbsp;|&nbsp; {HttpUtility.HtmlEncode(resource.Grade.GradeName)}</p>",
                };

                composeExtensionResult.Attachments.Add(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = adaptiveCard,
                }.ToMessagingExtensionAttachment(previewCard.ToAttachment()));
            }

            return composeExtensionResult;
        }

        /// <summary>
        /// Get team posts result for Messaging Extension.
        /// </summary>
        /// <param name="resourceList">List of user search result.</param>
        /// <returns><see cref="Task"/>Returns Messaging Extension result object, which will be used for providing the card.</returns>
        private async Task<MessagingExtensionResult> GetLearningModuleResultAsync(IEnumerable<LearningModule> resourceList)
        {
            MessagingExtensionResult composeExtensionResult = new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = AttachmentLayoutTypes.List,
                Attachments = new List<MessagingExtensionAttachment>(),
            };

            if (resourceList == null)
            {
                return composeExtensionResult;
            }

            foreach (var resource in resourceList)
            {
                var resourceModuleMapping = await this.unitOfWork.ResourceModuleRepository.FindAsync(resourceModule => resourceModule.LearningModuleId == resource.Id);

                var learningModuleDetails = new LearningModuleCardViewModel
                {
                    ImageUrl = resource.ImageUrl,
                    Title = resource.Title,
                    Description = resource.Description.Trim().Length > Constants.LearningModuleCardDescriptionLength ? $"{resource.Description.Trim().Substring(0, Constants.ResourceCardDescriptionLength)}..." : resource.Description.Trim(),
                    GradeLabel = this.localizer.GetString("grade"),
                    GradeName = resource.Grade.GradeName,
                    SubjectLabel = this.localizer.GetString("subject"),
                    SubjectName = resource.Subject.SubjectName,
                    Id = resource.Id,
                    ViewDetailLabel = this.localizer.GetString("viewDetail"),
                    TaskModuleData = new AdaptiveSubmitActionData { AdaptiveActionType = BotCommandConstants.ViewLearningModule, Id = resource.Id.ToString() },
                    ResourceCount = resourceModuleMapping.Count(),
                };

                var cardPayload = this.GetCardPayload(CacheKeysConstants.LearningModuleCardJSONTemplate, "LearningModuleCard.json");
                var template = new AdaptiveCardTemplate(cardPayload);
                var card = template.Expand(learningModuleDetails);
                AdaptiveCard adaptiveCard = AdaptiveCard.FromJson(card).Card;

                ThumbnailCard previewCard = new ThumbnailCard
                {
                    Images = new List<CardImage>
                        {
                            new CardImage(resource.ImageUrl),
                        },
                    Title = $"<p style='font-weight: 600;font-size: 14px;'>{HttpUtility.HtmlEncode(resource.Title)}</p>",
                    Text = $"<p style='font-size: 14px;'>{HttpUtility.HtmlEncode(resource.Subject.SubjectName)} &nbsp;|&nbsp; {HttpUtility.HtmlEncode(resource.Grade.GradeName)} &nbsp;|&nbsp; {HttpUtility.HtmlEncode(resourceModuleMapping.Count())} items</p>",
                };

                composeExtensionResult.Attachments.Add(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = adaptiveCard,
                }.ToMessagingExtensionAttachment(previewCard.ToAttachment()));
            }

            return composeExtensionResult;
        }

        /// <summary>
        /// Get comma separated tag string.
        /// </summary>
        private string FormateTagString(IEnumerable<ResourceTag> resourceTaglist)
        {
            var tags = resourceTaglist.Select(resourceTag => resourceTag.Tag.TagName);

            return string.Join(", ", tags);
        }

        /// <summary>
        /// Get card payload from memory.
        /// </summary>
        /// <param name="cardCacheKey">Card cache key.</param>
        /// <param name="cardJSONTemplateFileName">File name of JSON adaptive card template with file extension as .json to be provided.</param>
        /// <returns>Returns json adaptive card payload string.</returns>
        private string GetCardPayload(string cardCacheKey, string cardJSONTemplateFileName)
        {
            bool isCacheEntryExists = this.memoryCache.TryGetValue(cardCacheKey, out string cardPayload);

            if (!isCacheEntryExists)
            {
                // If cache duration is not specified then by default cache for 12 hours.
                var cacheDurationInHour = TimeSpan.FromHours(this.botOptions.Value.CardCacheDurationInHour);
                cacheDurationInHour = cacheDurationInHour.Hours <= 0 ? TimeSpan.FromHours(12) : cacheDurationInHour;

                var cardJsonFilePath = Path.Combine(this.env.ContentRootPath, $".\\Cards\\{cardJSONTemplateFileName}");
                cardPayload = File.ReadAllText(cardJsonFilePath);
                this.memoryCache.Set(cardCacheKey, cardPayload, cacheDurationInHour);
            }

            return cardPayload;
        }
    }
}