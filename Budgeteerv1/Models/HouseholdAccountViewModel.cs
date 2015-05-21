using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class HouseholdAccountViewModel
    {
        public int Householdid { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public Account Account { get; set; }
    }
}