using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeteerv1.Models;
using Budgeteerv1.Models.CustomAttributes;
using Microsoft.AspNet.Identity;
using System.Net;

namespace Budgeteerv1.Controllers
{
    [RequireHousehold]
    public class HouseAccountController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: HouseAccount
        public ActionResult Index(int householdid)
        {
            if(householdid == 0)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var model = new HouseholdAccountViewModel
            {
                Accounts = db.Accounts.Where(a => a.HouseHoldId == householdid).ToList(),
                Householdid = householdid
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(HouseholdAccountViewModel model)
        {
            //use user input (name and balance) and add householdid and add model to accounts table
            if(ModelState.IsValid)
            {
                model.Account.HouseHoldId = model.Householdid;
                if (model.Account.Balance > 0)
                {
                    //if initial balance is greater than 0 create a transaction
                    //within transaction reference the transaction to the "adjustment" category
                    var transaction = new Transaction
                    {
                        Amount = model.Account.Balance,
                        Created = System.DateTime.Now,
                        Description = "Initial Account Deposit",
                        Reconciled = 0,                        
                        CategoryId = db.HouseHolds.Find(model.Householdid).Categories.FirstOrDefault(f => f.Name == "Adjustment").Id,
                        UpdatedById = User.Identity.GetUserId()
                    };
                    model.Account.Transactions.Add(transaction);
                }
                db.Accounts.Add(model.Account);
                db.SaveChanges();                
            }
            return RedirectToAction("Index", new { householdid = model.Householdid});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(HouseholdAccountViewModel model, int accountId)
        {
            if(ModelState.IsValid)
            {
                var account = db.Accounts.Find(accountId);
                account.Name = model.Account.Name;
                db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { householdid = model.Householdid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int accountid, int householdid)
        {            
            var account = db.Accounts.Find(accountid);
            if(account == null)
            {
                ModelState.AddModelError("account","The account model doesn't exist buddy");
            }
            /*
             * 
             * possible solution to the modal problem can be to put it in a partial view. so create a partial view just for the modal
             * and a controller action to draw up the pv and then an action to accept its input
             * 
             * */
            //remove account from household accounts collection and dereference household from account so it becomes floating account data
            //account is "removed" but transactions attach to account still exists
            //var household = db.HouseHolds.Find(householdid);
            //household.Accounts.Remove(account);
            //account.HouseHoldId = null;
            //db.SaveChanges();
            //delete whole account + cascade delete transactions associated with account also
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index","HouseAccount", new { householdid = householdid});
        }
    }
}