// <copyright file="UsersService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.LearnNow.Helpers;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication;

    /// <summary>
    /// Implements the methods that are defined in <see cref="IUsersService"/>.
    /// </summary>
    public class UsersService : IUsersService
    {
        /// <summary>
        /// Instance of access token helper to get valid token to access Microsoft Graph.
        /// </summary>
        private readonly ITokenHelper accessTokenHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="accessTokenHelper">The instance of access token helper to get valid token to access Microsoft Graph</param>
        public UsersService(
            ITokenHelper accessTokenHelper)
        {
            this.accessTokenHelper = accessTokenHelper;
        }

        /// <summary>
        /// Get users information from Graph API.
        /// </summary>
        /// <param name="loggedInUserObjectId">Azure AD user id of the signed in user</param>
        /// <param name="authorizationHeader">Authorization header value. Usually this value will be provided in HTTP context of signed in user.</param>
        /// <param name="userObjectIds">Collection of AAD Object ids of users.</param>
        /// <returns>Returns user id and name key value pairs.</returns>
        public async Task<Dictionary<Guid, string>> GetUserDisplayNamesAsync(string loggedInUserObjectId, string authorizationHeader, IEnumerable<string> userObjectIds)
        {
            var accessToken = await this.accessTokenHelper.GetAccessTokenAsync(loggedInUserObjectId, authorizationHeader);

            var resourceOwnerDetails = await this.GetUserDetailsAsync(userObjectIds, accessToken);

            return resourceOwnerDetails.ToDictionary(k => Guid.Parse(k.Id), v => v.DisplayName);
        }

        /// <summary>
        /// Get users information from Graph API.
        /// </summary>
        /// <param name="userObjectIds">Collection of AAD Object ids of users.</param>
        /// <param name="accessToken">Access token.</param>
        /// <returns>A task that returns collection of user information.</returns>
        private async Task<IEnumerable<User>> GetUserDetailsAsync(IEnumerable<string> userObjectIds, string accessToken)
        {
            userObjectIds = userObjectIds ?? throw new ArgumentNullException(nameof(userObjectIds));
            var userDetails = new List<User>();
            var userObjectIdsBatch = userObjectIds.ToList()
                .SplitList();

            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    requestMessage =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                            Common.Constants.BearerAuthorizationScheme,
                            accessToken);
                        return Task.CompletedTask;
                    }));

            foreach (var userObjectIdBatch in userObjectIdsBatch)
            {
                var batchIds = new List<string>();
                var userDetailsBatch = new List<User>();

                using (var batchRequestContent = new BatchRequestContent())
                {
                    foreach (string userObjectId in userObjectIdBatch)
                    {
                        var request = graphClient
                            .Users[userObjectId]
                            .Request();

                        batchIds.Add(batchRequestContent.AddBatchRequestStep(request));
                    }

                    var response = await graphClient.Batch.Request().PostAsync(batchRequestContent);

                    foreach (var id in batchIds)
                    {
                        userDetailsBatch.Add(await response.GetResponseByIdAsync<User>(id));
                    }
                }

                userDetails.AddRange(userDetailsBatch);
            }

            return userDetails;
        }
    }
}