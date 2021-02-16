// <copyright file="GradeModelTest.cs" company="Microsoft Corporation">
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
    /// The GradeModelTest contains all the test cases for the grade model validation.
    /// </summary>
    [TestClass]
    public class GradeModelTest
    {
        /// <summary>
        /// Test grade model validation for valid model.
        /// </summary>
        [TestMethod]
        public void GradeModelValidationTest_ValidModel_NoValidationError()
        {
            var gradeModel = new GradeViewModel
            {
                GradeName = "Grade C",
            };
            Assert.IsTrue(!this.ValidateModel(gradeModel).Any());
        }

        /// <summary>
        /// Test grade model validation when grade name exceeds 25 character.
        /// </summary>
        [TestMethod]
        public void GradeModelValidationTest_GradeNameGreaterThen25Character_ModelValidationError()
        {
            var gradeModel = new GradeViewModel
            {
                GradeName = "TestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTest",
            };
            Assert.IsTrue(this.ValidateModel(gradeModel).Any(
                v => v.MemberNames.Contains("GradeName") &&
                     v.ErrorMessage.Contains("length")));
        }

        /// <summary>
        /// Test grade model validation when mandatory property grade name is not present in grade object.
        /// </summary>
        [TestMethod]
        public void GradeModelValidationTest_EmptyGradeName_ModelValidationError()
        {
            // Arrange
            var gradeModel = new GradeViewModel
            {
            };
            Assert.IsTrue(this.ValidateModel(gradeModel).Any(
                v => v.MemberNames.Contains("GradeName") &&
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