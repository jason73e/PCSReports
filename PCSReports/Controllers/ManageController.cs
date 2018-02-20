using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PCSReports.Models;

namespace PCSReports.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                Logins = await UserManager.GetLoginsAsync(userId),
            };
            return View(model);
        }

        //
        // GET: /Manage/Users
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Users()
        {
            ManageUsersViewModel usersViewModel = new ManageUsersViewModel();
            List<ApplicationUser> lsUsers = new List<ApplicationUser>();
            if (db.Users.Any())
            {
                lsUsers = db.Users.ToList();
            }
            usersViewModel.AllUsers = lsUsers;
            return View(usersViewModel);
        }

        //
        // GET: /Manage/Roles
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Roles()
        {
            ManageRolesViewModel rolesViewModel = new ManageRolesViewModel();
            List<IdentityRole> lsRoles = new List<IdentityRole>();
            if(db.Roles.Any())
            {
                lsRoles = db.Roles.ToList();
            }
            rolesViewModel.AllRoles = lsRoles;
            return View(rolesViewModel);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Admin")]
        public JsonResult FillRole(string username)
        {
            ManageRolesViewModel rolesViewModel = new ManageRolesViewModel();
            List<IdentityRole> lsRoles = new List<IdentityRole>();
            if (db.Roles.Any())
            {
                lsRoles = db.Roles.ToList();
            }
            rolesViewModel.AllRoles = lsRoles;
            rolesViewModel.UserName = username;
            rolesViewModel.slUsers = Utility.GetUsers();
            rolesViewModel.RoleChoices = Utility.GetRoles();
            IdentityUser user = UserManager.FindByName(rolesViewModel.UserName);
            if (user != null)
            {
                rolesViewModel.SelectedRoles = user.Roles.Select(m => m.RoleId).ToList();
                MultiSelectList sl = new MultiSelectList(rolesViewModel.RoleChoices, "Value", "Text", rolesViewModel.SelectedRoles);
                MvcHtmlString result = Utility.CheckBoxList("chklistitem", sl);
                rolesViewModel.roleChks = result.ToHtmlString();
            }
            else
            {
                rolesViewModel.roleChks = string.Empty;
            }
            return Json(rolesViewModel, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Manage/UserRoles
        [Audit]
        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles()
        {
            ManageRolesViewModel rolesViewModel = new ManageRolesViewModel();
            List<IdentityRole> lsRoles = new List<IdentityRole>();
            if (db.Roles.Any())
            {
                lsRoles = db.Roles.ToList();
            }
            rolesViewModel.AllRoles = lsRoles;
            //rolesViewModel.UserName = User.Identity.Name;
            rolesViewModel.slUsers = Utility.GetUsers();
            rolesViewModel.RoleChoices = Utility.GetRoles();
            if (rolesViewModel.UserName != null)
            {
                IdentityUser user = UserManager.FindByName(rolesViewModel.UserName);
                rolesViewModel.SelectedRoles = user.Roles.Select(m => m.RoleId).ToList();
            }
            return View(rolesViewModel);
        }

        // POST: ReportUserModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult UserRoles(ManageRolesViewModel rolesViewModel)
        {
            string sp = Request.Form["chklistitem"];
            List<string> lsRoleIDs = new List<string>();
            string sUserName = rolesViewModel.UserName;
            if (sp != null)
            {
                lsRoleIDs = sp.Split(',').ToList();
            }

            RemoveRolesForUser(sUserName);

            AddRolesForUser(sUserName, lsRoleIDs);

            return RedirectToAction("UserRoles");
        }

        [Authorize(Roles = "Admin")]
        [Audit]
        private void AddRolesForUser(string sUserName, List<string> lsRoleIDs)
        {
            IdentityUser user = UserManager.FindByName(sUserName);
            foreach (string s in lsRoleIDs)
            {
                UserManager.AddToRole(user.Id, db.Roles.Where(x => x.Id == s).Single().Name);
            }
        }

        [Authorize(Roles = "Admin")]
        [Audit]
        private void RemoveRolesForUser(string sUserName)
        {
            IdentityUser user = UserManager.FindByName(sUserName);
            List<IdentityUserRole> currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                UserManager.RemoveFromRole(user.Id, db.Roles.Where(x => x.Id == role.RoleId).Single().Name);
            }
        }

        //
        // GET: /Manage/AddRole
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult AddRole()
        {
            return View();
        }

        // POST: ReportModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult AddRole(ManageRolesViewModel rolesViewModel)
        {
            if (ModelState.IsValid)
            {
                if (db.Roles.Where(x => x.Name == rolesViewModel.RoleName).Any())
                {
                    ViewBag.ErrorMessage = "A role already exists with this Name.";
                    return View(rolesViewModel);
                }
                else
                {
                    try
                    {
                        IdentityRole identityRole = new IdentityRole { Name = rolesViewModel.RoleName };
                        db.Roles.Add(identityRole);
                        db.SaveChanges();
                        return RedirectToAction("Roles");
                    }
                    catch(Exception e)
                    {
                        ViewBag.ErrorMessage = e.Message;
                        return View(rolesViewModel);
                    }
                }
            }

            return View(rolesViewModel);
        }

        //
        // GET: /Manage/DeleteRole/5
        [Audit]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteRole(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = db.Roles.Where(x => x.Id == id.ToString()).SingleOrDefault();
            if (role == null)
            {
                return HttpNotFound();
            }
            ManageRolesViewModel rolesViewModel = new ManageRolesViewModel();
            rolesViewModel.role = role;
            return View(rolesViewModel);
        }

        // POST: ReportModels/Delete/5
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult DeleteRoleConfirmed(Guid id)
        {
            IdentityRole role = db.Roles.Where(x => x.Id == id.ToString()).SingleOrDefault();
            db.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Roles");
        }

        //
        // GET: /Manage/DeleteUser/5
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult DeleteUser(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Where(x => x.Id == id.ToString()).SingleOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            ManageUsersViewModel usersViewModel = new ManageUsersViewModel();
            usersViewModel.user = user;
            return View(usersViewModel);
        }

        // POST: ReportModels/Delete/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult DeleteUserConfirmed(Guid id)
        {
            ApplicationUser user = db.Users.Where(x => x.Id == id.ToString()).SingleOrDefault();
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Users");
        }
        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Manage/SetUserPassword
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult SetUserPassword(string UserName)
        {
            SetUserPasswordViewModel vm = new SetUserPasswordViewModel();
            vm.slUsers = Utility.GetUsers();
            vm.UserName = UserName;
            return View(vm);
        }

        //
        // POST: /Manage/SetUserPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public async Task<ActionResult> SetUserPassword(SetUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindByName(model.UserName);
                string sToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, sToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}