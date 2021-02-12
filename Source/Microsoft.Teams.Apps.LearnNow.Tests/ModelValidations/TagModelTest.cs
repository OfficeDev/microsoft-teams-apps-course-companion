// <copyright file="TagModelTest.cs" company="Microsoft Corporation">
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
    /// The TagModelTest contains all the test cases for the tag model validation.
    /// </summary>
    [TestClass]
    public class TagModelTest
    {
        /// <summary>
        /// Test tag model validation for valid model.
        /// </summary>
        [TestMethod]
        public void TagModelValidationTest_ValidModel_NoValidationError()
        {
            var tagModel = new TagViewModel
            {
                TagName = "Tag C",
            };
            Assert.IsTrue(!this.ValidateModel(tagModel).Any());
        }

        /// <summary>
        /// Test tag model validation when tag name exceeds 25 character.
        /// </summary>
        [TestMethod]
        public void TagModelValidationTest_TagNameGreaterThen25Character_ModelValidationError()
        {
            var tagModel = new TagViewModel
            {
                TagName = "TestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTest",
            };
            Assert.IsTrue(this.ValidateModel(tagModel).Any(
                v => v.MemberNames.Contains("TagName") &&
                     v.ErrorMessage.Contains("length")));
        }

        /// <summary>
        /// Test tag model validation when mandatory property tag name is not present in tag object.
        /// </summary>
        [TestMethod]
        public void TagModelValidationTest_EmptyTagName_ModelValidationError()
        {
            // Arrange
            var tagModel = new TagViewModel
            {
            };
            Assert.IsTrue(this.ValidateModel(tagModel).Any(
                v => v.MemberNames.Contains("TagName") &&
                     v.ErrorMessage.Contains("required")));
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