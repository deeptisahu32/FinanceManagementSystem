using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanceManagementSystem_Prj.ViewModels
{
    public class VerifyOtpViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}