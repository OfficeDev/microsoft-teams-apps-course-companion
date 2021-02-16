// <copyright file="UserLearningModuleViewModelTest.cs" company="Microsoft Corporation">
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
    /// The UserLearningModuleViewModelTest contains all the test cases for the user resource model validation.
    /// </summary>
    [TestClass]
    public class UserLearningModuleViewModelTest
    {
        /// <summary>
        /// Test user learning view model validaiton for valid model.
        /// </summary>
        [TestMethod]
        public void UserLearningModule_ValidModel_NoModelValidationError()
        {
            // Arrange- Setting valid learning module Id.
            var userLearningModuleViewModel = new UserLearningModuleViewModel
            {
                LearningModuleId = Guid.NewGuid(),
            };
            Assert.IsTrue(!this.ValidateModel(userLearningModuleViewModel).Any());
        }

        /// <summary>
        /// Test user learning view model validaiton for empty learning module id.
        /// </summary>
        [TestMethod]
        public void UserLearningModule_EmptyLearningModuleId_ModelValidationError()
        {
            // Arrange- Setting empty learning module Id.
            var userLearningModuleViewModel = new UserLearningModuleViewModel
            {
                LearningModuleId = Guid.Empty,
            };
            var modelValidationResult = this.ValidateModel(userLearningModuleViewModel);
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