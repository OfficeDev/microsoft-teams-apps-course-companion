// <copyright file="FileDownloadControllerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The FileDownloadControllerTest contains all the test cases for the file download operations.
    /// </summary>
    [TestClass]
    public class FileDownloadControllerTest
    {
        // Define constants
        private const string TempUri = "http://www.teststorage.com/";
        private const string TempToken = "teststoragetoken";
        private Guid tempResourceId = new Guid("d36d4b16-f0dd-44a0-a720-e3d6bfc1e13f");

        // Define mock variables
        private FileDownloadController controller;
        private Mock<IFileDownloadProvider> fileDownloadProvider;
        private Mock<ILogger<FileDownloadController>> logger;
        private Mock<IUnitOfWork> unitOfWork;
        private TelemetryClient telemetryClient;
        private Mock<IResourceRepository> resourceRepository;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<FileDownloadController>>();
            this.fileDownloadProvider = new Mock<IFileDownloadProvider>();
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());

            this.controller = new FileDownloadController(
                this.unitOfWork.Object,
                this.fileDownloadProvider.Object,
                this.telemetryClient,
                this.logger.Object);

            // Mock behavior of the GetDownloadUri method
            this.fileDownloadProvider.Setup(x => x.GetDownloadUriAsync(It.IsAny<string>())).ReturnsAsync(TempUri + TempToken);

            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = FakeHttpContext.GetMockHttpContextWithUserClaims(),
            };

            this.resourceRepository = new Mock<IResourceRepository>();
        }

        /// <summary>
        /// Test success scenario of the GetBlobUrlWithSASToken method
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_GetBlobUrlWithSASToken_Success()
        {
            // ARRANGE
            this.unitOfWork.Setup(uow => uow.ResourceRepository).Returns(() => this.resourceRepository.Object);

            // Call GetDownloadFileUri method
            await this.controller.GetDownloadFileUriAsync(this.tempResourceId);

            // ASSERT
            this.resourceRepository.Verify(r => r.GetAsync(It.IsAny<Guid>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Test empty blob uri with token scenario of the GetBlobUrlWithSASToken method
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_GetBlobUrlWithSASToken_EmptyBlobUrlWithToken()
        {
            // ARRANGE
            this.unitOfWork.Setup(uow => uow.ResourceRepository).Returns(() => this.resourceRepository.Object);

            // Mock behavior of theGetDownloadUri method
            this.fileDownloadProvider.Setup(x => x.GetDownloadUriAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);

            // Call GetDownloadFileUri method
            var result = await this.controller.GetDownloadFileUriAsync(this.tempResourceId);
            var output = (ObjectResult)result;

            // Assert result
            Assert.AreEqual(output.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test empty blob uri with token scenario of the GetBlobUrlWithSASToken method
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_GetBlobUrlWithSASToken_EmptyResourceId()
        {
            // ARRANGE
            this.unitOfWork.Setup(uow => uow.ResourceRepository).Returns(() => this.resourceRepository.Object);

            // Call GetDownloadFileUri method
            var result = await this.controller.GetDownloadFileUriAsync(Guid.Empty);
            var output = (ObjectResult)result;

            // Assert result
            Assert.AreEqual(output.StatusCode, StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Test exception scenario of the GetBlobUrlWithSASToken method
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_GetBlobUrlWithSASToken_Exception()
        {
            // ARRANGE
            this.unitOfWork.Setup(uow => uow.ResourceRepository).Returns(() => this.resourceRepository.Object);

            // Mock behavior of the GetDownloadUri method
            this.fileDownloadProvider.Setup(x => x.GetDownloadUriAsync(It.IsAny<string>())).Throws(new Exception());

            try
            {
                // Call GetDownloadFileUri method
                var result = await this.controller.GetDownloadFileUriAsync(this.tempResourceId);
                var output = (ObjectResult)result;
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }
    }
}