using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models
{
    public class LineChartViewModel
    {
        public DateTime Month { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
    }
}