// <copyright file="LearningModuleMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// A model class that contains methods related to  learning module model mappings.
    /// </summary>
    public class LearningModuleMapper : ILearningModuleMapper
    {
        /// <summary>
        /// Gets  learning module entity model from view model.
        /// </summary>
        /// <param name="learningModuleViewModel"> Learning module view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a  learning module entity model</returns>
        public LearningModule MapToDTO(
           LearningModuleViewModel learningModuleViewModel,
           Guid userAadObjectId)
        {
            learningModuleViewModel = learningModuleViewModel ?? throw new ArgumentNullException(nameof(learningModuleViewModel));

            return new LearningModule
            {
                Id = learningModuleViewModel.Id,
                Title = learningModuleViewModel.Title,
                Description = learningModuleViewModel.Description,
                SubjectId = learningModuleViewModel.SubjectId,
                GradeId = learningModuleViewModel.GradeId,
                ImageUrl = learningModuleViewModel.ImageUrl,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedBy = userAadObjectId,
                UpdatedBy = userAadObjectId,
                LearningModuleTag = learningModuleViewModel.LearningModuleTag.ToList(),
            };
        }

        /// <summary>
        /// Gets  learning module view model from entity model.
        /// </summary>
        /// <param name="learningModule"> Learning module entity model object.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a  learning module view model object.</returns>
        public LearningModuleViewModel MapToViewModel(
            LearningModule learningModule,
            Dictionary<Guid, string> idToNameMap)
        {
            learningModule = learningModule ?? throw new ArgumentNullException(nameof(learningModule));
            idToNameMap = idToNameMap ?? throw new ArgumentNullException(nameof(idToNameMap));

            return new LearningModuleViewModel
            {
                Id = learningModule.Id,
                Title = learningModule.Title,
                Description = learningModule.Description,
                GradeId = learningModule.GradeId,
                SubjectId = learningModule.SubjectId,
                Subject = learningModule.Subject,
                Grade = learningModule.Grade,
                ImageUrl = learningModule.ImageUrl,
                CreatedBy = learningModule.CreatedBy,
                UpdatedBy = learningModule.UpdatedBy,
                CreatedOn = learningModule.CreatedOn,
                UpdatedOn = learningModule.UpdatedOn,
                LearningModuleTag = learningModule.LearningModuleTag,
                IsLikedByUser = false,
                VoteCount = 0,
                ResourceCount = 0,
                UserDisplayName = idToNameMap[learningModule.CreatedBy],
            };
        }

        /// <summary>
        /// Gets learning module entity model from view model.
        /// </summary>
        /// <param name="learningModuleViewModel"> Learning module view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a  learning module entity model</returns>
        public LearningModule PatchAndMapToDTO(
            LearningModuleViewModel learningModuleViewModel,
            Guid userAadObjectId)
        {
            learningModuleViewModel = learningModuleViewModel ?? throw new ArgumentNullException(nameof(learningModuleViewModel));

            return new LearningModule
            {
                Id = learningModuleViewModel.Id,
                Title = learningModuleViewModel.Title,
                Description = learningModuleViewModel.Description,
                SubjectId = learningModuleViewModel.SubjectId,
                GradeId = learningModuleViewModel.GradeId,
                ImageUrl = learningModuleViewModel.ImageUrl,
                UpdatedOn = DateTimeOffset.Now,
                UpdatedBy = userAadObjectId,
                CreatedOn = learningModuleViewModel.CreatedOn,
                CreatedBy = learningModuleViewModel.CreatedBy,
            };
        }

        /// <summary>
        /// Gets  learning module view model from entity model.
        /// </summary>
        /// <param name="learningModule"> Learning module entity model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="learningModuleVotes">List of learning module votes.</param>
        /// <param name="resourceCount">Count of learning module resources.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a  learning module view model object.</returns>
        public LearningModuleViewModel PatchAndMapToViewModel(
            LearningModule learningModule,
            Guid userAadObjectId,
            IEnumerable<LearningModuleVote> learningModuleVotes,
            int resourceCount,
            Dictionary<Guid, string> idToNameMap)
        {
            learningModule = learningModule ?? throw new ArgumentNullException(nameof(learningModule));
            learningModuleVotes = learningModuleVotes ?? throw new ArgumentNullException(nameof(learningModuleVotes));
            idToNameMap = idToNameMap ?? throw new ArgumentNullException(nameof(idToNameMap));

            return new LearningModuleViewModel
            {
                Id = learningModule.Id,
                Title = learningModule.Title,
                Description = learningModule.Description,
                GradeId = learningModule.GradeId,
                SubjectId = learningModule.SubjectId,
                Subject = learningModule.Subject,
                Grade = learningModule.Grade,
                ImageUrl = learningModule.ImageUrl,
                CreatedBy = learningModule.CreatedBy,
                UpdatedBy = learningModule.UpdatedBy,
                CreatedOn = learningModule.CreatedOn,
                UpdatedOn = learningModule.UpdatedOn,
                LearningModuleTag = learningModule.LearningModuleTag,
                IsLikedByUser = learningModuleVotes.Any(vote => vote.UserId == userAadObjectId),
                VoteCount = learningModuleVotes.Count(),
                ResourceCount = resourceCount,
                UserDisplayName = idToNameMap[learningModule.CreatedBy],
            };
        }

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
            Dictionary<Guid, string> idToNameMap)
        {
            moduleWithVotesAndResources = moduleWithVotesAndResources ?? throw new ArgumentNullException(nameof(moduleWithVotesAndResources));
            idToNameMap = idToNameMap ?? throw new ArgumentNullException(nameof(idToNameMap));

            var learningModuleDetails = new List<LearningModuleViewModel>();
            foreach (var learningModule in moduleWithVotesAndResources)
            {
                var module = learningModule.Value.FirstOrDefault();
                learningModuleDetails.Add(new LearningModuleViewModel()
                {
                    Id = learningModule.Key,
                    Title = module.Title,
                    VoteCount = (int)module.Votes.Count(),
                    IsLikedByUser = (bool)module.Votes?.Any(v => v.UserId == userAadObjectId),
                    Description = module.Description,
                    GradeId = (Guid)module.GradeId,
                    SubjectId = (Guid)module.SubjectId,
                    Subject = module.Subject,
                    Grade = module.Grade,
                    ImageUrl = module.ImageUrl,
                    CreatedBy = (Guid)module.CreatedBy,
                    UpdatedBy = (Guid)module.UpdatedBy,
                    CreatedOn = module.CreatedOn,
                    UpdatedOn = module.UpdatedOn,
                    LearningModuleTag = module.LearningModuleTag,
                    UserDisplayName = idToNameMap[(Guid)module.CreatedBy],
                    ResourceCount = (int)module.ResourceModuleMappings.Count(),
                });
            }

            return learningModuleDetails;
        }

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
            Dictionary<Guid, string> idToNameMap)
        {
            learningModule = learningModule ?? throw new ArgumentNullException(nameof(learningModule));
            idToNameMap = idToNameMap ?? throw new ArgumentNullException(nameof(idToNameMap));

            return new LearningModuleViewModel
            {
                Id = learningModule.Id,
                Title = learningModule.Title,
                Description = learningModule.Description,
                Subject = learningModule.Subject,
                SubjectId = learningModule.SubjectId,
                Grade = learningModule.Grade,
                GradeId = learningModule.GradeId,
                ImageUrl = learningModule.ImageUrl,
                LearningModuleTag = learningModule.LearningModuleTag,
                IsLikedByUser = learningModuleVotes.Any(vote => vote.UserId == userAadObjectId),
                UserDisplayName = idToNameMap[learningModule.CreatedBy],
                VoteCount = learningModuleVotes.Count(),
            };
        }
    }
}