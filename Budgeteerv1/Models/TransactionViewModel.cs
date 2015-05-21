using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class TransactionViewModel
    {
        public int AccountId {get; set;}
        public Transaction Transaction { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}