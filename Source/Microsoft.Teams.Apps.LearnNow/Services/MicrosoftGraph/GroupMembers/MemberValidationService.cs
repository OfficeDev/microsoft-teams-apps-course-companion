// <copyright file="MemberValidationService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication;

    /// <summary>
    /// Class handles methods to validate member of a security group.
    /// </summary>
    public class MemberValidationService : IMemberValidationService
    {
        /// <summary>
        /// Sends logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<MemberValidationService> logger;

        /// <summary>
        /// Instance of access token helper to get valid token to access Microsoft Graph.
        /// </summary>
        private readonly ITokenHelper accessTokenHelper;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private readonly IGroupMembersService groupMembersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberValidationService"/> class.
        /// </summary>
        /// <param name="accessTokenHelper">Instance of access token helper to get valid token to access Microsoft Graph</param>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="groupMembersService">Instance group member service.</param>
        public MemberValidationService(
            ITokenHelper accessTokenHelper,
            ILogger<MemberValidationService> logger,
            IGroupMembersService groupMembersService)
        {
            this.accessTokenHelper = accessTokenHelper;
            this.logger = logger;
            this.groupMembersService = groupMembersService;
        }

        /// <summary>
        /// Method to validate whether current user is a member of teacher's security group.
        /// </summary>
        /// <param name="userAadObjectId">Azure Active Directory id of current user.</param>
        /// <param name="groupId">Group id.</param>
        /// <param name="authorizationHeaders">HttpRequest authorization headers.</param>
        /// <returns>Returns true if current user is a member of teacher's security group.</returns>
        public async Task<bool> ValidateMemberAsync(string userAadObjectId, string groupId, string authorizationHeaders)
        {
            var accessToken = await this.accessTokenHelper.GetAccessTokenAsync(userAadObjectId, authorizationHeaders);
            if (string.IsNullOrEmpty(accessToken))
            {
                this.logger.LogError("Token to access graph API is null.");
                return false;
            }

            // Check whether current user is a member of teacher's security group.
            return await this.groupMembersService.GetGroupMemberAsync(groupId, userAadObjectId, accessToken);
        }
    }
}