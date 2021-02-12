// <copyright file="IUnitOfWork.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for handling operations related to different entity collection.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets repository for handling operations on Grade entity.
        /// </summary>
        IGradeRepository GradeRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Subject entity.
        /// </summary>
        ISubjectRepository SubjectRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Tag entity.
        /// </summary>
        ITagRepository TagRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Tag entity.
        /// </summary>
        IResourceTagRepository ResourceTagRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Resource entity.
        /// </summary>
        IResourceRepository ResourceRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Vote entity for resource module.
        /// </summary>
        IResourceVoteRepository ResourceVoteRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Vote entity for learning module.
        /// </summary>
        ILearningModuleVoteRepository LearningModuleVoteRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Learning Module entity.
        /// </summary>
        ILearningModuleRepository LearningModuleRepository { get; }

        /// <summary>
        /// Gets repository instance for working with Resource Learning Module mapping entity.
        /// </summary>
        IResourceModuleRepository ResourceModuleRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Tab Configuration entity.
        /// </summary>
        ITabConfigurationRepository TabConfigurationRepository { get; }

        /// <summary>
        /// Gets repository instance for working with User Resource entity.
        /// </summary>
        IUserResourceRepository UserResourceRepository { get; }

        /// <summary>
        /// Gets repository instance for working with User Learning Module entity.
        /// </summary>
        IUserLearningModuleRepository UserLearningModuleRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on Learning Module Tag entity.
        /// </summary>
        ILearningModuleTagRepository LearningModuleTagRepository { get; }

        /// <summary>
        /// Gets repository for handling operations on User Setting entity.
        /// </summary>
        IUserSettingRepository UserSettingRepository { get; }

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing saving resource entity.</returns>
        Task SaveChangesAsync();
    }
}
