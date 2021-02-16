// <copyright file="SubjectController.cs" company="Microsoft Corporation">
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
    /// Controller to handle subject API operations.
    /// </summary>
    [Route("api/subject")]
    [ApiController]
    [Authorize]
    public class SubjectController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<SubjectController> logger;

        /// <summary>
        /// Instance for handling common operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The instance of subject mapper class to work with subject models.
        /// </summary>
        private readonly ISubjectMapper subjectMapper;

        /// <summary>
        /// Instance of user service to get user data.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">Subject repository for working with subject data.</param>
        /// <param name="subjectMapper">The instance of subject mapper class to work with models.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        public SubjectController(
            ILogger<SubjectController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            ISubjectMapper subjectMapper,
            IUsersService usersService)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.subjectMapper = subjectMapper;
            this.usersService = usersService;
        }

        /// <summary>
        /// Get subject data for subject id from storage.
        /// </summary>
        /// <param name="id">User selected subject id.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            this.logger.LogInformation($"Get call initiated for subject id: {id} ");
            this.RecordEvent("Subject - HTTP Get call initiated.", RequestType.Initiated);

            try
            {
                var subject = await this.unitOfWork.SubjectRepository.GetAsync(id);

                if (subject == null)
                {
                    this.logger.LogInformation($"No subject record found for subject id: {id}");
                    return this.NotFound();
                }

                this.RecordEvent("Subject - HTTP Get call succeeded.", RequestType.Succeeded);
                return this.Ok(subject);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Subject - HTTP Get call failed.", RequestType.Failed);
                this.logger.LogError(ex, $"Error while fetching subject details");
                throw;
            }
        }

        /// <summary>
        /// Get call to fetch all the subjects from the storage.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            this.logger.LogInformation("Get call initiated for getting all subjects");
            this.RecordEvent("Subject - HTTP Get call  initiated for all subjects", RequestType.Initiated);

            try
            {
                var subjectDetails = await this.unitOfWork.SubjectRepository.GetAllAsync();

                // Get userId and user display name.
                var userAADObjectIds = subjectDetails.Select(subject => subject.UpdatedBy).Distinct().Select(userObjectId => userObjectId.ToString());
                var userDetails = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var subjects = this.subjectMapper.MapToViewModel(subjectDetails, userDetails);
                this.RecordEvent("Subject - HTTP Get call succeeded for all subjects data", RequestType.Succeeded);

                return this.Ok(subjects);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Subject - HTTP Get call failed for getting subjects data", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching all subject details");
                throw;
            }
        }

        /// <summary>
        /// Post call to store subject details in storage.
        /// </summary>
        /// <param name="subjectDetail">Holds subject detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> PostAsync([FromBody] SubjectViewModel subjectDetail)
        {
            if (subjectDetail == null)
            {
                return this.BadRequest("Error while saving subject details to storage.");
            }

            this.RecordEvent("Subject - HTTP Post call to add subject details.", RequestType.Initiated);
            this.logger.LogInformation("Call to add subject details.");

            if (subjectDetail == null)
            {
                return this.BadRequest("Error while saving subject details to storage.");
            }

            try
            {
                var subjects = await this.unitOfWork.SubjectRepository.FindAsync(subject => subject.SubjectName == subjectDetail.SubjectName);
                if (subjects.Any())
                {
                    this.logger.LogInformation($"Subject title already exists with subjectName :{subjectDetail.SubjectName}");
                    this.RecordEvent("Subject - HTTP Post call to add subject details.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status409Conflict);
                }

                var subjectEntityModel = this.subjectMapper.MapToDTO(
                    subjectDetail,
                    this.UserObjectId);

                this.unitOfWork.SubjectRepository.Add(subjectEntityModel);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Subject - HTTP Post call succeeded.", RequestType.Succeeded);
                return this.Ok(subjectEntityModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Subject - HTTP Post call failed for saving subject data", RequestType.Failed);
                this.logger.LogError(ex, "Error while saving subject details");
                throw;
            }
        }

        /// <summary>
        /// update the value in the database.
        /// </summary>
        /// <param name="id">Holds subject id for subject which needs to be updated.</param>
        /// <param name="subjectDetail">Holds subject detail entity data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] SubjectViewModel subjectDetail)
        {
            if (subjectDetail == null)
            {
                this.RecordEvent("Subject - HTTP Patch call failed.", RequestType.Failed);
                return this.BadRequest("SubjectDetails obtained is null. Error while updating subject details to storage.");
            }

            this.RecordEvent("Subject - HTTP Patch call initiated.", RequestType.Initiated);
            this.logger.LogInformation("Call to patch subject details.");

            try
            {
                if (subjectDetail.Id == null || subjectDetail.Id == Guid.Empty || subjectDetail.Id != id)
                {
                    this.logger.LogError($"Subject Id is either null, empty or does not match the id:{id}.");
                    this.RecordEvent("Subject - HTTP Patch call failed.", RequestType.Failed);

                    return this.BadRequest("Subject Id cannot be null or empty or it does not match the id");
                }

                var subjects = await this.unitOfWork.SubjectRepository.FindAsync(subject => subject.SubjectName == subjectDetail.SubjectName);
                if (subjects.Any())
                {
                    this.logger.LogInformation($"Subject title already exists with subjectName :{subjectDetail.SubjectName}");
                    this.RecordEvent("Subject - HTTP Post call to add subject details.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status409Conflict);
                }

                var existingSubject = await this.unitOfWork.SubjectRepository.GetAsync(id);
                if (existingSubject == null)
                {
                    this.logger.LogError("Subject - HTTP Patch call failed.");
                    this.RecordEvent("Subject - HTTP Patch call failed.", RequestType.Failed);

                    return this.NotFound($"No subject detail found for given subject Id: {id}.");
                }

                existingSubject.SubjectName = subjectDetail.SubjectName;
                existingSubject.UpdatedBy = this.UserObjectId;
                existingSubject.UpdatedOn = DateTimeOffset.Now;

                this.unitOfWork.SubjectRepository.Update(existingSubject);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Subject - HTTP Patch call succeeded.", RequestType.Succeeded);

                return this.Ok(existingSubject);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Subject - HTTP Patch call failed for saving subject data", RequestType.Failed);
                this.logger.LogError(ex, "Error while updating subject details");
                throw;
            }
        }

        /// <summary>
        /// Deletes subject details from the storage.
        /// </summary>
        /// <param name="subjectRequestsData">Holds subject detail entity data.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("subjectsdelete")]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> DeleteAsync([FromBody] List<AdminConfigBaseModel> subjectRequestsData)
        {
            this.RecordEvent("Subject - HTTP Delete call to delete subjects.", RequestType.Initiated);
            this.logger.LogInformation("Call to delete subject details.");

            try
            {
                if (EnumerableExtension.IsNullOrEmpty(subjectRequestsData))
                {
                    this.RecordEvent("No subject details data passed in request", RequestType.Failed);
                    this.logger.LogInformation("No subject details data passed in delete request");
                    return this.BadRequest("Delete request data should not be null");
                }

                IEnumerable<Subject> subjectCollection = new List<Subject>();
                subjectCollection = subjectRequestsData
                    .Select(subject => new Subject
                    {
                        Id = subject.Id,
                    });

                this.unitOfWork.SubjectRepository.DeleteSubjects(subjectCollection);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Subject - HTTP Delete call succeeded.", RequestType.Succeeded);

                return this.Ok(true);
            }
            catch (DbUpdateException ex)
            {
                var errorCode = ((SqlException)ex.InnerException).Number;
                return this.StatusCode(errorCode);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Subject - HTTP Delete call failed for saving subject data", RequestType.Failed);
                this.logger.LogError(ex, "Error while deleting subject details");
                throw;
            }
        }
    }
}