using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WPMAsignmentHandling.Models;
using WPMAsignmentHandling.Additional;

namespace WPMAsignmentHandling.Controllers
{
    //[RequireHttpsAttribute]
    [Authorize(Roles = "Administrator, Winkhardt-MA, Messe-MA")]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [AllowAnonymous]
        public ActionResult checkUserLoggedIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Json(true);
            }

            return Json(false);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string  WebShop = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!String.IsNullOrEmpty(WebShop) || (returnUrl != null && returnUrl.IndexOf("WebShop", StringComparison.OrdinalIgnoreCase) > 0))
            {
                ViewBag.WebShop = true;
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "WebShop");
                }
                return View("~/Views/WebShop/Login.cshtml");
            }
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View("/Account/Login.cshtml", model);
            }

            var userMail = await UserManager.FindByEmailAsync(model.Email);
            ViewBag.MailExists = false;
            if (userMail != null)
            {
                ViewBag.MailExists = true;
                bool MailConfirmed = UserManager.IsEmailConfirmed(userMail.Id);
                if (!MailConfirmed)
                {
                    ViewBag.NotConfirmed = "true";
                    ViewBag.Email = model.Email;
                    return View(model);
                }
            }


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Benutzername oder Passwort stimmen nicht");
                    return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginWebshop(LoginViewModel model, string returnUrl="/WebShop")
        {


            ViewBag.MailExists = false;

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "WebShop");
            }
            string viewName = "~/Views/WebShop/Login.cshtml";
            ViewBag.WebShop = true;
            if (!ModelState.IsValid)
            {
                return View(viewName, model);
            }

            var userMail = await UserManager.FindByEmailAsync(model.Email);
            if (userMail != null)
            {
                ViewBag.MailExists = true;
                bool MailConfirmed = UserManager.IsEmailConfirmed(userMail.Id);
                if (!MailConfirmed)
                {
                    ViewBag.NotConfirmed = "true";
                    return View(viewName, model);
                }
            }
            
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Benutzername oder Passwort stimmen nicht");
                    return View(viewName, model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(FormCollection fc, string WebShop)
        {
            
            string viewName = "";
            if (!String.IsNullOrEmpty(WebShop))
            {
                ViewBag.ReturnUrl = "/WebShop";
                viewName = "~/Views/WebShop/Register.cshtml";
            }
            return View(viewName);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string briefdruckRegister, string messeRegister, string WebShop = "")
        {
            string viewName = "";
            if (WebShop == "WebShop")
            {
                ViewBag.ReturnUrl = "/WebShop";
                viewName = "~/Views/WebShop/Register.cshtml";
                ViewBag.ReturnUrl = "/WebShop";
            }
            
            if (ModelState.IsValid)
            {
                if (WebShop == "WebShop")
                {
                    if (!(model.Email.CaseInsensitiveContains("@briefdruck.de") || model.Email.CaseInsensitiveContains("@messe-stuttgart.de")))
                    {
                        ModelState.AddModelError("", "Nicht Authorisiert!");
                        return View(viewName, model);
                    }
                }
                else
                {
                    if (!(model.Email.CaseInsensitiveContains("@briefdruck.de")))
                    {
                        ModelState.AddModelError("", "Nicht Authorisiert!");
                        return View(viewName, model);
                    }
                }

                var Emailexists = UserManager.Users.SingleOrDefault(r => r.Email.ToLower().Contains(model.Email.ToLower()));
                if (Emailexists != null)
                {
                    ModelState.AddModelError("", "Die Emailadresse '" + model.Email.ToString() + "' is bereits vergeben!");
                    return View(viewName, model);
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Vorname = model.Vorname, Nachname = model.Nachname, Telefonnummer = model.Telefonnummer };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code, WebShop=WebShop }, protocol: Request.Url.Scheme);
                    string message = "Sehr geehrte(r) Frau/Herr " + user.Nachname + "<br/><br/>"
                    + "Bitte klicken Sie <a href=\"" + callbackUrl + "\">hier</a> um Ihre Email-Adresse zu bestätigen." + "<br/><br/><br/>"
                    + "Mit freundlichen Grüßen" + "<br/>"
                    + "Ihr Direct Mail Service Winkhardt Team" + "<br/><br/>"
                    + "Tel.: 0711/78198830 " + "<br/>"
                    + "Email: werbemittel@briefdruck.de";
                    await UserManager.SendEmailAsync(user.Id, "Bestätigen der Benutzerregistrierung", message);
                    ViewBag.Registered = "true";
                    ViewBag.Email = model.Email;
                    Extensions.NotificationUserRegistration(user);
                    if (model.Email.CaseInsensitiveContains("@messe-stuttgart.de"))
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Messe-MA");
                        ViewBag.Registered = true;
                        return View("~/Views/WebShop/Register.cshtml");
                    }
                    else
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Winkhardt-MA");
                        return View();
                    }
                }
                //var result2 = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                //if (result.Succeeded)
                //{
                    
                //    //System.Diagnostics.Debug.Write()

                //    switch (result2)
                //    {

                //        case SignInStatus.Success:
                //            if (model.Email.Contains("@messe-stuttgart.de"))
                //            {
                //                await UserManager.AddToRoleAsync(user.Id, "Messe-MA");
                //                return RedirectToAction("Index", "WebShop");
                //            }
                //            else
                //            {
                //                await UserManager.AddToRoleAsync(user.Id, "Winkhardt-MA");
                //                return RedirectToAction("Index", "Home");
                //            }
                //        case SignInStatus.LockedOut:
                //            return View("Lockout");
                //        case SignInStatus.Failure:
                //        default:
                //            ModelState.AddModelError("", "Benutzername oder Passwort stimmen nicht");
                //            return View(viewName, model);
                //    }
                    

                //}
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(viewName, model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmationMail(LoginViewModel model, string WebShop="")
        {
            string viewName = "Login";
            if (WebShop == "WebShop")
            {
                ViewBag.WebShop = true;
                viewName = "~/Views/WebShop/Login.cshtml";
            }
            if (model.Email != null)
            {
                ViewBag.ConfirmationResend = true;
                var user = await UserManager.FindByEmailAsync(model.Email);
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code, WebShop = WebShop }, protocol: Request.Url.Scheme);
                string message = "Sehr geehrte(r) Frau/Herr " + user.Nachname + "<br/><br/>"
                     + "Bitte klicken Sie <a href=\"" + callbackUrl + "\">hier</a> um Ihre Email-Adresse zu bestätigen." + "<br/><br/><br/>"
                     + "Mit freundlichen Grüßen" + "<br/>"
                     + "Ihr Direct Mail Service Winkhardt Team" + "<br/><br/>"
                     + "Tel.: 0711/78198830 " + "<br/>"
                     + "Email: werbemittel@briefdruck.de";
                await UserManager.SendEmailAsync(user.Id, "Bestätigen der Benutzerregistrierung", message);
                Extensions.NotificationResendEmailConfirmationLink(user);
                ViewBag.Email = model.Email;
                return View(viewName);
            }
            return View(viewName);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code, string WebShop="")
        {
            if (userId == null || code == null)
            {

                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            string viewName = "ConfirmEmail";
            if (WebShop == "WebShop")
            {
                ViewBag.WebShop = true;
                viewName = "~/Views/WebShop/ConfirmEmail.cshtml";
            }
            var user = await UserManager.FindByIdAsync(userId);
            Extensions.NotificationUserEmailComfirmation(user);
            return View(result.Succeeded ? viewName : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword(FormCollection fc, string WebShop)
        {
            
            string viewName = "";
            if (WebShop == "WebShop")
            {
                ViewBag.WebShop = "WebShop";
                viewName = "~/Views/WebShop/ForgotPassword.cshtml";
            }
            return View(viewName);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model, string WebShop="")
        {
            string viewName = "";
            if (WebShop == "WebShop")
            {
                ViewBag.WebShop = "WebShop";
                viewName = "~/Views/WebShop/ForgotPassword.cshtml";

            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.ForgottConfirmation = "true";
                    return View(viewName);
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, WebShop = WebShop }, protocol: Request.Url.Scheme);
                string message = "Sehr geehrte(r) Frau/Herr " + user.Nachname + "<br/><br/>"
                                  + "Bitte klicken Sie <a href=\"" + callbackUrl + "\">hier</a> um ein neues Passwort zu vergeben." + "<br/><br/><br/>"
                                  + "Mit freundlichen Grüßen" + "<br/>"
                                  + "Ihr Direct Mail Service Winkhardt Team" + "<br/><br/>"
                                  + "Tel.: 0711/78198830 " + "<br/>"
                                  + "Email: werbemittel@briefdruck.de";
                await UserManager.SendEmailAsync(user.Id, "Neues Passwort vergeben", message);
                Extensions.NotificationUserResetPassword(user);
                ViewBag.ForgottConfirmation = "true";
                ViewBag.Email = model.Email;
                return View(viewName);
            }

            // If we got this far, something failed, redisplay form
            return View(viewName, model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation(string WebShop)
        //{
        //    if (WebShop == "WebShop")
        //    {
        //        return View("~/Views/WebShop/ForgotPasswordConfirmation.cshtml");
        //    }
        //    return View();
        //}

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string WebShop="")
        {
            string viewName = "";
            if (WebShop == "WebShop")
            {
                ViewBag.WebShop = "WebShop";
                viewName = "~/Views/WebShop/ResetPassword.cshtml";
            }
            return code == null ? View("Error") : View(viewName);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model, string WebShop="")
        {
            string viewName = "";
            if (WebShop == "WebShop")
            {
                ViewBag.WebShop = "WebShop";
                viewName = "~/Views/WebShop/ResetPassword.cshtml";
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return View(viewName);
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                ViewBag.ResetSuccess = "true";
                Extensions.NotificationUserResetPassword(user);
                return View(viewName);
            }
            AddErrors(result);
            return View(viewName);
        }

        //
        // GET: /Account/ResetPasswordConfirmation

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff(string WebShop="")
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if (WebShop == "LogOff")
            {
                ViewBag.ReturnUrl = "/WebShop";
                return RedirectToAction("Index", "WebShop");
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}