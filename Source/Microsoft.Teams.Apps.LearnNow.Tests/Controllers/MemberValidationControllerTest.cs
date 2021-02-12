// <copyright file="MemberValidationControllerTest.cs" company="Microsoft Corporation">
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
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Unit test class for Member validation controller
    /// </summary>
    [TestClass]
    public class MemberValidationControllerTest
    {
        /// <summary>
        /// Sends logs to the Application Insights service.
        /// </summary>
        private Mock<ILogger<MemberValidationController>> logger;

        /// <summary>
        /// Instance of MemberValidationService to validate member.
        /// </summary>
        private Mock<IMemberValidationService> memberValidationService;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private IOptions<SecurityGroupSettings> securityGroupOptions;

        private MemberValidationController controller;

        private TelemetryClient telemetryClient;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<MemberValidationController>>();
            this.securityGroupOptions = Options.Create<SecurityGroupSettings>(new SecurityGroupSettings());
            this.memberValidationService = new Mock<IMemberValidationService>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());

            this.controller = new MemberValidationController(
                this.telemetryClient,
                this.logger.Object,
                this.memberValidationService.Object,
                this.securityGroupOptions);

            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = FakeHttpContext.GetDefaultContextWithUserIdentity(),
            };
        }

        /// <summary>
        /// Validate if user is member of group
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateIfUserIsMemberOfSecurityGroupAsync_Succeed()
        {
            // arrange
            this.memberValidationService.Setup(svc => svc.ValidateMemberAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            this.securityGroupOptions.Value.AdminGroupId = Guid.NewGuid().ToString();
            this.securityGroupOptions.Value.TeacherSecurityGroupId = Guid.NewGuid().ToString();

            // act
            var response = await this.controller.ValidateIfUserIsMemberOfSecurityGroupAsync();
            var responseStatus = (ObjectResult)response;

            // assert
            Assert.AreEqual(StatusCodes.Status200OK, responseStatus.StatusCode);
            Assert.AreEqual(true, ((UserRole)responseStatus.Value).IsAdmin);
            Assert.AreEqual(true, ((UserRole)responseStatus.Value).IsTeacher);
        }

        /// <summary>
        /// Validate failure scenario user is not a member of group
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateIfUserIsMemberOfSecurityGroupAsync_Failed()
        {
            // arrange
            this.memberValidationService.Setup(svc => svc.ValidateMemberAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(false);

            this.securityGroupOptions.Value.AdminGroupId = Guid.NewGuid().ToString();
            this.securityGroupOptions.Value.TeacherSecurityGroupId = Guid.NewGuid().ToString();

            // act
            var response = await this.controller.ValidateIfUserIsMemberOfSecurityGroupAsync();
            var responseStatus = (ObjectResult)response;

            // assert
            Assert.AreEqual(StatusCodes.Status200OK, responseStatus.StatusCode);
            Assert.AreEqual(false, ((UserRole)responseStatus.Value).IsAdmin);
            Assert.AreEqual(false, ((UserRole)responseStatus.Value).IsTeacher);
        }

        /// <summary>
        /// Validate failure scenario user is moderator
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateIfUserIsModerator_Succeed()
        {
            // arrange
            this.memberValidationService.Setup(svc => svc.ValidateMemberAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            this.securityGroupOptions.Value.ModeratorGroupId = Guid.NewGuid().ToString();

            // act
            var response = await this.controller.ValidateIfUserIsModeratorAsync();
            var responseStatus = (ObjectResult)response;

            // assert
            Assert.AreEqual(StatusCodes.Status200OK, responseStatus.StatusCode);
            Assert.AreEqual(true, responseStatus.Value);
        }
    }
}
