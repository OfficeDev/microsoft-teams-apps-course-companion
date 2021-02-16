// <copyright file="ILearningModuleMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Interface for handling operations related to model mappings.
    /// </summary>
    public interface ILearningModuleMapper
    {
        /// <summary>
        /// Gets learning module entity model from view model.
        /// </summary>
        /// <param name="learningModuleViewModel">LearningModule view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a learning module entity model</returns>
        public LearningModule MapToDTO(
            LearningModuleViewModel learningModuleViewModel,
            Guid userAadObjectId);

        /// <summary>
        /// Gets learning module view model from entity model.
        /// </summary>
        /// <param name="learningModule">Learning module entity model object.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a learning module view model object.</returns>
        public LearningModuleViewModel MapToViewModel(
            LearningModule learningModule,
            Dictionary<Guid, string> idToNameMap);

        /// <summary>
        /// Gets learning module entity model from view model.
        /// </summary>
        /// <param name="learningModuleViewModel">LearningModule view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a learning module entity model</returns>
        public LearningModule PatchAndMapToDTO(
            LearningModuleViewModel learningModuleViewModel,
            Guid userAadObjectId);

        /// <summary>
        /// Gets learning module view model from entity model.
        /// </summary>
        /// <param name="learningModule">Learning module entity model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="learningModuleVotes">List of learning module votes.</param>
        /// <param name="resourceCount">Count of learning module resources</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a learning module view model object.</returns>
        public LearningModuleViewModel PatchAndMapToViewModel(
            LearningModule learningModule,
            Guid userAadObjectId,
            IEnumerable<LearningModuleVote> learningModuleVotes,
            int resourceCount,
            Dictionary<Guid, string> idToNameMap);

        /// <summary>
        /// Gets learning module view model from entity model.
        /// </summary>
        /// <param name="learningModule">Learning module entity model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="learningModuleVotes">List of learning module votes.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a learning module view model object.</returns>
        public LearningModuleViewModel MapToViewModel(
            LearningModule learningModule,
            Guid userAadObjectId,
            IEnumerable<LearningModuleVote> learningModuleVotes,
            Dictionary<Guid, string> idToNameMap);

        /// <summary>
        /// Gets learning module view models from entity models.
        /// </summary>
        /// <param name="moduleWithVotesAndResources">Learning module entity object collection.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a collection of learning module view models.</returns>
        public IEnumerable<LearningModuleViewModel> MapToViewModels(
            Dictionary<Guid, List<LearningModuleDetailModel>> moduleWithVotesAndResources,
            Guid userAadObjectId,
            Dictionary<Guid, string> idToNameMap);
    }
}