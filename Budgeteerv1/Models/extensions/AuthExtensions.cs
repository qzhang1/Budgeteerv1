using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
namespace Budgeteerv1.Models.extensions
{
    public static class AuthExtensions
    {
        public static string GetHouseholdId(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseHoldId");
            if(HouseholdClaim != null)
            {
                return HouseholdClaim.Value;
            }
            else
            {
                return null;
            }
        }

        public static bool IsInHousehold(this IIdentity user)
        {
            var householdClaim = ((ClaimsIdentity)user).Claims.FirstOrDefault(c => c.Type == "HouseHoldId");
            return householdClaim != null && !string.IsNullOrWhiteSpace(householdClaim.Value);
        }

        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
    }
}