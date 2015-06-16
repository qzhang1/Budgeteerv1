using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Budgeteerv1.Models.extensions;
using Budgeteerv1.Models;
namespace Budgeteerv1.Models.helper
{
    public class ChartHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();


        //calculate all positive and negative transactions and overall balance
        //calculates total users in current household
        public ChartViewModel Calculate(int? hhId)
        {
            decimal OverallBalance = 0;
            decimal MonthlyIncome = 0;
            decimal MonthlyExpense = 0;
            decimal IncomeChange = 0;
            decimal ExpenseChange = 0;
            
            //find all the accounts in hh. if it's a new household with no accounts then return 0
            var hh = db.HouseHolds.Find(hhId);
            if(hh.Accounts.Count == 0)
            {
                return new ChartViewModel { UserCount = 0, MonthlyExpenses = 0, MonthlyIncome = 0, IncomeChange = 0, OverallBalance = 0, ExpenseChange = 0 };
            }


            var accts = hh.Accounts.ToList();
            int usercount = hh.Users.Count;
            //var tmp = accts[1].Transactions.Where(i => i.IsIncome == false).Sum(a => a.Amount);
            //error: currently calculating all income and expense transactions not just the ones for the current month
            foreach(var a in accts)
            {
                MonthlyExpense += a.Transactions.Where(i => i.IsIncome == false).Where(c => c.Created.Month == System.DateTime.Today.Month).Sum(s => s.Amount);
                ExpenseChange += a.Transactions.Where(i => i.IsIncome == false).Where(c => c.Created.Month == (System.DateTime.Today.Month - 1)).Sum(s => s.Amount);
                MonthlyIncome += a.Transactions.Where(i => i.IsIncome == true).Where(c => c.Created.Month == System.DateTime.Today.Month).Sum(s => s.Amount);
                IncomeChange += a.Transactions.Where(i => i.IsIncome == true).Where(c => c.Created.Month == (System.DateTime.Today.Month - 1)).Sum(s => s.Amount);
                OverallBalance += a.Balance;
                
            }
            IncomeChange = (IncomeChange == 0)? 0:100*(IncomeChange - MonthlyIncome) / (MonthlyIncome);
            ExpenseChange = (ExpenseChange == 0) ? 0 : 100*(ExpenseChange - MonthlyExpense) / (MonthlyExpense);
            
            return new ChartViewModel { UserCount = usercount, MonthlyExpenses = MonthlyExpense, MonthlyIncome = MonthlyIncome, OverallBalance = OverallBalance,
                ExpenseChange =(int)ExpenseChange, IncomeChange=(int)IncomeChange };

        }
       
    }
}