// <copyright file="MustBeTeamMemberUserPolicyHandlerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Authentication
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Authentication.AuthenticationPolicy;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test class for Team member policy handler
    /// </summary>
    [TestClass]
    public class MustBeTeamMemberUserPolicyHandlerTest
    {
        /// <summary>
        /// Instance of Mocked MemberValidationService to validate member.
        /// </summary>
        private Mock<IMemberValidationService> memberValidationService;

        /// <summary>
        /// Instance of IOptions to read security group data from azure application configuration.
        /// </summary>
        private IOptions<BotSettings> botSettings;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private IMemoryCache memoryCache;

        private MustBeTeamMemberUserPolicyHandler policyHandler;

        private AuthorizationHandlerContext authContext;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.memberValidationService = new Mock<IMemberValidationService>();
            this.botSettings = Options.Create(new BotSettings()
            {
                MicrosoftAppId = "xxxx-xxxx-xxxx",
                AppBaseUri = "https://foo",
            });
        }

        /// <summary>
        /// Validate Team member user policy handler
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Succeed()
        {
            // Arrange
            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamMember();
            this.memoryCache = new FakeMemoryCache();
            this.policyHandler = new MustBeTeamMemberUserPolicyHandler(
                this.memoryCache,
                this.botSettings,
                this.memberValidationService.Object);

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsTrue(this.authContext.HasSucceeded);
        }

        /// <summary>
        /// Validate Team member user policy handler
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Failed()
        {
            // Arrange
            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamMember();
            var mockMemoryCache = new Mock<IMemoryCache>();
            mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.policyHandler = new MustBeTeamMemberUserPolicyHandler(
                mockMemoryCache.Object,
                this.botSettings,
                this.memberValidationService.Object);

            this.memberValidationService
                .Setup(svc => svc.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => false);

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsFalse(this.authContext.HasSucceeded);
        }
    }
}
