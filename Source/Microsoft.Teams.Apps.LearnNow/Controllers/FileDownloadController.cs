// <copyright file="FileDownloadController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;

    /// <summary>
    /// Controller to handle file download operations.
    /// </summary>
    [Route("api/file/download")]
    [ApiController]
    [Authorize]
    public class FileDownloadController : BaseController
    {
        /// <summary>
        /// The instance of unit of work to access repository.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Provider for handling file download.
        /// </summary>
        private readonly IFileDownloadProvider fileDownloadProvider;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<FileDownloadController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The instance of unit of work to access repository.</param>
        /// <param name="fileDownloadProvider">Provider for handling Azure Blob Storage operations.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="logger">Instance to send logs to the telemetry service.</param>
        public FileDownloadController(
            IUnitOfWork unitOfWork,
            IFileDownloadProvider fileDownloadProvider,
            TelemetryClient telemetryClient,
            ILogger<FileDownloadController> logger)
            : base(telemetryClient)
        {
            this.fileDownloadProvider = fileDownloadProvider;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Method to get blob URL with SAS token to access the blob to download the file.
        /// </summary>
        /// <param name="resourceId">Resource id for which attachment needs to be downloaded.</param>
        /// <returns>Returns success if file is downloaded from blob successfully.</returns>
        [HttpGet("{resourceId}")]
        public async Task<IActionResult> GetDownloadFileUriAsync(Guid resourceId)
        {
            try
            {
                if (resourceId == null || resourceId == Guid.Empty)
                {
                    this.logger.LogError(StatusCodes.Status400BadRequest, $"Resource id to get file URL is null for userId :{this.UserObjectId}.");
                    this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call failed.", RequestType.Failed);
                    return this.BadRequest($"Resource id to get file URL is null for userId :{this.UserObjectId}.");
                }

                // Get resource details for storage.
                var resourceDetails = await this.unitOfWork.ResourceRepository.GetAsync(resourceId);

                if (resourceDetails == null)
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"The resource detail that user is trying to get does not exist. Resource id: {resourceId}, for userId :{this.UserObjectId} ");
                    this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call failed.", RequestType.Failed);
                    return this.NotFound($"The resource detail that user is trying to get does not exist for userId :{this.UserObjectId}");
                }

                var filePath = resourceDetails.AttachmentUrl;

                if (string.IsNullOrEmpty(filePath))
                {
                    this.logger.LogError(StatusCodes.Status404NotFound, $"The filePath  that user is trying to get does not exist. Resource id: {resourceId}, for userId :{this.UserObjectId} ");
                    this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call failed.", RequestType.Failed);
                    return this.NotFound($"The file path that user is trying to get does not exist for userId :{this.UserObjectId}");
                }

                this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call is initiated.", RequestType.Initiated);
                var filePathWithToken = await this.fileDownloadProvider.GetDownloadUriAsync(filePath);

                if (string.IsNullOrEmpty(filePathWithToken))
                {
                    this.logger.LogError($"Could not get file URL with SAS token for : {filePath}, for userId :{this.UserObjectId}.");
                    this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call failed.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status404NotFound, $"Could not get file URL with SAS token for userId :{this.UserObjectId}.");
                }

                this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call succeeded.", RequestType.Succeeded);
                return this.Ok(filePathWithToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while getting file URL with SAS token for userId :{this.UserObjectId}.");
                this.RecordEvent("GetDownloadFileUriAsync - HTTP Get call failed.", RequestType.Failed);
                throw;
            }
        }
    }
}