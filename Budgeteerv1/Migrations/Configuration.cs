namespace Budgeteerv1.Migrations
{
    using Budgeteerv1.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Budgeteerv1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Budgeteerv1.Models.ApplicationDbContext context)
        {
            //Create Users
            var UserManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            ApplicationUser User0;
            if(!context.Users.Any(r => r.Email == "qzhang112@gmail.com"))
            {
                User0 = new ApplicationUser
                {
                    UserName = "qzhang112@gmail.com",
                    Email = "qzhang112@gmail.com",
                    DisplayName = "Qi Zhang"
                };
                UserManager.Create(User0, "MasterSw0rd");
            }
            else
            {
                User0 = context.Users.Single(u => u.Email == "qzhang112@gmail.com");
            }            


        }
    }
}
