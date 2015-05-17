using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Frequency { get; set; }
        public bool IsIncome { get; set; }
        public int? CategoryId { get; set; }
        public int HouseHoldId { get; set; }
        public DateTime Created { get; set; }

        public virtual Category Category { get; set; }
        public virtual HouseHold HouseHold { get; set; }
    }
}