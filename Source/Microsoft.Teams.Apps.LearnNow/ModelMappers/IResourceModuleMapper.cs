// <copyright file="IResourceModuleMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Interface for handling operations related to resource module mapping model mappings.
    /// </summary>
    public interface IResourceModuleMapper
    {
        /// <summary>
        /// Gets resource module model from view model.
        /// </summary>
        /// <param name="resourceModuleViewModel">Grade entity view model object.</param>
        /// <returns>Returns a resource module mapping entity model object.</returns>
        public ResourceModuleMapping MapToDTO(
            ResourceModuleViewModel resourceModuleViewModel);
    }
}