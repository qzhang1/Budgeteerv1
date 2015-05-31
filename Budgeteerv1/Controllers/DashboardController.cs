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


        [HttpGet]
        public JsonResult ChartData()
        {
            //var income = db.Transactions.Where(i => i.IsIncome == true).Where(c => c.Created.Month == System.DateTime.Today.Month).Sum(s => s.Amount);
            return Json(new[] { 500, 1000, 2000, 3000, 4000, 3000}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProfileSettings()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Users.Find(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeProfile([Bind(Include="ProfileUrl,Description,DisplayName")]ApplicationUser model, HttpPostedFileBase image)
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
                    user.ProfileUrl = model.ProfileUrl;
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

        public ActionResult DataCharts()
        {
            int? hhId = db.Users.Find(User.Identity.GetUserId()).HouseHoldId;
            var model = ch.Calculate(hhId);
            return View(model);
        }
    }
}