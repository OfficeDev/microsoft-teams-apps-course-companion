// <copyright file="MemberValidationController.cs" company="Microsoft Corporation">
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
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;

    /// <summary>
    /// Controller to handle API operation for security group members.
    /// </summary>
    [Route("api/groupmember")]
    [ApiController]
    [Authorize]
    public class MemberValidationController : BaseController
    {
        /// <summary>
        /// Sends logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<MemberValidationController> logger;

        /// <summary>
        /// Instance of MemberValidationService to validate member.
        /// </summary>
        private readonly IMemberValidationService memberValidationService;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private readonly IOptions<SecurityGroupSettings> securityGroupOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberValidationController"/> class.
        /// </summary>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="securityGroupOptions">Security group configuration settings.</param>
        /// <param name="memberValidationService">Instance of MemberValidationService to validate member of a security group.</param>
        public MemberValidationController(
             TelemetryClient telemetryClient,
             ILogger<MemberValidationController> logger,
             IMemberValidationService memberValidationService,
             IOptions<SecurityGroupSettings> securityGroupOptions)
            : base(telemetryClient)
        {
            this.logger = logger;
            this.memberValidationService = memberValidationService;
            this.securityGroupOptions = securityGroupOptions;
        }

        /// <summary>
        /// Validate if user is a member of teachers or administrators security group.
        /// </summary>
        /// <returns>Returns whether current logged-in user is a part of security group or not to check if user is a administrator, teacher or student.</returns>
        [HttpGet]
        public async Task<IActionResult> ValidateIfUserIsMemberOfSecurityGroupAsync()
        {
            this.RecordEvent("ValidateIfUserIsMemberOfSecurityGroupAsync - HTTP Get call initiated.", RequestType.Initiated);
            try
            {
                var userRoleDetails = new UserRole
                {
                    IsTeacher = await this.memberValidationService.ValidateMemberAsync(
                        this.UserObjectId.ToString(),
                        this.securityGroupOptions.Value.TeacherSecurityGroupId,
                        this.Request.Headers["Authorization"].ToString()),

                    IsAdmin = await this.memberValidationService.ValidateMemberAsync(
                        this.UserObjectId.ToString(),
                        this.securityGroupOptions.Value.AdminGroupId,
                        this.Request.Headers["Authorization"].ToString()),
                };

                this.RecordEvent("ValidateIfUserIsMemberOfSecurityGroupAsync - HTTP Get call succeeded.", RequestType.Succeeded);
                return this.Ok(userRoleDetails);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while validating if user is member of security group.");
                this.RecordEvent("ValidateIfUserIsMemberOfSecurityGroupAsync - HTTP Get call failed.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Validate if user is a member of moderator security group.
        /// </summary>
        /// <returns>Returns whether current logged-in user is a part of moderator security group.</returns>
        [HttpGet("moderator")]
        public async Task<IActionResult> ValidateIfUserIsModeratorAsync()
        {
            this.RecordEvent("ValidateIfUserIsModeratorAsync - HTTP Get call initiated.", RequestType.Initiated);
            try
            {
                var isModerator = await this.memberValidationService.ValidateMemberAsync(
                        this.UserObjectId.ToString(),
                        this.securityGroupOptions.Value.ModeratorGroupId,
                        this.Request.Headers["Authorization"].ToString());
                this.RecordEvent("ValidateIfUserIsModeratorAsync - HTTP Get call succeeded.", RequestType.Succeeded);

                return this.Ok(isModerator);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while validating if user is member of moderator security group.");
                this.RecordEvent("ValidateIfUserIsModeratorAsync - HTTP Get call failed.", RequestType.Failed);
                throw;
            }
        }
    }
}