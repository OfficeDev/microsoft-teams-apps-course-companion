// <copyright file="LearningModuleControllerTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// This class contains unit test cases of learning module controller.
    /// </summary>
    [TestClass]
    public class LearningModuleControllerTests
    {
        private Mock<ILogger<LearningModuleController>> logger;
        private TelemetryClient telemetryClient;
        private LearningModuleController learningModuleController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<IUsersService> usersService;
        private Mock<ILearningModuleMapper> learningModuleMapper;
        private Mock<IResourceModuleMapper> resourceModuleMapper;
        private Mock<IResourceMapper> resourcMapper;
        private Mock<IMemberValidationService> memberValidationService;
        private IOptions<SecurityGroupSettings> securityGroupOptions;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<LearningModuleController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.learningModuleMapper = new Mock<ILearningModuleMapper>();
            this.resourceModuleMapper = new Mock<IResourceModuleMapper>();
            this.usersService = new Mock<IUsersService>();
            this.resourcMapper = new Mock<IResourceMapper>();
            this.memberValidationService = new Mock<IMemberValidationService>();
            this.securityGroupOptions = Options.Create<SecurityGroupSettings>(new SecurityGroupSettings());

            this.learningModuleController = new LearningModuleController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.usersService.Object,
                this.learningModuleMapper.Object,
                this.resourceModuleMapper.Object,
                this.resourcMapper.Object,
                this.memberValidationService.Object,
                this.securityGroupOptions)
            {
                ControllerContext = new ControllerContext(),
            };
            this.learningModuleController.ControllerContext.HttpContext =
                    FakeHttpContext.GetDefaultContextWithUserIdentity();
        }

        /// <summary>.
        /// Test GetLearningModuleDetailAsync to return module detail for given module Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetLearningModuleDetailAsync_ModuleExistsForId_ReturnsOkResult()
        {
            // ARRANGE
            var learningModuleId = Guid.Parse(FakeData.Id);
            var lmMapping = new List<ResourceModuleMapping>();
            var moduleVotes = new List<LearningModuleVote>();
            IEnumerable<LearningModuleViewModel> modules = new List<LearningModuleViewModel>
            {
                new LearningModuleViewModel
                {
                    Title = FakeData.GetLearningModules().FirstOrDefault().Title,
                    Id = learningModuleId,
                },
            };
            Dictionary<Guid, List<ResourceDetailModel>> resourcesWithVotes = new Dictionary<Guid, List<ResourceDetailModel>>();
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(FakeData.GetLearningModules().FirstOrDefault);
            this.unitOfWork.Setup(uow => uow.LearningModuleVoteRepository.FindAsync(It.IsAny<Expression<Func<LearningModuleVote, bool>>>())).ReturnsAsync(moduleVotes);
            this.unitOfWork.Setup(uow => uow.ResourceModuleRepository.FindAsync(It.IsAny<Expression<Func<ResourceModuleMapping, bool>>>())).ReturnsAsync(lmMapping);
            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetResourcesWithVotes(It.IsAny<IEnumerable<Resource>>())).Returns(resourcesWithVotes);
            this.learningModuleMapper.Setup(learningModuleMapper => learningModuleMapper.MapToViewModel(It.IsAny<LearningModule>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<LearningModuleVote>>(), It.IsAny<Dictionary<Guid, string>>())).Returns(FakeData.GetPayLoadLearningModule());

            // ACT
            var result = (ObjectResult)await this.learningModuleController.GetLearningModuleDetailAsync(learningModuleId);
            var resultValue = (ModuleResourceViewModel)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.LearningModule.Id, learningModuleId);
            Assert.AreEqual(resultValue.LearningModule.Title, FakeData.GetLearningModules().FirstOrDefault().Title);
            Assert.AreEqual(resultValue.Resources.Count(), 0);
        }

        /// <summary>
        /// Test whether we can get all details by page count
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task SearchAsync_ExactMatchTrue_ReturnsOkResult()
        {
            // ARRANGE
            IEnumerable<LearningModuleViewModel> modules = new List<LearningModuleViewModel>
            {
                new LearningModuleViewModel
                {
                    Title = FakeData.GetLearningModules().FirstOrDefault().Title,
                },
            };
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.GetLearningModulesAsync(It.IsAny<FilterModel>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(FakeData.GetLearningModules);
            this.learningModuleMapper.Setup(learningModuleMapper => learningModuleMapper.MapToViewModels(It.IsAny<Dictionary<Guid, List<LearningModuleDetailModel>>>(), It.IsAny<Guid>(), It.IsAny<Dictionary<Guid, string>>())).Returns(modules);
            var filterModel = new FilterModel();

            // ACT
            var result = (ObjectResult)await this.learningModuleController.SearchAsync(1, true, true, filterModel);
            var resultValue = (IEnumerable<LearningModuleViewModel>)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.Count(), 1);
            Assert.AreEqual(resultValue.FirstOrDefault().Title, FakeData.GetLearningModules().FirstOrDefault().Title);
        }

        /// <summary>
        /// Test whether we can post learning module details
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveLearningModule_ReturnsOkResult()
        {
            // ARRANGE
            var learningModule = new LearningModuleViewModel
            {
                Title = "Test title",
                Description = "Test description",
                ImageUrl = "https://test.jpg",
            };
            this.learningModuleMapper.Setup(resourceMapper => resourceMapper.MapToDTO(It.IsAny<LearningModuleViewModel>(), It.IsAny<Guid>())).Returns(FakeData.GetLearningModules().FirstOrDefault());
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.Add(It.IsAny<LearningModule>())).Returns(FakeData.GetLearningModules().FirstOrDefault());
            this.learningModuleMapper.Setup(resourceMapper => resourceMapper.MapToViewModel(It.IsAny<LearningModule>(), It.IsAny<Dictionary<Guid, string>>())).Returns(learningModule);
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(FakeData.GetLearningModules().FirstOrDefault);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.PostAsync(learningModule);
            var resultValue = (LearningModuleViewModel)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.Title, learningModule.Title);
            Assert.AreEqual(resultValue.Description, learningModule.Description);
            Assert.AreEqual(resultValue.ImageUrl, learningModule.ImageUrl);
        }

        /// <summary>
        /// Test whether we can delete learning module details
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteModule_ReturnsOkResult()
        {
            // ARRANGE
            var moduleId = Guid.Parse(FakeData.Id);
            var moduleVotes = new List<LearningModuleVote>();
            var resourceModuleMapping = new List<ResourceModuleMapping>();
            var userModulesMapping = new List<UserLearningModule>();
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(FakeData.GetLearningModules().FirstOrDefault);
            this.unitOfWork.Setup(uow => uow.LearningModuleTagRepository.Delete(It.IsAny<LearningModuleTag>()));
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.Delete(It.IsAny<LearningModule>()));
            this.unitOfWork.Setup(uow => uow.LearningModuleVoteRepository.FindAsync(It.IsAny<Expression<Func<LearningModuleVote, bool>>>())).ReturnsAsync(moduleVotes);
            this.unitOfWork.Setup(uow => uow.ResourceModuleRepository.FindAsync(It.IsAny<Expression<Func<ResourceModuleMapping, bool>>>())).ReturnsAsync(resourceModuleMapping);
            this.unitOfWork.Setup(uow => uow.UserLearningModuleRepository.FindAsync(It.IsAny<Expression<Func<UserLearningModule, bool>>>())).ReturnsAsync(userModulesMapping);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.DeleteAsync(moduleId);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test whether we can post resource module mapping
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostResourceModuleMappingAsync_SaveResourceModuleMapping_ReturnsOkResult()
        {
            // ARRANGE
            var learningModuleId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var resourceModuleViewModel = new ResourceModuleViewModel
            {
                LearningModuleId = learningModuleId,
                ResourceId = resourceId,
            };
            this.unitOfWork.Setup(uow => uow.ResourceModuleRepository.Add(It.IsAny<ResourceModuleMapping>())).Returns(() => null);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.PostResourceModuleMappingAsync(resourceModuleViewModel);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test whether we can post votes in learning module
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostVoteAsync_SaveModuleVote_ReturnsOkResult()
        {
            // ARRANGE
            var learningModuleId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.LearningModuleVoteRepository.Add(It.IsAny<LearningModuleVote>())).Returns(() => null);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.PostVoteAsync(learningModuleId);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test whether we can delete votes in learning module
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteVoteAsync_DeleteModuleVote_ReturnsOkResult()
        {
            // ARRANGE
            var learningModuleId = Guid.NewGuid();

            this.unitOfWork.Setup(uow => uow.LearningModuleVoteRepository.FindAsync(It.IsAny<Expression<Func<LearningModuleVote, bool>>>())).ReturnsAsync(FakeData.GetLearningModuleVotes);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.DeleteVoteAsync(learningModuleId);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test whether we can delete votes in learning module
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteVoteAsync_ModuleVoteNotFound_ReturnsOkResult()
        {
            // ARRANGE
            var learningModuleId = Guid.NewGuid();
            var lmVotes = new List<LearningModuleVote>();

            this.unitOfWork.Setup(uow => uow.LearningModuleVoteRepository.FindAsync(It.IsAny<Expression<Func<LearningModuleVote, bool>>>())).ReturnsAsync(lmVotes);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.DeleteVoteAsync(learningModuleId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
        }

        /// <summary>
        /// Test whether user trying to update learning module which is not created by him
        /// and user is also not an administrator. It should returns unauthorized error response.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_UpdateLearningModuleWithNonAdminUser_ReturnsUnauthorizedResult()
        {
            // ARRANGE
            var learningModuleModel = new ResourceModuleViewPatchModel
            {
                LearningModule = new LearningModuleViewModel
                {
                    Title = "Test title",
                    Description = "Test description",
                    ImageUrl = "https://test.jpg",
                },
            };
            var module = FakeData.GetLearningModule();
            module.CreatedBy = Guid.NewGuid();
            this.memberValidationService.Setup(validationService => validationService.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(module);

            // ACT
            var result = (ObjectResult)await this.learningModuleController.PatchAsync(Guid.NewGuid(), learningModuleModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status401Unauthorized);
        }

        /// <summary>
        /// Test whether user trying to delete learning module which is not created by him
        /// and user is also not an administrator. It should returns unauthorized error response.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteLearningModuleWithNonAdminUser_ReturnsUnauthorizedResult()
        {
            // ARRANGE
            var module = FakeData.GetLearningModule();
            module.CreatedBy = Guid.NewGuid();
            this.memberValidationService.Setup(validationService => validationService.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            this.unitOfWork.Setup(uow => uow.LearningModuleRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(module);
            this.securityGroupOptions.Value.AdminGroupId = Guid.NewGuid().ToString();

            // ACT
            var result = (ObjectResult)await this.learningModuleController.DeleteAsync(Guid.NewGuid());

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status401Unauthorized);
        }
    }
}