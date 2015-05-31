using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class BudgetViewModel
    {
        public BudgetItem BudgetItem { get; set; }
        public IList<BudgetItem> Income { get; set; }
        public IList<BudgetItem> Expenses { get; set; }
        public int householdId { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}