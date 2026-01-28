using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManagementSystem_Prj.ViewModels
{
    public class PendingUserViewModel
    {
        // User table
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        // Bank details table
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
    }
}