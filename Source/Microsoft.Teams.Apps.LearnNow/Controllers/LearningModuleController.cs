// <copyright file="LearningModuleController.cs" company="Microsoft Corporation">
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
    /// Controller to handle learning module API operations.
    /// </summary>
    [Route("api/learningmodules")]
    [ApiController]
    [Authorize]
    public class LearningModuleController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<LearningModuleController> logger;

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
        /// The instance of resource module mapper class to work with models.
        /// </summary>
        private readonly IResourceModuleMapper resourceModuleMapper;

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
        /// Initializes a new instance of the <see cref="LearningModuleController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">Learning module repository for working with learning module data.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        /// <param name="learningModuleMapper">The instance of learning module mapper class to work with models.</param>
        /// <param name="resourceModuleMapper">The instance of resource module mapper class to work with models</param>
        /// <param name="resourceMapper">The instance of resource mapper class to work with models.</param>
        /// <param name="memberValidationService">Instance of MemberValidationService to validate member of a security group.</param>
        /// <param name="securityGroupOptions">Security group configuration settings.</param>
        public LearningModuleController(
            ILogger<LearningModuleController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            IUsersService usersService,
            ILearningModuleMapper learningModuleMapper,
            IResourceModuleMapper resourceModuleMapper,
            IResourceMapper resourceMapper,
            IMemberValidationService memberValidationService,
            IOptions<SecurityGroupSettings> securityGroupOptions)
           : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.usersService = usersService;
            this.learningModuleMapper = learningModuleMapper;
            this.resourceModuleMapper = resourceModuleMapper;
            this.resourceMapper = resourceMapper;
            this.memberValidationService = memberValidationService;
            this.securityGroupOptions = securityGroupOptions;
        }

        /// <summary>
        /// Fetch learning modules according to page count and filter.
        /// </summary>
        /// <param name="page">Page number to get search data.</param>
        /// <param name="excludeEmptyModules">Represents whether filter should include learning modules which has resources associated with it.</param>
        /// <param name="exactMatch">Represents whether learning module title search should be exact match or not.</param>
        /// <param name="filterModel">User selected filter based on which learning module entity needs to be filtered.</param>
        /// <returns>A collection of learning modules based on provided filters.</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync(int page, bool excludeEmptyModules, bool exactMatch, FilterModel filterModel)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));
            this.logger.LogInformation("Learning modules search- HTTP Post Call initiated.");
            this.RecordEvent("Learning module search- HTTP Post call initiated.", RequestType.Initiated);

            try
            {
                if (page < 0)
                {
                    this.logger.LogError("Learning module search- HTTP Post call Failed, parameter pageCount is less than zero.");
                    this.RecordEvent("Learning module search- HTTP Post call Failed.", RequestType.Failed);
                    return this.BadRequest($"Parameter {nameof(page)} cannot be less than zero.");
                }

                var skipRecords = page * Constants.LazyLoadPerPagePostCount;

                var learningModules = await this.unitOfWork.LearningModuleRepository.GetLearningModulesAsync(filterModel, Constants.LazyLoadPerPagePostCount, skipRecords, exactMatch, excludeEmptyModules);
                var moduleWithVotesAndResources = this.unitOfWork.LearningModuleRepository.GetModulesWithVotesAndResources(learningModules);

                // Get userId and user display name.
                var createdByObjectIds = learningModules.Select(module => module.CreatedBy).Distinct().Select(userObjectId => userObjectId.ToString());
                Dictionary<Guid, string> idToNameMap = new Dictionary<Guid, string>();
                if (createdByObjectIds.Any())
                {
                    idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), createdByObjectIds);
                }

                var learningModuleDetails = this.learningModuleMapper.MapToViewModels(
                    moduleWithVotesAndResources,
                    this.UserObjectId,
                    idToNameMap);

                this.logger.LogInformation("Learning module search- HTTP Post Call succeeded.");
                this.RecordEvent("Learning module search- HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(learningModuleDetails);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Learning module search- HTTP Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Learning module search- HTTP Post call failed.");
                throw;
            }
        }

        /// <summary>
        /// Post call to store learning module data in storage.
        /// </summary>
        /// <param name="learningModuleDetail">Learning module fields entered by user.</param>
        /// <returns>Returns a learning module object..</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> PostAsync(LearningModuleViewModel learningModuleDetail)
        {
            this.RecordEvent("Learning module - HTTP Post call initiated.", RequestType.Initiated);
            this.logger.LogInformation("Learning module - HTTP Post call initiated.");

            try
            {
                if (learningModuleDetail == null)
                {
                    this.logger.LogError($"Error while saving learning module details to storage for userId: {this.UserObjectId}");
                    this.RecordEvent("Learning module - HTTP Post call failed.", RequestType.Failed);
                    return this.BadRequest("Error while saving learning module details in storage.");
                }

                var learningModuleEntityModel = this.learningModuleMapper.MapToDTO(learningModuleDetail, this.UserObjectId);

                // Storage call to save resource details.
                var learningModuleDetails = this.unitOfWork.LearningModuleRepository.Add(learningModuleEntityModel);
                await this.unitOfWork.SaveChangesAsync();

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = new string[] { this.UserObjectId.ToString() };
                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var learningModuleViewModel = this.learningModuleMapper.MapToViewModel(learningModuleDetails, idToNameMap);
                this.RecordEvent("Learning module - HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(learningModuleViewModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Learning module - HTTP Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error while saving learning module details.");
                throw;
            }
        }

        /// <summary>
        /// Patch call to update a learning module detail in storage.
        /// </summary>
        /// <param name="id">Holds resource id for resource which needs to be updated.</param>
        /// <param name="moduleResourceDetails">Learning module details.</param>
        /// <returns>Returns a learning module object.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> PatchAsync(Guid id, ResourceModuleViewPatchModel moduleResourceDetails)
        {
            try
            {
                this.logger.LogError("Learning module - HTTP Patch call initiated.");
                this.RecordEvent("Learning module - HTTP Patch call initiated", RequestType.Initiated);

                if (moduleResourceDetails == null)
                {
                    this.logger.LogError($"Learning module details is null for userId: {this.UserObjectId}");
                    this.RecordEvent("Learning module - HTTP Patch call failed.", RequestType.Failed);

                    return this.BadRequest("Learning module details cannot be null.");
                }

                if (id == null || id == Guid.Empty)
                {
                    this.logger.LogError($"Learning module - HTTP Patch module Id is either null or empty guid for userId: {this.UserObjectId}");
                    this.RecordEvent("Learning module - HTTP patch call failed.", RequestType.Failed);
                    return this.BadRequest("Learning module Id cannot be null or empty.");
                }

                var existingLearningModuleDetails = await this.unitOfWork.LearningModuleRepository.GetAsync(id);

                if (existingLearningModuleDetails == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"Learning module detail that user is trying to update does not exist for learning module id: {id} and userId: {this.UserObjectId}");
                    this.RecordEvent("Learning module - HTTP patch call failed.", RequestType.Failed);
                    return this.NotFound("Learning module not found.");
                }

                if (existingLearningModuleDetails.CreatedBy != this.UserObjectId)
                {
                    var isAdmin = await this.memberValidationService.ValidateMemberAsync(this.UserObjectId.ToString(), this.securityGroupOptions.Value.AdminGroupId, this.Request.Headers["Authorization"].ToString());
                    if (!isAdmin)
                    {
                        this.logger.LogError(StatusCodes.Status401Unauthorized, $"The current user who is trying to update learning module detail have not created learning module or not a part of administrator group for learning module id: {id} ");
                        this.RecordEvent("Resource - HTTP Patch call failed, user is not creator of learning module or not part of an administrator group.", RequestType.Failed);
                        return this.Unauthorized("Current user does not have permission to update given learning module");
                    }
                }

                // Delete existing resource tag from storage.
                this.unitOfWork.LearningModuleTagRepository.DeleteLearningModuleTag(existingLearningModuleDetails.LearningModuleTag);

                // Delete resource module mapping from storage.
                var existingResourceModuleMappings = await this.unitOfWork.ResourceModuleRepository.FindAsync(module => module.LearningModuleId == existingLearningModuleDetails.Id);
                if (existingResourceModuleMappings.Any() && moduleResourceDetails.Resources.Count() < existingResourceModuleMappings.Count())
                {
                    var resourceModuleMappingsToRetain = moduleResourceDetails.Resources;
                    foreach (var resourceModuleMapping in existingResourceModuleMappings)
                    {
                        var resourceMapping = resourceModuleMappingsToRetain.FirstOrDefault(k => k.Id == resourceModuleMapping.ResourceId);
                        if (resourceMapping == null)
                        {
                            this.unitOfWork.ResourceModuleRepository.Delete(resourceModuleMapping);
                        }
                    }
                }

                // Update learning module details to storage.
                var resourceTag = moduleResourceDetails.LearningModule.LearningModuleTag;
                moduleResourceDetails.LearningModule.LearningModuleTag = null;
                moduleResourceDetails.LearningModule.CreatedOn = existingLearningModuleDetails.CreatedOn;
                moduleResourceDetails.LearningModule.CreatedBy = existingLearningModuleDetails.CreatedBy;

                var learningModuleEntityModel = this.learningModuleMapper.PatchAndMapToDTO(moduleResourceDetails.LearningModule, this.UserObjectId);
                this.unitOfWork.LearningModuleRepository.Update(learningModuleEntityModel);

                // Add selected learning module Tags to database.
                List<LearningModuleTag> learningModuleTags = resourceTag.Select(x => new LearningModuleTag { LearningModuleId = learningModuleEntityModel.Id, TagId = x.TagId }).ToList();

                this.unitOfWork.LearningModuleTagRepository.AddLearningModuleTag(learningModuleTags);

                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Learning module - HTTP patch call succeeded.", RequestType.Succeeded);
                this.logger.LogInformation("Learning module - HTTP patch call succeeded");

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = new string[] { learningModuleEntityModel.CreatedBy.ToString() };
                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var learningModuleVotes = await this.GetLearningModuleVotesAsync(learningModuleEntityModel.Id);
                var moduledata = await this.unitOfWork.LearningModuleRepository.GetAsync(id);
                var resourceCount = await this.GetLearningModuleResourcesAsync(learningModuleEntityModel.Id);

                var learningModule = this.learningModuleMapper.PatchAndMapToViewModel(
                    moduledata,
                    this.UserObjectId,
                    learningModuleVotes,
                    resourceCount,
                    idToNameMap);

                return this.Ok(learningModule);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while updating learning module with Id: {id}");
                this.RecordEvent("Learning module - HTTP patch call failed.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Delete resource details from the storage.
        /// </summary>
        /// <param name="id">Learning module id to be deleted.</param>
        /// <returns>Returns success status code.</returns>
        [HttpDelete("{id}")]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            this.RecordEvent("Learning module - HTTP Delete call initiated.", RequestType.Initiated);
            this.logger.LogInformation("Learning module - HTTP Delete call initiated.");
            try
            {
                var learningModuleRequestsData = await this.unitOfWork.LearningModuleRepository.GetAsync(id);

                if (learningModuleRequestsData == null)
                {
                    this.RecordEvent("Learning module - HTTP Delete call failed.", RequestType.Failed);
                    this.logger.LogError(StatusCodes.Status404NotFound, $"No record found for learning module id: {id}.");
                    return this.NotFound("No record found for provided learning module id.");
                }

                if (learningModuleRequestsData.CreatedBy != this.UserObjectId)
                {
                    var isAdmin = await this.memberValidationService.ValidateMemberAsync(this.UserObjectId.ToString(), this.securityGroupOptions.Value.AdminGroupId, this.Request.Headers["Authorization"].ToString());
                    if (!isAdmin)
                    {
                        this.logger.LogError(StatusCodes.Status401Unauthorized, $"The current user who is trying to delete learning module detail have not created learning module or not a part of administrator group for learning module id: {id} ");
                        this.RecordEvent("Resource - HTTP Delete call failed, user is not a creator of learning module or not part of administrator group.", RequestType.Failed);
                        return this.Unauthorized("Current user does not have permission to delete given learning module");
                    }
                }

                // Delete existing learning module tag from storage.
                this.unitOfWork.LearningModuleTagRepository.DeleteLearningModuleTag(learningModuleRequestsData.LearningModuleTag);

                // Delete module vote
                var moduleVotes = await this.unitOfWork.LearningModuleVoteRepository.FindAsync(lmVotes => lmVotes.ModuleId == id);

                if (moduleVotes.Any())
                {
                    this.unitOfWork.LearningModuleVoteRepository.DeleteLearningModuleVotes(moduleVotes);
                }

                // Delete resource module mapping
                var resourceModuleMappings = await this.unitOfWork.ResourceModuleRepository.FindAsync(lmVotes => lmVotes.LearningModuleId == id);

                if (resourceModuleMappings.Any())
                {
                    this.unitOfWork.ResourceModuleRepository.DeleteResourceModuleMappings(resourceModuleMappings);
                }

                // Delete user modules
                var userModules = await this.unitOfWork.UserLearningModuleRepository.FindAsync(lmVotes => lmVotes.LearningModuleId == id);

                if (userModules.Any())
                {
                    this.unitOfWork.UserLearningModuleRepository.DeleteUserModules(userModules);
                }

                this.unitOfWork.LearningModuleRepository.Delete(learningModuleRequestsData);
                await this.unitOfWork.SaveChangesAsync();
                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Learning module - HTTP Delete call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error while deleting learning module details.");
                throw;
            }
        }

        /// <summary>
        /// Get details of a learning module by resource Id.
        /// </summary>
        /// <param name="id">Unique identifier of learning module which user wants to edit.</param>
        /// <returns>Returns a learning module and its associated resources.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLearningModuleDetailAsync(Guid id)
        {
            this.logger.LogInformation("Learning module resources - HTTP Get call initiated.");
            this.RecordEvent("Learning module - HTTP Get call initiated.", RequestType.Initiated);

            try
            {
                if (id == null || id == Guid.Empty)
                {
                    this.logger.LogError("learning module id is either null or empty guid.");
                    this.RecordEvent("Learning module - HTTP Get call failed", RequestType.Failed);

                    return this.BadRequest("Learning module id cannot be null or empty.");
                }

                var learningModule = await this.unitOfWork.LearningModuleRepository.GetAsync(id);
                if (learningModule == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"No record found for learning module id: {id}.");
                    this.RecordEvent("Learning module - HTTP Get call failed.", RequestType.Failed);

                    return this.NotFound("Learning module record not found");
                }

                var learningModuleVotes = await this.GetLearningModuleVotesAsync(id);

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = new string[] { learningModule.CreatedBy.ToString() };
                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var learningModuleViewModel = this.learningModuleMapper.MapToViewModel(learningModule, this.UserObjectId, learningModuleVotes, idToNameMap);
                var resources = await this.unitOfWork.ResourceModuleRepository.FindResourcesForModuleAsync(id);
                resources = resources.Reverse();
                var resourcesWithVote = this.unitOfWork.ResourceRepository.GetResourcesWithVotes(resources);
                var resourceViewModels = this.resourceMapper.MapToViewModels(resourcesWithVote, this.UserObjectId);

                var moduleResourceViewModel = new ModuleResourceViewModel
                {
                    LearningModule = learningModuleViewModel,
                    Resources = resourceViewModels,
                };

                this.RecordEvent($"Learning module - HTTP Get call succeeded", RequestType.Succeeded);
                return this.Ok(moduleResourceViewModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Learning module resource - HTTP Get call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching learning module resource details.");
                throw;
            }
        }

        /// <summary>
        /// Post call to store learning modules resource mapping details in storage.
        /// </summary>
        /// <param name="resourceLearningModuleDetail">Holds learning modules detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost("{id}/resources")]
        [Authorize(PolicyNames.MustBeTeacherOrAdminPolicy)]
        public async Task<IActionResult> PostResourceModuleMappingAsync(ResourceModuleViewModel resourceLearningModuleDetail)
        {
            this.RecordEvent("Resource module - HTTP post call initiated.", RequestType.Initiated);
            this.logger.LogInformation("Resource module - HTTP post call initiated.");

            try
            {
                var existingModuleResourceMapping =
                    await this.unitOfWork.ResourceModuleRepository
                    .FindAsync(resourceModuleMapping => resourceModuleMapping.ResourceId == resourceLearningModuleDetail.ResourceId && resourceModuleMapping.LearningModuleId == resourceLearningModuleDetail.LearningModuleId);
                if (existingModuleResourceMapping.Any())
                {
                    this.RecordEvent("Resource module - HTTP post call failed.", RequestType.Failed);
                    return this.Conflict();
                }

                var resourceLearningModuleEntityModel = this.resourceModuleMapper.MapToDTO(resourceLearningModuleDetail);
                this.unitOfWork.ResourceModuleRepository.Add(resourceLearningModuleEntityModel);
                await this.unitOfWork.SaveChangesAsync();

                this.RecordEvent("Resource module - HTTP post call succeeded.", RequestType.Succeeded);
                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Resource module - HTTP post call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error while saving resource module mapping details.");
                throw;
            }
        }

        /// <summary>
        /// Post call to store learning module vote details in database.
        /// </summary>
        /// <param name="id">Learning module id for which vote  details needs to be stored in storage.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        [Route("{id}/upvote")]
        public async Task<IActionResult> PostVoteAsync(Guid id)
        {
            this.logger.LogInformation("Learning module vote - HTTP Post call initiated.");
            this.RecordEvent("Learning module vote - HTTP Post call initiated.", RequestType.Initiated);

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"Learning Module Id is either null or empty guid for userId: {this.UserObjectId}");
                this.RecordEvent("Learning module vote - HTTP Post call failed.", RequestType.Failed);
                return this.BadRequest("Learning Module Id cannot be null or empty.");
            }

            try
            {
                var learningModuleVotes = await this.unitOfWork.LearningModuleVoteRepository.FindAsync(moduleVote => moduleVote.UserId == this.UserObjectId && moduleVote.ModuleId == id);
                if (learningModuleVotes.Any())
                {
                    this.logger.LogError(StatusCodes.Status409Conflict, $"Learning module vote already exists for learning module Id: {id} and user Id: {this.UserObjectId}.");
                    this.RecordEvent("Learning module vote - HTTP Post call failed.", RequestType.Failed);
                    return this.Conflict($"Learning module vote already exists for learning module Id: {id} and user Id: {this.UserObjectId}.");
                }

                LearningModuleVote vote = new LearningModuleVote
                {
                    ModuleId = id,
                    UserId = this.UserObjectId,
                    CreatedOn = DateTime.UtcNow,
                };

                this.unitOfWork.LearningModuleVoteRepository.Add(vote);
                await this.unitOfWork.SaveChangesAsync();

                this.logger.LogInformation("Learning module vote - HTTP Post call succeeded.");
                this.RecordEvent("Learning module vote - HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Learning module vote - HTTP Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while saving vote details for userId: {this.UserObjectId}");
                throw;
            }
        }

        /// <summary>
        /// Deletes learning module vote details from the database.
        /// </summary>
        /// <param name="id">Learning Module id for which vote details needs to be deleted from storage.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the vote delete operation.</returns>
        [HttpPost]
        [Route("{id}/downvote")]
        public async Task<IActionResult> DeleteVoteAsync(Guid id)
        {
            this.logger.LogInformation("Learning module vote delete- HTTP Post call initiated.");
            this.RecordEvent("Learning module vote delete- HTTP Post call initiated.", RequestType.Initiated);

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"Learning module Id is either null or empty guid for userId: {this.UserObjectId}");
                this.RecordEvent("Learning module vote delete- HTTP Post call failed.", RequestType.Failed);
                this.BadRequest("Learning Module Id cannot be null or empty.");
            }

            try
            {
                var voteDetails = await this.unitOfWork.LearningModuleVoteRepository.FindAsync(vote => vote.UserId == this.UserObjectId && vote.ModuleId == id);

                if (!voteDetails.Any())
                {
                    this.RecordEvent("Learning module vote delete- HTTP Post call failed.", RequestType.Failed);
                    this.logger.LogError(StatusCodes.Status404NotFound, $"Learning module vote details not found for given module Id: {id} and userId: {this.UserObjectId}");
                    return this.Ok($"Learning module vote details not found for given module Id: {id} and userId: {this.UserObjectId}");
                }
                else
                {
                    this.unitOfWork.LearningModuleVoteRepository.Delete(voteDetails.First());
                    await this.unitOfWork.SaveChangesAsync();
                }

                this.logger.LogInformation("Learning module vote delete- HTTP Post call succeeded.");
                this.RecordEvent("Learning module vote delete- HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (Exception ex)
            {
                this.RecordEvent($"Learning module vote delete- HTTP Post call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while deleting vote details for userId: {this.UserObjectId}");
                throw;
            }
        }

        /// <summary>
        /// Fetch resources of given learning module.
        /// </summary>
        /// <param name="id">Learning module unique identifier.</param>
        /// <returns>List of resources associated with given learning module id.</returns>
        [HttpGet]
        [Route("{id}/resources")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            this.logger.LogInformation("Learning module resources- HTTP Get call initiated.");
            this.RecordEvent("Learning module resources- HTTP Get call initiated.", RequestType.Initiated);

            if (id == null || id == Guid.Empty)
            {
                this.logger.LogError($"Learning module Id is either null or empty guid.");
                this.RecordEvent("Learning module resources- HTTP Get call failed.", RequestType.Failed);
                this.BadRequest("Learning module id cannot be null or empty.");
            }

            try
            {
                var resourceDetailsEntity = await this.unitOfWork.ResourceModuleRepository.FindResourcesForModuleAsync(id);
                var resourcesWithVote = this.unitOfWork.ResourceRepository.GetResourcesWithVotes(resourceDetailsEntity);

                // Get userId and user display name.
                IEnumerable<string> userAADObjectIds = resourceDetailsEntity.Select(resource => resource.CreatedBy.ToString()).Distinct();
                var idToNameMap = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var resourceDetails = this.resourceMapper.MapToViewModels(
                    resourcesWithVote,
                    this.UserObjectId,
                    idToNameMap);

                this.logger.LogInformation("Learning module resources- HTTP Get call succeeded.");
                this.RecordEvent("Learning module resources- HTTP Get call succeeded.", RequestType.Succeeded);

                return this.Ok(resourceDetails);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while learning module resources for learning module id: {id} and userId: {this.UserObjectId}.");
                this.RecordEvent("Learning module resources- HTTP Get call failed.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Get learning module creator user names.
        /// </summary>
        /// <param name="recordCount">Maximum number of records that needs to be fetched</param>
        /// <returns>Returns unique user names.</returns>
        [HttpGet("authors")]
        public async Task<IActionResult> GetUniqueUserNamesAsync(int recordCount)
        {
            try
            {
                this.RecordEvent("GetUniqueUserNamesAsync learning module- HTTP Get call Initiated.", RequestType.Initiated);
                this.logger.LogInformation("GetUniqueUserNamesAsync learning module- HTTP Get call Initiated.");

                var createdByObjectIds = await this.unitOfWork.LearningModuleRepository.GetCreatedByObjectIdsAsync(recordCount);

                var usernames = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), createdByObjectIds.Select(userId => userId.ToString()));
                var creatorDetails = usernames.Select(k => new UserDetail() { UserId = k.Key, DisplayName = k.Value });
                this.logger.LogInformation("GetUniqueUserNamesAsync learning module- HTTP Get call succeeded.");
                this.RecordEvent("GetUniqueUserNamesAsync learning module- HTTP Get call succeeded", RequestType.Succeeded);
                return this.Ok(creatorDetails);
            }
            catch (Exception ex)
            {
                this.RecordEvent("GetUniqueUserNamesAsync learning module- HTTP Get call failed", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching unique user names.");
                throw;
            }
        }

        /// <summary>
        /// Method to get votes for specified learning module using its id.
        /// </summary>
        /// <param name="learningModuleId">id of learning module for which vote needs to be obtained.</param>
        /// <returns>Returns votes for a specified learning module.</returns>
        private async Task<IEnumerable<LearningModuleVote>> GetLearningModuleVotesAsync(Guid learningModuleId)
        {
            this.logger.LogInformation("GetLearningModuleVotesAsync- call initiated.");
            this.RecordEvent("GetLearningModuleVotesAsync - call initiated.", RequestType.Initiated);

            var learningModuleVotes = await this.unitOfWork.LearningModuleVoteRepository.FindAsync(vote => vote.ModuleId == learningModuleId);

            this.logger.LogInformation("GetLearningModuleVotesAsync - call succeeded.");
            this.RecordEvent("GetLearningModuleVotesAsync - call succeeded.", RequestType.Succeeded);
            return learningModuleVotes;
        }

        /// <summary>
        /// Method to get resource count for specified learning module using its id.
        /// </summary>
        /// <param name="learningModuleId">Id of learning module for which resource count needs to be obtained.</param>
        /// <returns>Returns resource count for a specified learning module.</returns>
        private async Task<int> GetLearningModuleResourcesAsync(Guid learningModuleId)
        {
            var resourceModuleMapping = await this.unitOfWork.ResourceModuleRepository.FindAsync(resourceModule => resourceModule.LearningModuleId == learningModuleId);
            return resourceModuleMapping.Count();
        }
    }
}