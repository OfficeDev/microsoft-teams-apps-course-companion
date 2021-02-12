// <copyright file="UserSettingControllerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
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
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// This class contains test cases of user setting controller.
    /// </summary>
    [TestClass]
    public class UserSettingControllerTest
    {
        private Mock<ILogger<UserSettingsController>> logger;
        private TelemetryClient telemetryClient;
        private UserSettingsController userSettingsController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<IUserSettingMapper> userSettingMapper;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<UserSettingsController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.userSettingMapper = new Mock<IUserSettingMapper>();

            this.userSettingsController = new UserSettingsController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.userSettingMapper.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = FakeHttpContext.GetMockHttpContextWithUserClaims(),
                },
            };
        }

        /// <summary>
        /// Test PostAsync for saving tab new user settings to storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveUserSetting_ReturnsOkResult()
        {
            // ARRANGE
            IEnumerable<Guid> guids = new List<Guid>()
            {
                Guid.NewGuid(), Guid.NewGuid(),
            };
            var userSettingsModel = new UserSettingsModel
            {
                GradeIds = guids,
            };

            var userSettingDTO = new UserSettings
            {
                ResourceGradeIds = "test",
            };

            this.userSettingMapper.Setup(userSettingMapper => userSettingMapper.CreateMap(It.IsAny<UserSettingsModel>(), It.IsAny<string>(), It.IsAny<Guid>())).Returns(userSettingDTO);
            this.unitOfWork.Setup(uow => uow.UserSettingRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.UserSettingRepository.Add(It.IsAny<UserSettings>())).Returns(userSettingDTO);

            // ACTfilterModel
            var result = (ObjectResult)await this.userSettingsController.PostAsync("resource", userSettingsModel);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test PostAsync for updating existing user settings to storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_UpdateUserSetting_ReturnsOkResult()
        {
            // ARRANGE
            IEnumerable<Guid> guids = new List<Guid>()
            {
                Guid.NewGuid(), Guid.NewGuid(),
            };
            var userSettingsModel = new UserSettingsModel
            {
                GradeIds = guids,
            };

            var userSettingDTO = new UserSettings
            {
                ResourceGradeIds = "test",
            };

            this.userSettingMapper.Setup(userSettingMapper => userSettingMapper.CreateMap(It.IsAny<UserSettingsModel>(), It.IsAny<string>(), It.IsAny<Guid>())).Returns(userSettingDTO);
            this.unitOfWork.Setup(uow => uow.UserSettingRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(userSettingDTO);
            this.unitOfWork.Setup(uow => uow.UserSettingRepository.Update(It.IsAny<UserSettings>())).Returns(userSettingDTO);

            // ACTfilterModel
            var result = (ObjectResult)await this.userSettingsController.PostAsync("resource", userSettingsModel);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test GetAsync method to return existing user settings for user object Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_UserSettingExistsForUserObjectId_ReturnsOkResult()
        {
            // ARRANGE
            IEnumerable<Guid> guids = new List<Guid>()
            {
                Guid.NewGuid(), Guid.NewGuid(),
            };
            var filterModel = new FilterModel
            {
                GradeIds = guids,
            };
            var userSettingsModel = new UserSettingsModel
            {
                GradeIds = guids,
            };
            var userSettingDTO = new UserSettings
            {
                ResourceGradeIds = "test",
            };
            this.unitOfWork.Setup(uow => uow.UserSettingRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(userSettingDTO);
            this.userSettingMapper.Setup(userSettingMapper => userSettingMapper.CreateMapToViewModel(It.IsAny<UserSettings>(), It.IsAny<string>())).Returns(userSettingsModel);

            // ACT
            var result = (ObjectResult)await this.userSettingsController.GetAsync("resource");
            var resultValue = (UserSettingsModel)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.GradeIds, guids);
        }

        /// <summary>
        /// Test GetAsync method when record not exists for given user object Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_RecordNotexistForGivenUserObjectId_ReturnsNotFound()
        {
            // ARRANGE
            IEnumerable<Guid> guids = new List<Guid>()
            {
                Guid.NewGuid(), Guid.NewGuid(),
            };
            var filterModel = new FilterModel
            {
                GradeIds = guids,
            };

            var userSettingDTO = new UserSettings
            {
                ResourceGradeIds = "test",
            };
            this.unitOfWork.Setup(uow => uow.UserSettingRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);

            // ACT
            var result = (StatusCodeResult)await this.userSettingsController.GetAsync("resource");

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }
    }
}