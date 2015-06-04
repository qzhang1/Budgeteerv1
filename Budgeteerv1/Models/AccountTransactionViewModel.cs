using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class AccountTransactionViewModel
    {
        public IEnumerable<Budgeteerv1.Models.Account> Accounts { get; set; }
        public IEnumerable<Budgeteerv1.Models.Transaction> Transactions { get; set; }
    }
}