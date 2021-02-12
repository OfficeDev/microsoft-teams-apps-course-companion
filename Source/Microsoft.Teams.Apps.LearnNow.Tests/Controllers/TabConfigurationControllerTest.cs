// <copyright file="TabConfigurationControllerTest.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// This class contains test cases of tab configuration controller.
    /// </summary>
    [TestClass]
    public class TabConfigurationControllerTest
    {
        private Mock<ILogger<TabConfigurationController>> logger;
        private TelemetryClient telemetryClient;
        private TabConfigurationController tabConfigurationController;
        private Mock<IUnitOfWork> unitOfWork;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<TabConfigurationController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();

            this.tabConfigurationController = new TabConfigurationController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = FakeHttpContext.GetMockHttpContextWithUserClaims(),
                },
            };
        }

        /// <summary>
        /// Test PostAsync for saving tab configuration to storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveTabConfiguration_ReturnsOkResult()
        {
            // ARRANGE
            var tabConfigurationDetail = new TabConfigurationViewModel
            {
                TeamId = "team 1",
                ChannelId = "channel 1",
                LearningModuleId = Guid.NewGuid(),
            };

            var tabConfigurationEntity = new TabConfiguration
            {
                TeamId = tabConfigurationDetail.TeamId,
                ChannelId = tabConfigurationDetail.ChannelId,
                LearningModuleId = tabConfigurationDetail.LearningModuleId,
            };

            var groupId = Guid.NewGuid().ToString();

            this.unitOfWork.Setup(uow => uow.TabConfigurationRepository.Add(It.IsAny<TabConfiguration>())).Returns(tabConfigurationEntity);

            // ACT
            var result = (ObjectResult)await this.tabConfigurationController.PostAsync(groupId, tabConfigurationDetail);
            var resultValue = (TabConfiguration)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.LearningModuleId, tabConfigurationDetail.LearningModuleId);
            Assert.AreEqual(resultValue.TeamId, tabConfigurationDetail.TeamId);
            Assert.AreEqual(resultValue.ChannelId, tabConfigurationDetail.ChannelId);
        }

        /// <summary>
        /// Test GetAsync method to return existing tab configuration for id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_TabConfigurationExistsForId_ReturnsOkResult()
        {
            // ARRANGE
            var tabConfigurationEntities = new List<TabConfiguration>();
            tabConfigurationEntities.Add(new TabConfiguration
            {
                TeamId = "team 1",
                ChannelId = "channel 1",
                LearningModuleId = Guid.NewGuid(),
            });

            this.unitOfWork.Setup(uow => uow.TabConfigurationRepository.FindAsync(It.IsAny<Expression<Func<TabConfiguration, bool>>>()))
                .ReturnsAsync(tabConfigurationEntities);

            // ACT
            var result = (ObjectResult)await this.tabConfigurationController.GetAsync(Guid.NewGuid().ToString(), Guid.NewGuid());
            var resultValue = (TabConfiguration)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.TeamId, tabConfigurationEntities[0].TeamId);
            Assert.AreEqual(resultValue.ChannelId, tabConfigurationEntities[0].ChannelId);
            Assert.AreEqual(resultValue.LearningModuleId, tabConfigurationEntities[0].LearningModuleId);
        }

        /// <summary>
        /// Test GetAsync method when record not exists for given tab Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_RecordNotexistForGivenTabId_ReturnsNotFound()
        {
            // ARRANGE
            this.unitOfWork.Setup(uow => uow.TabConfigurationRepository
            .FindAsync(It.IsAny<Expression<Func<TabConfiguration, bool>>>()))
            .ReturnsAsync(() => null);

            // ACT
            var result = (ObjectResult)await this.tabConfigurationController.GetAsync(Guid.NewGuid().ToString(), Guid.NewGuid());

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PatchAsync method for updating the tab configurations.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_UpdateTabConfiguration_ReturnsOkResult()
        {
            // ARRANGE
            var id = Guid.NewGuid();
            var tabConfigurationDetail = new TabConfigurationViewModel
            {
                TeamId = "team 1",
                ChannelId = "channel 1",
                LearningModuleId = Guid.NewGuid(),
            };

            var tabConfigurationEntities = new List<TabConfiguration>();
            tabConfigurationEntities.Add(new TabConfiguration
            {
                TeamId = "team 1",
                ChannelId = "channel 1",
                LearningModuleId = Guid.NewGuid(),
            });

            this.unitOfWork.Setup(uow => uow.TabConfigurationRepository
            .FindAsync(It.IsAny<Expression<Func<TabConfiguration, bool>>>()))
            .ReturnsAsync(() => tabConfigurationEntities);

            this.unitOfWork.Setup(uow => uow.TabConfigurationRepository.Update(It.IsAny<TabConfiguration>())).Returns(tabConfigurationEntities[0]);

            // ACT
            var result = (ObjectResult)await this.tabConfigurationController.PatchAsync(Guid.NewGuid().ToString(), id, tabConfigurationDetail);
            var resultValue = (TabConfiguration)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.LearningModuleId, tabConfigurationDetail.LearningModuleId);
            Assert.AreEqual(resultValue.Id, tabConfigurationDetail.Id);
        }

        /// <summary>
        /// Test PatchAsync method when request route tab Id is empty guid.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_EmptyTabID_ReturnsBadRequest()
        {
            // ARRANGE
            var tabConfigurationDetail = new TabConfigurationViewModel
            {
                LearningModuleId = Guid.NewGuid(),
            };

            // ACT
            var result = (ObjectResult)await this.tabConfigurationController.PatchAsync(Guid.NewGuid().ToString(), Guid.Empty, tabConfigurationDetail);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Test PatchAsync method when tab configuration record does not exists for given tab id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_RecordNotexistForGivenTabId_ReturnsNotFound()
        {
            // ARRANGE
            var tabConfigurationDetail = new TabConfigurationViewModel
            {
                TeamId = "team 1",
                ChannelId = "channel 1",
                LearningModuleId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };

            this.unitOfWork.Setup(uow => uow.TabConfigurationRepository.FindAsync(It.IsAny<Expression<Func<TabConfiguration, bool>>>()))
                .ReturnsAsync(() => null);

            // ACT
            var result = (ObjectResult)await this.tabConfigurationController.PatchAsync(Guid.NewGuid().ToString(), tabConfigurationDetail.Id, tabConfigurationDetail);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }
    }
}