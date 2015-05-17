using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class HouseHold
    {
        public HouseHold()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Accounts = new HashSet<Account>();
            this.Budgets = new HashSet<BudgetItem>();
            this.Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<BudgetItem> Budgets { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        
    }
}