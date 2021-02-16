// <copyright file="ResourceModelTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.ModelValidations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.Teams.Apps.LearnNow.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The ResourceModelTest contains all the test cases for the resource model validation.
    /// </summary>
    [TestClass]
    public class ResourceModelTest
    {
        /// <summary>
        /// Test whether resource can be created with correct model.
        /// </summary>
        [TestMethod]
        public void ResourceModelValidationTest_CorrectModel_NoValidationError()
        {
            var resource = new ResourceViewModel
            {
                Title = "Resource C",
                Description = "test description",
                SubjectId = Guid.NewGuid(),
                GradeId = Guid.NewGuid(),
                ImageUrl = "https://www.google.com/",
                LinkUrl = "https://www.google.com/",
                AttachmentUrl = "https://www.google.com/",
                ResourceType = 1,
            };
            Assert.IsTrue(!this.ValidateModel(resource).Any());
        }

        /// <summary>
        /// Test resource title length validation.
        /// </summary>
        [TestMethod]
        public void ResourceModelValidationTest_ResourceNameMaxLength_ValidationError()
        {
            var resource = new ResourceViewModel
            {
                Title = "Resource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource CResource C",
                Description = "test description",
                SubjectId = Guid.NewGuid(),
                GradeId = Guid.NewGuid(),
                ImageUrl = "https://www.google.com/",
                LinkUrl = "https://www.google.com/",
                AttachmentUrl = "https://www.google.com/",
            };
            Assert.IsTrue(this.ValidateModel(resource).Any(
                v => v.MemberNames.Contains("Title") &&
                     v.ErrorMessage.Contains("length")));
        }

        /// <summary>
        /// Test LinkUrl regular expression.
        /// </summary>
        [TestMethod]
        public void ResourceModelValidationTest_InValidURL_ValidationError()
        {
            var resource = new ResourceViewModel
            {
                Title = "Resource C",
                Description = "test description",
                SubjectId = Guid.NewGuid(),
                GradeId = Guid.NewGuid(),
                ImageUrl = "https://www.google.com/",
                LinkUrl = "Test url",
                AttachmentUrl = "https://www.google.com/",
                ResourceType = 1,
            };
            Assert.IsTrue(this.ValidateModel(resource).Any(
                v => v.MemberNames.Contains("LinkUrl") &&
                     v.ErrorMessage.Contains("regular expression")));
        }

        /// <summary>
        /// Test missing field validation.
        /// </summary>
        [TestMethod]
        public void ResourceModelValidationTest_MissingRequiredField_ValidationError()
        {
            var resource = new ResourceViewModel
            {
                Description = "test description",
                ImageUrl = "https://www.google.com/",
                ResourceType = 5,
            };
            Assert.IsTrue(this.ValidateModel(resource).Any());
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}