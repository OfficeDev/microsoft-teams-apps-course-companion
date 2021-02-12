// <copyright file="PolicyNames.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy
{
    /// <summary>
    /// This class list the policy name of custom authorizations implemented in project.
    /// </summary>
    public static class PolicyNames
    {
        /// <summary>
        /// The name of the authorization policy, MustBeTeamMemberUserPolicy.
        /// Indicates that user is a part of team and has permission to edit created project.
        /// </summary>
        public const string MustBeTeamMemberUserPolicy = "MustBeTeamMemberUserPolicy";

        /// <summary>
        /// The name of the authorization policy, MustBeTeamMemberUserPolicy.
        /// Indicates that user is a part of team and has permission to edit created project.
        /// </summary>
        public const string MustBeTeacherOrAdminPolicy = "MustBeTeacherOrAdminPolicy";

        /// <summary>
        /// The name of the authorization policy, MustBeModeratorPolicy.
        /// Indicates that user is a part of moderator team and has permission to manage grade, subject and tags.
        /// </summary>
        public const string MustBeModeratorPolicy = "MustBeModeratorPolicy";
    }
}
