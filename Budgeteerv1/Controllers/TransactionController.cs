using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeteerv1.Models;
using Budgeteerv1.Models.CustomAttributes;
namespace Budgeteerv1.Controllers
{
    [RequireHousehold]
    public class TransactionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Transaction
        public ActionResult Index(int accountId)
        {
            var model = new TransactionViewModel
            {
                AccountId = accountId,
                Transactions = db.Accounts.Find(accountId).Transactions
            };
            ViewBag.Categories = new SelectList(db.Categories, "Id","Name");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionViewModel model)
        {
            if(ModelState.IsValid)
            {
                var account = db.Accounts.Find(model.AccountId);

            }
            return RedirectToAction("Index", new { accountId = model.AccountId });
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