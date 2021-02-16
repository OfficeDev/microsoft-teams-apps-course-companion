// <copyright file="SubjectModelTest.cs" company="Microsoft Corporation">
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
    /// The SubjectModelTest contains all the test cases for the subject model validation.
    /// </summary>
    [TestClass]
    public class SubjectModelTest
    {
        /// <summary>
        /// Test subject model validation for valid model.
        /// </summary>
        [TestMethod]
        public void SubjectModelValidationTest_ValidModel_NoValidationError()
        {
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "Subject C",
            };
            Assert.IsTrue(!this.ValidateModel(subjectModel).Any());
        }

        /// <summary>
        /// Test subject model validation when subject name exceeds 25 character.
        /// </summary>
        [TestMethod]
        public void SubjectModelValidationTest_SubjectNameGreaterThen25Character_ModelValidationError()
        {
            var subjectModel = new SubjectViewModel
            {
                SubjectName = "TestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTest",
            };
            Assert.IsTrue(this.ValidateModel(subjectModel).Any(
                v => v.MemberNames.Contains("SubjectName") &&
                     v.ErrorMessage.Contains("length")));
        }

        /// <summary>
        /// Test subject model validation when mandatory property subject name is not present in subject object.
        /// </summary>
        [TestMethod]
        public void SubjectModelValidationTest_EmptySubjectName_ModelValidationError()
        {
            // Arrange
            var subjectModel = new SubjectViewModel
            {
            };
            Assert.IsTrue(this.ValidateModel(subjectModel).Any(
                v => v.MemberNames.Contains("SubjectName") &&
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