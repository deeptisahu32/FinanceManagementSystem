using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanceManagementSystem_Prj.Validators
{
    public class AgeValidator
    {
        public static ValidationResult ValidateDOB(DateTime dob, ValidationContext context)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            if (age < 18)
                return new ValidationResult("User must be at least 18 years old to apply for EMI");

            return ValidationResult.Success;
        }
    }
}