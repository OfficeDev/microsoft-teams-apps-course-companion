// <copyright file="ImageController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Common.Interfaces;

    /// <summary>
    /// Controller to handle images API operations.
    /// </summary>
    [Route("api/image")]
    [ApiController]
    [Authorize]
    public class ImageController : BaseController
    {
        /// <summary>
        /// Service to get filtered image URL's.
        /// </summary>
        private readonly IImageProviderService imageProviderService;

        /// <summary>
        /// Used to perform logging of errors and information.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageController"/> class.
        /// </summary>
        /// <param name="imageProviderService">Service to get filtered image URL's.</param>
        /// <param name="logger">Used to perform logging of errors and information.</param>
        /// <param name="telemetryClient">Instance of telemetry client.</param>
        public ImageController(
            IImageProviderService imageProviderService,
            ILogger<ImageController> logger,
            TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.imageProviderService = imageProviderService;
            this.logger = logger;
        }

        /// <summary>
        /// Get list of image URL's from image provider service.
        /// </summary>
        /// <param name="searchText">Search text to get filtered image URL's from provider service.</param>
        /// <returns>A collection of image URL's.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string searchText)
        {
            try
            {
                this.logger.LogInformation($"Image search API call initiated with search text: {searchText}");
                this.RecordEvent("Images - HTTP Get call.", RequestType.Initiated);

                var images = await this.imageProviderService.GetSearchResultAsync(searchText);

                if (!images.Any())
                {
                    this.logger.LogError("Empty response received from search API.");
                    this.RecordEvent("Empty response received from search API.", RequestType.Failed);

                    return this.NotFound("Empty response received from search API.");
                }

                this.logger.LogInformation($"Images - HTTP Get call succeeded with search text: {searchText}");
                this.RecordEvent("Images - HTTP Get call succeeded.", RequestType.Succeeded);

                return this.Ok(images);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while making call to get image URL's.");
                this.RecordEvent("Images - HTTP Get call failed.", RequestType.Failed);
                throw;
            }
        }
    }
}