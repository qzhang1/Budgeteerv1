using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class ProgressBarViewModel
    {
        public string CategoryName { get; set; }
        public decimal AmtPercent { get; set; }
        public decimal BudgetPercent { get; set; }
    }
}