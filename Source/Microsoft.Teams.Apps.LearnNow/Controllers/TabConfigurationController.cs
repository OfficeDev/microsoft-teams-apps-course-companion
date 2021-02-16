// <copyright file="TabConfigurationController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Controller to handle tab configuration API operations.
    /// </summary>
    [Route("api/tab-configuration")]
    [ApiController]
    [Authorize]
    public class TabConfigurationController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<TabConfigurationController> logger;

        /// <summary>
        /// Instance for handling common operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabConfigurationController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">TabConfigurationRepository repository for working with tab configuration data.</param>
        public TabConfigurationController(
            ILogger<TabConfigurationController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Post call to store team preference details in storage.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <param name="tabConfigurationDetail">Holds tab configuration detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeTeamMemberUserPolicy)]
        public async Task<IActionResult> PostAsync([FromQuery] string groupId, [FromBody] TabConfigurationViewModel tabConfigurationDetail)
        {
            this.RecordEvent("TabConfiguration - HTTP Post call.", RequestType.Initiated);
            this.logger.LogInformation("Call to add tab configuration details.");

            if (tabConfigurationDetail == null)
            {
                this.RecordEvent("TabConfiguration - HTTP Post call.", RequestType.Failed);
                return this.BadRequest("Error while saving tab configuration details to storage.");
            }

            try
            {
                var tabConfigurationEntityModel = new TabConfiguration()
                {
                    // intentionally fetching group id from query string, as the same is validated by MustBeTeamMemberUserPolicy
                    GroupId = groupId,
                    TeamId = tabConfigurationDetail.TeamId,
                    ChannelId = tabConfigurationDetail.ChannelId,
                    LearningModuleId = tabConfigurationDetail.LearningModuleId,
                    CreatedBy = this.UserObjectId,
                    UpdatedBy = this.UserObjectId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                };

                this.unitOfWork.TabConfigurationRepository.Add(tabConfigurationEntityModel);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("TabConfiguration - HTTP Post call.", RequestType.Succeeded);
                return this.Ok(tabConfigurationEntityModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("TabConfiguration - HTTP Post call.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while saving tab configuration details.");
                throw;
            }
        }

        /// <summary>
        /// Get details of a tab configuration by entity Id.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <param name="id">Unique identifier of teams tab.</param>
        /// <returns>Returns tab configuration details received from storage.</returns>
        [HttpGet("{id}")]
        [Authorize(PolicyNames.MustBeTeamMemberUserPolicy)]
        public async Task<IActionResult> GetAsync([FromQuery] string groupId, Guid id)
        {
            try
            {
                this.logger.LogInformation("Initiated call for fetching tab configuration details from storage");
                this.RecordEvent("TabConfiguration - HTTP Get call.", RequestType.Initiated);

                var tabConfigurations = await this.unitOfWork.TabConfigurationRepository
                    .FindAsync(tab => tab.Id == id && tab.GroupId == groupId);

                if (tabConfigurations == null || tabConfigurations.FirstOrDefault() == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"The tab configuration detail that user is trying to get does not exists for tab Id: {id}.");
                    this.RecordEvent("Resource - HTTP Get call failed.", RequestType.Failed);
                    return this.NotFound($"No tab configuration detail found for Id: {id}.");
                }

                // it is must that only one record from database should present
                if (tabConfigurations.Count() > 1)
                {
                    this.logger.LogError("Database error: more than one records found for same tab id and group id combination. This could happen if someone manipulated the database.");
                    this.RecordEvent("Resource - HTTP Get call failed.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "An exception occured while fetching tab configuration details.");
                }

                var tabConfiguration = tabConfigurations.First();

                this.logger.LogInformation("GET call for fetching tab configuration details from storage is successful.");
                this.RecordEvent("TabConfiguration - HTTP Get call", RequestType.Succeeded);

                return this.Ok(tabConfiguration);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while getting tab configuration details from storage for tab ID : {id}");
                this.RecordEvent($"TabConfiguration - HTTP Get call", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Patch call to update tab configuration details in storage.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <param name="id">Tab identifier.</param>
        /// <param name="tabConfigurationDetail">Holds tab configuration detail entity data.</param>
        /// <returns>Returns updated tab configuration details.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeTeamMemberUserPolicy)]
        public async Task<IActionResult> PatchAsync([FromQuery] string groupId, [FromRoute] Guid id, [FromBody] TabConfigurationViewModel tabConfigurationDetail)
        {
            this.RecordEvent("TabConfiguration - HTTP Patch call.", RequestType.Initiated);
            this.logger.LogInformation("TabConfiguration - HTTP Patch call initiated.");

            if (tabConfigurationDetail == null)
            {
                this.RecordEvent("TabConfiguration - HTTP Patch call.", RequestType.Failed);
                return this.BadRequest("Error while updating tab configuration details to storage.");
            }

            try
            {
                if (id == null || id == Guid.Empty)
                {
                    this.logger.LogError("Tab Id is either null or empty.");
                    this.RecordEvent("TabConfiguration - HTTP Patch call failed.", RequestType.Failed);
                    return this.BadRequest("Tab Id cannot be null or empty guid.");
                }

                var existingTabConfigurations = await this.unitOfWork.TabConfigurationRepository
                    .FindAsync(tab => tab.Id == id && tab.GroupId == groupId);

                if (existingTabConfigurations == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"The tab configuration either not exists for id: {id} or is not mapped with provided group id.");
                    this.RecordEvent("TabConfiguration - HTTP Patch call failed.", RequestType.Failed);
                    return this.NotFound($"No tab configuration detail exists for tab Id: {id}.");
                }

                // it is must that only one record from database should present
                if (existingTabConfigurations.Count() > 1)
                {
                    this.logger.LogError("Database error: more than one records found for same tab id and group id combination. This could happen if someone manipulated the database.");
                    this.RecordEvent("Resource - HTTP Get call failed.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "An exception occured while fetching tab configuration details.");
                }

                var existingTabConfiguration = existingTabConfigurations.First();

                existingTabConfiguration.UpdatedOn = DateTime.UtcNow;
                existingTabConfiguration.UpdatedBy = this.UserObjectId;
                existingTabConfiguration.LearningModuleId = tabConfigurationDetail.LearningModuleId;

                this.unitOfWork.TabConfigurationRepository.Update(existingTabConfiguration);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("TabConfiguration - HTTP Patch call.", RequestType.Succeeded);
                return this.Ok(existingTabConfiguration);
            }
            catch (Exception ex)
            {
                this.RecordEvent("TabConfiguration - HTTP Patch call.", RequestType.Failed);
                this.logger.LogError(ex, $"TabConfiguration - HTTP Patch call failed, for tab Id: {id}.");
                throw;
            }
        }
    }
}