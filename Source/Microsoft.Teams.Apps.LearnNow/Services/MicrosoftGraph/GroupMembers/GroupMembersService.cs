// <copyright file="GroupMembersService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Group Members Service.
    /// This gets the groups members.
    /// </summary>
    public class GroupMembersService : IGroupMembersService
    {
        /// <summary>
        /// Constant value for max group members count to fetch in request.
        /// </summary>
        private const int TopGroupMembersCount = 999;

        /// <summary>
        /// Constant value for total number of retries for members request.
        /// </summary>
        private const int RetryCount = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMembersService"/> class.
        /// </summary>
        public GroupMembersService()
        {
        }

        /// <summary>
        /// Get security group member.
        /// </summary>
        /// <param name="groupId">Group id of the security group to find and get member id.</param>
        /// <param name="userAadObjectId">Azure Active Directory user object id</param>
        /// <param name="accessToken">User authorization access token</param>
        /// <returns>A task that returns true of user exists in security group otherwise false.</returns>
        public async Task<bool> GetGroupMemberAsync(string groupId, string userAadObjectId, string accessToken)
        {
            var graphClient = new GraphServiceClient(
            new DelegateAuthenticationProvider(
            requestMessage =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                Common.Constants.BearerAuthorizationScheme,
                accessToken);
                return Task.CompletedTask;
            }));

            var response = await graphClient
                .Groups[groupId]
                .TransitiveMembers
                .Request()
                .Top(TopGroupMembersCount)
                .WithMaxRetry(RetryCount)
                .GetAsync();

            var users = response.OfType<User>().ToList();
            while (response.NextPageRequest != null)
            {
                response = await response.NextPageRequest.GetAsync();
                users?.AddRange(response.OfType<User>() ?? new List<User>());
            }

            return users.Any(user => user.Id == userAadObjectId);
        }
    }
}