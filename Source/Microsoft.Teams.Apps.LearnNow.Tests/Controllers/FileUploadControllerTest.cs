// <copyright file="FileUploadControllerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The FileUploadControllerTest contains all the test cases for the file upload operations.
    /// </summary>
    [TestClass]
    public class FileUploadControllerTest
    {
        // Define constants
        private const string BlobUri = "http://www.testblobstorage.com/";
        private const string FileName = "dummy.docx";
        private const string EmptyFileError = "File information received for uploading file is null.";
        private const string EmptyFileUriError = "File Uri obtained is null or empty. Error while uploading the file.";

        // Define mock variables
        private FileUploadController controller;
        private Mock<ILogger<FileUploadController>> logger;
        private Mock<IFileUploadProvider> fileUploadProvider;
        private TelemetryClient telemetryClient;

        // Define file to upload
        private IFormFile fromFile;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<FileUploadController>>();
            this.fileUploadProvider = new Mock<IFileUploadProvider>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.controller = new FileUploadController(this.fileUploadProvider.Object, this.logger.Object, this.telemetryClient);
            this.fromFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", FileName);

            // Mock behaviour of the UploadFileAsync method
            this.fileUploadProvider.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>())).Returns(Task.FromResult(BlobUri));

            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = FakeHttpContext.GetMockHttpContextWithUserClaims(),
            };
        }

        /// <summary>
        /// Test success scenario of the UploadFileAsync method
        /// </summary>
        /// <returns>Representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_UploadFileAsync_Success()
        {
            // Call UploadFileAsync method
            var result = (ObjectResult)await this.controller.UploadFileAsync(this.fromFile);

            // Assert response
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(result.Value, BlobUri);
        }

        /// <summary>
        /// Test empty file scenario of the UploadFileAsync method
        /// </summary>
        /// <returns>Representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_UploadFileAsync_EmptyFile()
        {
            this.fromFile = null;

            // Call UploadFileAsync method
            var result = (ObjectResult)await this.controller.UploadFileAsync(this.fromFile);

            // Assert response
            Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);
            Assert.AreEqual(result.Value, EmptyFileError);
        }

        /// <summary>
        /// Test empty blob uri scenario of the UploadFileAsync method
        /// </summary>
        /// <returns>Representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_UploadFileAsync_EmptyBlobUri()
        {
            // Mock behaviour of the UploadFileAsync method
            this.fileUploadProvider.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>())).Returns(Task.FromResult(string.Empty));

            // Call UploadFileAsync method
            var result = (ObjectResult)await this.controller.UploadFileAsync(this.fromFile);

            // Assert response
            Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);
            Assert.AreEqual(result.Value, EmptyFileUriError);
        }

        /// <summary>
        /// Test exception scenario of the UploadFileAsync method
        /// </summary>
        /// <returns>Representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_UploadFileAsync_ExceptionAsync()
        {
            // Mock behaviour of the UploadFileAsync method
            this.fileUploadProvider.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>())).Throws(new Exception());
            ObjectResult result;
            try
            {
                // Call UploadFileAsync method
                result = (ObjectResult)await this.controller.UploadFileAsync(this.fromFile);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }
    }
}
