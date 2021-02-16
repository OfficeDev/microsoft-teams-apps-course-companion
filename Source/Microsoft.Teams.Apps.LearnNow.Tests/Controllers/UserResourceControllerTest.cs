// <copyright file="UserResourceControllerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The class contains the test cases of UserResourceController action methods.
    /// </summary>
    [TestClass]
    public class UserResourceControllerTest
    {
        private Mock<ILogger<UserResourceController>> logger;
        private TelemetryClient telemetryClient;
        private UserResourceController userResourceController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<IResourceMapper> resourceMapper;
        private Mock<IUsersService> usersServiceMock;
        private ITokenHelper accessTokenHelper;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<UserResourceController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.resourceMapper = new Mock<IResourceMapper>();
            this.usersServiceMock = new Mock<IUsersService>();
            this.accessTokenHelper = new FakeAccessTokenHelper();
            this.userResourceController = new UserResourceController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.usersServiceMock.Object,
                this.resourceMapper.Object)
            {
                ControllerContext = new ControllerContext(),
            };

            this.userResourceController.ControllerContext.HttpContext = FakeHttpContext.GetMockHttpContextWithUserClaims();
        }

        /// <summary>
        /// Test PostAsync for saving user resource details to storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveUserResource_ReturnsOkResult()
        {
            // ARRANGE
            var userResource = new UserResourceViewModel
            {
                ResourceId = Guid.NewGuid(),
            };
            var userresourceEntityModel = new UserResource
            {
                ResourceId = userResource.ResourceId,
            };
            var userResourceList = new List<UserResource>();
            this.unitOfWork.Setup(uow => uow.UserResourceRepository.FindAsync(It.IsAny<Expression<Func<UserResource, bool>>>())).ReturnsAsync(userResourceList);

            // ACT
            var result = (ObjectResult)await this.userResourceController.PostAsync(userResource);
            var resultValue = result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(result.Value, true);
        }

        /// <summary>
        /// Test PostAsync method when user learning module record already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_RecordAlreadyExist_ReturnsConflictStatus()
        {
            // ARRANGE
            var userResource = new UserResourceViewModel
            {
                ResourceId = Guid.NewGuid(),
            };
            var userresourceEntityModel = new UserResource
            {
                ResourceId = userResource.ResourceId,
            };
            var userResourceList = new List<UserResource>
            {
                userresourceEntityModel,
            };

            this.unitOfWork.Setup(uow => uow.UserResourceRepository.FindAsync(It.IsAny<Expression<Func<UserResource, bool>>>())).ReturnsAsync(userResourceList);

            // ACT
            var result = (ObjectResult)await this.userResourceController.PostAsync(userResource);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test DeleteAsync for deleting user resource from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteUserResource_ReturnsOkResult()
        {
            // ARRANGE
            var resourceId = Guid.NewGuid();
            var userResourceList = new List<UserResource>
            {
                new UserResource { ResourceId = Guid.NewGuid() },
            };
            this.unitOfWork.Setup(uow => uow.UserResourceRepository.FindAsync(It.IsAny<Expression<Func<UserResource, bool>>>())).ReturnsAsync(userResourceList);

            // ACT
            var result = (ObjectResult)await this.userResourceController.DeleteAsync(resourceId);
            var resultValue = result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test DeleteAsync when record not exists for given resource Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_RecordNotExists_ReturnsNotFound()
        {
            // ARRANGE
            var resourceId = Guid.NewGuid();
            var userResourceList = new List<UserResource>(); // Empty user resource collection
            this.unitOfWork.Setup(uow => uow.UserResourceRepository.FindAsync(It.IsAny<Expression<Func<UserResource, bool>>>())).ReturnsAsync(userResourceList);

            // ACT
            var result = (ObjectResult)await this.userResourceController.DeleteAsync(resourceId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }
    }
}