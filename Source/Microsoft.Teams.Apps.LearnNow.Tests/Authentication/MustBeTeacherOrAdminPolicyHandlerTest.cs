// <copyright file="MustBeTeacherOrAdminPolicyHandlerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Authentication
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy.AuthenticationPolicy;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test class for Teacher or admin policy handler
    /// </summary>
    [TestClass]
    public class MustBeTeacherOrAdminPolicyHandlerTest
    {
        /// <summary>
        /// Instance of Mocked MemberValidationService to validate member.
        /// </summary>
        private Mock<IMemberValidationService> memberValidationService;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private IOptions<SecurityGroupSettings> securityGroupOptions;

        private MustBeTeacherOrAdminUserPolicyHandler policyHandler;

        private AuthorizationHandlerContext authContext;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.memberValidationService = new Mock<IMemberValidationService>();
            this.securityGroupOptions = Options.Create(new SecurityGroupSettings());

            this.policyHandler = new MustBeTeacherOrAdminUserPolicyHandler(
                this.memberValidationService.Object,
                this.securityGroupOptions);
        }

        /// <summary>
        /// Validate auth handle for success
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.></returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Succeed()
        {
            // Arrange
            this.memberValidationService
                .Setup(svc => svc.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeacherOrAdmin();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsTrue(this.authContext.HasSucceeded);
        }

        /// <summary>
        /// Validate auth handle for success
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.></returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Failed()
        {
            // Arrange
            this.memberValidationService
                .Setup(svc => svc.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => false);

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeacherOrAdmin();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsFalse(this.authContext.HasSucceeded);
        }
    }
}