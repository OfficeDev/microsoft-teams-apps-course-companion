// <copyright file="SubjectControllerTests.cs" company="Microsoft Corporation">
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
    /// The SubjectControllerTests contains test cases for the subject controller.
    /// </summary>
    [TestClass]
    public class SubjectControllerTests
    {
        private Mock<ILogger<SubjectController>> logger;
        private TelemetryClient telemetryClient;
        private SubjectController subjectController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<ISubjectMapper> subjectMapper;
        private Mock<IUsersService> usersServiceMock;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<SubjectController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.subjectMapper = new Mock<ISubjectMapper>();
            this.usersServiceMock = new Mock<IUsersService>();

            this.subjectController = new SubjectController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.subjectMapper.Object,
                this.usersServiceMock.Object)
            {
                ControllerContext = new ControllerContext(),
            };
            this.subjectController.ControllerContext.HttpContext =
                FakeHttpContext.GetDefaultContextWithUserIdentity();
        }

        /// <summary>
        /// Test GetAsync method to get subject collection from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_GetSubjectRecords_ReturnsOkResult()
        {
            // ARRANGE
            var subjectViewModel = new List<SubjectViewModel>();
            subjectViewModel.Add(
                new SubjectViewModel()
                {
                    SubjectName = "subjectName",
                    Id = Guid.NewGuid(),
                    UserDisplayName = "Test user",
                });

            this.unitOfWork.Setup(uow => uow.SubjectRepository.GetAllAsync()).Returns(Task.FromResult(FakeData.GetSubjects()));
            this.subjectMapper.Setup(subjectMapper => subjectMapper.MapToViewModel(It.IsAny<IEnumerable<Subject>>(), It.IsAny<Dictionary<Guid, string>>())).Returns(subjectViewModel);
            this.usersServiceMock.Setup(usersService => usersService.GetUserDisplayNamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(FakeData.GetUserDetails);

            // ACT
            var result = (ObjectResult)await this.subjectController.GetAsync();
            var resultValue = (IEnumerable<SubjectViewModel>)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(subjectViewModel.Count, resultValue.Count());
            Assert.AreEqual(subjectViewModel.First().SubjectName, resultValue.First().SubjectName);
            Assert.AreEqual(subjectViewModel.First().UserDisplayName, resultValue.First().UserDisplayName);
        }

        /// <summary>
        /// Test GetAsync method to get subject detail for provided Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_GetSubjectById_ReturnsOkResult()
        {
            // ARRANGE
            var subjectId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.SubjectRepository.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(FakeData.GetSubjects().FirstOrDefault()));

            // ACT
            var result = (ObjectResult)await this.subjectController.GetAsync(subjectId);
            var resultValue = (Subject)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.SubjectName, FakeData.GetSubjects().FirstOrDefault().SubjectName);
            Assert.AreEqual(resultValue.UpdatedBy, FakeData.GetSubjects().FirstOrDefault().UpdatedBy);
            Assert.AreEqual(resultValue.Id, FakeData.GetSubjects().FirstOrDefault().Id);
        }

        /// <summary>
        /// Test GetAsync method for record not exists for given subject Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_RecordNotExists_ReturnsNotFoundResult()
        {
            // ARRANGE
            var subjectId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.SubjectRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);

            // ACT
            var result = (StatusCodeResult)await this.subjectController.GetAsync(subjectId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PostAsync for saving subject details.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveSubjectDetail_ReturnsOkResult()
        {
            // ARRANGE
            var subject = new Subject
            {
                SubjectName = "Subject A",
                Id = Guid.NewGuid(),
            };
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "Subject A",
            };

            this.unitOfWork.Setup(uow => uow.SubjectRepository.Add(It.IsAny<Subject>())).Returns(() => subject);
            this.subjectMapper.Setup(subjectMapper => subjectMapper.MapToDTO(It.IsAny<SubjectViewModel>(), It.IsAny<Guid>())).Returns(() => subject);

            // ACT
            var result = (ObjectResult)await this.subjectController.PostAsync(subjectModel);
            var resultValue = (Subject)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.SubjectName, subject.SubjectName);
            Assert.AreEqual(resultValue.Id, subject.Id);
        }

        /// <summary>
        /// Test PostAsync when record with same subject name already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_DuplicateSubjectName_ReturnsConflict()
        {
            // ARRANGE
            var subjectCollection = new List<Subject>();
            var subject = new Subject
            {
                SubjectName = "Subject A",
                Id = Guid.NewGuid(),
            };
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "Subject A",
            };
            subjectCollection.Add(subject);

            this.unitOfWork.Setup(uow => uow.SubjectRepository.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(subjectCollection);

            // ACT
            var result = (StatusCodeResult)await this.subjectController.PostAsync(subjectModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test PatchAsync for updating subject details.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_UpdateSubjectDetail_ReturnsOkResult()
        {
            // ARRANGE
            var subjectCollection = new List<Subject>();
            var subject = new Subject
            {
                SubjectName = "Subject A",
            };
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "Subject A New",
                Id = Guid.NewGuid(),
            };
            this.unitOfWork.Setup(uow => uow.SubjectRepository.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(subjectCollection);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(subject);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.Update(It.IsAny<Subject>())).Returns(() => subject);

            // ACT
            var result = (ObjectResult)await this.subjectController.PatchAsync(subjectModel.Id, subjectModel);
            var resultValue = (Subject)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.SubjectName, subject.SubjectName);
            Assert.AreEqual(resultValue.Id, subject.Id);
        }

        /// <summary>
        /// Test PatchAsync method test when record not exists for given Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_RecordNotExists_ReturnsNotFound()
        {
            // ARRANGE
            var subjectCollection = new List<Subject>();
            var subject = new Subject
            {
                SubjectName = "Subject A",
            };
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "Subject A",
                Id = Guid.NewGuid(),
            };
            this.unitOfWork.Setup(uow => uow.SubjectRepository.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(subjectCollection);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.Update(It.IsAny<Subject>())).Returns(() => subject);

            // ACT
            var result = (ObjectResult)await this.subjectController.PatchAsync(subjectModel.Id, subjectModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PatchAsync method test when record with same name already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_DuplicateSubjectName_ReturnsConflict()
        {
            // ARRANGE
            var subjectCollection = new List<Subject>();
            var subject = new Subject
            {
                SubjectName = "Subject A",
            };
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "Subject A",
                Id = Guid.NewGuid(),
            };
            subjectCollection.Add(subject);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.FindAsync(It.IsAny<Expression<Func<Subject, bool>>>())).ReturnsAsync(subjectCollection);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.SubjectRepository.Update(It.IsAny<Subject>())).Returns(() => subject);

            // ACT
            var result = (StatusCodeResult)await this.subjectController.PatchAsync(subjectModel.Id, subjectModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test DeleteAsync method for deleting subject records from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteSubjectRecords_ReturnsOkResult()
        {
            // ARRANGE
            var subjects = new List<Subject>
            {
                new Subject() { Id = Guid.NewGuid() },
            };

            var subjectsCollection = new List<AdminConfigBaseModel>();
            foreach (var subject in subjects)
            {
                subjectsCollection.Add(new AdminConfigBaseModel() { Id = subject.Id });
            }

            this.unitOfWork.Setup(uow => uow.SubjectRepository.DeleteSubjects(It.IsAny<List<Subject>>()));

            // ACT
            var result = (ObjectResult)await this.subjectController.DeleteAsync(subjectsCollection);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }
    }
}