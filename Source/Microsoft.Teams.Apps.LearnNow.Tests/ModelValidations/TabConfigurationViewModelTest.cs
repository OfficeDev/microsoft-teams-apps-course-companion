// <copyright file="TabConfigurationViewModelTest.cs" company="Microsoft Corporation">
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
    /// The TabConfigurationViewModelTest contains test cases for the tab configuration model validation.
    /// </summary>
    [TestClass]
    public class TabConfigurationViewModelTest
    {
        /// <summary>
        /// Test whether tab configuration can be created with correct model.
        /// </summary>
        [TestMethod]
        public void TabConfigurationViewModel_ValidModel_NoValidationError()
        {
            var teamPreferenceViewModel = new TabConfigurationViewModel
            {
                TeamId = "test",
                ChannelId = "test",
                LearningModuleId = Guid.NewGuid(),
            };
            Assert.IsTrue(!this.ValidateModel(teamPreferenceViewModel).Any());
        }

        /// <summary>
        /// Test tab configuration required field model validations.
        /// </summary>
        [TestMethod]
        public void TabConfigurationViewModel_MissingRequiredField_ModelValidationError()
        {
            var teamPreferenceViewModel = new TabConfigurationViewModel
            {
                LearningModuleId = Guid.NewGuid(),
            };
            var modelValidationResult = this.ValidateModel(teamPreferenceViewModel);
            Assert.IsTrue(modelValidationResult.Any(
                v => v.MemberNames.Contains("ChannelId") && v.ErrorMessage.Contains("required")));
            Assert.IsTrue(modelValidationResult.Any(
                v => v.MemberNames.Contains("TeamId") && v.ErrorMessage.Contains("required")));
        }

        /// <summary>
        /// Test tab configuration learning module Id guid validations.
        /// </summary>
        [TestMethod]
        public void TabConfigurationViewModel_EmptyLearningModuleIDGuid_ModelValidationError()
        {
            var teamPreferenceViewModel = new TabConfigurationViewModel
            {
                TeamId = "test",
                ChannelId = "test",
                LearningModuleId = Guid.Empty,
            };
            var modelValidationResult = this.ValidateModel(teamPreferenceViewModel);
            Assert.IsTrue(modelValidationResult.Any(
                v => v.ErrorMessage.Contains("empty GUID")));
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