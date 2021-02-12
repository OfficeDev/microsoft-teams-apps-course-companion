// <copyright file="GradeController.cs" company="Microsoft Corporation">
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
    /// Controller to handle grade API operations.
    /// </summary>
    [Route("api/grade")]
    [ApiController]
    [Authorize]
    public class GradeController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger<GradeController> logger;

        /// <summary>
        /// Instance for handling common operations with entity collection.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The instance of grade mapper class to work with grade models.
        /// </summary>
        private readonly IGradeMapper gradeMapper;

        /// <summary>
        /// Instance of user service to get user data.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeController"/> class.
        /// </summary>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="unitOfWork">Grade repository for working with grade data.</param>
        /// <param name="gradeMapper">The instance of grade mapper class to work with models.</param>
        /// <param name="usersService">Instance of user service to get user data.</param>
        public GradeController(
            ILogger<GradeController> logger,
            TelemetryClient telemetryClient,
            IUnitOfWork unitOfWork,
            IGradeMapper gradeMapper,
            IUsersService usersService)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.gradeMapper = gradeMapper;
            this.usersService = usersService;
        }

        /// <summary>
        /// Get grade data for provided grade id.
        /// </summary>
        /// <param name="id">grade id by using which data to be fetched.</param>
        /// <returns>Returns grade detail data.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            this.logger.LogInformation($"Grade - HTTP Get call initiated for grade id: {id}");
            this.RecordEvent("Grade - HTTP Get call.", RequestType.Initiated);
            try
            {
                var grade = await this.unitOfWork.GradeRepository.GetAsync(id);
                if (grade == null)
                {
                    this.logger.LogInformation($"No grade record found for grade id: {id} ");
                    this.RecordEvent("Grade - No grade record found for grade.", RequestType.Failed);
                    return this.NotFound();
                }

                this.logger.LogInformation($"Grade - HTTP Get call succeeded for grade id: {id} ");
                this.RecordEvent("Grade - HTTP Get call succeeded.", RequestType.Succeeded);

                return this.Ok(grade);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Grade - HTTP Get call failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching grade details");
                throw;
            }
        }

        /// <summary>
        /// Get call to fetch all the grades from the storage.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the all grades result of the asynchronous operation.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            this.logger.LogInformation("Get call initiated for getting all grades");
            this.RecordEvent("Grade - HTTP Get call initiated for all grades.", RequestType.Initiated);
            try
            {
                var gradeDetails = await this.unitOfWork.GradeRepository.GetAllAsync();
                this.logger.LogInformation("Grade - HTTP Get call succeeded for all grades data.");

                // Get userId and user display name.
                var userAADObjectIds = gradeDetails.Select(grade => grade.UpdatedBy).Distinct().Select(userObjectId => userObjectId.ToString());
                var userDetails = await this.usersService.GetUserDisplayNamesAsync(this.UserObjectId.ToString(), this.Request.Headers["Authorization"].ToString(), userAADObjectIds);

                var grades = this.gradeMapper.MapToViewModel(gradeDetails, userDetails);
                this.RecordEvent("Grade - HTTP Get call succeeded for all grades data.", RequestType.Succeeded);

                return this.Ok(grades);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Grade - HTTP Get failed for getting all grades data.", RequestType.Failed);
                this.logger.LogError(ex, "Error while fetching all grade details");
                throw;
            }
        }

        /// <summary>
        /// Post call to store grade details in storage.
        /// </summary>
        /// <param name="gradeDetail">Holds grade detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> PostAsync([FromBody] GradeViewModel gradeDetail)
        {
            if (gradeDetail == null)
            {
                return this.BadRequest("Error while saving grade details to storage.");
            }

            this.RecordEvent("Grade - HTTP Post call to add grade details.", RequestType.Initiated);
            this.logger.LogInformation("Call to add grade details.");

            if (gradeDetail == null)
            {
                this.logger.LogInformation("Error while saving grade details to storage.");
                this.RecordEvent("Grade - HTTP Post call to add grade details.", RequestType.Failed);
                return this.BadRequest("Error while saving grade details to storage.");
            }

            try
            {
                var grades = await this.unitOfWork.GradeRepository.FindAsync(grade => grade.GradeName == gradeDetail.GradeName);
                if (grades.Any())
                {
                    this.logger.LogInformation($"Grade title already exists with gradeName :{gradeDetail.GradeName}");
                    this.RecordEvent("Grade - HTTP Post call to add grade details.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status409Conflict);
                }

                var gradeEntityModel = this.gradeMapper.MapToDTO(
                    gradeDetail,
                    this.UserObjectId);

                this.unitOfWork.GradeRepository.Add(gradeEntityModel);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Grade - HTTP Post call succeeded.", RequestType.Succeeded);

                return this.Ok(gradeEntityModel);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Grade - HTTP Post call failed for saving grade data", RequestType.Failed);
                this.logger.LogError(ex, $"Error while saving grade details");
                throw;
            }
        }

        /// <summary>
        /// Patch call to update grade details in storage.
        /// </summary>
        /// <param name="id">Holds grade id for grade which needs to be updated.</param>
        /// <param name="gradeDetail">Holds grade detail entity data.</param>
        /// <returns>Returns true for successful operation.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] GradeViewModel gradeDetail)
        {
            if (gradeDetail == null)
            {
                this.RecordEvent("Grade - HTTP Patch call failed.", RequestType.Failed);
                return this.BadRequest("Error while updating grade details to storage.");
            }

            this.RecordEvent("Grade - HTTP Patch call to update grade details for grade id {gradeDetail.Id}.", RequestType.Initiated);
            this.logger.LogInformation("Call to patch grade details.");

            try
            {
                if (gradeDetail.Id == null || gradeDetail.Id == Guid.Empty || gradeDetail.Id != id)
                {
                    this.logger.LogError($"Grade Id  is either null, empty or does not match the id: {id}.");
                    this.RecordEvent("Grade - HTTP Patch call failed.", RequestType.Failed);

                    return this.BadRequest("Grade Id cannot be null or empty.");
                }

                var grades = await this.unitOfWork.GradeRepository.FindAsync(grade => grade.GradeName == gradeDetail.GradeName);
                if (grades.Any())
                {
                    this.logger.LogInformation($"Grade title already exists with gradeName :{gradeDetail.GradeName}");
                    this.RecordEvent("Grade - HTTP Patch call to update grade details.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status409Conflict);
                }

                var existingGrade = await this.unitOfWork.GradeRepository.GetAsync(id);
                if (existingGrade == null)
                {
                    this.logger.LogError("Grade - HTTP Patch call failed.");
                    this.RecordEvent("Grade - HTTP Patch call failed.", RequestType.Failed);

                    return this.NotFound($"No grade detail found for given grade Id: {id}.");
                }

                existingGrade.GradeName = gradeDetail.GradeName;
                existingGrade.UpdatedBy = this.UserObjectId;
                existingGrade.UpdatedOn = DateTimeOffset.Now;

                this.unitOfWork.GradeRepository.Update(existingGrade);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Grade - HTTP Patch call succeeded.", RequestType.Succeeded);

                return this.Ok(existingGrade);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Grade - HTTP Patch call failed for updating grade data.", RequestType.Failed);
                this.logger.LogError(ex, "Error while updating grade details.");
                throw;
            }
        }

        /// <summary>
        /// Deletes grade details from the storage.
        /// </summary>
        /// <param name="gradeRequestsData">Holds grade detail entity data.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("gradesdelete")]
        [Authorize(PolicyNames.MustBeModeratorPolicy)]
        public async Task<IActionResult> DeleteAsync([FromBody] List<AdminConfigBaseModel> gradeRequestsData)
        {
            this.RecordEvent("Grade - HTTP Delete call to delete grades is initiated.", RequestType.Initiated);
            this.logger.LogInformation("Call to delete grade details.");
            try
            {
                if (EnumerableExtension.IsNullOrEmpty(gradeRequestsData))
                {
                    this.RecordEvent("Grade - No grade details data passed in request", RequestType.Failed);
                    this.logger.LogInformation("No grade details data passed in delete request");
                    return this.BadRequest("Delete request data should not be null");
                }

                var grades = gradeRequestsData
                    .Select(grade => new Grade
                    {
                        Id = grade.Id,
                    });

                this.unitOfWork.GradeRepository.DeleteGrades(grades);
                await this.unitOfWork.SaveChangesAsync();
                this.RecordEvent("Grade - HTTP Delete call succeeded.", RequestType.Succeeded);
                return this.Ok(true);
            }
            catch (DbUpdateException ex)
            {
                var errorCode = ((SqlException)ex.InnerException).Number;
                return this.StatusCode(errorCode);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Grade - HTTP Delete call failed for grade data", RequestType.Failed);
                this.logger.LogError(ex, "Error while deleting grade details");
                throw;
            }
        }
    }
}
