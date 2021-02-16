// <copyright file="FakeAccessTokenHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Fakes
{
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication;

    /// <summary>
    /// Fake access token helper class to provide fake tokens
    /// </summary>
    public class FakeAccessTokenHelper : ITokenHelper
    {
        /// <summary>
        /// Returns a fake access token for testing purpose only.
        /// </summary>
        /// <param name="userObjectId">user object id</param>
        /// <param name="authorizationHeader">authorization header value</param>
        /// <returns>fake user access token</returns>
        public Task<string> GetAccessTokenAsync(string userObjectId, string authorizationHeader)
        {
            return Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        }
    }
}
