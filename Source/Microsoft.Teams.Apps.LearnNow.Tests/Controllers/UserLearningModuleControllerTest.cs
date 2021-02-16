// <copyright file="UserLearningModuleControllerTest.cs" company="Microsoft Corporation">
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
    /// The UserLearningModuleControllerTest contains the test cases for the UserLearningModule Controller.
    /// </summary>
    [TestClass]
    public class UserLearningModuleControllerTest
    {
        private Mock<ILogger<UserLearningModuleController>> logger;
        private TelemetryClient telemetryClient;
        private UserLearningModuleController userLearningModuleController;
        private Mock<IUnitOfWork> unitOfWork;
        private ITokenHelper accessTokenHelper;
        private Mock<IUsersService> usersService;
        private Mock<ILearningModuleMapper> learningModuleMapper;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<UserLearningModuleController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.learningModuleMapper = new Mock<ILearningModuleMapper>();
            this.usersService = new Mock<IUsersService>();
            this.accessTokenHelper = new FakeAccessTokenHelper();

            this.userLearningModuleController = new UserLearningModuleController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.usersService.Object,
                this.learningModuleMapper.Object)
            {
                ControllerContext = new ControllerContext(),
            };

            this.userLearningModuleController.ControllerContext.HttpContext = FakeHttpContext.GetMockHttpContextWithUserClaims();
        }

        /// <summary>
        /// Test PostAsync for saving user learning module details to storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveUserLearningModule_RetrunsOkResult()
        {
            // ARRANGE
            var userlearningModule = new UserLearningModuleViewModel
            {
                LearningModuleId = Guid.NewGuid(),
            };
            var userlearningModuleEntityModel = new UserLearningModule
            {
                LearningModuleId = userlearningModule.LearningModuleId,
            };
            var userLearningModuleList = new List<UserLearningModule>();
            this.unitOfWork.Setup(uow => uow.UserLearningModuleRepository.FindAsync(It.IsAny<Expression<Func<UserLearningModule, bool>>>())).ReturnsAsync(userLearningModuleList);

            // ACT
            var result = (ObjectResult)await this.userLearningModuleController.PostAsync(userlearningModule);
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
        public async Task PostAsync_RecordAlreadyExist_ReturnsConflictResult()
        {
            // ARRANGE
            var userlearningModule = new UserLearningModuleViewModel
            {
                LearningModuleId = Guid.NewGuid(),
            };
            var userlearningModuleEntityModel = new UserLearningModule
            {
                LearningModuleId = userlearningModule.LearningModuleId,
            };
            var userLearningModuleList = new List<UserLearningModule>
            {
                userlearningModuleEntityModel,
            };

            this.unitOfWork.Setup(uow => uow.UserLearningModuleRepository.FindAsync(It.IsAny<Expression<Func<UserLearningModule, bool>>>())).ReturnsAsync(userLearningModuleList);

            // ACT
            var result = (ObjectResult)await this.userLearningModuleController.PostAsync(userlearningModule);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test DeleteAsync for deleting user learning module record from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteUserLearningModule_ReturnsOkResult()
        {
            // ARRANGE
            var learningModuleId = Guid.NewGuid();
            var userLearningModuleList = new List<UserLearningModule>
            {
                new UserLearningModule { LearningModuleId = Guid.NewGuid() },
            };
            this.unitOfWork.Setup(uow => uow.UserLearningModuleRepository.FindAsync(It.IsAny<Expression<Func<UserLearningModule, bool>>>())).ReturnsAsync(userLearningModuleList);

            // ACT
            var result = (ObjectResult)await this.userLearningModuleController.DeleteAsync(learningModuleId);
            var resultValue = result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test DeleteAsync when record not exists for given learning module Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_RecordNotExists_ReturnsNotFound()
        {
            // ARRANGE
            var learningModuleId = Guid.NewGuid();
            var userLearningModuleList = new List<UserLearningModule>(); // Empty user learning module collection
            this.unitOfWork.Setup(uow => uow.UserLearningModuleRepository.FindAsync(It.IsAny<Expression<Func<UserLearningModule, bool>>>())).ReturnsAsync(userLearningModuleList);

            // ACT
            var result = (ObjectResult)await this.userLearningModuleController.DeleteAsync(learningModuleId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }
    }
}