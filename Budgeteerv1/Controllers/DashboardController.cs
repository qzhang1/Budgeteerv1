using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Budgeteerv1.Models;
using Budgeteerv1.Models.CustomAttributes;
using Budgeteerv1.Models.helper;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using System.Data.Entity;
using Budgeteerv1.Models.extensions;
using Newtonsoft.Json;

namespace Budgeteerv1.Controllers
{
    [RequireHousehold]
    public class DashboardController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();
        ChartHelper ch = new ChartHelper();
        // GET: Dashboard
        public ActionResult Index()
        {
            
            ViewBag.ErrorMsg = (string)TempData["ErrorMsg"];
            var userId = User.Identity.GetUserId();
            var model = db.Users.Find(userId);            
            model.Notifications = db.Notifications.Where(a => a.ReceiverId == userId).ToList();
            return View(model);
        }

        [HttpGet]
        public JsonResult ItemCount(int householdId)
        {
            var hh = db.HouseHolds.Find(householdId);
            var accts = db.Accounts.Where(a => a.HouseHoldId == householdId).ToList();
            var ac = 0;
            foreach(var acct in accts)
            {
                ac += acct.Transactions.Count;
            }

            return Json(new { 
                TransCount = ac,
                BudgetCount = hh.Budgets.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProfileSettings()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Users.Find(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeProfile(ApplicationUser model, HttpPostedFileBase image)
        {
            //check if image is valid image file
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".gif")
                {
                    ModelState.AddModelError("image", "Invalid format.");
                }
            }

            if(ModelState.IsValid)
            {
                //relative server path
                if (image != null)
                {
                    var filePath = "/Images/";
                    var absPath = Server.MapPath("~" + filePath);
                    model.ProfileUrl = filePath + image.FileName;
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                try
                {
                    //db.Users.Attach(model);
                    //db.Entry(model).Property("DisplayName").IsModified = true;
                    //db.Entry(model).Property("ProfileUrl").IsModified = true;
                    //db.Entry(model).Property("Description").IsModified = true;
                    //db.Entry(model).Property("UserName").IsModified = true;
                    //db.Entry(model).State = EntityState.Modified;
                    var user = db.Users.Find(User.Identity.GetUserId());
                    if(model.ProfileUrl != null)
                    {
                        user.ProfileUrl = model.ProfileUrl;
                    }
                    
                    if(model.Description != null && model.DisplayName != null)
                    {
                        user.Description = model.Description;
                        user.DisplayName = model.DisplayName;
                    }
                    db.SaveChanges();
                }
                catch(DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                //ApplicationUser a = manager.FindById(User.Identity.GetUserId());

                //try
                //{
                //    if(image != null)
                //    {
                //        a.ProfileUrl = model.ProfileUrl;
                //    }
                    
                //    a.DisplayName = model.DisplayName;
                //    manager.Update(a);
                //}
                //catch (DbEntityValidationException dbEx)
                //{
                //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                //    {
                //        foreach (var validationError in validationErrors.ValidationErrors)
                //        {
                //            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                //        }
                //    }
                //}
            }
            return RedirectToAction("ProfileSettings");
        }

        public ActionResult ProfileImage()
        {
            return PartialView("_ProfileImage",db.Users.Find(User.Identity.GetUserId()));
        }


        /*
         *      Dashboard Data Charts
         * 
         * */
        //Index
        public ActionResult DataCharts()
        {
            var hhId = Int32.Parse(User.Identity.GetHouseholdId());
            var model = ch.Calculate(hhId);
            model.HouseholdId = hhId;
            return View(model);
        }


        [HttpGet]
        public ActionResult ChartData()
        {
            var hhId = Int32.Parse(User.Identity.GetHouseholdId());

            var household = db.HouseHolds.Find(hhId);

            //var monthsToDate = Enumerable.Range(1, DateTime.Today.Month)
            //                    .Select(m => new DateTime(DateTime.Today.Year, m, 1))
            //                    .ToList();

            var monthsToDate = Enumerable.Range(1, 12)
                                .Select(m => new DateTime(DateTime.Today.Year, m, 1))
                                .ToList();

            var sums = from month in monthsToDate
                       select new
                       {
                           month = month,

                           income = (from account in household.Accounts
                                     from transaction in account.Transactions
                                     where transaction.IsIncome && transaction.Created.Month == month.Month
                                     select transaction.Amount).DefaultIfEmpty().Sum(),

                           expense = (from account in household.Accounts
                                      from transaction in account.Transactions
                                      where !transaction.IsIncome && transaction.Created.Month == month.Month
                                      select transaction.Amount).DefaultIfEmpty().Sum(),

                           //budget = household.Budgets.Where(i => i.IsIncome)
                           //.Where(c => c.Created.Month >= month.Month && 
                           //    c.Created.Month <= (month.Month + ((c.Created.Month + c.Frequency >12)? (12-c.Frequency):(c.Created.Month + c.Frequency))))
                           //    .Select(b => b.Amount * (1/b.Frequency)).DefaultIfEmpty().Sum()
                           budgetexpense = (from budget in household.Budgets
                                     where budget.Created.Month <= month.Month && (budget.Created.Month + budget.Frequency) >= month.Month
                                     where !budget.IsIncome
                                     select budget.Amount/budget.Frequency).DefaultIfEmpty().Sum(),

                           budgetincome = (from budget in household.Budgets
                                            where budget.Created.Month <= month.Month && (budget.Created.Month + budget.Frequency) >= month.Month
                                            where budget.IsIncome
                                            select budget.Amount / budget.Frequency).DefaultIfEmpty().Sum()
                       };

            var flotData = new {
                income = sums.ToDictionary( k=> k.month, v=>v.income),
                expense = sums.ToDictionary( k=> k.month, v=>v.expense),
                budgetexpense = sums.ToDictionary( k=> k.month, v=>v.budgetexpense),
                budgetincome = sums.ToDictionary( k=> k.month, v=>v.budgetincome)
            };

            return Content( JsonConvert.SerializeObject(flotData),"application/json");

        }

        [HttpGet]
        public ActionResult fillmydonut()
        {
            var hhId = Int32.Parse(User.Identity.GetHouseholdId());
            var household = db.HouseHolds.Find(hhId);
            var categories = household.Categories.ToList();
            var donutData = from category in categories
                            select new
                            {
                                category = category.Name,

                                amount = (from account in household.Accounts
                                          from transaction in account.Transactions
                                          where transaction.Category.Name == category.Name
                                          select transaction.Amount).DefaultIfEmpty().Sum()
                            };
            var x = 1;
            return Content(JsonConvert.SerializeObject(donutData), "application/json");
        }

        [HttpGet]
        public ActionResult drawmyline()
        {
            //objective: find the balance at the end of each month and plot them as points in a line chart
            //balance = income - expense for each acct for each month
            //steps: find household generate month enumerable 

        }


        public ActionResult RecentTransactions()
        {
            var hhId = Int32.Parse(User.Identity.GetHouseholdId());
            var household = db.HouseHolds.Find(hhId);

            var RecentTrans = (from account in household.Accounts
                               from transaction in account.Transactions
                               orderby transaction.Created descending
                               select transaction).Take(5);
            var RecentAcct = (from account in household.Accounts
                              select account).Take(3);

            var model = new AccountTransactionViewModel
            {
                Transactions = RecentTrans,
                Accounts = RecentAcct
            };

            return PartialView("_RecentTransactions", model);
        }
    }
}