// <copyright file="IResourceMapper.cs" company="Microsoft Corporation">
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
    public interface IResourceMapper
    {
        /// <summary>
        /// Gets resource entity model from view model.
        /// </summary>
        /// <param name="resourceViewModel">Resource view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a resource entity model</returns>
        public Resource MapToDTO(
            ResourceViewModel resourceViewModel,
            Guid userAadObjectId);

        /// <summary>
        /// Gets resource view model from entity model.
        /// </summary>
        /// <param name="resource">Resource entity model object.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a resource view model object.</returns>
        public ResourceViewModel MapToViewModel(
            Resource resource,
            Dictionary<Guid, string> idToNameMap);

        /// <summary>
        /// Gets resource entity model from view model.
        /// </summary>
        /// <param name="resourceViewModel">Resource view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a resource entity model</returns>
        public Resource PatchAndMapToDTO(
            ResourceViewModel resourceViewModel,
            Guid userAadObjectId);

        /// <summary>
        /// Gets resource view model from entity model.
        /// </summary>
        /// <param name="resource">Resource entity model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="resourceVotes">List of resource votes.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a resource view model object.</returns>
        public ResourceViewModel PatchAndMapToViewModel(
            Resource resource,
            Guid userAadObjectId,
            IEnumerable<ResourceVote> resourceVotes,
            Dictionary<Guid, string> idToNameMap);

        /// <summary>
        /// Gets resource view models from entity models.
        /// </summary>
        /// <param name="filteredResources">Resource entity object collection.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a collection of resource view models.</returns>
        IEnumerable<ResourceViewModel> MapToViewModels(
            Dictionary<Guid, List<ResourceDetailModel>> filteredResources,
            Guid userAadObjectId,
            Dictionary<Guid, string> idToNameMap);

        /// <summary>
        /// Gets resource view models from entity models.
        /// </summary>
        /// <param name="filteredResources">Resource entity object collection.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a collection of resource view models.</returns>
        public IEnumerable<ResourceViewModel> MapToViewModels(
            Dictionary<Guid, List<ResourceDetailModel>> filteredResources,
            Guid userAadObjectId);

        /// <summary>
        /// Gets resource view model from entity model.
        /// </summary>
        /// <param name="resource">Resource entity model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <param name="resourceVotes">List of resource votes.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns a resource view model object.</returns>
        public ResourceViewModel MapToViewModel(
            Resource resource,
            Guid userAadObjectId,
            IEnumerable<ResourceVote> resourceVotes,
            Dictionary<Guid, string> idToNameMap);
    }
}