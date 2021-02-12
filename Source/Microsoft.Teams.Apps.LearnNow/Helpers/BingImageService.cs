// <copyright file="BingImageService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Common.Interfaces;
    using Microsoft.Teams.Apps.LearnNow.Models.BingApiRequestModel;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Newtonsoft.Json;

    /// <summary>
    /// Service class for getting images from Bing image search API service.
    /// </summary>
    public class BingImageService : IImageProviderService
    {
        /// <summary>
        /// Bing Image height size
        /// </summary>
        public const int BingImageHeight = 200;

        /// <summary>
        /// Bing Image width size
        /// </summary>
        public const int BingImageWidth = 200;

        /// <summary>
        /// Bing cognitive service setting.
        /// </summary>
        private readonly IOptions<BingSearchServiceSettings> options;

        /// <summary>
        /// Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Used to perform logging of errors and information.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BingImageService"/> class.
        /// </summary>
        /// <param name="options">Bing cognitive service settings</param>
        /// <param name="httpClient">Instance of HttpClient.</param>
        /// <param name="logger">Used to perform logging of errors and information.</param>
        public BingImageService(IOptions<BingSearchServiceSettings> options, HttpClient httpClient, ILogger<BingImageService> logger)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Method to get image URL's from Bing Image search API for given search text.
        /// </summary>
        /// <param name="searchQueryTerm">Find image URL's based on search query term.</param>
        /// <returns>Returns a collection of image URL from Bing Image API service.</returns>
        public async Task<IEnumerable<string>> GetSearchResultAsync(string searchQueryTerm)
        {
            try
            {
                var contentUrls = new List<string>();

                // Make the search request to the Bing Image API, and get the results.
                this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.options.Value.Key);

                string requestUri = this.options.Value.Endpoint
                    + "?q=" + Uri.EscapeDataString(searchQueryTerm)
                    + "&height=" + BingImageHeight
                    + "&width=" + BingImageWidth
                    + "&safeSearch=" + this.options.Value.SafeSearch;

                HttpResponseMessage response = await this.httpClient.GetAsync(new Uri(requestUri));
                response.EnsureSuccessStatusCode();
                string contentString = await response.Content.ReadAsStringAsync();
                var bingApiResponse = JsonConvert.DeserializeObject<BingApiResponse>(contentString);

                var filteredUrlResults = bingApiResponse.Images.Where(image => image.ContentUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                    .Select(image => image.ContentUrl);
                contentUrls.AddRange(filteredUrlResults);

                return contentUrls;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while fetching Bing search results for search text: {searchQueryTerm}");
                return null;
            }
        }
    }
}