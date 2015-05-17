using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class Account
    {
        public Account()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        public decimal Reconciled { get; set; }

        public int? HouseHoldId { get; set; }

        public virtual HouseHold HouseHold { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}