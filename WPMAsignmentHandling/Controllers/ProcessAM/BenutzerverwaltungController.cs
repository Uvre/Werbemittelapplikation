using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WPMAsignmentHandling.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace WPMAsignmentHandling.Controllers.ProcessAM
{
    [Authorize(Roles = "Administrator")]
    public class BenutzerverwaltungController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();
        

        public ActionResult Index()
        {
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            IQueryable<ApplicationUser> user = UserManager.Users;
            //user.
            return View(user);
        }

        public ActionResult BenutzerdatenAendern(string Vorname, string Nachname, string Email, string Tel, string Role, string UserID)
        {
            string RoleName= "";
            string currentRoleName = "";

            switch(Role){
                case "2a21d867-e861-4623-bae6-a798e9a7eb6b":
                    RoleName = "Messe-MA";
                    break;
                case "93ccbba4-e1b4-4472-ae01-4dce86d5f588":
                    RoleName = "Winkhardt-MA";
                    break;
                case "0bfe036a-0eb6-4798-89d6-23c258ceeeb5":
                    RoleName = "Administrator";
                    break;
            }

            
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var User = UserManager.Users.SingleOrDefault(r => r.Id == UserID);

            switch(User.Roles.ElementAt(0).RoleId){
                case "2a21d867-e861-4623-bae6-a798e9a7eb6b":
                    currentRoleName = "Messe-MA";
                    break;
                case "93ccbba4-e1b4-4472-ae01-4dce86d5f588":
                    currentRoleName = "Winkhardt-MA";
                    break;
                case "0bfe036a-0eb6-4798-89d6-23c258ceeeb5":
                    currentRoleName = "Administrator";
                    break;
            }

            if(User !=null){
                User.Email = Email;
                User.Vorname = Vorname;
                User.Nachname = Nachname;
                User.Telefonnummer = Tel;
                if (!(User.Roles.ElementAt(0).RoleId == Role))
                {
                    UserManager.RemoveFromRole(User.Id, currentRoleName);
                    UserManager.AddToRole(User.Id, RoleName );
                }
            }
            UserManager.Update(User);
            return Json(true);
        }

        public ActionResult RemoveUser(string UserID)
        {
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = UserManager.Users.Single(r => r.Id == UserID);
            var logins = user.Logins;
            foreach (var login in logins.ToList())
            {
                UserManager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }

            var rolesForUser = UserManager.GetRoles(user.Id);

            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    UserManager.RemoveFromRole(user.Id, item);
                }
            }

            UserManager.Delete(user);

            return RedirectToAction("Index");
        }
    }
}
