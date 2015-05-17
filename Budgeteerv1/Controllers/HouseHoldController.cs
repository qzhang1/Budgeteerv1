using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeteerv1.Models;
using Microsoft.AspNet.Identity;
using System.Net;

namespace Budgeteerv1.Controllers
{
    //Create a household when an user registers OR
    //put a user in a household if they are accepting an invitation to a household
    public class HouseHoldController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


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
                db.HouseHolds.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        //public ActionResult LeaveHousehold(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    HouseHold hh = db.HouseHolds.Find(id);
        //    if (hh == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    if(hh.Users.Count == 1)
        //    {
        //        ViewBag.lastmember = "You are the last member of household " + hh.Name + " are you sure you wish to leave?";
        //    }
        //    return View("LeaveConfirmation");

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult LeaveConfirm(string userid, int? householdid)
        {
            //set user householdId to null if user is the last user then "delete" the household         
            var household = db.HouseHolds.Find(householdid);
            if (household.Users.Count == 1)
            {
                //remove household reference and delete household
                var user = db.Users.Find(userid);
                user.HouseHoldId = null;
                db.HouseHolds.Remove(household);
                db.SaveChanges();
            }
            else
            {
                //just remove household reference
                var user = db.Users.Find(userid);
                user.HouseHoldId = null;
            }

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
