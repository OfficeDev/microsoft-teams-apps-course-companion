// <copyright file="MustBeTeamMemberUserPolicyRequirement.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// This authorization class implements the marker interface
    /// <see cref="IAuthorizationRequirement"/> to check if user meets teams member specific requirements
    /// for accessing resources.
    /// </summary>
    public class MustBeTeamMemberUserPolicyRequirement : IAuthorizationRequirement
    {
    }
}
