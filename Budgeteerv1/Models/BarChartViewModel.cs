using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Budgeteerv1.Models;

namespace Budgeteerv1.Models
{
    public class BarChartViewModel
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public int? HouseholdId { get; set; }

        public decimal[] ActualIncome { get; set; }
        public decimal[] ActualExpense { get; set; }
        public decimal[] BudgetIncome { get; set; }
        public decimal[] BudgetExpense { get; set; }

        //sum of actual income for each month
        //sum the incomes from month 1-12 => index 0-11
        public BarChartViewModel(int hhId)
        {
            var household = context.HouseHolds.Find(hhId);

            var monthsToDate = Enumerable.Range(1, DateTime.Today.Month)
                                .Select(m => new DateTime(DateTime.Today.Year, m, 1))
                                .ToList();


            var sums = from month in monthsToDate
                       select new { 
                            income = (from account in household.Accounts
                                     from transaction in account.Transactions
                                     where transaction.IsIncome && transaction.Created.Month == month.Month
                                     select transaction.Amount).DefaultIfEmpty().Sum(),

                            expense = (from account in household.Accounts
                                     from transaction in account.Transactions
                                     where !transaction.IsIncome && transaction.Created.Month == month.Month
                                     select transaction.Amount).DefaultIfEmpty().Sum(),

                            budget = household.Budgets.Select( b => b.Amount * (b.Frequency / 12)).DefaultIfEmpty().Sum()
                       };

            var x = 1;
        }

    }
}