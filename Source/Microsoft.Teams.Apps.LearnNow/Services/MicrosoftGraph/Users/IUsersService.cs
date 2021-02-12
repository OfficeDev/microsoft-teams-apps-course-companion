// <copyright file="IUsersService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Get the User data.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Get users information from Graph API.
        /// </summary>
        /// <param name="loggedInUserObjectId">Azure AD user id of the signed in user</param>
        /// <param name="authorizationHeader">Authorization header value. Usually this value will be provided in HTTP context of signed in user.</param>
        /// <param name="userObjectIds">Collection of AAD Object ids of users.</param>
        /// <returns>Returns user id and name key value pairs.</returns>
        Task<Dictionary<Guid, string>> GetUserDisplayNamesAsync(string loggedInUserObjectId, string authorizationHeader, IEnumerable<string> userObjectIds);
    }
}
