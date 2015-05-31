using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class Notification
    {
        public string Name { get; set; }
        public int NotificationId { get; set; }
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public int? HouseholdId { get; set; }

        public virtual ApplicationUser Receiver { get; set; }
        public virtual ApplicationUser Sender { get; set; }

    }
}