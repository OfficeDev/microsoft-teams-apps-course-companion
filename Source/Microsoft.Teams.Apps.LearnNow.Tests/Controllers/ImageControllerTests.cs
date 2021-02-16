// <copyright file="ImageControllerTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common.Interfaces;
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The ImageControllerTests contains all the test cases for Bing image search.
    /// </summary>
    [TestClass]
    public class ImageControllerTests
    {
        // Constants
        private const string SearchText = "nature";
        private const string EmptyError = "Empty response received from search API.";

        // Define mock variables
        private ImageController imageController;
        private Mock<IImageProviderService> previewImagesHelper;
        private Mock<ILogger<ImageController>> logger;
        private TelemetryClient telemetryClient;
        private IEnumerable<string> bingSearchResult;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<ImageController>>();
            this.previewImagesHelper = new Mock<IImageProviderService>();
            TelemetryConfiguration mockTelemetryConfig = new TelemetryConfiguration();
            this.telemetryClient = new TelemetryClient(mockTelemetryConfig);
            this.imageController = new ImageController(this.previewImagesHelper.Object, this.logger.Object, this.telemetryClient);
            this.bingSearchResult = new List<string>()
            {
                "https://test/natureImage1.jpg",
                "https://test/natureImage2.jpg",
                "https://test/natureImage3.jpg",
                "https://test/natureImage4.jpg",
            };

            // Mock behaviour of the GetSearchResultAsync method.
            this.previewImagesHelper.Setup(x => x.GetSearchResultAsync(It.IsAny<string>())).Returns(Task.FromResult(this.bingSearchResult));

            this.imageController.ControllerContext = new ControllerContext();
            this.imageController.ControllerContext.HttpContext =
                FakeHttpContext.GetMockHttpContextWithUserClaims();
        }

        /// <summary>
        /// Test success scenario of the GetPreviewImagesAsync method
        /// </summary>
        /// <returns>Returns a task.</returns>
        [TestMethod]
        public async Task Test_GetPreviewImagesAsync_SuccessAsync()
        {
            // Call GetPreviewImagesAsync method
            var result = (ObjectResult)await this.imageController.GetAsync(SearchText);

            // Assert result
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.IsNotNull(result.Value);
        }

        /// <summary>
        /// Test empty Bing search result scenario of the GetPreviewImagesAsync method.
        /// </summary>
        /// <returns>Returns a task.</returns>
        [TestMethod]
        public async Task Test_GetPreviewImagesAsync_EmptyAsync()
        {
            IEnumerable<string> tempResult = new List<string>();

            // Mock behaviour of the GetSearchResultAsync method.
            this.previewImagesHelper.Setup(x => x.GetSearchResultAsync(It.IsAny<string>())).Returns(Task.FromResult(tempResult));

            // Call GetPreviewImagesAsync method.
            var result = (ObjectResult)await this.imageController.GetAsync(SearchText);

            // Assert result
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
            Assert.AreEqual(result.Value, EmptyError);
        }

        /// <summary>
        /// Test exception scenario of the GetPreviewImagesAsync method
        /// </summary>
        /// <returns>Returns a task.</returns>
        [TestMethod]
        public async Task Test_GetPreviewImagesAsync_ExceptionAsync()
        {
            // Mock behaviour of the GetSearchResultAsync method
            this.previewImagesHelper.Setup(x => x.GetSearchResultAsync(It.IsAny<string>())).Throws(new Exception());
            ObjectResult result;

            try
            {
                // Call GetPreviewImagesAsync method
                result = (ObjectResult)await this.imageController.GetAsync(SearchText);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }
    }
}