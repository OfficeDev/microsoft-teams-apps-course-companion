// <copyright file="GradeControllerTests.cs" company="Microsoft Corporation">
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
    /// The GradeControllerTests contains test cases for the grade controller.
    /// </summary>
    [TestClass]
    public class GradeControllerTests
    {
        private Mock<ILogger<GradeController>> logger;
        private TelemetryClient telemetryClient;
        private GradeController gradeController;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<IGradeMapper> gradeMapper;
        private Mock<IUsersService> usersServiceMock;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<GradeController>>();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            this.unitOfWork = new Mock<IUnitOfWork>();
            this.gradeMapper = new Mock<IGradeMapper>();
            this.usersServiceMock = new Mock<IUsersService>();

            this.gradeController = new GradeController(
                this.logger.Object,
                this.telemetryClient,
                this.unitOfWork.Object,
                this.gradeMapper.Object,
                this.usersServiceMock.Object)
            {
                ControllerContext = new ControllerContext(),
            };
            this.gradeController.ControllerContext.HttpContext =
                FakeHttpContext.GetDefaultContextWithUserIdentity();
        }

        /// <summary>
        /// Test GetAsync method to get grade collection from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_GetGradeRecords_ReturnsOkResult()
        {
            // ARRANGE
            var gradeViewModel = new List<GradeViewModel>();
            gradeViewModel.Add(
                new GradeViewModel()
                {
                    GradeName = "gradeName",
                    Id = Guid.NewGuid(),
                    UserDisplayName = "Test user",
                });

            this.unitOfWork.Setup(uow => uow.GradeRepository.GetAllAsync()).Returns(Task.FromResult(FakeData.GetGrades()));
            this.gradeMapper.Setup(gradeMapper => gradeMapper.MapToViewModel(It.IsAny<IEnumerable<Grade>>(), It.IsAny<Dictionary<Guid, string>>())).Returns(gradeViewModel);
            this.usersServiceMock.Setup(usersService => usersService.GetUserDisplayNamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(FakeData.GetUserDetails);

            // ACT
            var result = (ObjectResult)await this.gradeController.GetAsync();
            var resultValue = (IEnumerable<GradeViewModel>)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(gradeViewModel.Count, resultValue.Count());
            Assert.AreEqual(gradeViewModel.First().GradeName, resultValue.First().GradeName);
            Assert.AreEqual(gradeViewModel.First().UserDisplayName, resultValue.First().UserDisplayName);
        }

        /// <summary>
        /// Test GetAsync method to get grade detail for provided Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_GetGradeById_ReturnsOkResult()
        {
            // ARRANGE
            var gradeId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.GradeRepository.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(FakeData.GetGrades().FirstOrDefault()));

            // ACT
            var result = (ObjectResult)await this.gradeController.GetAsync(gradeId);
            var resultValue = (Grade)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.GradeName, FakeData.GetGrades().FirstOrDefault().GradeName);
            Assert.AreEqual(resultValue.UpdatedBy, FakeData.GetGrades().FirstOrDefault().UpdatedBy);
            Assert.AreEqual(resultValue.Id, FakeData.GetGrades().FirstOrDefault().Id);
        }

        /// <summary>
        /// Test GetAsync method for record not exists for given grade Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetAsync_RecordNotExists_ReturnsNotFoundResult()
        {
            // ARRANGE
            var gradeId = Guid.NewGuid();
            this.unitOfWork.Setup(uow => uow.GradeRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);

            // ACT
            var result = (StatusCodeResult)await this.gradeController.GetAsync(gradeId);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PostAsync for saving grade details.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_SaveGradeDetail_ReturnsOkResult()
        {
            // ARRANGE
            var grade = new Grade
            {
                GradeName = "Grade A",
                Id = Guid.NewGuid(),
            };
            var gradeModel = new GradeViewModel
            {
                GradeName = "Grade A",
            };

            this.unitOfWork.Setup(uow => uow.GradeRepository.Add(It.IsAny<Grade>())).Returns(() => grade);
            this.gradeMapper.Setup(gradeMapper => gradeMapper.MapToDTO(It.IsAny<GradeViewModel>(), It.IsAny<Guid>())).Returns(() => grade);

            // ACT
            var result = (ObjectResult)await this.gradeController.PostAsync(gradeModel);
            var resultValue = (Grade)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.GradeName, grade.GradeName);
            Assert.AreEqual(resultValue.Id, grade.Id);
        }

        /// <summary>
        /// Test PostAsync when record with same grade name already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PostAsync_DuplicateGradeName_ReturnsConflict()
        {
            // ARRANGE
            var gradeCollection = new List<Grade>();
            var grade = new Grade
            {
                GradeName = "Grade A",
                Id = Guid.NewGuid(),
            };
            var gradeModel = new GradeViewModel
            {
                GradeName = "Grade A",
            };
            gradeCollection.Add(grade);

            this.unitOfWork.Setup(uow => uow.GradeRepository.FindAsync(It.IsAny<Expression<Func<Grade, bool>>>())).ReturnsAsync(gradeCollection);

            // ACT
            var result = (StatusCodeResult)await this.gradeController.PostAsync(gradeModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test PatchAsync for updating grade details.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_UpdateGradeDetail_ReturnsOkResult()
        {
            // ARRANGE
            var gradeCollection = new List<Grade>();
            var grade = new Grade
            {
                GradeName = "Grade A",
            };
            var gradeModel = new GradeViewModel
            {
                GradeName = "Grade A New",
                Id = Guid.NewGuid(),
            };
            this.unitOfWork.Setup(uow => uow.GradeRepository.FindAsync(It.IsAny<Expression<Func<Grade, bool>>>())).ReturnsAsync(gradeCollection);
            this.unitOfWork.Setup(uow => uow.GradeRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(grade);
            this.unitOfWork.Setup(uow => uow.GradeRepository.Update(It.IsAny<Grade>())).Returns(() => grade);

            // ACT
            var result = (ObjectResult)await this.gradeController.PatchAsync(gradeModel.Id, gradeModel);
            var resultValue = (Grade)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue.GradeName, grade.GradeName);
            Assert.AreEqual(resultValue.Id, grade.Id);
        }

        /// <summary>
        /// Test PatchAsync method test when record not exists for given Id.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_RecordNotExists_ReturnsNotFound()
        {
            // ARRANGE
            var gradeCollection = new List<Grade>();
            var grade = new Grade
            {
                GradeName = "Grade A",
            };
            var gradeModel = new GradeViewModel
            {
                GradeName = "Grade A",
                Id = Guid.NewGuid(),
            };
            this.unitOfWork.Setup(uow => uow.GradeRepository.FindAsync(It.IsAny<Expression<Func<Grade, bool>>>())).ReturnsAsync(gradeCollection);
            this.unitOfWork.Setup(uow => uow.GradeRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.GradeRepository.Update(It.IsAny<Grade>())).Returns(() => grade);

            // ACT
            var result = (ObjectResult)await this.gradeController.PatchAsync(gradeModel.Id, gradeModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Test PatchAsync method test when record with same name already exists.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task PatchAsync_DuplicateGradeName_ReturnsConflict()
        {
            // ARRANGE
            var gradeCollection = new List<Grade>();
            var grade = new Grade
            {
                GradeName = "Grade A",
            };
            var gradeModel = new GradeViewModel
            {
                GradeName = "Grade A",
                Id = Guid.NewGuid(),
            };
            gradeCollection.Add(grade);
            this.unitOfWork.Setup(uow => uow.GradeRepository.FindAsync(It.IsAny<Expression<Func<Grade, bool>>>())).ReturnsAsync(gradeCollection);
            this.unitOfWork.Setup(uow => uow.GradeRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            this.unitOfWork.Setup(uow => uow.GradeRepository.Update(It.IsAny<Grade>())).Returns(() => grade);

            // ACT
            var result = (StatusCodeResult)await this.gradeController.PatchAsync(gradeModel.Id, gradeModel);

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
        }

        /// <summary>
        /// Test DeleteAsync method for deleting grade records from storage.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DeleteAsync_DeleteGradeRecords_ReturnsOkResult()
        {
            // ARRANGE
            var grades = new List<Grade>
            {
                new Grade() { Id = Guid.NewGuid() },
            };

            var gradesCollection = new List<AdminConfigBaseModel>();
            foreach (var grade in grades)
            {
                gradesCollection.Add(new AdminConfigBaseModel() { Id = grade.Id });
            }

            this.unitOfWork.Setup(uow => uow.GradeRepository.DeleteGrades(It.IsAny<List<Grade>>()));

            // ACT
            var result = (ObjectResult)await this.gradeController.DeleteAsync(gradesCollection);
            var resultValue = (bool)result.Value;

            // ASSERT
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
            Assert.AreEqual(resultValue, true);
        }
    }
}