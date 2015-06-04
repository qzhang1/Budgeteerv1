using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Budgeteerv1.Models;


namespace Budgeteerv1.Models
{
    public class ChartViewModel
    {

        public int? HouseholdId { get; set; }

        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int UserCount { get; set; }
        public decimal OverallBalance { get; set; }
        public int IncomeChange { get; set; }
        public int ExpenseChange { get; set; }
        
    }
}