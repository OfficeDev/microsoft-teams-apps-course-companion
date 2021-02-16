// <copyright file="UserResourceViewModelTest.cs" company="Microsoft Corporation">
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
    /// The UserResourceViewModelTest contains all the test cases for the user resource model validation.
    /// </summary>
    [TestClass]
    public class UserResourceViewModelTest
    {
        /// <summary>
        /// Test user resource modle validation for valid model.
        /// </summary>
        [TestMethod]
        public void UserResource_ValidModel_NoModelValidationError()
        {
            var userResourceViewModel = new UserResourceViewModel
            {
                ResourceId = Guid.NewGuid(),
            };
            Assert.IsTrue(!this.ValidateModel(userResourceViewModel).Any());
        }

        /// <summary>
        /// Test user respirce view model validaiton for empty resource id.
        /// </summary>
        [TestMethod]
        public void UserResource_EmptyResourceId_ModelValidationError()
        {
            var userResourceViewModel = new UserResourceViewModel
            {
                ResourceId = Guid.Empty,
            };
            var modelValidationResult = this.ValidateModel(userResourceViewModel);
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