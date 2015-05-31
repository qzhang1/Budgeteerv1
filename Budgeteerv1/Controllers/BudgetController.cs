using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeteerv1.Models;
namespace Budgeteerv1.Controllers
{
    public class BudgetController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Budget
        public ActionResult Index(int householdid)
        {
            var bi = db.Budget.Where(u => u.HouseHoldId == householdid);
            var model = new BudgetViewModel
            {
                Income = bi.Where(b => b.IsIncome == true).ToList(),
                Expenses = bi.Where(b => b.IsIncome == false).ToList(),    
                householdId = householdid
            };
            ViewBag.CategoryId = new SelectList(db.Categories.Where(a => a.HouseholdId == householdid), "Id", "Name");
            return View(model);
        }

        // GET: Budget/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

       
        // POST: Budget/Create
        [HttpPost]
        public ActionResult Create(BudgetViewModel model, int hhId)
        {
            if(ModelState.IsValid)
            {
                var hh = db.HouseHolds.Find(hhId);
                var a = new BudgetItem
                {
                    Description = model.BudgetItem.Description,
                    Category = db.Categories.Find(model.CategoryId),
                    Amount = model.BudgetItem.Amount,
                    Frequency = model.BudgetItem.Frequency,
                    HouseHoldId = hhId,
                    IsIncome = model.BudgetItem.IsIncome,
                    Created = System.DateTime.Now
                };
                hh.Budgets.Add(a);
                db.Budget.Add(a);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { householdId = hhId });
        }

        [HttpPost]
        public ActionResult CreateCat(BudgetViewModel model, int hhId)
        {
            if(ModelState.IsValid)
            {
                var cat = new Category
                {
                    Name = model.Category.Name,
                    HouseholdId = hhId
                };
                var hh = db.HouseHolds.Find(hhId);
                hh.Categories.Add(cat);
                db.Categories.Add(cat);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { householdid = hhId });
        }

        // GET: Budget/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Budget/Edit/5
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

        // GET: Budget/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Budget/Delete/5
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
