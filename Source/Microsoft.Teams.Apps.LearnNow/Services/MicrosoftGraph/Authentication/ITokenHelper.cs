// <copyright file="ITokenHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to provide methods for handling user access tokens.
    /// </summary>
    public interface ITokenHelper
    {
        /// <summary>
        /// Get token from access token provider based on user id and authorization header values.
        /// </summary>
        /// <param name="userObjectId">Azure AD user id of the signed in user</param>
        /// <param name="authorizationHeader">Authorization header value. Usually this value will be provided in HTTP context of signed in user.</param>
        /// <returns>Access token</returns>
        Task<string> GetAccessTokenAsync(string userObjectId, string authorizationHeader);
    }
}