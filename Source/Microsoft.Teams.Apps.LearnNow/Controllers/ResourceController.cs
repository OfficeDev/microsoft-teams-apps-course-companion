// <copyright file="ResourceController.cs" company="Microsoft Corporation">
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;

    /// <summary>
    /// Controller to handle resource operations.
    /// </summary>
    [Route("api/resources")]
    [ApiController]
    [Authorize]
    public class ResourceController : BaseController
    {
        /// <summary>
        /// The instance of unit of work to access repository.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<ResourceController> logger;

        /// <summary>
        /// Instance of user service to get user data.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// The instance of resource mapper class to work with models.
        /// </summary>
        private readonly IResourceMapper resourceMapper;

        /// <summary>
        /// Instance of MemberValidationService to validate member.
        /// </summary>
        private readonly IMemberValidationService memberValidationService;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private readonly IOptions<SecurityGroupSettings> securityGroupOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The instance of unit of work to access repository.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        /// <param name="resourceMapper">The instance of resource mapper class to work with models.</param>
        /// <param name="memberValidationService">Instance of MemberValidationService to validate member of a security group.</param>
        /// <param name="securityGroupOptions">Security group configuration settings.</param>
        public ResourceController(
            IUnitOfWork unitOfWork,
            TelemetryClient telemetryClient,
            ILogger<ResourceController> logger,
            IUsersService usersService,
            IResourceMapper resourceMapper,
            IMemberValidationService memberValidationService,
            IOptions<SecurityGroupSettings> securityGroupOptions)
           : base(telemetryClient)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.resourceMapper = resourceMapper;
            this.usersService = usersService;
            this.memberValidationService = memberValidationService;
            this.securityGroupOptions = securityGroupOptions;
        }

        /// <summary>
        /// Save resource details entered by user to storage
        /// </summary>
        /// <param name="resourceDetail">Resource fields entered by user.</param>
        /// <returns>Returns success if data is saved successfully.</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> PostAsync(ResourceViewModel resourceDetail)
        {
            try
            {
                this.logger.LogInformation("Resource - HTTP Post call initiated.");
                this.RecordEvent("Resource - HTTP Post call initiated.", RequestType.Initiated);

                if (resourceDetail == null)
                {
                    this.logger.LogError($"Resource - HTTP Post call failed, resource detail is null for userId: {this.UserObjectId}.");
                    this.RecordEvent("Resource - HTTP Post call failed.", RequestType.Failed);
                    return this.BadRequest($"Resource detail can not be null.");
                }

                var resourceEntityModel = this.resourceMapper.MapToDTO(resourceDetail, this.UserObjectId);

                // Storage call to save resource details.
                var resourceDetails = this.unitOfWork.ResourceRepository.Add(resourceEntityModel);
                await this.unitOfWork.SaveChangesAsync();

                this.logger.LogInformation($"Resource - HTTP Post call succeeded.");
                this.RecordEvent("Resource - HTTP Post call succeeded.", RequestType.Succeeded);

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = new string[] { this.UserObjectId.ToString() };
                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);
                var resourceViewModel = this.resourceMapper.MapToViewModel(resourceDetails, idToNameMap);

                return this.Ok(resourceViewModel);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while saving resource details to storage for user Id: {this.UserObjectId}.");
                this.RecordEvent("Resource - HTTP Post call failed.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Get details of a resource by resource Id.
        /// </summary>
        /// <param name="id">Unique identifier of resource which user wants to edit.</param>
        /// <returns>Returns resource details received from storage.</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetResourceDetailAsync(Guid id)
        {
            try
            {
                this.logger.LogInformation("Resource - HTTP Get call initiated.");
                this.RecordEvent("Resource - HTTP Get call initiated.", RequestType.Initiated);
                var resourceDetails = await this.unitOfWork.ResourceRepository.GetAsync(id);

                if (resourceDetails == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"The resource detail with id: {id} that user is trying to get does not exists, for userId: {this.UserObjectId}.");
                    this.RecordEvent("Resource - HTTP Get call failed.", RequestType.Failed);
                    return this.NotFound($"No resource record found for resource Id: {id}.");
                }

                this.logger.LogInformation("Resource - HTTP Get call succeeded.");
                this.RecordEvent("Resource - HTTP Get call succeeded.", RequestType.Succeeded);

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = new string[] { resourceDetails.CreatedBy.ToString() };

                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);
                var resourceVotes = await this.GetResourceVotesAsync(resourceDetails.Id);

                var resourceViewModel = this.resourceMapper.MapToViewModel(resourceDetails, this.UserObjectId, resourceVotes, idToNameMap);
                return this.Ok(resourceViewModel);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Resource - HTTP Get call failed, for resource Id: {id} and userId: {this.UserObjectId}");
                this.RecordEvent("Resource - HTTP Get call failed.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Patch call to update a resource detail in storage.
        /// </summary>
        /// <param name="id">Holds resource id for resource which needs to be updated.</param>
        /// <param name="resourceDetail">Resource fields entered by user.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> PatchAsync(Guid id, ResourceViewModel resourceDetail)
        {
            try
            {
                this.logger.LogInformation($"Resource - HTTP Patch call initiated.");
                this.RecordEvent("Resource - HTTP Patch call initiated.", RequestType.Initiated);

                if (resourceDetail == null)
                {
                    this.logger.LogError($"Resource - HTTP Patch call failed, resource details is null for userId: {this.UserObjectId}");
                    this.RecordEvent("Resource - HTTP Patch call failed.", RequestType.Failed);

                    return this.BadRequest($"Resource details cannot be null.");
                }

                if (id == null || id == Guid.Empty)
                {
                    this.logger.LogError($"Resource - HTTP Patch call failed, resource Id is either null or empty guid.");
                    this.RecordEvent("Resource - HTTP Patch call failed.", RequestType.Failed);
                    return this.BadRequest("Resource Id cannot be null or empty guid.");
                }

                var existingResourceDetails = await this.unitOfWork.ResourceRepository.GetAsync(id);

                if (existingResourceDetails == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"The resource detail that user is trying to update does not exist. Resource Id: {id} ");
                    this.RecordEvent("Resource - HTTP Patch call failed.", RequestType.Failed);
                    return this.NotFound($"No resource detail found for given resource Id: {id}.");
                }

                if (existingResourceDetails.CreatedBy != this.UserObjectId)
                {
                    var isAdmin = await this.memberValidationService.ValidateMemberAsync(this.UserObjectId.ToString(), this.securityGroupOptions.Value.AdminGroupId, this.Request.Headers["Authorization"].ToString());
                    if (!isAdmin)
                    {
                        this.logger.LogError(StatusCodes.Status401Unauthorized, $"The current user who is trying to update resource detail have not created resource or not a part of administrator group for resource id: {id} ");
                        this.RecordEvent("Resource - HTTP Patch call failed, user is not creator of resource or not part of an administrator group.", RequestType.Failed);
                        return this.Unauthorized("Current user does not have permission to update given resource");
                    }
                }

                // Delete existing resource tag from storage.
                this.unitOfWork.ResourceTagRepository.Delete(existingResourceDetails.ResourceTag);

                // Update resource details to storage.
                var resourceTag = resourceDetail.ResourceTag;
                resourceDetail.ResourceTag = null;
                resourceDetail.CreatedOn = existingResourceDetails.CreatedOn;
                resourceDetail.Id = id;

                var resourceEntityModel = this.resourceMapper.PatchAndMapToDTO(resourceDetail, this.UserObjectId);
                var updatedResourceDetails = this.unitOfWork.ResourceRepository.Update(resourceEntityModel);

                this.logger.LogInformation($"Update resource details call is successful for userId: {this.UserObjectId}.");
                this.RecordEvent("Update resource detail to storage call successful.", RequestType.Succeeded);

                if (updatedResourceDetails == null)
                {
                    this.logger.LogError($"Could not update resource data for resource: {id} and userId: {this.UserObjectId}");
                    this.RecordEvent("Resource - HTTP PatchAsync failed.", RequestType.Failed);
                    return this.NotFound($"No resource found for resource: {id} and for userId: {this.UserObjectId}.");
                }

                // Add selected resource Tags to database.
                List<ResourceTag> resourceTags = resourceTag.Select(x => new ResourceTag { ResourceId = updatedResourceDetails.Id, TagId = x.TagId }).ToList();

                this.unitOfWork.ResourceTagRepository.Add(resourceTags);
                await this.unitOfWork.SaveChangesAsync();

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = new string[] { resourceDetail.CreatedBy.ToString() };

                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);
                var resourceVotes = await this.GetResourceVotesAsync(resourceDetail.Id);
                var resourcedata = await this.unitOfWork.ResourceRepository.GetAsync(updatedResourceDetails.Id);

                var resource = this.resourceMapper.PatchAndMapToViewModel(
                    resourcedata,
                    this.UserObjectId,
                    resourceVotes,
                    idToNameMap);
                this.RecordEvent("Resource - HTTP patch call is succeeded.", RequestType.Succeeded);
                this.logger.LogInformation("Resource - HTTP patch call is succeeded.");

                return this.Ok(resource);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Resource - HTTP Patch call failed for resource Id: {id} and userId: {this.UserObjectId}");
                this.RecordEvent("Resource - HTTP Patch call failed", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Deletes resource details from the storage.
        /// </summary>
        /// <param name="id">Holds resource detail resource id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("{id}")]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            this.RecordEvent("Resource - HTTP Delete call initiated.", RequestType.Initiated);
            this.logger.LogInformation("Resource - HTTP Delete call initiated.");
            try
            {
                if (id == null || id == Guid.Empty)
                {
                    this.logger.LogError("Resource - HTTP Delete call failed, resource Id is either null or empty guid.");
                    this.RecordEvent("Resource - HTTP Delete call failed", RequestType.Failed);

                    return this.BadRequest("Resource Id cannot be null or empty.");
                }

                var resourceRequestsData = await this.unitOfWork.ResourceRepository.GetAsync(id);

                if (resourceRequestsData == null)
                {
                    this.RecordEvent("Resource - HTTP Delete call failed", RequestType.Failed);
                    this.logger.LogError($"Resource - HTTP Delete call failed, no resource detail found for provided resource Id {id} and userId: {this.UserObjectId}");
                    return this.NotFound($"No resource record found for provided resource Id: {id}.");
                }

                if (resourceRequestsData.CreatedBy != this.UserObjectId)
                {
                    var isAdmin = await this.memberValidationService.ValidateMemberAsync(this.UserObjectId.ToString(), this.securityGroupOptions.Value.AdminGroupId, this.Request.Headers["Authorization"].ToString());
                    if (!isAdmin)
                    {
                        this.logger.LogError(StatusCodes.Status401Unauthorized, $"The current user who is trying to delete resource detail have not created resource or not a part of administrator group for resource id: {id} ");
                        this.RecordEvent("Resource - HTTP Delete call failed, user is not a creator of resource or not part of administrator group.", RequestType.Failed);
                        return this.Unauthorized("Current user does not have permission to delete given resource");
                    }
                }

                // Delete existing resource tag from storage.
                this.unitOfWork.ResourceTagRepository.Delete(resourceRequestsData.ResourceTag);

                // Delete resource Vote
                var resourceVotes = await this.unitOfWork.ResourceVoteRepository.FindAsync(lmVotes => lmVotes.ResourceId == id);

                if (resourceVotes.Any())
                {
                    this.unitOfWork.ResourceVoteRepository.DeleteResourceVotes(resourceVotes);
                }

                // Delete Resource Module Mapping
                var resourceModuleMappings = await this.unitOfWork.ResourceModuleRepository.FindAsync(lmVotes => lmVotes.ResourceId == id);

                if (resourceModuleMappings.Any())
                {
                    this.unitOfWork.ResourceModuleRepository.DeleteResourceModuleMappings(resourceModuleMappings);
                }

                // Delete user resource
                var userResources = await this.unitOfWork.UserResourceRepository.FindAsync(lmVotes => lmVotes.ResourceId == id);

                if (userResources.Any())
                {
                    this.unitOfWork.UserResourceRepository.DeleteUserResources(userResources);
                }

                this.unitOfWork.ResourceRepository.Delete(resourceRequestsData);
                await this.unitOfWork.SaveChangesAsync();

                this.RecordEvent("Resource - HTTP delete call succeeded.", RequestType.Succeeded);
                this.logger.LogInformation("Resource - HTTP delete call succeeded.");

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Resource - HTTP delete call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Resource - HTTP delete call failed, for resource Id: {id} and userId: {this.UserObjectId}");
                throw;
            }
        }

        /// <summary>
        /// Post call to store vote details in database.
        /// </summary>
        /// <param name="id">Resource id for which vote  details needs to be stored in storage.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        [Route("{id}/upvote")]
        public async Task<IActionResult> PostVoteAsync(Guid id)
        {
            this.logger.LogInformation($"Resource vote- HTTP Post call initiated.");
            this.RecordEvent("Resource vote- HTTP Post call initiated.", RequestType.Initiated);

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"Resource vote- HTTP Post call failed, resource Id is either null or empty guid for userId: {this.UserObjectId}");
                this.RecordEvent("Resource vote- HTTP Post call failed.", RequestType.Failed);
                return this.BadRequest("Resource Id cannot be null or empty.");
            }

            try
            {
                var resourceVotes = await this.unitOfWork.ResourceVoteRepository.FindAsync(resourceVote => resourceVote.UserId == this.UserObjectId && resourceVote.ResourceId == id);
                if (resourceVotes.Any())
                {
                    this.logger.LogError(StatusCodes.Status409Conflict, $"Resource vote already exists for resource Id: {id} and user Id: {this.UserObjectId}.");
                    this.RecordEvent("Resource vote - HTTP Post call failed.", RequestType.Failed);
                    return this.Conflict($"Resource vote already exists for resource Id: {id} and user Id: {this.UserObjectId}.");
                }

                ResourceVote vote = new ResourceVote
                {
                    ResourceId = id,
                    UserId = this.UserObjectId,
                    CreatedOn = DateTime.UtcNow,
                };

                this.unitOfWork.ResourceVoteRepository.Add(vote);
                await this.unitOfWork.SaveChangesAsync();

                this.logger.LogInformation("Resource vote- HTTP Post call succeeded.");
                this.RecordEvent("Resource vote- HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Resource vote- HTTP Post failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while saving resource vote details for given resource ID: {id} and userId: {this.UserObjectId}");
                throw;
            }
        }

        /// <summary>
        /// Deletes vote details from the database.
        /// </summary>
        /// <param name="id">Resource id for which vote details needs to be deleted from storage.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the vote delete operation.</returns>
        [HttpPost]
        [Route("{id}/downvote")]
        public async Task<IActionResult> DeleteVoteAsync(Guid id)
        {
            this.logger.LogInformation("Resource vote delete- HTTP Post call initiated.");
            this.RecordEvent("Resource vote delete- HTTP Post call initiated.", RequestType.Initiated);

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"Resource Id is either null or empty guid for userId: {this.UserObjectId}");
                this.RecordEvent("Resource vote delete- HTTP Post call failed.", RequestType.Failed);
                return this.BadRequest("Resource Id cannot be null or empty.");
            }

            try
            {
                var voteDetails = await this.unitOfWork.ResourceVoteRepository.FindAsync(vote => vote.UserId == this.UserObjectId && vote.ResourceId == id);

                if (!voteDetails.Any())
                {
                    this.RecordEvent("Resource vote delete- HTTP Post call failed.", RequestType.Failed);
                    this.logger.LogError($"Resource vote details not found for given resource ID: {id} and userId: {this.UserObjectId}");
                    return this.Ok($"Resource vote details not found for given resource ID: {id} and userId: {this.UserObjectId}");
                }
                else
                {
                    this.unitOfWork.ResourceVoteRepository.Delete(voteDetails.First());
                    await this.unitOfWork.SaveChangesAsync();
                }

                this.logger.LogInformation("Resource vote delete- Post vote call succeeded.");
                this.RecordEvent("Resource vote delete- Post vote call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Resource vote delete- HTTP Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while deleting vote details for given resource ID: {id} and userId: {this.UserObjectId}");
                throw;
            }
        }

        /// <summary>
        /// Fetch resources posts according to page count.
        /// </summary>
        /// <param name="page">Page number to get search data.</param>
        /// <param name="exactMatch">Represents whether resource title search should be exact match or not.</param>
        /// <param name="filterModel">User Selected filter based on which learning module entity needs to be filtered.</param>
        /// <returns>List of posts.</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync(int page, bool exactMatch, FilterModel filterModel)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));
            this.logger.LogInformation($"Resource search- HTTP Post call initiated.");
            this.RecordEvent("Resource search- HTTP Post call initiated.", RequestType.Initiated);

            if (page < 0)
            {
                this.logger.LogError($"{nameof(page)} is found to be less than zero during {nameof(this.SearchAsync)} call.");
                this.RecordEvent("Resource search- HTTP Post call succeeded.", RequestType.Succeeded);
                return this.BadRequest($"Parameter {nameof(page)} cannot be less than zero.");
            }

            var skipRecords = page * Constants.LazyLoadPerPagePostCount;

            try
            {
                var resources = await this.unitOfWork.ResourceRepository.GetResourcesAsync(filterModel, skipRecords, Constants.LazyLoadPerPagePostCount, exactMatch);
                this.RecordEvent("Resource search- HTTP Post call succeeded", RequestType.Succeeded);

                // Get userId and user display name.
                var createdByObjectIds = resources.Select(resource => resource.CreatedBy).Distinct().Select(userObjectId => userObjectId.ToString());
                Dictionary<Guid, string> idToNameMap = new Dictionary<Guid, string>();
                if (createdByObjectIds.Any())
                {
                    idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), createdByObjectIds);
                }

                var resourcesWithVote = this.unitOfWork.ResourceRepository.GetResourcesWithVotes(resources);
                var resourceDetails = this.resourceMapper.MapToViewModels(
                    resourcesWithVote,
                    this.UserObjectId,
                    idToNameMap);

                this.logger.LogInformation($"Resources search- HTTP Post call succeeded.");
                this.RecordEvent("Resource search- HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(resourceDetails);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Resource search- HTTP Post call failed for userId: {this.UserObjectId}.");
                this.RecordEvent("Resource search- HTTP Post call failed.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Get resource creator user names.
        /// </summary>
        /// <param name="recordCount">Maximum number of records that needs to be fetched</param>
        /// <returns>Returns unique user names.</returns>
        [HttpGet("authors")]
        public async Task<IActionResult> GetUniqueUserNamesAsync(int recordCount)
        {
            try
            {
                this.RecordEvent("GetUniqueUserNamesAsync resource- HTTP Get call Initiated", RequestType.Initiated);
                this.logger.LogInformation("GetUniqueUserNamesAsync resource- HTTP Get call Initiated.");

                var resourceAuthourIds = await this.unitOfWork.ResourceRepository.GetCreatedByObjectIdsAsync(recordCount);

                var usernames = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), resourceAuthourIds.Select(userId => userId.ToString()));
                var creatorDetails = usernames.Select(k => new UserDetail() { UserId = k.Key, DisplayName = k.Value });
                this.logger.LogInformation($"GetUniqueUserNamesAsync resource- HTTP Get call succeeded for userId: {this.UserObjectId}.");
                this.RecordEvent("GetUniqueUserNamesAsync resource- HTTP Get call succeeded", RequestType.Succeeded);
                return this.Ok(creatorDetails);
            }
            catch (Exception ex)
            {
                this.RecordEvent("GetUniqueUserNamesAsync resource- HTTP Get call failed", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching unique user names.");
                throw;
            }
        }

        /// <summary>
        /// Method to get votes for specified resource using resourceId.
        /// </summary>
        /// <param name="resourceId">ResourceId of resource for which vote needs to be obtained.</param>
        /// <returns>Returns votes for a specified resource.</returns>
        private async Task<IEnumerable<ResourceVote>> GetResourceVotesAsync(Guid resourceId)
        {
            this.logger.LogInformation("GetResourceVotesAsync - call initiated.");
            this.RecordEvent("GetResourceVotesAsync - call initiated.", RequestType.Initiated);

            var resourceVotes = await this.unitOfWork.ResourceVoteRepository.FindAsync(vote => vote.ResourceId == resourceId);

            this.logger.LogInformation($"GetResourceVotesAsync - call succeeded for resource: {resourceId}.");
            this.RecordEvent("GetResourceVotesAsync - call succeeded.", RequestType.Succeeded);
            return resourceVotes;
        }
    }
}