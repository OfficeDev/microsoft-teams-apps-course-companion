// <copyright file="IFileUploadProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for handling file upload operation.
    /// </summary>
    public interface IFileUploadProvider
    {
        /// <summary>
        /// Upload file to specified container.
        /// </summary>
        /// <param name="containerName">Name of the container to which file needs to be uploaded.</param>
        /// <param name="fileStream">Local file path from which file is to be uploaded.</param>
        /// <param name="contentType">Content type of the file that is to be uploaded.</param>
        /// <returns>Returns file URI if file upload is successful.</returns>
        Task<string> UploadFileAsync(string containerName, Stream fileStream, string contentType);
    }
}