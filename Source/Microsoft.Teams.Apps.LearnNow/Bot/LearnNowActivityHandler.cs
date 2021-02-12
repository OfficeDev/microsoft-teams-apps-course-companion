// <copyright file="LearnNowActivityHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Common.Interfaces;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The LearnNowActivityHandler is responsible for reacting to incoming events from Teams sent from BotFramework.
    /// </summary>
    public sealed class LearnNowActivityHandler : TeamsActivityHandler
    {
        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotSettings> botOptions;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<LearnNowActivityHandler> logger;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// A set of key/value application configuration properties for telemetry settings.
        /// </summary>
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Messaging Extension search helper.
        /// </summary>
        private readonly IMessagingExtensionHelper messagingExtensionHelper;

        /// <summary>
        /// Task module height.
        /// </summary>
        private readonly int taskModuleHeight = 500;

        /// <summary>
        /// Task module width.
        /// </summary>
        private readonly int taskModuleWidth = 800;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnNowActivityHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client. </param>
        /// <param name="messagingExtensionHelper">Messaging Extension helper dependency injection.</param>
        /// <param name="botOptions">A set of key/value application configuration properties for activity handler.</param>
        public LearnNowActivityHandler(
            ILogger<LearnNowActivityHandler> logger,
            IStringLocalizer<Strings> localizer,
            TelemetryClient telemetryClient,
            IMessagingExtensionHelper messagingExtensionHelper,
            IOptions<BotSettings> botOptions)
        {
            this.logger = logger;
            this.localizer = localizer;
            this.telemetryClient = telemetryClient;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.messagingExtensionHelper = messagingExtensionHelper;
        }

        /// <summary>
        /// Invoked when the user opens the Messaging Extension or searching any content in it.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="query">Contains Messaging Extension query keywords.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Messaging extension response object to fill compose extension section.</returns>
        /// <remarks>
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.builder.teams.teamsactivityhandler.onteamsmessagingextensionqueryasync?view=botbuilder-dotnet-stable.
        /// </remarks>
        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(
            ITurnContext<IInvokeActivity> turnContext,
            MessagingExtensionQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                this.RecordEvent(nameof(this.OnTeamsMessagingExtensionQueryAsync), turnContext);

                var activity = turnContext.Activity;

                var messagingExtensionQuery = JsonConvert.DeserializeObject<MessagingExtensionQuery>(activity.Value.ToString());
                var searchQuery = this.messagingExtensionHelper.GetSearchResult(messagingExtensionQuery);

                return new MessagingExtensionResponse
                {
                    ComposeExtension = await this.messagingExtensionHelper.GetTeamPostSearchResultAsync(searchQuery, messagingExtensionQuery.CommandId, activity.From.AadObjectId, messagingExtensionQuery.QueryOptions.Count, messagingExtensionQuery.QueryOptions.Skip),
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to handle the Messaging Extension command {turnContext.Activity.Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// When OnTurn method receives a fetch invoke activity on bot turn, it calls this method.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="taskModuleRequest">Task module invoke request value payload.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents a task module response.</returns>
        /// <remarks>
        /// Reference link: https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.builder.teams.teamsactivityhandler.onteamstaskmodulefetchasync?view=botbuilder-dotnet-stable.
        /// </remarks>
        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(
            ITurnContext<IInvokeActivity> turnContext,
            TaskModuleRequest taskModuleRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                this.RecordEvent(nameof(this.OnTeamsTaskModuleFetchAsync), turnContext);

                var activity = turnContext.Activity;
                var activityValue = JObject.Parse(activity.Value?.ToString())["data"].ToString();

                AdaptiveSubmitActionData adaptiveSubmitActionData = JsonConvert.DeserializeObject<AdaptiveSubmitActionData>(JObject.Parse(taskModuleRequest?.Data.ToString()).SelectToken("data").ToString());
                if (adaptiveSubmitActionData == null)
                {
                    this.logger.LogInformation("Value obtained from task module fetch action is null");
                }

                var adaptiveActionType = adaptiveSubmitActionData.AdaptiveActionType;
                var postId = adaptiveSubmitActionData.Id;
                switch (adaptiveActionType.ToUpperInvariant())
                {
                    case BotCommandConstants.ViewLearningModule:
                        this.logger.LogInformation("Fetch task module to show learning module detail.");
                        return this.GetTaskModuleResponse(string.Format(CultureInfo.InvariantCulture, $"{this.botOptions.Value.AppBaseUri}/learningmodulepreview?viewMode=1&learningModuleId={postId}"), this.localizer.GetString("learningModuleDetails"));

                    case BotCommandConstants.ViewResource:
                        this.logger.LogInformation("Fetch task module to show resource detail.");
                        return this.GetTaskModuleResponse(string.Format(CultureInfo.InvariantCulture, $"{this.botOptions.Value.AppBaseUri}/previewcontent?viewMode=1&resourceId={postId}"), this.localizer.GetString("resourceDetails"));

                    default:
                        this.logger.LogInformation($"Invalid command for task module fetch activity. Command is: {adaptiveActionType}");
                        var reply = this.localizer.GetString("invalidCommand");
                        await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
                        return null;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching task module");
                throw;
            }
        }

        /// <summary>
        /// Method overridden to show task module response.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="action">Messaging extension action commands.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
#pragma warning disable CS1998 // Method is overridden from Microsoft Bot Framework.
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(
            ITurnContext<IInvokeActivity> turnContext,
            MessagingExtensionAction action,
            CancellationToken cancellationToken)
#pragma warning restore CS1998 // Method is overridden from Microsoft Bot Framework.
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            this.RecordEvent(nameof(this.OnTeamsMessagingExtensionFetchTaskAsync), turnContext);

            return new MessagingExtensionActionResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Type = "continue",
                    Value = new TaskModuleTaskInfo()
                    {
                        Url = $"{this.botOptions.Value.AppBaseUri}/configure",
                        Height = this.taskModuleHeight,
                        Width = this.taskModuleWidth,
                        Title = this.localizer.GetString("configureTaskModuleTitle"),
                    },
                },
            };
        }

        /// <summary>
        /// Records event data to Application Insights telemetry client
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="turnContext">Provides context for a turn in a bot.</param>
        private void RecordEvent(string eventName, ITurnContext turnContext)
        {
            var teamsChannelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            this.telemetryClient.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "userId", turnContext.Activity.From.AadObjectId },
                { "tenantId", turnContext.Activity.Conversation.TenantId },
                { "teamId", teamsChannelData?.Team?.Id },
                { "channelId", teamsChannelData?.Channel?.Id },
            });
        }

        /// <summary>
        /// Get task module response object.
        /// </summary>
        /// <param name="url">Task module URL.</param>
        /// <param name="title">Title for task module.</param>
        /// <returns>TaskModuleResponse object.</returns>
        private TaskModuleResponse GetTaskModuleResponse(string url, string title)
        {
            return new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Type = "continue",
                    Value = new TaskModuleTaskInfo()
                    {
                        Url = url,
                        Height = Constants.TaskModuleHeight,
                        Width = Constants.TaskModuleWidth,
                        Title = title,
                    },
                },
            };
        }
    }
}