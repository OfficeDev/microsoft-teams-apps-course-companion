// <copyright file="ILearningModuleTagRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling operations related to learning module tag entity collection.
    /// </summary>
    public interface ILearningModuleTagRepository : IRepository<LearningModuleTag>
    {
        /// <summary>
        /// Gets repository for handling operations on Subject entity.
        /// </summary>
        /// <param name="learningModuleTagsCollection"> List of learning module tags that needs to be deleted.</param>
        void DeleteLearningModuleTag(IEnumerable<LearningModuleTag> learningModuleTagsCollection);

        /// <summary>
        /// Gets repository for handling operations on Tag entity.
        /// </summary>
        /// <param name="learningModuleTagsCollection"> List of learning module tags that needs to be added.</param>
        void AddLearningModuleTag(IEnumerable<LearningModuleTag> learningModuleTagsCollection);
    }
}