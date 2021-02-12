// <copyright file="GuidValidationAttribute.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Helpers.CustomValidations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    /// <summary>
    /// Validate input id is a valid GUID.
    /// </summary>
    public sealed class GuidValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate whether input id is a valid GUID.
        /// </summary>
        /// <param name="value">String containing input id like tab id etc.</param>
        /// <param name="validationContext">Context for getting object which needs to be validated.</param>
        /// <returns>Validation result (either error message for failed validation or success).</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var inputId = Convert.ToString(value, CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(inputId))
            {
                return new ValidationResult("Input id cannot be null or empty.");
            }

            if (!Guid.TryParse(inputId, out var validInputId))
            {
                return new ValidationResult($"Input id: {inputId} is not a valid GUID.");
            }

            if (validInputId == Guid.Empty)
            {
                return new ValidationResult($"Input id: {inputId} is a empty GUID.");
            }

            // Input id is a valid GUID.
            return ValidationResult.Success;
        }
    }
}
