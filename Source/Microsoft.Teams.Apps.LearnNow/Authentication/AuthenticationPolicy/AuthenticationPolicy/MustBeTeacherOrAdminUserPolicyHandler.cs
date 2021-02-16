// <copyright file="MustBeTeacherOrAdminUserPolicyHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy.AuthenticationPolicy
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;

    /// <summary>
    /// This authorization handler is created to validate whether user is member/owner of security group.
    /// The class implements AuthorizationHandler for handling MustBeMemberOfSecurityGroupPolicyRequirement authorization.
    /// </summary>
    public class MustBeTeacherOrAdminUserPolicyHandler : IAuthorizationHandler
    {
        /// <summary>
        /// Instance of MemberValidationService to validate member.
        /// </summary>
        private readonly IMemberValidationService memberValidationService;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private readonly IOptions<SecurityGroupSettings> securityGroupOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MustBeTeacherOrAdminUserPolicyHandler"/> class.
        /// </summary>
        /// <param name="memberValidationService">Instance of member validation service to validate whether is valid user,</param>
        /// <param name="securityGroupOptions">Security group configuration settings.</param>
        public MustBeTeacherOrAdminUserPolicyHandler(IMemberValidationService memberValidationService, IOptions<SecurityGroupSettings> securityGroupOptions)
        {
            this.memberValidationService = memberValidationService;
            this.securityGroupOptions = securityGroupOptions;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            string teamId = string.Empty;
            var oidClaimType = Constants.OidClaimType;

            var oidClaim = context.User.Claims.FirstOrDefault(p => oidClaimType == p.Type);

            foreach (var requirement in context.Requirements)
            {
                if (requirement is MustBeTeacherOrAdminUserPolicyRequirement)
                {
                    if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
                    {
                        // Wrap the request stream so that we can rewind it back to the start for regular request processing.
                        authorizationFilterContext.HttpContext.Request.EnableBuffering();

                        var isUserPartOfTeachersGroup = await this.memberValidationService.ValidateMemberAsync(oidClaim.Value, this.securityGroupOptions.Value.TeacherSecurityGroupId, authorizationFilterContext.HttpContext.Request.Headers["Authorization"].ToString());
                        if (isUserPartOfTeachersGroup)
                        {
                            context.Succeed(requirement);
                        }

                        // Check whether user is part of Administrator group or not. Administrator has access to edit and delete other teachers content.
                        var isUserPartOfAdminGroup = await this.memberValidationService.ValidateMemberAsync(oidClaim.Value, this.securityGroupOptions.Value.AdminGroupId, authorizationFilterContext.HttpContext.Request.Headers["Authorization"].ToString());
                        if (isUserPartOfAdminGroup)
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }
    }
}