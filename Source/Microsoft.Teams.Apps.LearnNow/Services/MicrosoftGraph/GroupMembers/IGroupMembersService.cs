// <copyright file="IGroupMembersService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Group Members Service.
    /// </summary>
    public interface IGroupMembersService
    {
        /// <summary>
        /// Get security group member.
        /// </summary>
        /// <param name="groupId">Group id of the security group to find and get member id.</param>
        /// <param name="userAadObjectId">Azure Active Directory user object id</param>
        /// <param name="accessToken">access token</param>
        /// <returns>A task that returns true of user exists in security group otherwise false.</returns>
        Task<bool> GetGroupMemberAsync(string groupId, string userAadObjectId, string accessToken);
    }
}