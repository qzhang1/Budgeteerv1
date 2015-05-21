using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeteerv1.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using Budgeteerv1.Models.helper;
using Budgeteerv1.Models.extensions;
using Budgeteerv1.Models.CustomAttributes;
using System.Threading.Tasks;

namespace Budgeteerv1.Controllers
{
    //Create a household when an user registers OR
    //put a user in a household if they are accepting an invitation to a household
    public class HouseHoldController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        householdHelpers helper = new householdHelpers();

        // GET: HouseHold
        public ActionResult Index(int id)
        {
            return View();
        }

        // GET: HouseHold/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HouseHold/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HouseHold/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(HouseHold model)
        {
            if(ModelState.IsValid)
            {
                //if current user creates a household let them automatically join the household
                var userId = User.Identity.GetUserId();
                model.Users.Add(db.Users.Find(userId));                
                model.IsDeleted = false;
                db.HouseHolds.Add(model);
                db.SaveChanges();

                //add default category(s)
                helper.AddCategory(model.Id, "Adjustment");

                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        //Ajax form
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHousehold]
        public ActionResult Invite(ApplicationUser model)
        {
            if(ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                var note = new Notification
                {
                    ReceiverId = db.Users.FirstOrDefault(u => u.Email == model.Email).Id,
                    Created = System.DateTime.Now,
                    Message = user.DisplayName + " has invited you to Household " + user.HouseHold.Name + ".",
                    HouseholdId = model.HouseHoldId,
                    Name = "Invite"
                };
                db.Notifications.Add(note);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Join(int noteId, int? householdId)
        {
            if(ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                if(user.HouseHoldId != null)
                {
                    ViewBag.ErrorMsg = "You are already in a Household. You can't join another Household until you leave the current Household.";
                    return RedirectToAction("Index", "Dashboard");
                }
                var household = db.HouseHolds.Find(householdId);
                if(household != null)
                {
                    //if user is not already in a household and the household they are
                    //joining exists then add user to the household and delete the notification
                    household.Users.Add(user);
                    db.Notifications.Remove(db.Notifications.Find(noteId));
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHousehold]
        public async Task<ActionResult> LeaveConfirm(string userid, int? householdid)
        {
            //set all "user" references to the household to null and toggle IsDeleted to true  if the user leaving is the Last user  
            //all the other connections to household like budgetitem,accounts,and categories should stay except users
            var household = db.HouseHolds.Find(householdid);
            var user = db.Users.Find(userid);
            if (household.Users.Count == 1)
            {
                //remove household references and toggle IsDeleted                
                user.HouseHoldId = null;
                household.IsDeleted = true;
                db.Entry(user).Property("HouseHoldId").IsModified = true;
                db.SaveChanges();
            }
            else
            {
                //just remove household reference               
                user.HouseHoldId = null;
                db.Entry(user).Property("HouseHoldId").IsModified = true;
                db.SaveChanges();
            }

            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Index","Dashboard");
        }

        // GET: HouseHold/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HouseHold/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HouseHold/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HouseHold/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
