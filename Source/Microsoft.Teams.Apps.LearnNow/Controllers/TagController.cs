// <copyright file="TagController.cs" company="Microsoft Corporation">
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
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Helpers;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;

    /// <summary>
    /// Controller to handle tag API operations.
    /// </summary>
    [Route("api/tag")]
    [ApiController]
    [Authorize]
    public class TagController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<TagController> logger;

        /// <summary>
        /// Instance for handling common operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The instance of tag mapper class to work with tag models.
        /// </summary>
        private readonly ITagMapper tagMapper;

        /// <summary>
        /// Instance of user service to get user data.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">Tag repository for working with tag data.</param>
        /// <param name="tagMapper">The instance of tag mapper class to work with models.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        public TagController(
            ILogger<TagController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            ITagMapper tagMapper,
            IUsersService usersService)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.tagMapper = tagMapper;
            this.usersService = usersService;
        }

        /// <summary>
        /// Get tag detail for tag id from storage.
        /// </summary>
        /// <param name="id">User selected tag id.</param>
        /// <returns>Returns tag detail.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            this.logger.LogInformation($"Get call initiated for tag id: {id}");
            this.RecordEvent("Tags - HTTP Get call initiated.", RequestType.Initiated);

            try
            {
                var tag = await this.unitOfWork.TagRepository.GetAsync(id);

                if (tag == null)
                {
                    this.logger.LogInformation($"No tag record found for id: {id} ");
                    return this.NotFound();
                }

                this.RecordEvent("Tags - HTTP Get call succeeded.", RequestType.Succeeded);
                return this.Ok(tag);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Tags - HTTP Get call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while fetching tag details");
                throw;
            }
        }

        /// <summary>
        /// Get call to fetch all the tags from the storage.
        /// </summary>
        /// <returns>Returns tag collection.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            this.logger.LogInformation("Get call initiated for getting all tags");
            this.RecordEvent("Tags - HTTP Get call initiated for all tags", RequestType.Initiated);

            try
            {
                var tagDetails = await this.unitOfWork.TagRepository.GetAllAsync();

                // Get userId and user display name.
                var userAADObjectIds = tagDetails.Select(tag => tag.UpdatedBy).Distinct().Select(userObjectId => userObjectId.ToString());
                var userDetails = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var tags = this.tagMapper.MapToViewModel(tagDetails, userDetails);
                this.RecordEvent("Tags - HTTP Get call succeeded for all tags data", RequestType.Succeeded);

                return this.Ok(tags);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Tags - HTTP Get call failed for getting all tags data", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching all tag details");
                throw;
            }
        }

        /// <summary>
        /// Post call to store tag details in storage.
        /// </summary>
        /// <param name="tagDetail">Holds tag detail entity data.</param>
        /// <returns>Returns saved tag details.</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> PostAsync([FromBody] TagViewModel tagDetail)
        {
            this.RecordEvent("Tags - HTTP Post call Initiated.", RequestType.Initiated);
            this.logger.LogInformation("Call to add tag details.");

            if (tagDetail == null)
            {
                this.RecordEvent("Tags - HTTP Post call failed.", RequestType.Failed);
                return this.BadRequest("Error while saving tag details to storage.");
            }

            try
            {
                var tags = await this.unitOfWork.TagRepository.FindAsync(tag => tag.TagName == tagDetail.TagName);
                if (tags.Any())
                {
                    this.logger.LogInformation($"Tag title already exists with tagName :{tagDetail.TagName}");
                    this.RecordEvent("Tag - HTTP Post call to add tag details.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status409Conflict);
                }

                var tagEntityModel = this.tagMapper.MapToDTO(
                    tagDetail,
                    this.UserObjectId);

                this.unitOfWork.TagRepository.Add(tagEntityModel);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Tags - HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(tagEntityModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Tags - HTTP Post call failed for saving tag data", RequestType.Failed);
                this.logger.LogError(ex, "Error while saving tag details");
                throw;
            }
        }

        /// <summary>
        /// Patch call to update tag detail in the database.
        /// </summary>
        /// <param name="id">Holds tag id for tag which needs to be updated.</param>
        /// <param name="tagDetail">Holds tag detail entity data.</param>
        /// <returns>Returns updated tag details.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] TagViewModel tagDetail)
        {
            this.RecordEvent("Tags - HTTP Patch call to update tag details.", RequestType.Initiated);
            this.logger.LogInformation("Call to patch tag details.");
            try
            {
                if (tagDetail == null)
                {
                    this.RecordEvent("Tags - HTTP Patch call failed.", RequestType.Failed);
                    return this.BadRequest("Tag details obtained is null. Error while updating tag details to storage.");
                }

                if (id == null || id == Guid.Empty || tagDetail.Id != id)
                {
                    this.logger.LogError($"Tag Id is either null, empty guid or does not match the id:{id}.");
                    this.RecordEvent("Tags - HTTP Patch call failed.", RequestType.Failed);

                    return this.BadRequest("Tag Id cannot be null or empty or does not match the tag id.");
                }

                var tags = await this.unitOfWork.TagRepository.FindAsync(tag => tag.TagName == tagDetail.TagName);
                if (tags.Any())
                {
                    this.logger.LogInformation($"Tag title already exists with tagName :{tagDetail.TagName}");
                    this.RecordEvent("Tag - HTTP Post call to add tag details.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status409Conflict);
                }

                var existingTag = await this.unitOfWork.TagRepository.GetAsync(id);
                if (existingTag == null)
                {
                    this.logger.LogError("Tag - HTTP Patch call failed.");
                    this.RecordEvent("Tag - HTTP Patch call failed.", RequestType.Failed);

                    return this.NotFound($"No tag detail found for given tag Id: {id}.");
                }

                existingTag.TagName = tagDetail.TagName;
                existingTag.UpdatedBy = this.UserObjectId;
                existingTag.UpdatedOn = DateTimeOffset.Now;

                this.unitOfWork.TagRepository.Update(existingTag);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Tags - HTTP Patch call succeeded.", RequestType.Succeeded);

                return this.Ok(existingTag);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Tags - HTTP Patch call failed for saving tag data", RequestType.Failed);
                this.logger.LogError(ex, "Error while updating tag details");
                throw;
            }
        }

        /// <summary>
        /// Deletes tag details from the storage.
        /// </summary>
        /// <param name="tagRequestsData">Holds tag detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost("tagsdelete")]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> DeleteAsync([FromBody] List<AdminConfigBaseModel> tagRequestsData)
        {
            this.RecordEvent("Tags - HTTP Delete call to delete tags initiated.", RequestType.Initiated);
            this.logger.LogInformation("Call to delete tag details.");

            try
            {
                if (EnumerableExtension.IsNullOrEmpty(tagRequestsData))
                {
                    this.RecordEvent("Tags - HTTP Delete, no tag details data passed in request.", RequestType.Failed);
                    this.logger.LogInformation("No tag details data passed in delete request.");
                    return this.BadRequest("Delete request data should not be null");
                }

                IEnumerable<Tag> tagsCollection = new List<Tag>();
                tagsCollection = tagRequestsData
                    .Select(tag => new Tag
                    {
                        Id = tag.Id,
                    });

                this.unitOfWork.TagRepository.DeleteTags(tagsCollection);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Tags - HTTP Delete call succeeded.", RequestType.Succeeded);
                return this.Ok(true);
            }
            catch (DbUpdateException ex)
            {
                var errorCode = ((SqlException)ex.InnerException).Number;
                return this.StatusCode(errorCode);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Tags - HTTP Delete call failed for deleting tag data", RequestType.Failed);
                this.logger.LogError(ex, "Error while deleting tag details");
                throw;
            }
        }
    }
}