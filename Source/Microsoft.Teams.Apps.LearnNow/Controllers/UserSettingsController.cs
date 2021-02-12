// <copyright file="UserSettingsController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;

    /// <summary>
    /// Controller to handle userSetting API operations.
    /// </summary>
    [Route("api/usersettings")]
    [ApiController]
    [Authorize]
    public class UserSettingsController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<UserSettingsController> logger;

        /// <summary>
        /// Instance for handling common operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The instance of userSetting mapper class to work with userSetting models.
        /// </summary>
        private readonly IUserSettingMapper userSettingMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">UserSetting repository for working with userSetting data.</param>
        /// <param name="userSettingMapper">The instance of userSetting mapper class to work with models.</param>
        public UserSettingsController(
            ILogger<UserSettingsController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            IUserSettingMapper userSettingMapper)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userSettingMapper = userSettingMapper;
        }

        /// <summary>
        /// Gets user filter setting from storage.
        /// </summary>
        /// <param name="entityType">Type of entity for which user setting details needs to be created or updated.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpGet("{entityType}")]
        public async Task<IActionResult> GetAsync(string entityType)
        {
            this.logger.LogInformation($"Get call initiated.");
            this.RecordEvent("UserSettings - HTTP Get call initiated.", RequestType.Initiated);

            try
            {
                var userSetting = await this.unitOfWork.UserSettingRepository.GetAsync(this.UserObjectId);

                if (userSetting == null)
                {
                    this.logger.LogInformation($"No userSetting record found for id: {this.UserObjectId} ");
                    return this.NotFound();
                }

                var userSettingViewModel = this.userSettingMapper.CreateMapToViewModel(userSetting, entityType);

                this.RecordEvent("UserSettings - HTTP Get call succeeded.", RequestType.Succeeded);
                return this.Ok(userSettingViewModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("UserSettings - HTTP Get call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching userSetting details");
                throw;
            }
        }

        /// <summary>
        /// Post call to store userSetting details in storage.
        /// </summary>
        /// <param name="entityType">Type of entity for which user setting details needs to be created or updated.</param>
        /// <param name="userSettingDetail">Holds userSetting detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost("{entityType}")]
        public async Task<IActionResult> PostAsync(string entityType, UserSettingsModel userSettingDetail)
        {
            if (userSettingDetail == null)
            {
                return this.BadRequest("User setting details can not be null.");
            }

            this.RecordEvent("UserSettings - HTTP Post call to add userSetting details.", RequestType.Initiated);
            this.logger.LogInformation("Call to add userSetting details.");

            try
            {
                var existingUserSetting = await this.unitOfWork.UserSettingRepository.GetAsync(this.UserObjectId);

                if (existingUserSetting == null)
                {
                    var userSettingEntityModel = this.userSettingMapper.CreateMap(userSettingDetail, entityType, this.UserObjectId);
                    this.unitOfWork.UserSettingRepository.Add(userSettingEntityModel);
                }
                else
                {
                    this.userSettingMapper.UpdateMap(userSettingDetail, existingUserSetting, entityType);
                    this.unitOfWork.UserSettingRepository.Update(existingUserSetting);
                }

                await this.unitOfWork.SaveChangesAsync();

                this.RecordEvent("UserSettings - HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("UserSettings - HTTP Post call failed for saving userSetting data", RequestType.Failed);
                this.logger.LogError(ex, "Error while saving userSetting details");
                throw;
            }
        }
    }
}