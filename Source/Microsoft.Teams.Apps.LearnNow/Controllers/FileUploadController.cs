// <copyright file="FileUploadController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Controller to handle file upload operations.
    /// </summary>
    [Route("api/file/upload")]
    [ApiController]
    public class FileUploadController : BaseController
    {
        /// <summary>
        /// Provider for handling file upload.
        /// </summary>
        private readonly IFileUploadProvider fileUploadProvider;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<FileUploadController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadController"/> class.
        /// </summary>
        /// <param name="fileUploadProvider">Provider for handling Azure Blob Storage operations.</param>
        /// <param name="logger">Instance to send logs to the telemetry service.</param>
        /// <param name="telemetryClient">Instance of telemetry client.</param>
        public FileUploadController(
            IFileUploadProvider fileUploadProvider,
            ILogger<FileUploadController> logger,
            TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.fileUploadProvider = fileUploadProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Method to upload the file.
        /// </summary>
        /// <param name="fileInfo">File information that is to be uploaded.</param>
        /// <returns>Returns success if file is uploaded successfully.</returns>
        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(IFormFile fileInfo)
        {
            try
            {
                if (fileInfo == null)
                {
                    this.logger.LogError(StatusCodes.Status400BadRequest, "File information received for uploading file is null.");
                    this.RecordEvent("UploadFile - HTTP Post call.", RequestType.Failed);
                    return this.BadRequest("File information received for uploading file is null.");
                }

                this.RecordEvent("UploadFile - HTTP Post call is initiated.", RequestType.Initiated);
                var fileUri = await this.UploadFileAndGetUriAsync(fileInfo);

                if (string.IsNullOrEmpty(fileUri))
                {
                    this.logger.LogInformation(StatusCodes.Status500InternalServerError, "File URL obtained after uploading the file is null or empty.");
                    this.RecordEvent("UploadFile - HTTP Post call.", RequestType.Failed);
                    return this.StatusCode(StatusCodes.Status400BadRequest, "File Uri obtained is null or empty. Error while uploading the file.");
                }

                this.RecordEvent("UploadFile - HTTP Post call succeeded.", RequestType.Succeeded);
                return this.Ok(fileUri);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while uploading file.");
                this.RecordEvent("UploadFile - HTTP Post call.", RequestType.Failed);
                throw;
            }
        }

        /// <summary>
        /// Method to upload the file.
        /// </summary>
        /// <param name="fileInfo">File information that is to be uploaded.</param>
        /// <returns>Returns URL of file if uploaded is successful.</returns>
        public async Task<string> UploadFileAndGetUriAsync(IFormFile fileInfo)
        {
            fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));

            string contentType = this.GetFileContentType(fileInfo.FileName);
            string folderName = Guid.NewGuid().ToString();
            string containerName = $"{folderName}/{fileInfo.FileName}";
            Stream fileStream = fileInfo.OpenReadStream();

            return await this.fileUploadProvider.UploadFileAsync(containerName, fileStream, contentType);
        }

        /// <summary>
        /// Get set file content type.
        /// </summary>
        /// <param name="fileName">Full name of the file</param>
        /// <returns>Returns create new change request page in task module.</returns>
        private string GetFileContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case FileType.XLSX:
                case FileType.XLS:
                    return ContentType.Excel;

                case FileType.PPT:
                case FileType.PPTX:
                    return ContentType.PPT;

                case FileType.DOCX:
                case FileType.DOC:
                    return ContentType.Word;

                case FileType.PDF:
                    return ContentType.PDF;
            }

            return null;
        }
    }
}