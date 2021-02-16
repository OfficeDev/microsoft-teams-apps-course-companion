// <copyright file="UserResourceController.cs" company="Microsoft Corporation">
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
    /// Controller to handle user resource entity API operations.
    /// </summary>
    [Route("api/me/resources")]
    [ApiController]
    [Authorize]
    public class UserResourceController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<UserResourceController> logger;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Instance of user service to get user data.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// The instance of resource mapper class to work with models.
        /// </summary>
        private readonly IResourceMapper resourceMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserResourceController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">User learning module repository for working with user learning module data.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        /// <param name="resourceMapper">The instance of resource mapper class to work with models.</param>
        public UserResourceController(
            ILogger<UserResourceController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            IUsersService usersService,
            IResourceMapper resourceMapper)
           : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.resourceMapper = resourceMapper;
            this.usersService = usersService;
        }

        /// <summary>
        /// Post call to store user resource details in storage.
        /// </summary>
        /// <param name="userResourceDetail">Holds user resource detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync(UserResourceViewModel userResourceDetail)
        {
            this.RecordEvent("User Resource- Http Post Call initiated.", RequestType.Initiated);
            this.logger.LogInformation($"User Resource- Http Post Call initiated.");

            try
            {
                if (userResourceDetail == null)
                {
                    this.logger.LogError($"Error while saving user resource details to storage for userId: {this.UserObjectId}");
                    this.RecordEvent("User Resource - Http Post call failed.", RequestType.Failed);
                    return this.BadRequest("Error while saving user resource details in storage.");
                }

                var userResourceEntity = new UserResource
                {
                    UserId = this.UserObjectId,
                    ResourceId = userResourceDetail.ResourceId,
                    CreatedOn = DateTime.UtcNow,
                };
                var userResorces = await this.unitOfWork.UserResourceRepository.FindAsync(userResorce => userResorce.ResourceId == userResourceEntity.ResourceId && userResorce.UserId == userResourceEntity.UserId);
                if (userResorces.Any())
                {
                    this.logger.LogInformation($"User Resource - Http Post call succeeded.");
                    this.RecordEvent("User Resource - Http Post call succeeded.", RequestType.Succeeded);
                    return this.Conflict(true);
                }

                this.unitOfWork.UserResourceRepository.Add(userResourceEntity);
                await this.unitOfWork.SaveChangesAsync();
                this.logger.LogInformation($"User Resource - Http Post call succeeded.");
                this.RecordEvent("User Resource - Http Post call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("User Resource - Http Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"User Resource - Http Post call failed for user id: {this.UserObjectId}.");
                throw;
            }
        }

        /// <summary>
        /// Deletes user resource details from the storage.
        /// </summary>
        /// <param name="id">Holds user resource id that is to be delete.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            this.RecordEvent("User Resource - HTTP Delete call initiated.", RequestType.Initiated);
            this.logger.LogInformation("User Resource - HTTP Delete call initiated.");

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"resourceId is either null or empty guid for userId: {this.UserObjectId}");
                this.RecordEvent("User Resource - HTTP Delete call failed.", RequestType.Failed);
                this.BadRequest("Resource Id cannot be null or empty.");
            }

            try
            {
                var userResourceRequestsData = await this.unitOfWork.UserResourceRepository.FindAsync(userResource => userResource.ResourceId == id && userResource.UserId == this.UserObjectId);

                if (!userResourceRequestsData.Any())
                {
                    this.logger.LogError($"User Resource - HTTP Delete call failed, user resource with Id: {id} not found for userId: {this.UserObjectId}");
                    this.RecordEvent("User Resource - HTTP Delete call failed.", RequestType.Failed);

                    return this.NotFound($"No user resource record found for provided resource Id: {id}.");
                }

                this.unitOfWork.UserResourceRepository.Delete(userResourceRequestsData.FirstOrDefault());
                await this.unitOfWork.SaveChangesAsync();

                this.RecordEvent("User Resource - HTTP Delete call succeeded.", RequestType.Succeeded);
                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("User Resource - HTTP Delete call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while deleting user resource details with id: {id}.");
                throw;
            }
        }

        /// <summary>
        /// Fetch user created or saved resources according to page count.
        /// </summary>
        /// <param name="page">Page number to get search data.</param>
        /// <param name="filterModel">User Selected filter based on which learning module entity needs to be filtered.</param>
        /// <returns>A collection of user resources based on provided filters.</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync(int page, UserLearningFilterModel filterModel)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));
            this.logger.LogInformation("User Resource search- HTTP Post call initiated.");
            this.RecordEvent("User Resource search- HTTP Post call initiated.", RequestType.Initiated);

            if (page < 0)
            {
                this.logger.LogError($"{nameof(page)} is found to be less than zero during {nameof(this.SearchAsync)} call.");
                this.RecordEvent("Resource search- HTTP Post call succeeded.", RequestType.Succeeded);
                return this.BadRequest($"Parameter {nameof(page)} cannot be less than zero.");
            }

            var skipRecords = page * Constants.LazyLoadPerPagePostCount;

            try
            {
                IEnumerable<Resource> resources = new List<Resource>();
                if (filterModel.IsSaved)
                {
                    resources = await this.unitOfWork.UserResourceRepository.GetUserSavedResourcesAsync(filterModel, skipRecords, Constants.LazyLoadPerPagePostCount);
                }
                else
                {
                    resources = await this.unitOfWork.ResourceRepository.GetUserResourcesAsync(filterModel, skipRecords, Constants.LazyLoadPerPagePostCount);
                }

                // Post userId and user display name.
                var userAADObjectIds = resources.Select(resource => resource.CreatedBy).Distinct().Select(userObjectId => userObjectId.ToString());

                Dictionary<Guid, string> idToNameMap = new Dictionary<Guid, string>();
                if (userAADObjectIds.Any())
                {
                    idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);
                }

                var resourcesWithVote = this.unitOfWork.ResourceRepository.GetResourcesWithVotes(resources);
                var resourceDetails = this.resourceMapper.MapToViewModels(
                    resourcesWithVote,
                    this.UserObjectId,
                    idToNameMap);

                this.logger.LogInformation("User Resources search- HTTP Post call succeeded.");
                this.RecordEvent("User Resource search- HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(resourceDetails);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"User Resource search- HTTP Post call failed for userId: {this.UserObjectId}.");
                this.RecordEvent("User Resource search- HTTP Post call failed.", RequestType.Failed);
                throw;
            }
        }
    }
}