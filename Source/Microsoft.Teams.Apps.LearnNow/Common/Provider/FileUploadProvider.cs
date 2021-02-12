// <copyright file="FileUploadProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Provider for handling file upload using Azure Blob storage.
    /// </summary>
    public class FileUploadProvider : IFileUploadProvider
    {
        /// <summary>
        /// Instance to send logs to the telemetry service.
        /// </summary>
        private readonly ILogger<FileUploadProvider> logger;

        /// <summary>
        /// Instance to hold Microsoft Azure Storage data.
        /// </summary>
        private readonly IOptionsMonitor<StorageSettings> storageOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadProvider"/> class.
        /// </summary>
        /// <param name="storageOptions">A set of key/value application storage configuration properties.</param>
        /// <param name="logger">Instance to send logs to the telemetry service.</param>
        public FileUploadProvider(IOptionsMonitor<StorageSettings> storageOptions, ILogger<FileUploadProvider> logger)
        {
            this.logger = logger;
            this.storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
        }

        /// <summary>
        /// Upload file to specified container and path on Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container in which file needs to be uploaded.</param>
        /// <param name="fileStream">File stream of file to be uploaded on blob.</param>
        /// <param name="contentType">Content type to be set on blob.</param>
        /// <returns>Returns uploaded file URI on blob.</returns>
        public async Task<string> UploadFileAsync(string containerName, Stream fileStream, string contentType)
        {
            try
            {
                CloudBlobContainer container = await this.GetContainerAsync();
                var token = await this.GetContainerSasUriAsync(container, Constants.SharedAccessPolicyName);
                StorageCredentials credentials = new StorageCredentials(token);

                var blobUri = container.GetBlockBlobReference(containerName);
                CloudBlockBlob blockBlob = new CloudBlockBlob(blobUri.Uri, credentials);

                // Set the blob's content type so that the browser knows how to treat file.
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.UploadFromStreamAsync(fileStream);

                return blockBlob.Uri.ToString();
            }
            catch (StorageException ex)
            {
                this.logger.LogError(ex, "Error while uploading file to Azure Blob Storage.");
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while uploading file to Azure Blob Storage.");
                throw;
            }
        }

        /// <summary>
        /// Get container where file is to be uploaded.
        /// </summary>
        /// <returns>A container where file is to be uploaded.</returns>
        private async Task<CloudBlobContainer> GetContainerAsync()
        {
            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = this.InitializeBlobClient();

            // Create a container for organizing blobs within the storage account.
            CloudBlobContainer container = blobClient.GetContainerReference(Constants.BaseContainerName);

            BlobRequestOptions requestOptions = new BlobRequestOptions();
            await container.CreateIfNotExistsAsync(requestOptions, null);

            return container;
        }

        /// <summary>
        /// Returns a URI containing a SAS for the blob container.
        /// </summary>
        /// <param name="container">A reference to the container.</param>
        /// <param name="storedPolicyName">A string containing the name of the stored access policy. If null, an ad-hoc SAS is created.</param>
        /// <returns>A string containing the URI for the container, with the SAS token appended.</returns>
        private async Task<string> GetContainerSasUriAsync(CloudBlobContainer container, string storedPolicyName)
        {
            // Create a new shared access policy and define its constraints.
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List |
                    SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create,
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
                this.logger.LogError(ex, "Invalid format of storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                throw;
            }
            catch (ArgumentException ex)
            {
                this.logger.LogError(ex, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while creating the blob client.");
                throw;
            }
        }
    }
}