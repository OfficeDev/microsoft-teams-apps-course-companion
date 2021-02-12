// <copyright file="ResourceControllerTests.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.GroupMembers;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The ResourceControllerTests contains all the test cases for the resource CRUD operations.
    /// </summary>
    [TestClass]
    public class ResourceControllerTests
    {
        private Mock<ILogger<ResourceController>> logger;
        private TelemetryClient telemetryClient;
        private ResourceController resourceController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<IResourceMapper> resourceMapper;
        private Mock<IUsersService> usersServiceMock;
        private ITokenHelper accessTokenHelper;
        private Mock<IMemberValidationService> memberValidationService;
        private IOptions<SecurityGroupSettings> securityGroupOptions;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<ResourceController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.resourceMapper = new Mock<IResourceMapper>();
            this.usersServiceMock = new Mock<IUsersService>();
            this.memberValidationService = new Mock<IMemberValidationService>();
            this.accessTokenHelper = new FakeAccessTokenHelper();
            this.securityGroupOptions = Options.Create(new SecurityGroupSettings());

            this.resourceController = new ResourceController(
                this.unitOfWork.Object,
                this.telemetryClient,
                this.logger.Object,
                this.usersServiceMock.Object,
                this.resourceMapper.Object,
                this.memberValidationService.Object,
                this.securityGroupOptions)
            {
                ControllerContext = new ControllerContext(),
            };

            this.resourceController.ControllerContext.HttpContext =
                    FakeHttpContext.GetDefaultContextWithUserIdentity();
        }

        /// <summary>
        /// Test GetResourceDetailAsync returns resource detail for Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetResourceDetailAsync_ResourceExistsForId_ReturnsOkResult()
        {
            var resourceId = Guid.Parse(FakeData.Id);
            this.unitOfWork.Setup(uow => uow.ResourceVoteRepository.FindAsync(It.IsAny<Expression<Func<ResourceVote, bool>>>())).ReturnsAsync(FakeData.GetResourceVotes());
            this.resourceMapper.Setup(resourceMapper => resourceMapper.MapToViewModel(It.IsAny<Resource>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<ResourceVote>>(), It.IsAny<Dictionary<Guid, string>>())).Returns(FakeData.GetPayLoadResource);
            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(FakeData.GetResources().FirstOrDefault()));

            // ACT
            var result = (ObjectResult)await this.resourceController.GetResourceDetailAsync(resourceId);
            var resultValue = (ResourceViewModel)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.Title, FakeData.GetPayLoadResource().Title);
        }

        /// <summary>
        /// Test PostAsync method for saving resource detail to storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveResource_ReturnsOkResult()
        {
            var resourceModel = new ResourceViewModel
            {
                Id = Guid.NewGuid(),
                Title = "Test title",
                Description = "Test description",
                ImageUrl = "https://test.jpg",
                ResourceType = 1,
            };
            this.resourceMapper.Setup(resourceMapper => resourceMapper.MapToDTO(It.IsAny<ResourceViewModel>(), It.IsAny<Guid>())).Returns(FakeData.GetResources().FirstOrDefault());
            this.unitOfWork.Setup(uow => uow.ResourceRepository.Add(It.IsAny<Resource>())).Returns(FakeData.GetResources().FirstOrDefault());
            this.resourceMapper.Setup(resourceMapper => resourceMapper.MapToViewModel(It.IsAny<Resource>(), It.IsAny<Dictionary<Guid, string>>())).Returns(resourceModel);

            // ACT
            var result = (ObjectResult)await this.resourceController.PostAsync(resourceModel);
            var resultValue = (ResourceViewModel)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.Title, resourceModel.Title);
        }

        /// <summary>
        /// Test PatchAsync method for updating the resource detail in storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_ResourceUpdate_ReturnsOkResult()
        {
            var resourceTagss = new List<ResourceTag>();
            var resourceTag = new ResourceTag()
            {
                TagId = Guid.Parse(FakeData.Id),
                ResourceId = Guid.Parse(FakeData.Id),
            };
            resourceTagss.Add(resourceTag);
            var resourceModel = new ResourceViewModel
            {
                Id = Guid.Parse(FakeData.Id),
                Title = "Test title",
                Description = "Test description",
                ImageUrl = "https://test.jpg",
                ResourceType = 1,
                ResourceTag = resourceTagss,
            };
            var resourceId = Guid.Parse(FakeData.Id);
            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(FakeData.GetResources().FirstOrDefault()));
            this.unitOfWork.Setup(uow => uow.ResourceTagRepository.Delete(It.IsAny<ResourceTag>()));
            this.unitOfWork.Setup(uow => uow.ResourceVoteRepository.FindAsync(It.IsAny<Expression<Func<ResourceVote, bool>>>())).ReturnsAsync(FakeData.GetResourceVotes());
            this.resourceMapper.Setup(resourceMapper => resourceMapper.PatchAndMapToDTO(It.IsAny<ResourceViewModel>(), It.IsAny<Guid>())).Returns(FakeData.GetResources().FirstOrDefault());
            this.unitOfWork.Setup(uow => uow.ResourceRepository.Update(It.IsAny<Resource>())).Returns(FakeData.GetResources().FirstOrDefault());
            this.resourceMapper.Setup(resourceMapper => resourceMapper.PatchAndMapToViewModel(It.IsAny<Resource>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<ResourceVote>>(), It.IsAny<Dictionary<Guid, string>>())).Returns(resourceModel);

            // ACT
            var result = (ObjectResult)await this.resourceController.PatchAsync(Guid.Parse(FakeData.Id), resourceModel);
            var resultValue = (ResourceViewModel)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.Title, resourceModel.Title);
        }

        /// <summary>
        /// Test PatchAsync method when resource not exists for given Id .
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_ResourceNotExistsForId_ReturnsBadRequest()
        {
            var resourceModel = new ResourceViewModel
            {
                Id = Guid.Parse(FakeData.Id),
                Title = "Test title",
                Description = "Test description",
                ImageUrl = "https://test.jpg",
                ResourceType = 1,
            };
            var resourceId = Guid.Parse(FakeData.Id);
            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(resourceId)).Returns(Task.FromResult(FakeData.GetResources().FirstOrDefault()));
            this.unitOfWork.Setup(uow => uow.ResourceTagRepository.Delete(FakeData.GetResources().FirstOrDefault().ResourceTag));
            this.resourceMapper.Setup(resourceMapper => resourceMapper.PatchAndMapToDTO(resourceModel, Guid.Parse(FakeData.UserID))).Returns(FakeData.GetResources().FirstOrDefault());

            // ACT
            var result = (ObjectResult)await this.resourceController.PatchAsync(Guid.NewGuid(), resourceModel);

            // ASSERT
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        /// <summary>
        /// Test whether we can delete resource details
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteModule_ReturnsOkResult()
        {
            // ARRANGE
            var resourceId = Guid.Parse(FakeData.Id);
            var resourceVotes = new List<ResourceVote>();
            var resourceModuleMapping = new List<ResourceModuleMapping>();
            var userModulesMapping = new List<UserResource>();
            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(FakeData.GetResources().FirstOrDefault);
            this.unitOfWork.Setup(uow => uow.ResourceTagRepository.Delete(It.IsAny<ResourceTag>()));
            this.unitOfWork.Setup(uow => uow.ResourceRepository.Delete(It.IsAny<Resource>()));
            this.unitOfWork.Setup(uow => uow.ResourceVoteRepository.FindAsync(It.IsAny<Expression<Func<ResourceVote, bool>>>())).ReturnsAsync(resourceVotes);
            this.unitOfWork.Setup(uow => uow.ResourceModuleRepository.FindAsync(It.IsAny<Expression<Func<ResourceModuleMapping, bool>>>())).ReturnsAsync(resourceModuleMapping);
            this.unitOfWork.Setup(uow => uow.UserResourceRepository.FindAsync(It.IsAny<Expression<Func<UserResource, bool>>>())).ReturnsAsync(userModulesMapping);

            // ACT
            var result = (ObjectResult)await this.resourceController.DeleteAsync(resourceId);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test DeleteAsync method.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_ResourceNotExistForId_ReturnsNotFound()
        {
            // ARRANGE
            var resourceId = Guid.Parse(FakeData.Id);
            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(resourceId)).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.ResourceTagRepository.Delete(FakeData.GetResources().FirstOrDefault().ResourceTag));
            this.unitOfWork.Setup(uow => uow.ResourceRepository.Delete(FakeData.GetResources().FirstOrDefault()));

            // ACT
            var result_resourceNotfound = (ObjectResult)await this.resourceController.DeleteAsync(resourceId);

            // ASSERT
            Assert.AreEqual(result_resourceNotfound.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PostVoteAsync method.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostVoteAsync_SaveResourceVote_ReturnsOkResult()
        {
            var resourceId = Guid.Parse(FakeData.Id);
            ResourceVote vote = new ResourceVote
            {
                ResourceId = resourceId,
                UserId = Guid.Parse(FakeData.UserID),
                CreatedOn = DateTime.UtcNow,
            };
            this.unitOfWork.Setup(uow => uow.ResourceVoteRepository.Add(It.IsAny<ResourceVote>())).Returns(vote);

            // ACT
            var result = (ObjectResult)await this.resourceController.PostVoteAsync(resourceId);
            var resultValue = result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }

        /// <summary>
        /// Test PostVoteAsync method.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostVoteAsync_EmptyResourceId_ReturnsBadRequest()
        {
            var resourceId = Guid.Empty;
            ResourceVote vote = new ResourceVote
            {
                ResourceId = resourceId,
                UserId = Guid.Parse(FakeData.UserID),
                CreatedOn = DateTime.UtcNow,
            };
            this.unitOfWork.Setup(uow => uow.ResourceVoteRepository.Add(It.IsAny<ResourceVote>())).Returns(vote);

            // ACT
            var result = (ObjectResult)await this.resourceController.PostVoteAsync(resourceId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Test DeleteVoteAsync method.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteVoteAsync_DeleteResourceVote_ReturnsOkResult()
        {
            // ARRANGE
            var resourceId = Guid.Parse(FakeData.Id);
            var userID = Guid.Parse(FakeData.UserID);
            this.unitOfWork.Setup(uow => uow.ResourceVoteRepository.FindAsync(It.IsAny<Expression<Func<ResourceVote, bool>>>())).ReturnsAsync(FakeData.GetResourceVotes());

            // ACT
            var result = (ObjectResult)await this.resourceController.DeleteVoteAsync(resourceId);
            var resuletValue = result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resuletValue, true);
        }

        /// <summary>
        /// Test whether user trying to update resource which is not created by him
        /// and user is also not an administrator. It should returns unauthorized error response.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_ResourceUpdateWithNonAdminUser_ReturnsUnauthorizedResult()
        {
            // ARRANGE
            var resourceTagss = new List<ResourceTag>();
            var resourceTag = new ResourceTag()
            {
                TagId = Guid.Parse(FakeData.Id),
                ResourceId = Guid.Parse(FakeData.Id),
            };
            resourceTagss.Add(resourceTag);
            var resourceModel = new ResourceViewModel
            {
                Id = Guid.Parse(FakeData.Id),
                Title = "Test title",
                Description = "Test description",
                ImageUrl = "https://test.jpg",
                ResourceType = 1,
                ResourceTag = resourceTagss,
            };
            var resource = FakeData.GetResource();
            resource.CreatedBy = Guid.NewGuid();

            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(resource);
            this.memberValidationService.Setup(validationService => validationService.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            this.securityGroupOptions.Value.AdminGroupId = Guid.NewGuid().ToString();

            // ACT
            var result = (ObjectResult)await this.resourceController.PatchAsync(Guid.Parse(FakeData.Id), resourceModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status401Unauthorized);
        }

        /// <summary>
        /// Test whether user trying to delete resource which is not created by him
        /// and user is also not an administrator. It should returns unauthorized error response.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_ResourceDeleteWithNonAdminUser_ReturnsUnauthorizedResult()
        {
            // ARRANGE
            var resource = FakeData.GetResource();
            resource.CreatedBy = Guid.NewGuid();

            this.unitOfWork.Setup(uow => uow.ResourceRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(resource);
            this.memberValidationService.Setup(validationService => validationService.ValidateMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            this.securityGroupOptions.Value.AdminGroupId = Guid.NewGuid().ToString();

            // ACT
            var result = (ObjectResult)await this.resourceController.DeleteAsync(Guid.Parse(FakeData.Id));

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status401Unauthorized);
        }
    }
}