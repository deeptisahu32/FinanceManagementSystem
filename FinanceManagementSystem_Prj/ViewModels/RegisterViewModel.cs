using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace FinanceManagementSystem_Prj.ViewModels
{
    public class RegisterViewModel
    {
        //        1. User registers(NO card type)
        //2. Admin reviews user
        //3. Admin approves user
        //4. Admin assigns CardType(Gold / Titanium)
        //5. EMI Card is created

         

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10 digit mobile number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters")]
        public string Username { get; set; }

        // 🔐 Password: capital + special + min 6
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[@#$!%&*])(?=.*[a-zA-Z0-9]).{6,}$",
            ErrorMessage = "Password must contain one capital letter, one special character and minimum 6 characters"
        )]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MinLength(3, ErrorMessage = "Address must be at least 10 characters")]
        public string Address { get; set; }

        // Bank Details
        [Required(ErrorMessage = "Bank name is required")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Account number is required")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "IFSC code is required")]
        public string IFSCCode { get; set; }


    }
}