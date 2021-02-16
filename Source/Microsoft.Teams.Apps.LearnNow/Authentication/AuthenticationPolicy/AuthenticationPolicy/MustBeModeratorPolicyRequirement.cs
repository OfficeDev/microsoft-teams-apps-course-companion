// <copyright file="MustBeModeratorPolicyRequirement.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy.AuthenticationPolicy
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// This authorization class implements the authorization requirement interface
    /// <see cref="IAuthorizationRequirement"/> to check if user meets moderator group specific requirements
    /// for managing grades, subjects and tags.
    /// </summary>
    public class MustBeModeratorPolicyRequirement : IAuthorizationRequirement
    {
    }
}
