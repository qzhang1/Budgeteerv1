using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeteerv1.Models.helper
{
    public class householdHelpers
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void AddCategory(int householdId, string CatName)
        {
            var hh = db.HouseHolds.Find(householdId);
            var tmp = new Category
            {
                Name = CatName,
                HouseholdId = householdId
            };
            db.Categories.Add(tmp);
            hh.Categories.Add(tmp);
            db.SaveChanges();
        }
    }
}