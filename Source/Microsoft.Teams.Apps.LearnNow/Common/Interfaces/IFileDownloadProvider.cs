// <copyright file="IFileDownloadProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for handling file download.
    /// </summary>
    public interface IFileDownloadProvider
    {
        /// <summary>
        /// Get download URL string for the file.
        /// </summary>
        /// <param name="filePath">File path using which file is downloaded.</param>
        /// <returns>Return the file URL to download.</returns>
        Task<string> GetDownloadUriAsync(string filePath);
    }
}