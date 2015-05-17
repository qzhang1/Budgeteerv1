using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Budgeteerv1.Models;

namespace Budgeteerv1.Controllers
{
    public class DashboardController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dashboard
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Users.Find(userId);
            return View(model);
        }
    }
}