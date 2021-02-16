// <copyright file="ResourceMapperTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Mappers
{
    using System;
    using System.Linq;
    using Microsoft.Teams.Apps.LearnNow.ModelMappers;
    using Microsoft.Teams.Apps.LearnNow.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The ResourceMapperTest contains all the test cases for the resource model mappers operations.
    /// </summary>
    [TestClass]
    public class ResourceMapperTest
    {
        private ResourceMapper resourceMapper;

        /// <summary>
        /// Method for testing PatchAndMapToDTO method from mapper.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.resourceMapper = new ResourceMapper();
        }

        /// <summary>
        ///  Method for testing MapToDTO method from mapper.
        /// </summary>
        [TestMethod]
        public void MapToDTOTest()
        {
            // ACT
            var result = this.resourceMapper.MapToDTO(FakeData.GetPayLoadResource(), Guid.Parse(FakeData.UserID));

            // ASSERT
            Assert.AreEqual(FakeData.GetResource().Id, result.Id);
            Assert.AreEqual(FakeData.GetResource().Title, result.Title);
            Assert.AreEqual(FakeData.GetResource().CreatedBy, result.CreatedBy);
        }

        /// <summary>
        /// Method for testing MapToViewModel method from mapper.
        /// </summary>
        [TestMethod]
        public void MapToViewModelTest()
        {
            // ACT
            var result = this.resourceMapper.MapToViewModel(FakeData.GetResource(), FakeData.GetUserDetails());

            // ASSERT
            Assert.AreEqual(FakeData.GetResource().Id, result.Id);
            Assert.AreEqual(FakeData.GetResource().Title, result.Title);
            Assert.AreEqual(FakeData.GetResource().CreatedBy, result.CreatedBy);
            Assert.AreEqual(FakeData.GetUserDetails().First().Value, result.UserDisplayName);
        }

        /// <summary>
        /// Method for testing PatchAndMapToDTO method from mapper.
        /// </summary>
        [TestMethod]
        public void PatchAndMapToDTOTest()
        {
            // ACT
            var result = this.resourceMapper.PatchAndMapToDTO(FakeData.GetPayLoadResource(), Guid.NewGuid());

            // ASSERT
            Assert.AreEqual(FakeData.GetResource().Id, result.Id);
            Assert.AreEqual(FakeData.GetResource().Title, result.Title);
            Assert.AreEqual(FakeData.GetResource().CreatedBy, result.CreatedBy);
        }

        /// <summary>
        /// Method for testing PatchAndMapToViewModel method from mapper.
        /// </summary>
        [TestMethod]
        public void PatchAndMapToViewModelTest()
        {
            // ACT
            var result = this.resourceMapper.PatchAndMapToViewModel(FakeData.GetResource(), Guid.Parse(FakeData.UserID), FakeData.GetResourceVotes(), FakeData.GetUserDetails());

            // ASSERT
            Assert.AreEqual(FakeData.GetResource().Id, result.Id);
            Assert.AreEqual(FakeData.GetResource().Title, result.Title);
            Assert.AreEqual(FakeData.GetResource().CreatedBy, result.CreatedBy);
            Assert.AreEqual(true, result.IsLikedByUser);
            Assert.AreEqual(FakeData.GetResourceVotes().Count(), result.VoteCount);
            Assert.AreEqual(FakeData.GetUserDetails().First().Value, result.UserDisplayName);
        }
    }
}