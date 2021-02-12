// <copyright file="IMemberValidationService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for member validation Service.
    /// </summary>
    public interface IMemberValidationService
    {
        /// <summary>
        /// Method to validate whether current user is a valid member of provided group.
        /// </summary>
        /// <param name="userAadObjectId">Azure Active Directory id of current user.</param>
        /// <param name="groupId">Group id on which passed user object needs to be validated.</param>
        /// <param name="authorizationHeaders">HttpRequest authorization headers.</param>
        /// <returns>Returns true if current user is a valid member of provided group.</returns>
        Task<bool> ValidateMemberAsync(string userAadObjectId, string groupId, string authorizationHeaders);
    }
}