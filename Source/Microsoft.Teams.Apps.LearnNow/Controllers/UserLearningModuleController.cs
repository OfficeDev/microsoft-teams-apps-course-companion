// <copyright file="UserLearningModuleController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;

    /// <summary>
    /// Controller to handle user learning module API operations.
    /// </summary>
    [Route("api/me/learningmodules")]
    [ApiController]
    [Authorize]
    public class UserLearningModuleController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<UserLearningModuleController> logger;

        /// <summary>
        /// Instance for handling common operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Instance of user service to get user data.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// The instance of learning module mapper class to work with models.
        /// </summary>
        private readonly ILearningModuleMapper learningModuleMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLearningModuleController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">User learning module repository for working with user learning module data.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        /// <param name="learningModuleMapper">The instance of learning module mapper class to work with models.</param>
        public UserLearningModuleController(
            ILogger<UserLearningModuleController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            IUsersService usersService,
            ILearningModuleMapper learningModuleMapper)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.usersService = usersService;
            this.learningModuleMapper = learningModuleMapper;
        }

        /// <summary>
        /// Post call to store user learning module data in storage.
        /// </summary>
        /// <param name="userLearningModuleDetail">Holds user learning module entity object.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync(UserLearningModuleViewModel userLearningModuleDetail)
        {
            this.RecordEvent("User learning module - Http Post call initiated.", RequestType.Initiated);
            this.logger.LogInformation("User learning module - Http Post call initiated.");

            try
            {
                if (userLearningModuleDetail == null)
                {
                    this.logger.LogError($"Error while saving user learning module details to storage for userId: {this.UserObjectId}");
                    this.RecordEvent("User learning module - Http Post call failed.", RequestType.Failed);
                    return this.BadRequest("User learning module details cannot be null.");
                }

                var userLearningModuleEntity = new UserLearningModule()
                {
                    UserId = this.UserObjectId,
                    LearningModuleId = userLearningModuleDetail.LearningModuleId,
                    CreatedOn = DateTime.UtcNow,
                };
                var userResorces = await this.unitOfWork.UserLearningModuleRepository.FindAsync(userLearningModule => userLearningModule.LearningModuleId == userLearningModuleEntity.LearningModuleId && userLearningModule.UserId == userLearningModuleEntity.UserId);
                if (userResorces.Any())
                {
                    this.logger.LogInformation("User learning module - Http Post call succeeded.");
                    this.RecordEvent("User learning module - Http Post call succeeded.", RequestType.Succeeded);
                    return this.Conflict("User learning module already exists.");
                }

                this.unitOfWork.UserLearningModuleRepository.Add(userLearningModuleEntity);
                await this.unitOfWork.SaveChangesAsync();
                this.logger.LogInformation("User learning module - Http Post call succeeded.");
                this.RecordEvent("User learning module - Http Post call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("User learning module - Http Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while saving user learning module details for user id: {this.UserObjectId}.");
                throw;
            }
        }

        /// <summary>
        /// Deletes user learning module details from the storage.
        /// </summary>
        /// <param name="id">Holds user learning module id that is to be deleted.</param>
        /// <returns>Returns success status code if user learning module deleted successfully.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            this.RecordEvent("User learning module - HTTP Delete call initiated.", RequestType.Initiated);
            this.logger.LogInformation("User learning module - HTTP Delete call initiated.");

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"Learning module id is either null or empty guid for userId : {this.UserObjectId}");
                this.RecordEvent("User learning module - HTTP Delete call failed.", RequestType.Failed);
                this.BadRequest("Learning module id cannot be null or empty.");
            }

            try
            {
                var userLearningModuleRequestsData = await this.unitOfWork.UserLearningModuleRepository.FindAsync(userLearningModule => userLearningModule.LearningModuleId == id && userLearningModule.UserId == this.UserObjectId);

                if (!userLearningModuleRequestsData.Any())
                {
                    this.RecordEvent("User learning module - HTTP Delete call failed.", RequestType.Failed);
                    this.logger.LogError($"No record found for provided user learning module Id: {id}.");
                    return this.NotFound("No record found for provided user learning module Id.");
                }

                this.unitOfWork.UserLearningModuleRepository.Delete(userLearningModuleRequestsData.FirstOrDefault());
                await this.unitOfWork.SaveChangesAsync();

                this.RecordEvent("User learning module - HTTP Delete call succeeded.", RequestType.Succeeded);
                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("User learning module - HTTP Delete call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while deleting user learning module details with Id: {id}.");
                throw;
            }
        }

        /// <summary>
        /// Fetch user created or saved learning modules according to page count and filter.
        /// </summary>
        /// <param name="page">Page number to get filtered data.</param>
        /// <param name="filterModel">User selected filter based on which learning module entity needs to be filtered.</param>
        /// <returns>A collection of learning modules based on provided filters.</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync(int page, UserLearningFilterModel filterModel)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));
            this.logger.LogInformation("User Learning modules search- HTTP Post Call initiated.");
            this.RecordEvent("User Learning module search- HTTP Post call initiated.", RequestType.Initiated);

            if (page < 0)
            {
                this.logger.LogError("User Learning module search- HTTP Post call Failed, parameter pageCount is less than zero.");
                this.RecordEvent("User Learning module search- HTTP Post call Failed.", RequestType.Failed);
                return this.BadRequest($"Parameter {nameof(page)} cannot be less than zero.");
            }

            var skipRecords = page * Constants.LazyLoadPerPagePostCount;

            try
            {
                IEnumerable<LearningModule> learningModules = new List<LearningModule>();

                if (filterModel.IsSaved)
                {
                    learningModules = await this.unitOfWork.UserLearningModuleRepository.GetUserSavedModulesAsync(filterModel, skipRecords, Constants.LazyLoadPerPagePostCount);
                }
                else
                {
                    learningModules = await this.unitOfWork.LearningModuleRepository.GetUserModulesAsync(filterModel, Constants.LazyLoadPerPagePostCount, skipRecords);
                }

                // Post userId and user display name.
                var userAADObjectIds = learningModules.Select(resource => resource.CreatedBy).Distinct().Select(userObjectId => userObjectId.ToString());
                Dictionary<Guid, string> idToNameMap = new Dictionary<Guid, string>();
                if (userAADObjectIds.Any())
                {
                    idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);
                }

                var moduleWithVotesAndResources = this.unitOfWork.LearningModuleRepository.GetModulesWithVotesAndResources(learningModules);

                var learningModuleDetails = this.learningModuleMapper.MapToViewModels(
                    moduleWithVotesAndResources,
                    this.UserObjectId,
                    idToNameMap);

                this.logger.LogInformation("User Learning module search- HTTP Post Call succeeded.");
                this.RecordEvent("User Learning module search- HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(learningModuleDetails);
            }
            catch (Exception ex)
            {
                this.RecordEvent("User Learning module search- HTTP Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, "User Learning module search- HTTP Post call failed.");
                throw;
            }
        }
    }
}