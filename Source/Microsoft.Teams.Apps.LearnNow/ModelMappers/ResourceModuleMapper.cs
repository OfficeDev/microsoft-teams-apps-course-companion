// <copyright file="ResourceModuleMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// A model class that contains methods related to resource module mapping model mappings.
    /// </summary>
    public class ResourceModuleMapper : IResourceModuleMapper
    {
        /// <summary>
        /// Gets resource module model from view model.
        /// </summary>
        /// <param name="resourceModuleViewModel">Resource module entity view model object.</param>
        /// <returns>Returns a resource module entity model object.</returns>
        public ResourceModuleMapping MapToDTO(
            ResourceModuleViewModel resourceModuleViewModel)
        {
            resourceModuleViewModel = resourceModuleViewModel ?? throw new ArgumentNullException(nameof(resourceModuleViewModel));

            return new ResourceModuleMapping
            {
                ResourceId = resourceModuleViewModel.ResourceId,
                LearningModuleId = resourceModuleViewModel.LearningModuleId,
                CreatedOn = DateTimeOffset.Now,
            };
        }
    }
}