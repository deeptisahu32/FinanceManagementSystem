using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanceManagementSystem_Prj.ViewModels
{
    public class ForgotPasswordPhoneViewModel
    {
        
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
    }
}