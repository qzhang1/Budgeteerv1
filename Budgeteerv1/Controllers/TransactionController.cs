using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeteerv1.Models;
using Budgeteerv1.Models.CustomAttributes;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Data.Entity;
using System.Data;
namespace Budgeteerv1.Controllers
{
    [RequireHousehold]
    public class TransactionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Transaction
        public ActionResult Index(int accountId)
        {
            var acct = db.Accounts.Find(accountId);
            var model = new TransactionViewModel
            {
                AccountId = accountId,
                Account = acct,
                Transactions = acct.Transactions
            };
            ViewBag.CategoryId = new SelectList(db.Categories.Where(a => a.HouseholdId == acct.HouseHoldId), "Id", "Name");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionViewModel model, int accountId)
        {
            try
            {
                //find acct transaction is under -> take user input and create a transaction object -> add transaction to database -> add transaction to account collection -> save changes
                if(ModelState.IsValid)
                {
                    var account = db.Accounts.Find(accountId);

                    //can be refactored to more efficient code with automapper
                    var transaction = new Transaction
                    {
                        AccountId = model.AccountId,
                        Amount = model.Transaction.Amount,
                        Description = model.Transaction.Description,
                        IsIncome = model.Transaction.IsIncome,
                        Reconciled = model.Transaction.Reconciled,
                        CategoryId = model.CategoryId,
                        Created = model.Transaction.Created,
                        UpdatedBy = db.Users.Find(User.Identity.GetUserId())
                    };
                    if(transaction.IsIncome)
                    {
                        account.Balance += transaction.Amount;
                    }
                    else
                    {
                        account.Balance -= transaction.Amount;
                    }
                    db.Transactions.Add(transaction);
                    account.Transactions.Add(transaction);
                    db.SaveChanges();
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save create Transaction");
            }
            
            return RedirectToAction("Index", new { accountId = model.AccountId });
        }

        [HttpGet]
        public JsonResult EditTransactionInfo(int transId)
        {
            var trans = db.Transactions.Find(transId);
            return Json(new { Description = trans.Description,
                              Amount = trans.Amount,
                              Reconciled = trans.Reconciled,
                              IsIncome = trans.IsIncome,
                              Category = trans.Category.Name,
                              Created = trans.Created.Date
            }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult EditPartial(int transId, int acctId)
        {
            if(transId == 0 || acctId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var acct = db.Accounts.Find(acctId);
            var model = db.Transactions.Find(transId);

            ViewBag.CategoryId = new SelectList(db.Categories.Where(a => a.HouseholdId == acct.HouseHoldId), "Id", "Name",model.CategoryId);
            return PartialView("_EditTransaction", model);

        }
        
        //edit using bind attribute        
        //Edit Post action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Amount, CategoryId, Description, IsIncome, Reconciled, Created, AccountId, Id, UpdatedById")]Transaction model)
        {
            
            //retrieve transaction from db. update transaction properties with model properties
            //update current account balance then save entity state
            if(ModelState.IsValid)
            {

                var acct = db.Accounts.Find(model.AccountId);
                db.Transactions.Attach(model);

                //note: take model id and find the transaction in db if the old IsIncome  value is different from the new
                //then do the following to the acct balance
                var totalIncome = acct.Transactions.Where(i => i.IsIncome == true).Sum(s => s.Amount);
                var totalExpense = acct.Transactions.Where(i => i.IsIncome == false).Sum(s => s.Amount);
                acct.Balance = totalIncome - totalExpense;
                model.UpdatedById = User.Identity.GetUserId();
                db.Entry(model).State = EntityState.Modified;                

                db.SaveChanges();
                ViewBag.CategoryId = new SelectList(db.Categories.Where(a => a.HouseholdId == acct.HouseHoldId), "Id", "Name", model.CategoryId);
                return RedirectToAction("Index", new { accountId = model.AccountId});                   
            }
            else
            {
                ModelState.AddModelError("", "The transaction edit model is invalid");
            }
            return PartialView("_EditTransaction",model);
        }
        

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int accountId, int TransId)
        {
            if(accountId == 0 || TransId == 0)
            {
                return RedirectToAction("Index", new { accountId = accountId });
            }
            else
            {
                var acct = db.Accounts.Find(accountId);
                var trans = db.Transactions.Find(TransId);
                if (trans.IsIncome)
                {
                    acct.Balance -= trans.Amount;
                }
                else
                {
                    acct.Balance += trans.Amount;
                }
                acct.Transactions.Remove(trans);
                db.Transactions.Remove(trans);
                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = accountId });
            }
        }

        //[HttpPost]
        //public JsonResult Autocomplete(string category)
        //{
        //    var result = new List<KeyValuePair<string, string>>();
        //    IList<SelectListItem> List = new List<SelectListItem>();
        //    List.Add(new SelectListItem{})
        //}
    }
}