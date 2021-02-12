// <copyright file="IUserLearningModuleMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Interface for handling operations related to user learning module model mappings.
    /// </summary>
    public interface IUserLearningModuleMapper
    {
        /// <summary>
        /// Gets user learning module model from view model.
        /// </summary>
        /// <param name="userLearningModuleViewModel">UserLearningModule entity view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a user learning module entity model object.</returns>
        public UserLearningModule CreateMap(
            UserLearningModuleViewModel userLearningModuleViewModel,
            Guid userAadObjectId);
    }
}