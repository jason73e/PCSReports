using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PCSReports.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Controllers
{
    public class BDSetupController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: BDSetup
        [Audit]
        public async Task<ActionResult> Index()
        {
            string sUser = ConfigurationManager.AppSettings["DefaultUser"];
            string sPassword = ConfigurationManager.AppSettings["DefaultUserPassword"];
            string sEmail = ConfigurationManager.AppSettings["DefaultUserEmail"];
            //Add Admin Role
            if (!db.Roles.Where(x => x.Name == "Admin").Any())
            {
                IdentityRole identityRole = new IdentityRole { Name = "Admin" };
                db.Roles.Add(identityRole);
                db.SaveChanges();
            }
            IdentityRole adminRole = db.Roles.Where(x => x.Name == "Admin").SingleOrDefault();
            //Add Default user if Missing
            if (!db.Users.Where(x => x.UserName == sUser).Any())
            {
                ApplicationUser defaultuser = new ApplicationUser { UserName = sUser, Email = sEmail };
                var CreateUserResult = await UserManager.CreateAsync(defaultuser, sPassword);
            }
            //Reset Default user password
            ApplicationUser Resetuser = UserManager.FindByName(sUser);
            string sToken = await UserManager.GeneratePasswordResetTokenAsync(Resetuser.Id);
            var ResetResult = await UserManager.ResetPasswordAsync(Resetuser.Id, sToken, sPassword);

            //Add Default user to Admin Role if doesn't have.
            bool bHasRole = false;

            foreach(IdentityUserRole r in Resetuser.Roles)
            {
                if (r.RoleId == adminRole.Id)
                {
                    bHasRole = true;
                }
            }

            if(!bHasRole)
            {
                UserManager.AddToRole(Resetuser.Id, adminRole.Name);
            }


            return RedirectToAction("Index", "Report");
        }
    }
}