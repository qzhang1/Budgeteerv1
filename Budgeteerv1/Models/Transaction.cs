using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Budgeteerv1.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Description { get; set; }
        public decimal Reconciled { get; set; }
        public bool IsIncome { get; set; }

        public string UpdatedById { get; set; }
        public int AccountId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Account  Account { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }
    }
}