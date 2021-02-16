// <copyright file="IUserResourceMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Interface for handling operations related to user resource model mappings.
    /// </summary>
    public interface IUserResourceMapper
    {
        /// <summary>
        /// Gets user resource model from view model.
        /// </summary>
        /// <param name="userResourceViewModel">UserResource entity view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a user resource entity model object.</returns>
        public UserResource CreateMap(
            UserResourceViewModel userResourceViewModel,
            Guid userAadObjectId);
    }
}