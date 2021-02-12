// <copyright file="IImageProviderService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for getting images from provider service.
    /// </summary>
    public interface IImageProviderService
    {
        /// <summary>
        /// Get filtered images from external image provider service.
        /// </summary>
        /// <param name="searchQueryTerm">Find image URL's based on search query term.</param>
        /// <returns><see cref="Task"/>Returns a collection of image URL based on search term.</returns>
        Task<IEnumerable<string>> GetSearchResultAsync(string searchQueryTerm);
    }
}
