// <copyright file="TagControllerTests.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.LearnNow.Controllers;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Users;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The TagControllerTests contains test cases for the tag controller.
    /// </summary>
    [TestClass]
    public class TagControllerTests
    {
        private Mock<ILogger<TagController>> logger;
        private TelemetryClient telemetryClient;
        private TagController tagController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<ITagMapper> tagMapper;
        private Mock<IUsersService> usersServiceMock;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<TagController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.tagMapper = new Mock<ITagMapper>();
            this.usersServiceMock = new Mock<IUsersService>();

            this.tagController = new TagController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.tagMapper.Object,
                this.usersServiceMock.Object)
            {
                ControllerContext = new ControllerContext(),
            };
            this.tagController.ControllerContext.HttpContext =
                FakeHttpContext.GetDefaultContextWithUserIdentity();
        }

        /// <summary>
        /// Test GetAsync method to get tag collection from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_GetTagRecords_ReturnsOkResult()
        {
            // ARRANGE
            var tagViewModel = new List<TagViewModel>();
            tagViewModel.Add(
                new TagViewModel()
                {
                    TagName = "tagName",
                    Id = Guid.NewGuid(),
                    UserDisplayName = "Test user",
                });

            this.unitOfWork.Setup(uow => uow.TagRepository.GetAllAsync()).Returns(Task.FromResult(FakeData.GetTags()));
            this.tagMapper.Setup(tagMapper => tagMapper.MapToViewModel(It.IsAny<IEnumerable<Tag>>(), It.IsAny<Dictionary<Guid, string>>())).Returns(tagViewModel);
            this.usersServiceMock.Setup(usersService => usersService.GetUserDisplayNamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(FakeData.GetUserDetails);

            // ACT
            var result = (ObjectResult)await this.tagController.GetAsync();
            var resultValue = (IEnumerable<TagViewModel>)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(tagViewModel.Count, resultValue.Count());
            Assert.AreEqual(tagViewModel.First().TagName, resultValue.First().TagName);
            Assert.AreEqual(tagViewModel.First().UserDisplayName, resultValue.First().UserDisplayName);
        }

        /// <summary>
        /// Test GetAsync method to get tag detail for provided Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_GetTagById_ReturnsOkResult()
        {
            // ARRANGE
            var tagId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.TagRepository.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(FakeData.GetTags().FirstOrDefault()));

            // ACT
            var result = (ObjectResult)await this.tagController.GetAsync(tagId);
            var resultValue = (Tag)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.TagName, FakeData.GetTags().FirstOrDefault().TagName);
            Assert.AreEqual(resultValue.UpdatedBy, FakeData.GetTags().FirstOrDefault().UpdatedBy);
            Assert.AreEqual(resultValue.Id, FakeData.GetTags().FirstOrDefault().Id);
        }

        /// <summary>
        /// Test GetAsync method for record not exists for given tag Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_RecordNotExists_ReturnsNotFoundResult()
        {
            // ARRANGE
            var tagId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.TagRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);

            // ACT
            var result = (StatusCodeResult)await this.tagController.GetAsync(tagId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PostAsync for saving tag details.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveTagDetail_ReturnsOkResult()
        {
            // ARRANGE
            var tag = new Tag
            {
                TagName = "Tag A",
                Id = Guid.NewGuid(),
            };
            var tagModel = new TagViewModel
            {
                TagName = "Tag A",
            };

            this.unitOfWork.Setup(uow => uow.TagRepository.Add(It.IsAny<Tag>())).Returns(() => tag);
            this.tagMapper.Setup(tagMapper => tagMapper.MapToDTO(It.IsAny<TagViewModel>(), It.IsAny<Guid>())).Returns(() => tag);

            // ACT
            var result = (ObjectResult)await this.tagController.PostAsync(tagModel);
            var resultValue = (Tag)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.TagName, tag.TagName);
            Assert.AreEqual(resultValue.Id, tag.Id);
        }

        /// <summary>
        /// Test PostAsync when record with same tag name already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_DuplicateTagName_ReturnsConflict()
        {
            // ARRANGE
            var tagCollection = new List<Tag>();
            var tag = new Tag
            {
                TagName = "Tag A",
                Id = Guid.NewGuid(),
            };
            var tagModel = new TagViewModel
            {
                TagName = "Tag A",
            };
            tagCollection.Add(tag);

            this.unitOfWork.Setup(uow => uow.TagRepository.FindAsync(It.IsAny<Expression<Func<Tag, bool>>>())).ReturnsAsync(tagCollection);

            // ACT
            var result = (StatusCodeResult)await this.tagController.PostAsync(tagModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test PatchAsync for updating tag details.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_UpdateTagDetail_ReturnsOkResult()
        {
            // ARRANGE
            var tagCollection = new List<Tag>();
            var tag = new Tag
            {
                TagName = "Tag A",
            };
            var tagModel = new TagViewModel
            {
                TagName = "Tag A New",
                Id = Guid.NewGuid(),
            };
            this.unitOfWork.Setup(uow => uow.TagRepository.FindAsync(It.IsAny<Expression<Func<Tag, bool>>>())).ReturnsAsync(tagCollection);
            this.unitOfWork.Setup(uow => uow.TagRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(tag);
            this.unitOfWork.Setup(uow => uow.TagRepository.Update(It.IsAny<Tag>())).Returns(() => tag);

            // ACT
            var result = (ObjectResult)await this.tagController.PatchAsync(tagModel.Id, tagModel);
            var resultValue = (Tag)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.TagName, tag.TagName);
            Assert.AreEqual(resultValue.Id, tag.Id);
        }

        /// <summary>
        /// Test PatchAsync method test when record not exists for given Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_RecordNotExists_ReturnsNotFound()
        {
            // ARRANGE
            var tagCollection = new List<Tag>();
            var tag = new Tag
            {
                TagName = "Tag A",
            };
            var tagModel = new TagViewModel
            {
                TagName = "Tag A",
                Id = Guid.NewGuid(),
            };
            this.unitOfWork.Setup(uow => uow.TagRepository.FindAsync(It.IsAny<Expression<Func<Tag, bool>>>())).ReturnsAsync(tagCollection);
            this.unitOfWork.Setup(uow => uow.TagRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.TagRepository.Update(It.IsAny<Tag>())).Returns(() => tag);

            // ACT
            var result = (ObjectResult)await this.tagController.PatchAsync(tagModel.Id, tagModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PatchAsync method test when record with same name already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_DuplicateTagName_ReturnsConflict()
        {
            // ARRANGE
            var tagCollection = new List<Tag>();
            var tag = new Tag
            {
                TagName = "Tag A",
            };
            var tagModel = new TagViewModel
            {
                TagName = "Tag A",
                Id = Guid.NewGuid(),
            };
            tagCollection.Add(tag);
            this.unitOfWork.Setup(uow => uow.TagRepository.FindAsync(It.IsAny<Expression<Func<Tag, bool>>>())).ReturnsAsync(tagCollection);
            this.unitOfWork.Setup(uow => uow.TagRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.TagRepository.Update(It.IsAny<Tag>())).Returns(() => tag);

            // ACT
            var result = (StatusCodeResult)await this.tagController.PatchAsync(tagModel.Id, tagModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test DeleteAsync method for deleting tag records from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteTagRecords_ReturnsOkResult()
        {
            // ARRANGE
            var tags = new List<Tag>
            {
                new Tag() { Id = Guid.NewGuid() },
            };

            var tagsCollection = new List<AdminConfigBaseModel>();
            foreach (var tag in tags)
            {
                tagsCollection.Add(new AdminConfigBaseModel() { Id = tag.Id });
            }

            this.unitOfWork.Setup(uow => uow.TagRepository.DeleteTags(It.IsAny<List<Tag>>()));

            // ACT
            var result = (ObjectResult)await this.tagController.DeleteAsync(tagsCollection);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }
    }
}