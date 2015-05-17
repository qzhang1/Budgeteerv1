using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}