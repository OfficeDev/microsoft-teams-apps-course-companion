// <copyright file="FileDownloadProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Provider for handling file download using Azure Blob storage.
    /// </summary>
    public class FileDownloadProvider : IFileDownloadProvider
    {
        /// <summary>
        /// Instance to send logs to the telemetry service.
        /// </summary>
        private readonly ILogger<FileDownloadProvider> logger;

        /// <summary>
        /// Instance to hold Microsoft Azure Storage data.
        /// </summary>
        private readonly IOptionsMonitor<StorageSettings> storageOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadProvider"/> class.
        /// </summary>
        /// <param name="storageOptions">A set of key/value application storage configuration properties.</param>
        /// <param name="logger">Instance to send logs to the telemetry service.</param>
        public FileDownloadProvider(IOptionsMonitor<StorageSettings> storageOptions, ILogger<FileDownloadProvider> logger)
        {
            this.logger = logger;
            this.storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
        }

        /// <summary>
        /// Get Blob URL string for the container, including the SAS token.
        /// </summary>
        /// <param name="filePath">File path to downloaded the file.</param>
        /// <returns>Return the download URL of the file.</returns>
        public async Task<string> GetDownloadUriAsync(string filePath)
        {
            try
            {
                // Create a blob client for interacting with the blob service.
                CloudBlobClient blobClient = this.InitializeBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(Constants.BaseContainerName);

                var token = await this.GetContainerSASUriAsync(container, Constants.SharedAccessPolicyName);
                StorageCredentials credentials = new StorageCredentials(token);

                return filePath + token;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while downloading file from a container {filePath} blob.");
                throw;
            }
        }

        /// <summary>
        /// Returns a URI containing a SAS for the blob container.
        /// </summary>
        /// <param name="container">A reference to the container.</param>
        /// <param name="storedPolicyName">A string containing the name of the stored access policy. If null, an ad-hoc SAS is created.</param>
        /// <returns>A string containing the URI for the container, with the SAS token appended.</returns>
        private async Task<string> GetContainerSASUriAsync(CloudBlobContainer container, string storedPolicyName)
        {
            // Create a new shared access policy and define its constraints.
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30),
                Permissions = SharedAccessBlobPermissions.Read,
            };

            // Get the container's existing permissions.
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();

            // Add the new policy to the container's permissions, and set the container's permissions.
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add(storedPolicyName, sharedPolicy);
            await container.SetPermissionsAsync(permissions);

            return container.GetSharedAccessSignature(sharedPolicy, null);
        }

        /// <summary>
        /// Initialize a blob client for interacting with the blob service.
        /// </summary>
        /// <returns>Returns blob client for blob operations.</returns>
        private CloudBlobClient InitializeBlobClient()
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.storageOptions.CurrentValue.BlobConnectionString);

                // Create a blob client for interacting with the blob service.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                return blobClient;
            }
            catch (FormatException ex)
            {
                this.logger.LogError(ex, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                throw;
            }
            catch (ArgumentException ex)
            {
                this.logger.LogError(ex, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                throw;
            }
        }
    }
}