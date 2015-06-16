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
using System.Net;

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
            if(model.HouseHoldId == null)
            {
                return View("Landing", model);
            }
            return View(model);
        }


        public ActionResult FillProgressBar()
        {
            var hhId = Int32.Parse(User.Identity.GetHouseholdId());
            var household = db.HouseHolds.Find(hhId);
            //var categories = household.Categories.ToList();
            var stuff = from category in household.Categories
                        select new
                        {
                            cat = category.Name,
                            amt = (from acct in household.Accounts
                                   from trans in acct.Transactions
                                   where trans.CategoryId == category.Id
                                   where !trans.IsIncome
                                   where trans.Created.Month == System.DateTime.Now.Month
                                   select trans.Amount).DefaultIfEmpty().Sum(),
                            budgetamt = (from budget in household.Budgets
                                         where budget.CategoryId == category.Id
                                         where budget.Amount != 0
                                         select budget.Amount/budget.Frequency).DefaultIfEmpty().Sum()
                        };
            var tmp = from a in stuff
                    where a.amt != 0
                    select new
                    {
                        cat = a.cat,
                        amt = (a.amt),
                        budgetamt = a.budgetamt
                    };

            IEnumerable<ProgressBarViewModel> p = from s in tmp 
                                                  select new ProgressBarViewModel
                                                  {
                                                      CategoryName = s.cat,                                                      
                                                      AmtPercent = s.amt,
                                                      BudgetPercent = s.budgetamt
                                                  };
            
            var x = 0;
            return Content(JsonConvert.SerializeObject(p), "application/json");
        }


        public ActionResult DashNotification()
        {
            //var hhId = Int32.Parse(User.Identity.GetHouseholdId());
            var userid = User.Identity.GetUserId();
            IEnumerable<Notification> notifications = db.Notifications.Where(h => h.ReceiverId == userid).Take(5).ToList();
            return PartialView("_DashNotification", notifications);
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
            ViewBag.ChangedPasswordSuccess = TempData["ChangedPasswordSuccess"];
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
                
            }
            return RedirectToAction("ProfileSettings");
        }

        public ActionResult ProfileImage()
        {
            return PartialView("_ProfileImage",db.Users.Find(User.Identity.GetUserId()));
        }
     
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
                           
                           budgetexpense = (from budget in household.Budgets
                                     where budget.Created.Month <= month.Month && (budget.Created.Month + budget.Frequency) >= month.Month
                                     where !budget.IsIncome
                                     select budget.Amount/budget.Frequency).DefaultIfEmpty().Sum(),

                           budgetincome = (from budget in household.Budgets
                                            where budget.Created.Month <= month.Month && (budget.Created.Month + budget.Frequency) > month.Month
                                            where budget.IsIncome
                                            select budget.Amount / budget.Frequency).DefaultIfEmpty().Sum()
                       };

            
                      
            var flotData = new {
                income = sums.ToDictionary( k=> k.month, v=>v.income),
                expense = sums.ToDictionary( k=> k.month, v=>v.expense),
                budgetexpense = sums.ToDictionary( k=> k.month, v=>v.budgetexpense),
                budgetincome = sums.ToDictionary( k=> k.month, v=>v.budgetincome),
               
                
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
            
            return Content(JsonConvert.SerializeObject(donutData), "application/json");
        }


        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }

        public ActionResult RecentTransactions()
        {
            var userId = (User.Identity.GetUserId());
            var hhId = db.Users.FirstOrDefault(u => u.Id == userId).HouseHoldId;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}